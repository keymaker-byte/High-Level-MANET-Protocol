using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace NetLayer
{
    /// <summary>
    /// Clase que representa una máquina remota (usuario vecino de bajo nivel) conectada a la RED
    /// </summary>
    public class RemoteMachine
    {

        private IPAddress _ip;
        private TcpClient _tcpClient;
        private String _id;
        private Int32 _fails;
        private byte[] _buffer = new byte[100];
        private int _bufferFilled;

        
        

        /// <summary>
        /// Lock para envio de mensajes TCP
        /// </summary>
        private Object sendTCPLock;
        private Int32 closePoint;
        private NetworkStream senderStream;


        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="_ip">la ip de la maquina remota</param>
        /// <param name="_tcpClient">el objeto TcpClient asociado</param>
        public RemoteMachine(IPAddress _ip, TcpClient _tcpClient)
        {
            Ip = _ip;
            TcpClient = _tcpClient;
            Id = Guid.NewGuid().ToString();
            this.sendTCPLock = new Object();
            this.Fails = 0;
            this.closePoint = 0;
            this.BufferFilled = 0;
        }

        /// <summary>
        /// La Ip de la máquina remota
        /// </summary>
        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        /// El cliente TCP con el cual se puede enviar/recibir mensajes de la maquina remota
        /// </summary>
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        /// <summary>
        /// El ID unico de esta maquina remota
        /// </summary>
        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Cantidad de fallas seguidas que se han originado al intentar enviar un mensaje
        /// </summary>
        public Int32 Fails
        {
            get { return _fails; }
            set { _fails = value; }
        }

        /// <summary>
        /// El buffer de lectura datos
        /// </summary>
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// Cantidad ocupada del buffer
        /// </summary>
        public int BufferFilled
        {
            get { return _bufferFilled; }
            set { _bufferFilled = value; }
        }

        /// <summary>
        /// Envia un mensaje a la red mediante esta maquina remota
        /// </summary>
        /// <param name="netMessage">el mensaje a enviar</param>
        /// <param name="timeOutWriteTCP">el tiempo maximo de espera para el envio</param>
        public void sendNetMessage(NetMessage netMessage, Int32 timeOutWriteTCP)
        {
            lock (sendTCPLock)
            {
                Fails++;
                senderStream = new NetworkStream(TcpClient.Client, false);
                try
                {
                    //senderStream.WriteTimeout = timeOutWriteTCP;
                    byte[] lenght = BitConverter.GetBytes(netMessage.getSize());
                    byte[] netByteMessage = new byte[4 + netMessage.getSize()];

                    IAsyncResult result = senderStream.BeginWrite(lenght, 0, 4, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeOutWriteTCP, false);
                    if (!success)
                    {
                        throw new Exception("TCP: intento de conexión ha tardado demasiado");
                    }
                    else
                    {
                        senderStream.EndWrite(result);
                    }
                    result = senderStream.BeginWrite(netMessage.Body, 0, netMessage.getSize(), null, null);
                    success = result.AsyncWaitHandle.WaitOne(timeOutWriteTCP, false);
                    if (!success)
                    {
                        throw new Exception("TCP: intento de conexión ha tardado demasiado");
                    }
                    else
                    {
                        senderStream.EndWrite(result);
                    }

                    senderStream.Close();
                    Fails = 0;
                }
                catch (ThreadAbortException e)
                {
                    try
                    {
                        senderStream.Close();
                    }
                    catch (Exception)
                    {
                    } 
                    throw e;
                }
                catch (Exception)
                {
                    try
                    {
                        senderStream.Close();
                    }
                    catch (Exception)
                    {
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Cierra la conexion a esta mquina remota.. los thread abort exception no detienen la ejecución
        /// </summary>
        public void close()
        {
            if (Interlocked.CompareExchange(ref closePoint, 1, 0) == 0)
            {
                try
                {
                    senderStream.Close(); //no mata, no es suficiente
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception)
                {
                }
                try
                {
                    NetworkStream nStream = TcpClient.GetStream(); //cuelga un rato, es suficiente
                    nStream.Close();
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception)
                {
                }
                //try
                //{
                //    TcpClient.Client.Close(); //mata
                //}
                //catch (ThreadAbortException e)
                //{
                //    throw e;
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    TcpClient.Close(); //mata
                //}
                //catch (ThreadAbortException e)
                //{
                //    throw e;
                //}
                //catch (Exception)
                //{
                //}
                closePoint = 0; 
            }
        }


    }
}
