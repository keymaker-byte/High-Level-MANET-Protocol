using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

namespace NetLayer
{
    /// <summary>
    /// Clase que enumera los estados posibles de IP 
    /// </summary>
    internal static class IpState
    {
        public const int NOTFOUND = 0;
        public const int INVALID = 1;
        public const int DUPLICATE = 2;
        public const int VALID = 3;
    }

    /// <summary>
    /// Clase que enumera los estados posibles de la conexión inalambrica
    /// </summary>
    internal static class IphandlerState
    {
        public const Int32 STARTEDSTRONG = 1;
        public const Int32 STOPPED = 2;
        public const Int32 STARTEDWEAK = 3;
    }
    
    /// <summary>
    /// Clase que se preocupa de que no hayan conflictos con la IP
    /// </summary>
    internal class IpHandler
    {
        /// <summary>
        /// Declaración de delegado para evento de cambio de ip
        /// </summary>
        public delegate void ResetIpDelegate();
        /// <summary>
        /// Handler para cambio de ip
        /// </summary>
        /// <returns>vacio</returns>
        private ResetIpDelegate resetIpDelegate;
        /// <summary>
        /// Thread que verifica la IP
        /// </summary>
        private Thread checkIpThread;
        /// <summary>
        /// El estado de este objeto (un parametro de IpHandlerState)
        /// </summary>
        private Int32 state;
        /// <summary>
        /// Objeto de lock
        /// </summary>
        private Object stopLock;
        /// <summary>
        /// Cola de Ip's de mensajes i'm alive recibidos
        /// </summary>
        private Queue<IPAddress> queue;
        /// <summary>
        /// Objeto de lock para la cola
        /// </summary>
        private Object queueLock;
        /// <summary>
        /// Los datos de red
        /// </summary>
        private NetData netData;
        /// <summary>
        /// Contador de loliness
        /// </summary>
        private Int32 lolinessTimeOut;
        /// <summary>
        /// valor que cambia cuando el usuario esta correctamente difundiendo mensajes
        /// </summary>
        private Int64 lastAliveValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="netData">Los datos de configuración de red</param>
        /// <param name="resetIpDelegate">la función que se gatilla cuando la Ip debe ser cambiada</param>
        public IpHandler(NetData netData, ResetIpDelegate resetIpDelegate)
        {
            this.resetIpDelegate = resetIpDelegate;
            this.netData = netData;
            checkIpThread = new Thread(new ThreadStart(checkIp));
            state = IphandlerState.STOPPED;
            stopLock = new Object();
            queueLock = new Object();
            queue = new Queue<IPAddress>();
            lolinessTimeOut = 0;
            lastAliveValue = 0;
        }

        /// <summary>
        /// Comienza la verificación fuerte
        /// </summary>
        public void startStrongDAD()
        {
            state = IphandlerState.STARTEDSTRONG;
            checkIpThread.Start();
        }

        /// <summary>
        /// Comienza la verificación débil
        /// </summary>
        public void chageToWeakDAD()
        {
            state = IphandlerState.STARTEDWEAK;
        }

        /// <summary>
        /// Detiene la verificación
        /// </summary>
        public void stop()
        {
            lock (stopLock)
            {
                state = IphandlerState.STOPPED;
            }
        }

        /// <summary>
        /// Verifica que no exista Ip duplicada en el sistema operativo, si gatilla resetIpDelegate, asegura que el Thread se detendrá
        /// </summary>
        private void checkIp()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(netData.WaitTimeStart);
                //Chequea Strong DAD
                if (state.Equals(IphandlerState.STARTEDSTRONG))
                {
                    int ipState = SystemHandler.getIpState(netData.NetworkAdapter, netData.IpTcpListener);
                    switch (ipState)
                    {
                        case IpState.DUPLICATE:
                            {
                                resetIpDelegate();
                                return;
                            }
                        case IpState.NOTFOUND:
                            {
                                resetIpDelegate();
                                return;
                            }
                    }
                }
                else if (state.Equals(IphandlerState.STARTEDWEAK))
                {
                    //Chequea Weak DAD
                    lock (queueLock)
                    {
                        while (queue.Count > 0)
                        {
                            IPAddress outterIp = queue.Dequeue();
                            if (outterIp.Equals(netData.IpTcpListener))
                            {
                                resetIpDelegate();
                                return;
                            }
                        }
                    }
                    //chequea Strong DAD
                    int ipState = SystemHandler.getIpState(netData.NetworkAdapter, netData.IpTcpListener);
                    switch (ipState)
                    {
                        case IpState.DUPLICATE:
                            {
                                resetIpDelegate();
                                return;
                            }
                        case IpState.INVALID:
                            {
                                resetIpDelegate();
                                return;
                            }
                        case IpState.NOTFOUND:
                            {
                                resetIpDelegate();
                                return;
                            }
                    }
                    //Chequea loneliness
                    if (netData.OpSystem == OpSystemType.WIN7)
                    {
                        Int64 nBytes = SystemHandler.getAliveValue(netData.NetworkAdapter);
                        if (nBytes == lastAliveValue)
                        {
                            lolinessTimeOut++;
                            if (lolinessTimeOut >= netData.LolinessTimeOut)
                            {
                                resetIpDelegate();
                                return;
                            }
                        }
                        else
                        {
                            lolinessTimeOut = 0;
                        }
                        lastAliveValue = nBytes;
                    }

                    //Chequea operatividad del adaptador
                    if (netData.OpSystem == OpSystemType.WIN7)
                    {
                        if (!SystemHandler.isOperative(netData.NetworkAdapter))
                        {
                            resetIpDelegate();
                            return;
                        }
                    } 

                    
                }
                //Detiene o duerme segun corresponda
                lock (stopLock)
                {
                    if (state.Equals(IphandlerState.STOPPED))
                    {
                        return;
                    }
                }
                
            }
        }

        /// <summary>
        /// Coloca una ip a la cola de ips para chequear
        /// </summary>
        /// <param name="outterIp">la ip a encolar</param>
        public void put(IPAddress outterIp)
        {
            lock (queueLock)
            {
                queue.Enqueue(outterIp);
            }
        }
    }
}
