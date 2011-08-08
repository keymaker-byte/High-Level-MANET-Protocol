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
        private Thread _clientThread;
        private String _id;
        private Int32 _fails;

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
        /// <param name="_clientThread">el thread que maneja lectura de datos de la maquina</param>
        public RemoteMachine(IPAddress _ip, TcpClient _tcpClient, Thread _clientThread)
        {
            Ip = _ip;
            TcpClient = _tcpClient;
            ClientThread = _clientThread;
            Id = Guid.NewGuid().ToString();
            this.sendTCPLock = new Object();
            this.Fails = 0;
            this.closePoint = 0;
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
        /// El thread que escucha los mensajes TCP provenientes de la maquina remota
        /// </summary>
        public Thread ClientThread
        {
            get { return _clientThread; }
            set { _clientThread = value; }
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
        /// Envia un mensaje de red a la maquina remota
        /// </summary>
        /// <param name="netMessage">El mensaje de red</param>
        /// <param name="timeOutWriteTCP">Tiempo de espera para enviar el mensaje</param>
        public void sendNetMessage(NetMessage netMessage, Int32 timeOutWriteTCP)
        {
            lock (sendTCPLock)
            {
                Fails++;
                senderStream = new NetworkStream(TcpClient.Client, false);
                try
                {
                    senderStream.WriteTimeout = timeOutWriteTCP;
                    byte[] lenght = BitConverter.GetBytes(netMessage.getSize());
                    byte[] netByteMessage = new byte[4 + netMessage.getSize()];
                    senderStream.Write(lenght, 0, 4);
                    senderStream.Write(netMessage.Body, 0, netMessage.getSize());
                    senderStream.Close();
                    Fails = 0;
                }
                catch (ThreadAbortException e)
                {
                    try
                    {
                        senderStream.Close(0);
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
                        senderStream.Close(0);
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
                    senderStream.Close(0);
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
                    NetworkStream nStream = TcpClient.GetStream();
                    nStream.Close(0);
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
                    TcpClient.Client.Close();
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
                    TcpClient.Close();
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
                    ClientThread.Abort();
                    ClientThread.Join();
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception)
                {
                }
                closePoint = 0; 
            }
        }
    }
}
