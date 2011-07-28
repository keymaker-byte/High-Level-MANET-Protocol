using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Timers;
using System.Threading;
using NetLayer;
using CommLayer.Messages;

namespace CommLayer
{

    /// <summary>
    /// Clase que enumera los estados posibles de la comunicación
    /// </summary>
    public static class CommunicationState
    {
        /// <summary>
        /// Constante para el estado conectando
        /// </summary>
        public const Int32 STARTING = 1;

        /// <summary>
        /// Constante para el estado conectado
        /// </summary>
        public const Int32 STARTED = 2;

        /// <summary>
        /// Constante para el estado desconectando
        /// </summary>
        public const Int32 STOPPING = 3;

        /// <summary>
        /// Constante para el estado desconectado
        /// </summary>
        public const Int32 STOPPED = 4;

        /// <summary>
        /// Constante para eln estado iniciado
        /// </summary>
        public const Int32 INITIATED = 5;
    }

    /// <summary>
    /// Clase que enumera los eventos que lanza
    /// </summary>
    public static class CommunicationEvent
    {
        /// <summary>
        /// Constante para el evento connect
        /// </summary>
        public const Int32 CONNECT = 0;
        
        /// <summary>
        /// Constante para el evento connecting
        /// </summary>
        public const Int32 CONNECTING = 1;

        /// <summary>
        /// Constante para el evento disconnecting
        /// </summary>
        public const Int32 DISCONNECTING = 2;

        /// <summary>
        /// Constante para el evento disconnecting
        /// </summary>
        public const Int32 DISCONNECT = 12;
        
        /// <summary>
        /// Constante para el evento reconnecting
        /// </summary>
        public const Int32 RECONNECTING = 3;

        /// <summary>
        /// Constante para el evento adduser
        /// </summary>
        public const Int32 ADDUSER = 4;

        /// <summary>
        /// Constante para el evento removeuser
        /// </summary>
        public const Int32 REMOVEUSER = 5;

        /// <summary>
        /// Constante para el evento refreshuser
        /// </summary>
        public const Int32 REFRESHUSER = 6;

        /// <summary>
        /// Constante para el evento refreshlocaluser
        /// </summary>
        public const Int32 REFRESHLOCALUSER = 7;

        /// <summary>
        /// Constante para el evento net information
        /// </summary>
        public const Int32 NETINFORMATION = 8;

        /// <summary>
        /// Constante para el evento exception
        /// </summary>
        public const Int32 EXCEPTION = 9;

        /// <summary>
        /// Constante para el evento process subProtocol
        /// </summary>
        public const Int32 PROCESSMESSAGE = 10;

        /// <summary>
        /// Constante para el evento error subProtocol
        /// </summary>
        public const Int32 ERRORMESSAGE = 13;
    }

    /// <summary>
    /// Clase encargada de establecer la comunicación y el protocolo en la MANET
    /// </summary>
    public class Communication : CommHandlerI
    {
        /// <summary>
        /// Invoca threads de timer
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// Thread de timer
        /// </summary>
        private Thread timerThread;

        /// <summary>
        /// Punto de sincronización de detención
        /// </summary>
        private Int32 stopPoint;

        /// <summary>
        /// Punto de sincronización del timer
        /// </summary>
        private Int32 timerPoint;

        /// <summary>
        /// Punto de sincronización para el timer
        /// </summary>
        private Int32 timerWaitPoint;

        /// <summary>
        /// Thread de atencion de mensajes TCP
        /// </summary>
        private Thread tcpThread;

        /// <summary>
        /// Thread de atencion de mensajes UDP
        /// </summary>
        private Thread udpThreads;

        /// <summary>
        /// Thread de cola de mensajes a enviar
        /// </summary>
        private Thread messageThreads;

        /// <summary>
        /// Invoca threads de mochila
        /// </summary>
        private System.Timers.Timer bag;

        /// <summary>
        /// Thread de mochila
        /// </summary>
        private Thread bagThread;

        /// <summary>
        /// Punto de sincronización de mochila
        /// </summary>
        private Int32 bagPoint;

        /// <summary>
        /// Punto de sincronización para mochila
        /// </summary>
        private Int32 bagWaitPoint;

        /// <summary>
        /// Manejador de la red
        /// </summary>
        private NetHandler netHandler;

        /// <summary>
        /// Lista de usuarios de la red
        /// </summary>
        private NetUserList netUserList;

        /// <summary>
        /// Lock para lista de usuarios
        /// </summary>
        private Object userListLock;

        /// <summary>
        /// Datos de configuración
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Ruteador de mensajes
        /// </summary>
        private Router router;

        /// <summary>
        /// SubProtocolos
        /// </summary>
        private Hashtable subProtocols;

        /// <summary>
        /// Tipos extras de mensajes
        /// </summary>
        private Hashtable messageTypes;

        /// <summary>
        /// Declaración de delegados sin tipos
        /// </summary>
        public delegate void VoidEvent();

        /// <summary>
        /// Declaración de delagados que reciben un netUser
        /// </summary>
        /// <param name="netUser"></param>
        public delegate void NetUserEvent(NetUser netUser);

        /// <summary>
        /// Declaración de delegados que reciben un String
        /// </summary>
        /// <param name="aString"></param>
        public delegate void StringEvent(String aString);

        /// <summary>
        /// Declaración de delegados que reciben una excepción
        /// </summary>
        /// <param name="anException"></param>
        public delegate void ExceptionEvent(Exception anException);

        /// <summary>
        /// Declaración de delegados que reciben un mensaje
        /// </summary>
        /// <param name="anException"></param>
        public delegate void MessageEvent(Message anException);

        /// <summary>
        /// Se gatilla cuando el sistema se ha conectado a la MANET
        /// </summary>
        public event VoidEvent connectEvent;

        /// <summary>
        /// Se Gatilla cuando el sistema se ha desconectado de la MANET
        /// </summary>
        public event VoidEvent disconnectEvent;

        /// <summary>
        /// Se gatilla cuando el sistema está intentando conectarse a la MANET
        /// </summary>
        public event VoidEvent connectingEvent;

        /// <summary>
        /// Se gatilla cuando el sistema está intentando desconectarse de la MANET
        /// </summary>
        public event VoidEvent disconnectingEvent;

        /// <summary>
        /// Se gatilla cuando el sistema está intentando reconectarse a la MANET
        /// </summary>
        public event VoidEvent reconnectingEvent;

        /// <summary>
        /// Se gatilla cuando se ha conectado un nuevo usuario a la MANET
        /// </summary>
        public event NetUserEvent addUserEvent;

        /// <summary>
        /// Se gatilla cuando se ha desconectado un usuario de la MANET
        /// </summary>
        public event NetUserEvent removeUserEvent;

        /// <summary>
        /// Se gatilla cuando se han actualizado los datos de un usuario de la MANET
        /// </summary>
        public event NetUserEvent refreshUserEvent;

        /// <summary>
        /// Se gatilla cuando se han establecido o actualizado los datos del usuario local
        /// </summary>
        public event NetUserEvent refreshLocalUserEvent;

        /// <summary>
        /// Se gatilla cuando el sistema emite información relacionada con las funcionalidades (log)
        /// </summary>
        public event StringEvent netInformationEvent;

        /// <summary>
        /// Se gatilla cuando ha ocurrido un error en el sistema
        /// </summary>
        public event ExceptionEvent exceptionEvent;

        /// <summary>
        /// Se gatilla cuando se ha recibido un mensaje no manejable
        /// </summary>
        public event MessageEvent processMessageEvent;

        /// <summary>
        /// Se gatilla cuando no se ha podido enviar un mensaje no manejable
        /// </summary>
        public event MessageEvent errorMessageEvent;

        /// <summary>
        /// Cola de eventos
        /// </summary>
        private EventQueuePC eventQueuePC;

        /// <summary>
        /// Consumidor de eventos
        /// </summary>
        private Thread eventConsumer;

        /// <summary>
        /// Indica si el consumidor esta corriendo
        /// </summary>
        private bool eventConsumerStarted;

        /// <summary>
        /// Indica si el consumidor esta corriendo
        /// </summary>
        private Object eventConsumerLock;


        /// <summary>
        /// Constructo Parametrizado
        /// </summary>
        /// <param name="configuration">El objeto con los datos de configuración previamente llenado</param>
        /// <param name="subProtocols">Lista de SubProtocolos</param>
        /// <param name="extraMessageTypes">Tipos de mensajes no especificados en los sub protocolos</param>
        public Communication(Configuration configuration, SubProtocolList subProtocols, MessageTypeList extraMessageTypes)
        {
            try
            {
                this.configuration = configuration;
                if (extraMessageTypes != null)
                {
                    messageTypes = extraMessageTypes.Collection;
                }
                else
                {
                    messageTypes = new Hashtable();
                }
                if (subProtocols != null)
                {
                    Hashtable sub = subProtocols.Collection;
                    foreach (DictionaryEntry en in sub)
                    {
                        SubProtocolI subProtocol = (SubProtocolI)en.Value;
                        Hashtable spMessageTypes = subProtocol.getMessageTypes().Collection;
                        foreach (DictionaryEntry de in spMessageTypes)
                        {
                            messageTypes.Add(de.Key, de.Value);
                        }
                        subProtocol.sendMessageEvent += internalSendMessage;
                    }
                    this.subProtocols = sub;
                }
                netHandler = new NetHandler(configuration.NetData, this);
                eventQueuePC = new EventQueuePC();
                eventConsumer = new Thread(new ThreadStart(consumeEvent));
                eventConsumerStarted = false;
                eventConsumerLock = new Object();
                init();
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Datos de configuración
        /// </summary>
        public Configuration Configuration
        {
            get { return configuration; }
        }

        /// <summary>
        /// La lista de usuarios de la red
        /// </summary>
        public NetUserList NetUserList
        {
            get { return netUserList; }
        }

        /// <summary>
        /// Inicializa las variables locales
        /// </summary>
        private void init()
        {
            try
            {
                netUserList = new NetUserList();
                router = new Router(routerMessageErrorHandler, configuration.NetData.WaitForAck, messageTypes);
                stopPoint = 0;

                timerPoint = 0;
                timerWaitPoint = 0;
                timer = new System.Timers.Timer(configuration.NetData.TimeIntervalTimer);
                timer.Elapsed += new ElapsedEventHandler(communicationTimer);

                bagPoint = 0;
                bagWaitPoint = 0;
                bag = new System.Timers.Timer(configuration.NetData.TimeIntervalTimer);
                bag.Elapsed += new ElapsedEventHandler(communicationBag);

                tcpThread = new Thread(new ThreadStart(processTCPMessages));
                udpThreads = new Thread(new ThreadStart(processUDPMessages));
                messageThreads = new Thread(new ThreadStart(processNotSentMessages));

                userListLock = new Object();
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa el consumidor de eventos
        /// </summary>
        public void startEventConsumer()
        {
            lock (eventConsumerLock)
            {
                eventConsumer.Start();
                eventConsumerStarted = true; 
            }
        }

        /// <summary>
        /// Detiene el consumidor de eventos
        /// </summary>
        public void stopEventConsumer()
        {
            lock (eventConsumerLock)
            {
                eventConsumerStarted = false;
                try
                {
                    eventConsumer.Abort();
                }
                catch (Exception)
                {
                }
                try
                {
                    eventQueuePC.unblok();
                }
                catch (Exception)
                {
                }
                try
                {
                    eventConsumer.Join();
                }
                catch (Exception)
                {
                }
                eventQueuePC = new EventQueuePC();
            }
        }

        /// <summary>
        /// Consume un evento
        /// </summary>
        private void consumeEvent()
        {
            while (true)
            {
                Event eventHandler = eventQueuePC.draw();
                eventHandler.execute();
            }
        }

        /// <summary>
        /// Produce un evento
        /// </summary>
        /// <param name="eventType">el tipo del evento, un valor de CommunicationEvent</param>
        /// <param name="param">el parametro del evento</param>
        private void produceEvent(Int32 eventType, Object param)
        {
            if (eventConsumerStarted)
            {
                switch (eventType)
                {
                    case CommunicationEvent.ADDUSER:
                        {
                            if (addUserEvent != null)
                            {
                                eventQueuePC.put(new Event(addUserEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.CONNECT:
                        {
                            if (connectEvent != null)
                            {
                                eventQueuePC.put(new Event(connectEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.CONNECTING:
                        {
                            if (connectingEvent != null)
                            {
                                eventQueuePC.put(new Event(connectingEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.DISCONNECT:
                        {
                            if (disconnectEvent != null)
                            {
                                eventQueuePC.put(new Event(disconnectEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.DISCONNECTING:
                        {
                            if (disconnectingEvent != null)
                            {
                                eventQueuePC.put(new Event(disconnectingEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.ERRORMESSAGE:
                        {
                            if (errorMessageEvent != null)
                            {
                                eventQueuePC.put(new Event(errorMessageEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.EXCEPTION:
                        {
                            if (exceptionEvent != null)
                            {
                                eventQueuePC.put(new Event(exceptionEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.NETINFORMATION:
                        {
                            if (netInformationEvent != null)
                            {
                                eventQueuePC.put(new Event(netInformationEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.PROCESSMESSAGE:
                        {
                            if (processMessageEvent != null)
                            {
                                eventQueuePC.put(new Event(processMessageEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.RECONNECTING:
                        {
                            if (reconnectingEvent != null)
                            {
                                eventQueuePC.put(new Event(reconnectingEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.REFRESHLOCALUSER:
                        {
                            if (refreshLocalUserEvent != null)
                            {
                                eventQueuePC.put(new Event(refreshLocalUserEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.REFRESHUSER:
                        {
                            if (refreshUserEvent != null)
                            {
                                eventQueuePC.put(new Event(refreshUserEvent, param));
                            }
                            break;
                        }
                    case CommunicationEvent.REMOVEUSER:
                        {
                            if (removeUserEvent != null)
                            {
                                eventQueuePC.put(new Event(removeUserEvent, param));
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Inicia el proceso de conección a la MANET
        /// Este método no es bloqueante, se ejecuta en un Thread separado
        /// </summary>
        public void connect()
        {
            try
            {
                produceEvent(CommunicationEvent.CONNECTING, null);
                netHandler.connect();
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                disconnect();
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Inicia el proceso de desconección a la MANET
        /// Este método es bloqueante, el método retorna cuando se haya desconectado completamente de la MANET
        /// </summary>
        public void disconnect()
        {
            try
            {
                produceEvent(CommunicationEvent.DISCONNECTING, null);
                netHandler.disconnect();
                produceEvent(CommunicationEvent.DISCONNECT, null);
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Inicia el proceso de desconección a la MANET
        /// Este método no es bloqueante, se ejecuta en un Thread separado
        /// </summary>
        public void disconnectAsync()
        {
            try
            {
                //TODO posible threaed fuera de control
                Thread disThread = new Thread(new ThreadStart(disconnect));
                disThread.Start();
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Se gatilla cuando se ha formado la red. Da comienzo a la comunicación
        /// Este método es para uso interno, no debe ser llamado
        /// </summary>
        public void startNetworkingHandler()
        {
            try
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication...");
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: set IP");
                configuration.NetUser.Ip = netHandler.NetData.IpTcpListener;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: set user ID");
                configuration.NetUser.pickNewId();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: router config");
                updateRouter();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer on");
                timer.Start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection on");
                udpThreads.Start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process on");
                tcpThread.Start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages on");
                messageThreads.Start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag on");
                bag.Start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication... ok!");
                produceEvent(CommunicationEvent.CONNECT, null);
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication... failed! " + e.Message);
            }
        }

        /// <summary>
        /// Se gatilla cuando se ha detenido la red. Da termino a la comunicación.
        /// Este método es para uso interno, no debe ser llamado
        /// </summary>
        public void stopNetworkingHandler()
        {
            if (Interlocked.CompareExchange(ref stopPoint, 1, 0) == 0)
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: stop Communication...");
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer off");
                    timer.Stop();
                    try
                    {
                        timerThread.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    //bloquea la ejecución hasta que se detengan los eventos generados por timer
                    //solo si no es gatillado por excepcion en el proceso del timer para evitar dead lock
                    if (Interlocked.CompareExchange(ref timerWaitPoint, 0, 1) != 1)
                    {
                        int timeOut = 0;
                        while (Interlocked.CompareExchange(ref timerPoint, -1, 0) != 0)
                        {
                            Thread.Sleep(configuration.NetData.TimeIntervalTimer);
                            timeOut++;
                            if (timeOut > configuration.NetData.WaitForTimerClose)
                            {
                                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: impossible to wait for timer");
                                break;
                            }
                        }
                    }
                    timer.Dispose();
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer off error " + e.Message);
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages off");
                    try
                    {
                        messageThreads.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        router.NotSentMessageQueue.unblok();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        messageThreads.Join();
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages off error " + e.Message);
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection off");
                    try
                    {
                        udpThreads.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        netHandler.UdpMessageQueue.unblok();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        udpThreads.Join();
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection off error " + e.Message);
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process off");
                    try
                    {
                        tcpThread.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        netHandler.TcpMessageQueue.unblok();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        tcpThread.Join();
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process off error " + e.Message);
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag off");
                    bag.Stop();
                    try
                    {
                        bagThread.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    if (Interlocked.CompareExchange(ref bagWaitPoint, 0, 1) != 1)
                    {
                        int timeOut = 0;
                        while (Interlocked.CompareExchange(ref bagPoint, -1, 0) != 0)
                        {
                            Thread.Sleep(configuration.NetData.TimeIntervalTimer);
                            timeOut++;
                            if (timeOut > configuration.NetData.WaitForTimerClose)
                            {
                                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNUCATION WARNING: impossible to wait for bag");
                                break;
                            }
                        }
                    }
                    bag.Dispose();
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag off error " + e.Message);
                }

                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION,
                        "COMMUNICATION: messages stats " +
                        " send: " + router.NMessagesSent +
                        " confirmed: " + router.NMessagesConfirmed +
                        " destroy: " + router.NMessagesDestroyed +
                        " warnings: " + router.NMessagesDroped +
                        " failed: " + router.NMessagesFailed +
                        " received: " + router.NMessagesReceived +
                        " resent: " + router.NMessagesReplayed +
                        " routed: " + router.NMessagesRouted +
                    "");
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages stats error " + e.Message);
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: re init");
                    init();
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: re init error " + e.Message);
                }
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: stop communication... ok!");
                stopPoint = 0;
            }
        }

        /// <summary>
        /// Se gatilla cuando se resetea la conexión
        /// Este método es para uso interno, no debe ser llamado
        /// </summary>
        public void resetNetworkingHandler()
        {
            produceEvent(CommunicationEvent.RECONNECTING, null);
        }

        /// <summary>
        /// Se gatilla cuando ha ocurrido un error en la red
        /// Este método es para uso interno, no debe ser llamado
        /// </summary>
        /// <param name="e">la excepción generada en la red</param>
        public void errorNetworkingHandler(Exception e)
        {
            produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: network error");
            Thread disconnectThread = new Thread(new ThreadStart(disconnect));
            disconnectThread.Start();
            produceEvent(CommunicationEvent.EXCEPTION, e);
        }

        /// <summary>
        /// Se gatilla cuando la red envia algun mensage del estado
        /// Este método es para uso interno, no debe ser llamado
        /// </summary>
        /// <param name="message">el mensaje generado</param>
        public void informationNetworkingHandler(String message)
        {
            produceEvent(CommunicationEvent.NETINFORMATION, message);
        }

        /// <summary>
        /// Función invocada cada 1 segundos por el timer, invoca un thread de interación de timer
        /// </summary>
        /// <param name="sender">El sender</param>
        /// <param name="eventArgs">Los parámetros del evento</param>
        private void communicationTimer(Object sender, ElapsedEventArgs eventArgs)
        {
            if (Interlocked.CompareExchange(ref timerPoint, 1, 0) == 0)
            {
                timerThread = new Thread(new ThreadStart(communicationTimerInteration));
                timerThread.Start();
                timerThread.Join();
                timerPoint = 0;
            }
        }

        /// <summary>
        /// Timer: 
        /// Actualiza los datos y objetos de comunicación según el estado de la MANET
        /// Procesa los mensajes UDP
        /// Envía mensaje ImAlive
        /// </summary>
        private void communicationTimerInteration()
        {
            try
            {
                //Se checkea la lista de usuarios
                //controlHandler.netInformationHandler("processUdpMesages ... updateUserList");
                updateUserList();
                //Se actualiza el vecindario
                //controlHandler.netInformationHandler("processUdpMesages ... updateNeighborhood");
                updateNeighborhood();
                //Se actualizan los parametros del router
                //controlHandler.netInformationHandler("processUdpMesages ... updateRouter");
                updateRouter();
                //actualiza el estado
                //controlHandler.netInformationHandler("processUdpMesages ... updateState");
                updateState();

                //Se envía el mensaje de que este usuario está vivo
                internalSendMessage(new ImAliveMessage());
            }
            catch (Exception ex)
            {
                if (Interlocked.CompareExchange(ref stopPoint, 0, 0) == 0)
                {
                    timerWaitPoint = 1;
                    disconnect();
                    produceEvent(CommunicationEvent.EXCEPTION, ex);
                }
            }
        }

        /// <summary>
        /// Process:
        /// Procesa los mensajes TCP recibidos
        /// </summary>
        private void processTCPMessages()
        {
            try
            {
                while (true)
                {
                    NetMessage netMessage = netHandler.TcpMessageQueue.draw();
                    proccessMessage(router.attendMessage(netMessage));
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                disconnect();
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Process:
        /// Procesa los mensajes UDP recibidos
        /// </summary>
        private void processUDPMessages()
        {
            try
            {
                while (true)
                {
                    NetMessage netMessage = netHandler.UdpMessageQueue.draw();
                    proccessMessage(router.attendMessage(netMessage));
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                disconnect();
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Process:
        /// Procesa los mensajes UDP recibidos
        /// </summary>
        private void processNotSentMessages()
        {
            try
            {
                while (true)
                {
                    router.proccessNotSentMessage();
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                disconnect();
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /// <summary>
        /// Función invocada cada 1 segundos por el bag, inicializa un thread de interación de bag
        /// </summary>
        /// <param name="sender">El sender</param>
        /// <param name="eventArgs">Los parametros del evento</param>
        private void communicationBag(Object sender, ElapsedEventArgs eventArgs)
        {
            if (Interlocked.CompareExchange(ref bagPoint, 1, 0) == 0)
            {
                bagThread = new Thread(new ThreadStart(communicationBagIteration));
                bagThread.Start();
                bagThread.Join();
                bagPoint = 0;
            }
        }

        /// <summary>
        /// Bag:
        /// Procesa los mensajes envíados y administrados por el router
        /// </summary>
        private void communicationBagIteration()
        {
            try
            {
                processRouterMesages();
            }
            catch (Exception ex)
            {
                if (Interlocked.CompareExchange(ref stopPoint, 0, 0) == 0)
                {
                    bagWaitPoint = 1;
                    disconnect();
                    produceEvent(CommunicationEvent.EXCEPTION, ex);
                }
            }
        }



        /// <summary>
        /// Actualiza el estado de la comunicación (tráfico)
        /// </summary>
        private void updateState()
        {
            if (router.NotSentMessageQueue.size() > configuration.NetData.StateCritical || router.FailedMessagesSize > configuration.NetData.StateCritical || netHandler.TcpMessageQueue.size() > configuration.NetData.StateCritical)
            {
                configuration.NetUser.State = CommunicationQuality.CRITICAL;
            }
            else if (router.NotSentMessageQueue.size() > configuration.NetData.StateOverloaded || router.FailedMessagesSize > configuration.NetData.StateOverloaded || netHandler.TcpMessageQueue.size() > configuration.NetData.StateOverloaded)
            {
                configuration.NetUser.State = CommunicationQuality.OVERLOADED;
            }
            else
            {
                configuration.NetUser.State = CommunicationQuality.NORMAL;
            }
        }

        /// <summary>
        /// Actualiza los parámetros del router
        /// </summary>
        private void updateRouter()
        {
            router.updateRouterTable(netHandler, configuration.NetUser, netUserList);
            produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: max list size: " + router.MaxListSize + " used: " + router.ListSize);
        }

        /// <summary>
        /// Actualiza los datos de la lista de usuarios
        /// </summary>
        private void updateUserList()
        {
            NetUser[] netUsers = netUserList.userListToArray();
            for (int i = 0; i < netUsers.Length; i++)
            {
                //decrementa el time out, para algoritmo de perdida de señal
                netUsers[i].qualityDown(configuration.NetData);
                //si llega a cero el time out, se ha ido de la red, hay que sacarlo
                if (netUsers[i].Timeout < 0)
                {
                    disconnectNetUser(netUsers[i]);
                }
                else
                {
                    if (netUsers[i].SignalQuality == NetUserQuality.LOW || netUsers[i].SignalQuality == NetUserQuality.NORMAL)
                    {
                        RemoteMachine remoteMachine = netHandler.TcpServerList.getRemoteMachine(netUsers[i].Ip);
                        if (remoteMachine == null && netUsers[i].JumpsAway == 1)
                        {
                            if (netUsers[i].WaitTimeOut <= 0)
                            {
                                netUsers[i].WaitTimeOut = configuration.NetData.WaitForTCPConnection;
                                netHandler.connectTo(netUsers[i].Ip);
                            }
                            else
                            {
                                netUsers[i].WaitTimeOut--;
                            }
                        }
                    }
                    produceEvent(CommunicationEvent.REFRESHUSER, netUsers[i]);
                }
            }
        }

        /// <summary>
        /// Actualiza la vecindad TCP
        /// </summary>
        private void updateNeighborhood()
        {
            List<Guid> ids = new List<Guid>();
            RemoteMachine[] serverMachines = netHandler.TcpServerList.toObjectArray();
            for (int i = 0; i < serverMachines.Length; i++)
            {
                NetUser serverNetUser = netUserList.getUser(serverMachines[i].Ip);
                if (serverNetUser != null)
                {
                    ids.Add(serverNetUser.Id);
                }
            }
            configuration.NetUser.NeighborhoodIds = ids.ToArray();
            produceEvent(CommunicationEvent.REFRESHLOCALUSER, configuration.NetUser);
        }

        /// <summary>
        /// Procesa los mensajes administrados por el router
        /// </summary>
        private void processRouterMesages()
        {
            router.proccessFailedMessages();
            router.proccessNotConfirmedMessages();
        }

        /// <summary>
        /// Agrega un nuevo usuario de la red a la lista
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        private void newNetUser(NetUser netUser)
        {
            lock (userListLock)
            {
                if (!netUser.Id.Equals(configuration.NetUser.Id))
                {
                    NetUser newNetUser = new NetUser(netUser.Id, netUser.Name, netUser.Ip, netUser.NeighborhoodIds, configuration.NetData);
                    newNetUser.State = netUser.State;
                    newNetUser.UpLayerData = netUser.UpLayerData;
                    newNetUser.JumpsAway = netUser.JumpsAway;
                    if (netUserList.add(netUser.Ip, newNetUser))
                    {
                        produceEvent(CommunicationEvent.ADDUSER, newNetUser);
                    }
                    else
                    {
                        produceEvent(CommunicationEvent.REFRESHUSER, newNetUser);
                    }
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: [" + netUser.Name + "] connected");
                }
            }
        }

        /// <summary>
        /// Elimina a un usuario de la red de la lista de usuarios
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        private void disconnectNetUser(NetUser netUser)
        {
            lock (userListLock)
            {
                netUserList.remove(netUser.Ip);
                produceEvent(CommunicationEvent.REMOVEUSER, netUser);
                netHandler.disconnectFrom(netUser.Ip);
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: [" + netUser.Name + "] disconnected");
            }
        }

        /// <summary>
        /// Procesa un mensaje recibido
        /// </summary>
        /// <param name="message">El mensaje recibido</param>
        private void proccessMessage(Message message)
        {
            if (message != null)
            {
                //registra la ip del mensaje recibido en el manejador de Ips, previo chequeo de que el mensaje no es de 
                //este mismo usuario
                if (!message.SenderNetUser.Id.Equals(configuration.NetUser.Id))
                {
                    netHandler.registerIp(message.SenderNetUser.Ip);
                }

                //verifica que se recibe el mensaje de un usuario conocido
                NetUser listedNetUser = netUserList.getUser(message.SenderNetUser.Ip);

                //ImAliveMessage
                if (message.Type == MessageType.IMALIVE)
                {
                    if (listedNetUser == null)
                    {
                        newNetUser(message.SenderNetUser);
                    }
                    else if (!listedNetUser.Id.Equals(message.SenderNetUser.Id))
                    {
                        disconnectNetUser(listedNetUser);
                    }
                    else
                    {
                        listedNetUser.qualityUp(configuration.NetData);
                        listedNetUser.NeighborhoodIds = message.SenderNetUser.NeighborhoodIds;
                        listedNetUser.JumpsAway = message.Jumps;
                        listedNetUser.State = message.SenderNetUser.State;
                        listedNetUser.UpLayerData = message.SenderNetUser.UpLayerData;
                    }
                    //produceEvent(CommunicationEvent.NETINFORMATION, "recibido: " + subProtocol.ToString());
                }
                //AckMessage
                else if (message.Type == MessageType.ACK)
                {
                    AckMessage ackMessage = (AckMessage)message;
                    router.proccessAckMessage(ackMessage);
                    //produceEvent(CommunicationEvent.NETINFORMATION, "recibidos: " + subProtocol.ToString());
                }
                //Resto de los mensajes
                else
                {
                    if (listedNetUser != null && listedNetUser.Id.Equals(message.SenderNetUser.Id))
                    {
                        message.SenderNetUser = listedNetUser;
                        SubProtocolI subProtocol = (SubProtocolI)subProtocols[message.ProtocolType];
                        if (subProtocol != null)
                        {
                            eventQueuePC.put(new Event(new MessageEvent(subProtocol.proccessMessage), message));
                        }
                        else
                        {
                            produceEvent(CommunicationEvent.PROCESSMESSAGE, message);
                        }
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: received " + message.ToString());
                    }
                    else
                    {
                        message.SenderNetUser.Name = "Unknown (" + message.SenderNetUser.Ip.ToString() + ")";
                        SubProtocolI subProtocol = (SubProtocolI)subProtocols[message.ProtocolType];
                        if (subProtocol != null)
                        {
                            eventQueuePC.put(new Event(new MessageEvent(subProtocol.proccessMessage), message));
                        }
                        else
                        {
                            produceEvent(CommunicationEvent.PROCESSMESSAGE, message);
                        }
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: unknown message  " + message.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Procesa un mensaje que no ha podido ser envíado correctamente
        /// </summary>
        /// <param name="message">El mensaje</param>
        internal void routerMessageErrorHandler(Message message)
        {
            //ImAliveMessage
            if (message.Type == MessageType.IMALIVE)
            {
                ImAliveMessage imAliveMessage = (ImAliveMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + imAliveMessage.ToString());
            }
            //AckMessage
            else if (message.Type == MessageType.ACK)
            {
                AckMessage ackMessage = (AckMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + ackMessage.ToString());
            }
            //Resto de los mensajes
            else
            {
                SubProtocolI subProtocol = (SubProtocolI)subProtocols[message.ProtocolType];
                if (subProtocol != null)
                {
                    eventQueuePC.put(new Event(new MessageEvent(subProtocol.errorMessage), message)); 
                }
                else
                {
                    produceEvent(CommunicationEvent.ERRORMESSAGE, message);
                }
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + message.ToString());
            }
        }

        /// <summary>
        /// Envía un mensaje a la MANET
        /// </summary>
        /// <param name="message">el mensaje a enviar</param>
        public void send(Message message)
        {
            //ImAliveMessage
            if (message.Type == MessageType.IMALIVE)
            {
                ImAliveMessage imAliveMessage = (ImAliveMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: reserved kind of message " + imAliveMessage.ToString());
            }
            //AckMessage
            else if (message.Type == MessageType.ACK)
            {
                AckMessage ackMessage = (AckMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: reserved kind of message " + ackMessage.ToString());
            }
            //Resto de los mensajes
            else
            {
                internalSendMessage(message);
            }
        }

        /// <summary>
        /// Envía un mensaje de forma interna aplicando filtros
        /// </summary>
        /// <param name="message">el mensaje a enviar</param>
        internal void internalSendMessage(Message message)
        {
            try
            {
                if (configuration.NetUser.Ip != null)
                {
                    message.SenderNetUser = configuration.NetUser;
                    router.queueMessageToSend(message);
                    if (message.Type != MessageType.IMALIVE && message.Type != MessageType.ACK)
                    {
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: queued " + message.ToString());
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: impossible to send this message " + e.Message);
            }
        }
    }
}
/*
 * 
 * TODO: 30 segundos de tiempo limite

 * TODO internalSendMessaje debe asociar los saltos del receiberUser porque se saca del sender
 * 
 * TODO windows Vista mata el programa y muere la interfaz de red
 * 
 * TODO en la espera de la descarga crear un algoritmo que ordene la cola de prioridad segun el lugar en la cola de descarga de los usuarios remotos
 * 
 * TODO aveces la animacion del icono de estado se detiene en win vista
 *  
 * TODO al cambiar la IP no borrar la cola de mensajes, ni la lista de ids del router
 * 
 * TODO Optimizar busquedas y funciones 
 * 
//si ocurre Ip duplicada llamar a la funcion que elimina una IP del sistema operativo para que deje de aparecer el error (DeleteIPAddress Function)

On Windows Server 2003, Windows XP, and Windows 2000, if the IPv4 address in the Address parameter already exists on the network, the AddIPAddress function returns NO_ERROR and the IPv4 address added is 0.0.0.0.

On Windows Vista and later, if the IPv4 address passed in the Address parameter already exists on the network, the AddIPAddress function returns NO_ERROR and the duplicate IPv4 address is added with the IP_DAD_STATE member in the IP_ADAPTER_UNICAST_ADDRESS structure set to IpDadStateDuplicate. 

 * 
 * 
 * Found out what our problem was.  It was to do with the actual multicast group address used (the problem with source addresss 0.0.0.0 prompted me to look at the packets again).  We had been using 236.0.0.1 (reserved).  Switching to 224.0.0.x fixes the issue in Vista.  I guess the new protocol stack is picky about the actually address range whereas NT/2000/XP is not.
 * 
 * 
 * I had this problem while developing a simple java multicasting program for a university assignment. The application would just hang with three classes acting as devices trying to multicast their address location. The device classes have a source address as 0.0.0.0. All I did to correct the problem is turn of the IPV6 protocol in the LAN connection properties.
 * 
 */