package hlmp.CommLayer;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.UUID;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.atomic.AtomicInteger;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Constants.*;
import hlmp.CommLayer.Exceptions.ArgumentOutOfRangeException;
import hlmp.CommLayer.Interfaces.RouterMessageErrorDelegateI;
import hlmp.CommLayer.Messages.*;
import hlmp.CommLayer.Observers.*;
import hlmp.NetLayer.*;

/**
 * Clase encargada de establecer la comunicación y el protocolo en la MANET
 */
public class Communication implements CommHandlerI, RouterMessageErrorDelegateI{

	/**
	 * Invoca threads de timer
	 */
	private Timer timer;

	/**
	 * Thread de timer
	 */
	private Thread timerThread;

	/**
	 * Punto de sincronización de detención
	 */
	private AtomicInteger stopPoint;

	/**
	 * Punto de sincronización del timer
	 */
	private AtomicInteger timerPoint;

	/**
	 * Punto de sincronización para el timer
	 */
	private AtomicInteger timerWaitPoint;

	/**
	 * Thread de atencion de mensajes TCP
	 */
	private Thread tcpThread;

	/**
	 * Thread de atencion de mensajes UDP
	 */
	private Thread udpThreads;

	/**
	 * Thread de cola de mensajes a enviar
	 */
	private Thread messageThreads;

	/**
	 * Invoca threads de mochila
	 */
	private Timer bag;

	/**
	 * Thread de mochila
	 */
	private Thread bagThread;

	/**
	 * Punto de sincronización de mochila
	 */
	private AtomicInteger bagPoint;

	/**
	 * Punto de sincronización para mochila
	 */
	private AtomicInteger bagWaitPoint;

	/**
	 * Manejador de la red
	 */
	private NetHandler netHandler;

	/**
	 * Lista de usuarios de la red
	 */
	private NetUserList netUserList;

	/**
	 * Lock para lista de usuarios
	 */
	private Object userListLock;

	/**
	 * Datos de configuración
	 */
	private Configuration configuration;

	/**
	 * Ruteador de mensajes
	 */
	private Router router;

	/**
	 * SubProtocolos
	 */
	private SubProtocolList subProtocols;

	/**
	 * Tipos extras de mensajes
	 */
	private HashMap<Integer,Message> messageTypes;

	/**
	 * Se gatilla cuando el sistema se ha conectado a la MANET
	 */
	private Hashtable<Integer,VoidEventObserverI> connectEventObserverList;

	/**
	 * Se Gatilla cuando el sistema se ha desconectado de la MANET
	 */
	private Hashtable<Integer,VoidEventObserverI> disconnectEventObserverList;

	/**
	 * Se gatilla cuando el sistema está intentando conectarse a la MANET
	 */
	private Hashtable<Integer,VoidEventObserverI> connectingEventObserverList;

	/**
	 * Se gatilla cuando el sistema está intentando desconectarse de la MANET
	 */
	private Hashtable<Integer,VoidEventObserverI> disconnectingEventObserverList;

	/**
	 * Se gatilla cuando el sistema está intentando reconectarse a la MANET
	 */
	private Hashtable<Integer,VoidEventObserverI> reconnectingEventObserverList;

	/**
	 * Se gatilla cuando se ha conectado un nuevo usuario a la MANET
	 */
	private Hashtable<Integer,NetUserEventObserverI> addUserEventObserverList;

	/**
	 * Se gatilla cuando se ha desconectado un usuario de la MANET
	 */
	private Hashtable<Integer,NetUserEventObserverI> removeUserEventObserverList;

	/**
	 * Se gatilla cuando se han actualizado los datos de un usuario de la MANET
	 */
	private Hashtable<Integer,NetUserEventObserverI> refreshUserEventObserverList;

	/**
	 * Se gatilla cuando se han establecido o actualizado los datos del usuario local
	 */
	private Hashtable<Integer,NetUserEventObserverI> refreshLocalUserEventObserverList;

	/**
	 * Se gatilla cuando el sistema emite información relacionada con las funcionalidades (log)
	 */
	private Hashtable<Integer,StringEventObserverI> netInformationEventObserverList;

	/**
	 * Se gatilla cuando ha ocurrido un error en el sistema
	 */
	private Hashtable<Integer,ExceptionEventObserverI> exceptionEventObserverList;

	/**
	 * Se gatilla cuando se ha recibido un mensaje no manejable
	 */
	private Hashtable<Integer,MessageEventObserverI> processMessageEventObserverList;

	/**
	 * Se gatilla cuando no se ha podido enviar un mensaje no manejable
	 */
	private Hashtable<Integer,MessageEventObserverI> errorMessageEventObserverList;

	/**
	 * Cola de eventos
	 */
	private EventQueuePC eventQueuePC;

	/**
	 * Consumidor de eventos
	 */
	private Thread eventConsumer;

	/**
	 * Indica si el consumidor esta corriendo
	 */
	private boolean eventConsumerStarted;

	/**
	 * Indica si el consumidor esta corriendo
	 */
	private Object eventConsumerLock;


	/**
	 * Constructor Parametrizado
	 * @param configuration El objeto con los datos de configuración previamente llenado
	 * @param subProtocols Lista de SubProtocolos
	 * @param extraMessageTypes Tipos de mensajes no especificados en los sub protocolos
	 */
	//LISTO
	public Communication(Configuration configuration, SubProtocolList subProtocols, MessageTypeList extraMessageTypes)
	{
//		try
//		{
			this.configuration = configuration;
			if (extraMessageTypes != null)
			{
				messageTypes = extraMessageTypes.getCollection();
			}
			else
			{
				messageTypes = new HashMap<Integer, Message>();
			}
			if (subProtocols != null)
			{
				Collection<SubProtocol> sub = subProtocols.getCollection().values();
				for (SubProtocol subProtocol : sub)
				{
					messageTypes.putAll(subProtocol.getMessageTypes().getCollection());
					subProtocol.setComm(this);
				}
				this.subProtocols = subProtocols;
			}
			netHandler = new NetHandler(configuration.getNetData(), this);
			eventQueuePC = new EventQueuePC();
			eventConsumer = consumeEvent();
			eventConsumerStarted = false;
			eventConsumerLock = new Object();
			init();
//		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
//		catch (Exception e)
//		{
//			throw e;
//		}
	}

	/**
	 * Datos de configuración
	 */
	public Configuration getConfiguration() {
		return configuration;
	}


	/**
	 * La lista de usuarios de la red
	 */
	public NetUserList getNetUserList() {
		return netUserList;
	}

	/**
	 * Inicializa las variables locales
	 */
	private void init()
	{
		// listas de observers
		this.connectEventObserverList = new Hashtable<Integer, VoidEventObserverI>();
		this.disconnectEventObserverList = new Hashtable<Integer, VoidEventObserverI>();
		this.connectingEventObserverList = new Hashtable<Integer, VoidEventObserverI>();
		this.disconnectingEventObserverList = new Hashtable<Integer, VoidEventObserverI>();
		this.reconnectingEventObserverList = new Hashtable<Integer, VoidEventObserverI>();

		this.addUserEventObserverList = new Hashtable<Integer,NetUserEventObserverI>();
		this.removeUserEventObserverList = new Hashtable<Integer,NetUserEventObserverI>();
		this.refreshUserEventObserverList = new Hashtable<Integer,NetUserEventObserverI>();
		this.refreshLocalUserEventObserverList = new Hashtable<Integer,NetUserEventObserverI>();

		this.netInformationEventObserverList = new Hashtable<Integer, StringEventObserverI>();
		this.exceptionEventObserverList = new Hashtable<Integer, ExceptionEventObserverI>();

		this.processMessageEventObserverList = new Hashtable<Integer, MessageEventObserverI>();
		this.errorMessageEventObserverList = new Hashtable<Integer, MessageEventObserverI>();


//		try
//		{
			netUserList = new NetUserList();
			router = new Router(this, configuration.getNetData().getWaitForAck(), messageTypes);
			stopPoint = new AtomicInteger(0);

			timerPoint = new AtomicInteger(0);
			timerWaitPoint = new AtomicInteger(0);
//			timer = new System.Timers.Timer(configuration.NetData.TimeIntervalTimer);
//			timer.Elapsed += new ElapsedEventHandler(communicationTimer);

			bagPoint = new AtomicInteger(0);
			bagWaitPoint = new AtomicInteger(0);
//			bag = new System.Timers.Timer(configuration.NetData.TimeIntervalTimer);
//			bag.Elapsed += new ElapsedEventHandler(communicationBag);

			tcpThread = processTCPMessages();
			tcpThread.setName("TCP Communication Thread");
			udpThreads = processUDPMessages();
			udpThreads.setName("UDP Communication Thread");
			messageThreads = processNotSentMessages();
			messageThreads.setName("Message Communication Thread");

			userListLock = new Object();
//		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
//		catch (Exception e)
//		{
//			throw e;
//		}
	}

		/**
		 * Inicializa el timer
		 */
		private void timerStart(){
			timer = new Timer();
			timer.schedule(getCommunicationTimerTask(), 0, configuration.getNetData().getTimeIntervalTimer());
		}
		/**
		 * Inicializa el bag
		 */
		private void bagStart(){
			bag = new Timer();
			bag.schedule(getCommunicationBagTimerTask(), 0, configuration.getNetData().getTimeIntervalTimer());
		}

        /**
         * Inicializa el consumidor de eventos
         */
		//LISTO
        public void startEventConsumer()
        {
            synchronized (eventConsumerLock) {
            	eventConsumer.start();
                eventConsumerStarted = true;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: EventConsumer started");
			}
        }

        /**
         * Detiene el consumidor de eventos
         */
        //LISTO
        public void stopEventConsumer()
        {
            synchronized (eventConsumerLock)
            {
                eventConsumerStarted = false;
                try
                {
                    eventConsumer.interrupt();
                }
                catch (Exception e)
                {
                }
                try
                {
                    eventQueuePC.unblok();
                }
                catch (Exception e)
                {
                }
                try
                {
                    eventConsumer.join();
                }
                catch (Exception e)
                {
                }
                eventQueuePC = new EventQueuePC();
            }
        }

        /**
         * Genera el Thread que consume un evento
         */
      //LISTO
        private Thread consumeEvent()
        {
        	Thread t= new Thread(){

        		public void run(){
        			while (true)
        			{
        				Event event = eventQueuePC.draw();
        				switch (event.getEventType())
        				{
	        				case CommunicationEvent.ADDUSER:
	        				{
	        					notifyAddUserEventObservers((NetUser) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.CONNECT:
	        				{
	        					notifyConnectEventObservers();
	        					break;
	        				}
	        				case CommunicationEvent.CONNECTING:
	        				{
	        					notifyConnectingEventObservers();
	        					break;
	        				}
	        				case CommunicationEvent.DISCONNECT:
	        				{
	        					notifyDisconnectEventObservers();
	        					break;
	        				}
	        				case CommunicationEvent.DISCONNECTING:
	        				{
	        					notifyDisconnectingEventObservers();
	        					break;
	        				}
	        				case CommunicationEvent.ERRORMESSAGE:
	        				{
	        					notifyErrorMessageEventObservers((Message) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.EXCEPTION:
	        				{
	        					notifyExceptionEventObservers((Exception) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.NETINFORMATION:
	        				{
	        					notifyNetInformationEventObservers((String) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.PROCESSMESSAGE:
	        				{
	        					notifyProcessMessageEventObservers((Message) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.RECONNECTING:
	        				{
	        					notifyReconnectingEventObservers();
	        					break;
	        				}
	        				case CommunicationEvent.REFRESHLOCALUSER:
	        				{
	        					notifyRefreshLocalUserEventObservers((NetUser) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.REFRESHUSER:
	        				{
	        					notifyRefreshUserEventObservers((NetUser) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.REMOVEUSER:
	        				{
	        					notifyRemoveUserEventObservers((NetUser) event.getParam());
	        					break;
	        				}
	        				case CommunicationEvent.SUBPROTOCOLPROCESSMESSAGE:
	        				{
	        					Object[] arg = (Object[]) event.getParam();
	        					SubProtocol subProtocol = (SubProtocol) arg[0];
	        					Message message = (Message) arg[1];
	        					subProtocol.proccessMessage(message);
	        					break;
	        				}
	        				case CommunicationEvent.SUBPROTOCOLERRORMESSAGE:
	        				{
	        					Object[] arg = (Object[]) event.getParam();
	        					SubProtocol subProtocol = (SubProtocol) arg[0];
	        					Message message = (Message) arg[1];
	        					subProtocol.errorMessage(message);
	        					break;
	        				}
        				}
        			}
        		}
        	};
        	return t;

        }

        /**
         * Produce un evento
         * @param eventType el tipo del evento, un valor de CommunicationEvent
         * @param param el parametro del evento
         */
        //LISTO
        private void produceEvent(int eventType, Object param)
        {
            if (eventConsumerStarted)
            {
            	eventQueuePC.put(new Event(eventType, param));
            }
        }

        /**
         * Inicia el proceso de conección a la MANET
         * Este método no es bloqueante, se ejecuta en un Thread separado
         */
        //LISTO
        public void connect()
        {
            try
            {
                produceEvent(CommunicationEvent.CONNECTING, null);
                netHandler.connect();
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
            	e.printStackTrace();
//                disconnect();
//                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /**
         * Inicia el proceso de desconección a la MANET
         * Este método es bloqueante, el método retorna cuando se haya desconectado completamente de la MANET
         */
        //LISTO
        public void disconnect()
        {
            try
            {
                produceEvent(CommunicationEvent.DISCONNECTING, null);
                netHandler.disconnect();
                produceEvent(CommunicationEvent.DISCONNECT, null);
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /**
         * Inicia el proceso de desconección a la MANET
         * Este método no es bloqueante, se ejecuta en un Thread separado
         */
      //LISTO
        public void disconnectAsync()
        {
            try
            {
                //TODO posible threaed fuera de control
                Thread disThread = new Thread(){
                	public void run(){
                		disconnect();
                	}
                };
                disThread.start();
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.EXCEPTION, e);
            }
        }

        /**
         * Se gatilla cuando se ha formado la red. Da comienzo a la comunicación
         * Este método es para uso interno, no debe ser llamado
         */
        //LISTO
        public void startNetworkingHandler()
        {
            try
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication...");
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: set IP");
                configuration.getNetUser().setIp(netHandler.getNetData().getIpTcpListener());
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: set user ID");
                configuration.getNetUser().pickNewId();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: router config");
                updateRouter();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer on");
                timerStart();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection on");
                udpThreads.start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process on");
                tcpThread.start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages on");
                messageThreads.start();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag on");
                bagStart();
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication... ok!");
                produceEvent(CommunicationEvent.CONNECT, null);
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: start communication... failed! " + e.getMessage());
            }
        }

        /**
         * Se gatilla cuando se ha detenido la red. Da termino a la comunicación.
         * Este método es para uso interno, no debe ser llamado
         */
        //TODO
        public void stopNetworkingHandler()
        {
            if (stopPoint.compareAndSet(0, 1))
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: stop Communication...");
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer off");
                    timer.cancel();
//                    try
//                    {
                        timerThread.interrupt();
//                    }
//                    catch (InterruptedException e)
//                    {
//                    }
                    //bloquea la ejecución hasta que se detengan los eventos generados por timer
                    //solo si no es gatillado por excepcion en el proceso del timer para evitar dead lock
                    if (! timerWaitPoint.compareAndSet(1, 0))
                    {
                        int timeOut = 0;
                        while (! timerPoint.compareAndSet(0, -1))
                        {
                            Thread.sleep(configuration.getNetData().getTimeIntervalTimer());
                            timeOut++;
                            if (timeOut > configuration.getNetData().getWaitForTimerClose())
                            {
                                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: impossible to wait for timer");
                                break;
                            }
                        }
                    }
                    timer.cancel();
                    //timer.Dispose();
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: timer off error " + e.getMessage());
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages off");
                    messageThreads.interrupt();
                    try
                    {
                        router.getNotSentMessageQueue().unblok();
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        messageThreads.join();
                    }
                    catch (Exception e)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages off error " + e.getMessage());
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection off");
                    try
                    {
                        udpThreads.interrupt();
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        netHandler.getUdpMessageQueue().unblock();
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        udpThreads.join();
                    }
                    catch (Exception e)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: detection off error " + e.getMessage());
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process off");
                    try
                    {
                        tcpThread.interrupt();
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        netHandler.getTcpMessageQueue().unblock();
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        tcpThread.join();
                    }
                    catch (Exception e)
                    {
                    }
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: process off error " + e.getMessage());
                }
                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag off");
                    bag.cancel();
                    try
                    {
                        bagThread.interrupt();
                    }
                    catch (Exception e)
                    {
                    }
                    if (! bagWaitPoint.compareAndSet(1, 0))
                    {
                        int timeOut = 0;
                        while (! bagPoint.compareAndSet(0, -1))
                        {
                            Thread.sleep(configuration.getNetData().getTimeIntervalTimer());
                            timeOut++;
                            if (timeOut > configuration.getNetData().getWaitForTimerClose())
                            {
                                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNUCATION WARNING: impossible to wait for bag");
                                break;
                            }
                        }
                    }
                    bag.cancel();
                    //bag.Dispose();
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: bag off error " + e.getMessage());
                }

                try
                {
                    produceEvent(CommunicationEvent.NETINFORMATION,
                        "COMMUNICATION: messages stats " + "\n" +
                        " send: " + router.getNMessagesSent() + "\n" +
                        " confirmed: " + router.getNMessagesConfirmed() + "\n" +
                        " destroy: " + router.getNMessagesDestroyed() + "\n" +
                        " warnings: " + router.getNMessagesDroped() + "\n" +
                        " failed: " + router.getNMessagesFailed() + "\n" +
                        " received: " + router.getNMessagesReceived() + "\n" +
                        " resent: " + router.getNMessagesReplayed() + "\n" +
                        " routed: " + router.getNMessagesRouted() + "\n" +
                    "");
                }
                catch (Exception e)
                {
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: messages stats error " + e.getMessage());
                }
//                try
//                {
//                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: re init");
//                    init();
//                }
//                catch (Exception e)
//                {
//                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: re init error " + e.getMessage());
//                }
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: stop communication... ok!");
                stopPoint = new AtomicInteger(0);
            }
        }

        /**
         * Se gatilla cuando se resetea la conexión
         * Este método es para uso interno, no debe ser llamado
         */
        public void resetNetworkingHandler()
        {
            produceEvent(CommunicationEvent.RECONNECTING, null);
        }

        /**
         * Se gatilla cuando ha ocurrido un error en la red
         * Este método es para uso interno, no debe ser llamado
         * @param e la excepción generada en la red
         */
        public void errorNetworkingHandler(Exception e)
        {
            produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: network error");
            Thread disconnectThread = new Thread(){
            	public void run(){
            		disconnect();
            	}
            };
            disconnectThread.start();
            produceEvent(CommunicationEvent.EXCEPTION, e);
        }

        /**
         * Se gatilla cuando la red envia algun mensage del estado
         * Este método es para uso interno, no debe ser llamado
         * @param message el mensaje generado
         */
        public void informationNetworkingHandler(String message)
        {
            produceEvent(CommunicationEvent.NETINFORMATION, message);
        }

        /**
         * Función invocada cada 1 segundos por el timer, invoca un thread de interación de timer
         */
        private TimerTask getCommunicationTimerTask(){
        	TimerTask t = new TimerTask(){

				@Override
				public void run() {
					if (timerPoint.compareAndSet(0, 1))
		            {
		                timerThread = getCommunicationTimerInterationThread();
		                timerThread.setName("Timer Communication Thread");
		                timerThread.start();
		                try {
							timerThread.join();
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
		                timerPoint = new AtomicInteger(0);
		            }
					
				}
        		
        	};
        	return t;
        }

        /**
         * Timer: 
         * Actualiza los datos y objetos de comunicación según el estado de la MANET
         * Procesa los mensajes UDP
         * Envía mensaje ImAlive
         */
        //TODO
        private Thread getCommunicationTimerInterationThread()
        {
        	Thread t = new Thread(){

				@Override
				public void run() {
					try
		            {
						produceEvent(CommunicationEvent.NETINFORMATION, "*COMMUNICATION: TimerInteraction");
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
		                if (stopPoint.compareAndSet(0, 0))
		                {
		                    timerWaitPoint = new AtomicInteger(1);
		                    disconnect();
		                    produceEvent(CommunicationEvent.EXCEPTION, ex);
		                }
		            }
				}
        		
        	};
        	return t; 
        }

        /**
         * Process:
         * Procesa los mensajes TCP recibidos
         */
      //LISTO
        private Thread processTCPMessages()
        {
        	Thread t = new Thread(){

        		@Override
        		public void run() {
        			try
        			{
        				while (true)
        				{
        					NetMessage netMessage = netHandler.getTcpMessageQueue().draw();
        					proccessMessage(router.attendMessage(netMessage));
        				}
        			}
        			catch (Exception e)
        			{
        				disconnect();
        				produceEvent(CommunicationEvent.EXCEPTION, e);
        			}
        		}

        		@Override
        		public void interrupt() {
        			super.interrupt();
        			disconnect();
        		}


        	};
        	return t;

        }

        /**
         * Process:
         * Procesa los mensajes UDP recibidos
         */
      //LISTO
        private Thread processUDPMessages()
        {
        	Thread t = new Thread(){

        		@Override
        		public void run() {
        			try
        			{
        				while (true)
        				{
        					NetMessage netMessage = netHandler.getUdpMessageQueue().draw();
        					proccessMessage(router.attendMessage(netMessage));
        				}
        			}
        			catch (Exception e)
        			{
//        				disconnect();
        				produceEvent(CommunicationEvent.EXCEPTION, e);
        			}
        		}

//        		@Override
//        		public void interrupt() {
//        			super.interrupt();
//        			disconnect();
//        		}

        	};
        	return t;

        }

        /**
         * Process:
         * Procesa los mensajes UDP recibidos
         */
      //LISTO
        private Thread processNotSentMessages()
        {
        	Thread t = new Thread(){

        		@Override
        		public void run() {
        			try
        			{
        				while (true)
        				{
        					router.proccessNotSentMessage();
        				}
        			}
        			catch (Exception e)
        			{
//        				disconnect();
        				produceEvent(CommunicationEvent.EXCEPTION, e);
        			}
        		}
        	};
        	return t;
        }

        /**
         * Función invocada cada 1 segundos por el bag, inicializa un thread de interación de bag
         */
        private TimerTask getCommunicationBagTimerTask()
        {
        	TimerTask t = new TimerTask(){

				@Override
				public void run() {
					if (bagPoint.compareAndSet(0, 1))
		            {
		                bagThread = getCommunicationBagIterationThread();
		                bagThread.setName("Bag Communication Thread");
		                bagThread.start();
		                try {
							bagThread.join();
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
		                bagPoint = new AtomicInteger(0);
		            }
				}
        	};
        	return t;
        }

        /**
         * Bag:
         * Procesa los mensajes envíados y administrados por el router
         */
        private Thread getCommunicationBagIterationThread()
        {
        	Thread t = new Thread(){

        		@Override
        		public void run() {
        			try
        			{
        				//produceEvent(CommunicationEvent.NETINFORMATION, "*COMMUNICATION: BagInteraction");
        				processRouterMesages();
        			}
        			catch (Exception ex)
        			{
        				if (stopPoint.compareAndSet(0, 0))
        				{
        					bagWaitPoint = new AtomicInteger(1);
        					disconnect();
        					produceEvent(CommunicationEvent.EXCEPTION, ex);
        				}
        			}
        		}

        	};
        	return t;
        }



        /**
         * Actualiza el estado de la comunicación (tráfico)
         */
      //LISTO
        private void updateState()
        {
        	//produceEvent(CommunicationEvent.NETINFORMATION, "*COMMUNICATION: UpdateState");
            if (router.getNotSentMessageQueue().size() > configuration.getNetData().getStateCritical() || router.getFailedMessagesSize() > configuration.getNetData().getStateCritical() || netHandler.getTcpMessageQueue().size() > configuration.getNetData().getStateCritical())
            {
                configuration.getNetUser().setState(CommunicationQuality.CRITICAL);
            }
            else if (router.getNotSentMessageQueue().size() > configuration.getNetData().getStateOverloaded() || router.getFailedMessagesSize() > configuration.getNetData().getStateOverloaded() || netHandler.getTcpMessageQueue().size() > configuration.getNetData().getStateOverloaded())
            {
                configuration.getNetUser().setState(CommunicationQuality.OVERLOADED);
            }
            else
            {
                configuration.getNetUser().setState(CommunicationQuality.NORMAL);
            }
            //produceEvent(CommunicationEvent.NETINFORMATION, "*COMMUNICATION: UpdateState ok");
        }

        /**
         * Actualiza los parámetros del router
         */
      //LISTO
        private void updateRouter()
        {
            router.updateRouterTable(netHandler, configuration.getNetUser(), netUserList);
            //System.out.println(netUserList.toString());
            produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: max list size: " + router.getMaxListSize() + " used: " + router.getListSize());
        }

        /**
         * Actualiza los datos de la lista de usuarios
         */
      //LISTO
        private void updateUserList()
        {
            NetUser[] netUsers = netUserList.userListToArray();
            for (int i = 0; i < netUsers.length; i++)
            {
                //decrementa el time out, para algoritmo de perdida de señal
                netUsers[i].qualityDown(configuration.getNetData());
                //si llega a cero el time out, se ha ido de la red, hay que sacarlo
                if (netUsers[i].getTimeout() < 0)
                {
                    disconnectNetUser(netUsers[i]);
                }
                else
                {
                    if (netUsers[i].getSignalQuality() == NetUserQuality.LOW || netUsers[i].getSignalQuality() == NetUserQuality.NORMAL)
                    {
                        RemoteMachine remoteMachine = netHandler.getTcpServerList().getRemoteMachine(netUsers[i].getIp());
                        if (remoteMachine == null && netUsers[i].getJumpsAway() == 1)
                        {
                            if (netUsers[i].getWaitTimeOut() <= 0)
                            {
                                netUsers[i].setWaitTimeOut(configuration.getNetData().getWaitForTCPConnection());
                                netHandler.connectTo(netUsers[i].getIp().getHostAddress());
                            }
                            else
                            {
                                netUsers[i].waitTimeOutDown();
                            }
                        }
                    }
                    produceEvent(CommunicationEvent.REFRESHUSER, netUsers[i]);
                }
            }
        }

        /**
         * Actualiza la vecindad TCP
         */
      //LISTO
        private void updateNeighborhood()
        {
            ArrayList<UUID> ids = new ArrayList<UUID>();
            RemoteMachine[] serverMachines = netHandler.getTcpServerList().toObjectArray();
            for (int i = 0; i < serverMachines.length; i++)
            {
                NetUser serverNetUser = netUserList.getUser(serverMachines[i].getIp());
                if (serverNetUser != null)
                {
                    ids.add(serverNetUser.getId());
                }
            }
            UUID[] temp = new UUID[ids.size()];
            ids.toArray(temp);
            
            //configuration.getNetUser().setNeighborhoodIds((UUID[]) ids.toArray());
            configuration.getNetUser().setNeighborhoodIds(temp);
            produceEvent(CommunicationEvent.REFRESHLOCALUSER, configuration.getNetUser());
        }

        /**
         * Procesa los mensajes administrados por el router
         * @throws ArgumentOutOfRangeException 
         */
      //LISTO
        private void processRouterMesages() throws ArgumentOutOfRangeException
        {
            router.proccessFailedMessages();
            router.proccessNotConfirmedMessages();
        }

        /**
         * Agrega un nuevo usuario de la red a la lista
         * @param netUser El usuario de la red
         */
      //LISTO
        private void newNetUser(NetUser netUser)
        {
            synchronized (userListLock)
            {
                if (!netUser.getId().equals(configuration.getNetUser().getId()))
                {
                    NetUser newNetUser = new NetUser(netUser.getId(), netUser.getName(), netUser.getIp(), netUser.getNeighborhoodIds(), configuration.getNetData());
                    newNetUser.setState(netUser.getState());
                    newNetUser.setUpLayerData(netUser.getUpLayerData());
                    newNetUser.setJumpsAway(netUser.getJumpsAway());
                    if (netUserList.add(netUser.getIp(), newNetUser))
                    {
                        produceEvent(CommunicationEvent.ADDUSER, newNetUser);
                    }
                    else
                    {
                        produceEvent(CommunicationEvent.REFRESHUSER, newNetUser);
                    }
                    produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: [" + netUser.getName() + "] connected");
                }
            }
        }

        /**
         * Elimina a un usuario de la red de la lista de usuarios
         * @param netUser El usuario de la red
         */
      //LISTO
        private void disconnectNetUser(NetUser netUser)
        {
            synchronized (userListLock)
            {
                netUserList.remove(netUser.getIp());
                produceEvent(CommunicationEvent.REMOVEUSER, netUser);
                netHandler.disconnectFrom(netUser.getIp());
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: [" + netUser.getName() + "] disconnected");
            }
        }

        /**
         * Procesa un mensaje recibido
         * @param message El mensaje recibido
         */
        //LISTO
        private void proccessMessage(Message message)
        {
            if (message != null)
            {
                //registra la ip del mensaje recibido en el manejador de Ips, previo chequeo de que el mensaje no es de 
                //este mismo usuario
                if (!message.getSenderNetUser().getId().equals(configuration.getNetUser().getId()))
                {
                    netHandler.registerIp(message.getSenderNetUser().getIp().getHostAddress());
                }

                //verifica que se recibe el mensaje de un usuario conocido
                NetUser listedNetUser = netUserList.getUser(message.getSenderNetUser().getIp());

                //ImAliveMessage
                if (message.getType() == MessageType.IMALIVE)
                {
                    if (listedNetUser == null)
                    {
                        newNetUser(message.getSenderNetUser());
                    }
                    else if (!listedNetUser.getId().equals(message.getSenderNetUser().getId()))
                    {
                        disconnectNetUser(listedNetUser);
                    }
                    else
                    {
                        listedNetUser.qualityUp(configuration.getNetData());
                        listedNetUser.setNeighborhoodIds(message.getSenderNetUser().getNeighborhoodIds()); 
                        listedNetUser.setJumpsAway(message.getJumps());
                        listedNetUser.setState(message.getSenderNetUser().getState());
                        listedNetUser.setUpLayerData(message.getSenderNetUser().getUpLayerData());
                    }
                    //produceEvent(CommunicationEvent.NETINFORMATION, "recibido: " + subProtocol.ToString());
                }
                //AckMessage
                else if (message.getType() == MessageType.ACK)
                {
                    AckMessage ackMessage = (AckMessage)message;
                    router.proccessAckMessage(ackMessage);
                    //produceEvent(CommunicationEvent.NETINFORMATION, "recibidos: " + subProtocol.ToString());
                }
                //Resto de los mensajes
                else
                {
                    if (listedNetUser != null && listedNetUser.getId().equals(message.getSenderNetUser().getId()))
                    {
                        message.setSenderNetUser(listedNetUser);
                        SubProtocol subProtocol = (SubProtocol)subProtocols.getCollection().get(message.getProtocolType());
                        if (subProtocol != null)
                        {
                        	Object[] args = new Object[2];
                        	args[0] = subProtocol;
                        	args[1] = message;
                        	eventQueuePC.put(new Event(CommunicationEvent.SUBPROTOCOLPROCESSMESSAGE, args));
                            // eventQueuePC.put(new Event(new MessageEvent(subProtocol.proccessMessage), message));
                        }
                        else
                        {
                            produceEvent(CommunicationEvent.PROCESSMESSAGE, message);
                        }
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: received " + message.toString());
                    }
                    else
                    {
                        message.getSenderNetUser().setName("Unknown (" + message.getSenderNetUser().getIp().toString() + ")");
                        SubProtocol subProtocol = (SubProtocol) subProtocols.getCollection().get(message.getProtocolType());
                        if (subProtocol != null)
                        {
                        	Object[] args = new Object[2];
                        	args[0] = subProtocol;
                        	args[1] = message;
                        	eventQueuePC.put(new Event(CommunicationEvent.SUBPROTOCOLPROCESSMESSAGE, args));
                        	// eventQueuePC.put(new Event(new MessageEvent(subProtocol.proccessMessage), message));
                        }
                        else
                        {
                            produceEvent(CommunicationEvent.PROCESSMESSAGE, message);
                        }
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: unknown message  " + message.toString());
                    }
                }
            }
        }

        /**
         * Procesa un mensaje que no ha podido ser envíado correctamente
         * Interfaz para darle al router
         * @param message El mensaje
         */
        //LISTO
        private void routerMessageErrorHandler(Message message)
        {
            //ImAliveMessage
            if (message.getType() == MessageType.IMALIVE)
            {
                ImAliveMessage imAliveMessage = (ImAliveMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + imAliveMessage.toString());
            }
            //AckMessage
            else if (message.getType() == MessageType.ACK)
            {
                AckMessage ackMessage = (AckMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + ackMessage.toString());
            }
            //Resto de los mensajes
            else
            {
                SubProtocol subProtocol = (SubProtocol) subProtocols.getCollection().get(message.getProtocolType());
                if (subProtocol != null)
                {
                	Object[] args = new Object[2];
                	args[0] = subProtocol;
                	args[1] = message;
                	eventQueuePC.put(new Event(CommunicationEvent.SUBPROTOCOLERRORMESSAGE, args));
                    // eventQueuePC.put(new Event(new MessageEvent(subProtocol.errorMessage), message)); 
                }
                else
                {
                    produceEvent(CommunicationEvent.ERRORMESSAGE, message);
                }
                produceEvent(CommunicationEvent.NETINFORMATION, "ROUTER: message delivery fail " + message.toString());
            }
        }

        /**
         * Envía un mensaje a la MANET
         * @param message el mensaje a enviar
         */
        //LISTO
        public void send(Message message)
        {
            //ImAliveMessage
            if (message.getType() == MessageType.IMALIVE)
            {
                ImAliveMessage imAliveMessage = (ImAliveMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: reserved kind of message " + imAliveMessage.toString());
            }
            //AckMessage
            else if (message.getType() == MessageType.ACK)
            {
                AckMessage ackMessage = (AckMessage)message;
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION WARNING: reserved kind of message " + ackMessage.toString());
            }
            //Resto de los mensajes
            else
            {
                internalSendMessage(message);
            }
        }

        /**
         * Envía un mensaje de forma interna aplicando filtros
         * @param message el mensaje a enviar
         */
        //LISTO
        public void internalSendMessage(Message message)
        {
            try
            {
                if (this.configuration.getNetUser().getIp() != null)
                {
                    message.setSenderNetUser(configuration.getNetUser());
                    router.queueMessageToSend(message);
                    if (message.getType() != MessageType.IMALIVE && message.getType() != MessageType.ACK)
                    {
                        produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: queued " + message.toString());
                    }
                }
            }
//            catch (InterrruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
                produceEvent(CommunicationEvent.NETINFORMATION, "COMMUNICATION: impossible to send this message " + e.getMessage());
            }
        }
        
        
        
        /** Interfaz de observers para todos los eventos **/
      

    	/**
    	 * Suscribe un observer para el evento ConnectEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeConnectEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		this.connectEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ConnectEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeConnectEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		VoidEventObserverI val = this.connectEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ConnectEvent que se activo el evento
    	 */
    	private void notifyConnectEventObservers(){
    		Collection<VoidEventObserverI> list = this.connectEventObserverList.values();
    		for(VoidEventObserverI o: list){
    			o.update();
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento DisconnectEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeDisconnectEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		this.disconnectEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento DisconnectEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeDisconnectEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		VoidEventObserverI val = this.disconnectEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del DisconnectEvent que se activo el evento
    	 */
    	private void notifyDisconnectEventObservers(){
    		Collection<VoidEventObserverI> list = this.disconnectEventObserverList.values();
    		for(VoidEventObserverI o: list){
    			o.update();
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento ConnectingEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeConnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		this.connectingEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ConnectingEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeConnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		VoidEventObserverI val = this.connectingEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ConnectingEvent que se activo el evento
    	 */
    	private void notifyConnectingEventObservers(){
    		Collection<VoidEventObserverI> list = this.connectingEventObserverList.values();
    		for(VoidEventObserverI o: list){
    			o.update();
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento DisconnectingEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeDisconnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		this.disconnectingEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento DisconnectingEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeDisconnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		VoidEventObserverI val = this.disconnectingEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del DisconnectingEvent que se activo el evento
    	 */
    	private void notifyDisconnectingEventObservers(){
    		Collection<VoidEventObserverI> list = this.disconnectingEventObserverList.values();
    		for(VoidEventObserverI o: list){
    			o.update();
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento ReconnectingEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeReconnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		this.reconnectingEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ReconnectingEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeReconnectingEvent(VoidEventObserverI o){
    		int hash = o.hashCode();
    		VoidEventObserverI val = this.reconnectingEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ReconnectingEvent que se activo el evento
    	 */
    	private void notifyReconnectingEventObservers(){
    		Collection<VoidEventObserverI> list = this.reconnectingEventObserverList.values();
    		for(VoidEventObserverI o: list){
    			o.update();
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento AddUserEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeAddUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		this.addUserEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento AddUserEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeAddUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		NetUserEventObserverI val = this.addUserEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del AddUserEvent que se activo el evento
    	 * @param user NetUser que cambio
    	 */
    	private void notifyAddUserEventObservers(NetUser user){
    		Collection<NetUserEventObserverI> list = this.addUserEventObserverList.values();
    		for(NetUserEventObserverI o: list){
    			o.update(user);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento RemoveUserEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeRemoveUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		this.removeUserEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento RemoveUserEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeRemoveUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		NetUserEventObserverI val = this.removeUserEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del RemoveUserEvent que se activo el evento
    	 * @param user NetUser que cambio
    	 */
    	private void notifyRemoveUserEventObservers(NetUser user){
    		Collection<NetUserEventObserverI> list = this.removeUserEventObserverList.values();
    		for(NetUserEventObserverI o: list){
    			o.update(user);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento RefreshUserEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeRefreshUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		this.refreshUserEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento RefreshUserEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeRefreshUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		NetUserEventObserverI val = this.refreshUserEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del RefreshUserEvent que se activo el evento
    	 * @param user NetUser que cambio
    	 */
    	private void notifyRefreshUserEventObservers(NetUser user){
    		Collection<NetUserEventObserverI> list = this.refreshUserEventObserverList.values();
    		for(NetUserEventObserverI o: list){
    			o.update(user);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento RefreshLocalUserEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeRefreshLocalUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		this.refreshLocalUserEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento RefreshLocalUserEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeRefreshLocalUserEvent(NetUserEventObserverI o){
    		int hash = o.hashCode();
    		NetUserEventObserverI val = this.refreshLocalUserEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del RefreshLocalUserEvent que se activo el evento
    	 * @param user NetUser que cambio
    	 */
    	private void notifyRefreshLocalUserEventObservers(NetUser user){
    		Collection<NetUserEventObserverI> list = this.refreshLocalUserEventObserverList.values();
    		for(NetUserEventObserverI o: list){
    			o.update(user);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento NetInformationEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeNetInformationEvent(StringEventObserverI o){
    		int hash = o.hashCode();
    		this.netInformationEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento NetInformationEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeNetInformationEvent(StringEventObserverI o){
    		int hash = o.hashCode();
    		StringEventObserverI val = this.netInformationEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del NetInformationEvent que se activo el evento
    	 * @param s String enviado
    	 */
    	private void notifyNetInformationEventObservers(String s){
    		Collection<StringEventObserverI> list = this.netInformationEventObserverList.values();
    		for(StringEventObserverI o: list){
    			o.update(s);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento ExceptionEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeExceptionEvent(ExceptionEventObserverI o){
    		int hash = o.hashCode();
    		this.exceptionEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ExceptionEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeExceptionEvent(ExceptionEventObserverI o){
    		int hash = o.hashCode();
    		ExceptionEventObserverI val = this.exceptionEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ExceptionEvent que se activo el evento
    	 * @param e Excepcion que se levanto
    	 */
    	private void notifyExceptionEventObservers(Exception e){
    		Collection<ExceptionEventObserverI> list = this.exceptionEventObserverList.values();
    		for(ExceptionEventObserverI o: list){
    			o.update(e);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento ProcessMessageEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeProcessMessageEvent(MessageEventObserverI o){
    		int hash = o.hashCode();
    		this.processMessageEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ProcessMessageEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeProcessMessageEvent(MessageEventObserverI o){
    		int hash = o.hashCode();
    		MessageEventObserverI val = this.processMessageEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ProcessMessageEvent que se activo el evento
    	 * @param m Message en cuestion
    	 */
    	private void notifyProcessMessageEventObservers(Message m){
    		Collection<MessageEventObserverI> list = this.processMessageEventObserverList.values();
    		for(MessageEventObserverI o: list){
    			o.update(m);
    		}
    	}


    	/**
    	 * Suscribe un observer para el evento ErrorMessageEvent
    	 * @param o el observer que se subscribe
    	 * @return el entero usado como hash que se usa para almacenarlo
    	 */
    	public int subscribeErrorMessageEvent(MessageEventObserverI o){
    		int hash = o.hashCode();
    		this.errorMessageEventObserverList.put(hash, o);
    		return hash;
    	}
    	/**
    	 * Deja de escuchar al evento ErrorMessageEvent
    	 * @param o Observer que deja de escuchar
    	 */
    	public void unsubscribeErrorMessageEvent(MessageEventObserverI o){
    		int hash = o.hashCode();
    		MessageEventObserverI val = this.errorMessageEventObserverList.remove(hash);
    	}

    	/**
    	 * Avisa a todos los Observer del ErrorMessageEvent que se activo el evento
    	 * @param m Message en cuestion
    	 */
    	private void notifyErrorMessageEventObservers(Message m){
    		Collection<MessageEventObserverI> list = this.errorMessageEventObserverList.values();
    		for(MessageEventObserverI o: list){
    			o.update(m);
    		}
    	}

		@Override
		public void messageError(Message message) {
			routerMessageErrorHandler(message);
		}

}
