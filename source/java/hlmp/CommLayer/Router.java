package hlmp.CommLayer;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Random;
import java.util.UUID;

import hlmp.CommLayer.Constants.*;
import hlmp.CommLayer.Exceptions.ArgumentOutOfRangeException;
import hlmp.CommLayer.Interfaces.RouterMessageErrorHandlerI;
import hlmp.CommLayer.Messages.*;
import hlmp.NetLayer.NetHandler;
import hlmp.NetLayer.NetMessage;

/**
 * Clase que se encarga de la logica de entrega y recepción de mensajes
 */
public class Router {

	/**
	 * La colección de ids de mensajes
	 */
	private MessageIdCollection messageIdCollection;

	/**
	 * Promedio de mensajes que se reciben en cada iteracion
	 */
	private Double messageAverage;

	/**
	 * Número de iteraciones realizadas para el calculo del promedio
	 */
	private int iterations;

	/**
	 * Probabilidad de que se duplique un mensaje en la red
	 */
	private Double duplicationProbability;

	/**
	 * Cola de mensajes para envíar
	 */
	private MessageMonitoredQueue notSentMessageQueue;

	/**
	 * Cola de mensajes que han fallado en la entrega
	 */
	private MessageQueue failedMessageQueue;

	/**
	 * Cola de mensajes tipo Safe no confirmados con ACK
	 */
	private MessageList notConfirmedMessageList;

	/**
	 * Se gatilla cada vez que un mensaje a fallado
	 */
	private RouterMessageErrorHandlerI messageError;

	/**
	 * Veces que se espera para el reenvio de un mensaje tipo Safe
	 */
	private int waitForAck;

	/**
	 * El manejador de la red
	 */
	private NetHandler netHandler; 

	/**
	 * El netUser dueño de este router
	 */
	private NetUser netUser;

	/**
	 * La lista de usuarios de la MANET
	 */
	private NetUserList netUserList;

	/**
	 * Statics para número de mensajes enviados
	 */
	private int nMessagesSent;

	/**
	 * Statics para número de mensajes confirmados
	 */
	private int nMessagesConfirmed;

	/**
	 * Statics para número de mensajes fallidos
	 */
	private int nMessagesFailed;

	/**
	 * Statics para número de mensajes reenviados
	 */
	private int nMessagesReplayed;

	/**
	 * Statics para número de mensajes destruidos
	 */
	private int nMessagesDestroyed;

	/**
	 * Statics para número de mensajes avisados de no entrega
	 */
	private int nMessagesDroped;

	/**
	 * Statics para numero de mensajes recibidos
	 */
	private int nMessagesReceived;

	/**
	 * Statics para numero de mensajes ruteados
	 */
	private int nMessagesRouted;

	/**
	 * Statics para numero de mensajes recibidos en el ultimo update del router
	 */
	private int nMessagesCounted;

	/**
	 * Fabricador de mensajes
	 */
	private MessageFactory messageFactory;

	/**
	 * Generador de numeros aleatorios
	 */
	private Random rand = new Random();

	/**
	 * Default Constructor
	 */
	public Router(RouterMessageErrorHandlerI routerInformation, int waitForAck, HashMap<Integer, Message> messageTypes)
	{
		this.messageError = routerInformation;
		this.waitForAck = waitForAck;
		this.messageFactory = new MessageFactory(messageTypes);
		duplicationProbability = Math.pow(10, -100);
		messageAverage = 0.0;
		iterations = 0;
		messageIdCollection = new MessageIdCollection();
		failedMessageQueue = new MessageQueue();
		notSentMessageQueue = new MessageMonitoredQueue();
		notConfirmedMessageList = new MessageList();
		nMessagesConfirmed = 0;
		nMessagesFailed = 0;
		nMessagesReplayed = 0;
		nMessagesSent = 0;
		nMessagesDestroyed = 0;
		nMessagesReceived = 0;
		nMessagesRouted = 0;
		nMessagesDroped = 0;
		nMessagesCounted = 0;
	}

	/**
	 * Tamaño máximo de la lista de ids de mensajes
	 */
	public int getMaxListSize()
	{
		return messageIdCollection.getMaxSize();
	}

	/**
	 * Tamaño actual de la lista de ids de mensajes
	 */
	public int getListSize()
	{
		return messageIdCollection.size();
	}

	public Double getMessageAverage() {
		return messageAverage;
	}

	/**
	 * Tamaño de la lista de mensajes fallidos
	 */
	public int getFailedMessagesSize()
	{
		return failedMessageQueue.size();
	}

	/**
	 * Tamaño de la lista de mensajes no confirmados
	 */
	public int NotConfirmedSize()
	{
		return notConfirmedMessageList.size();
	}
	
	/**
	 * Tamaño de la lista de mensajes no confirmados
	 */
    public int getNotSentSize()
    {
        return notSentMessageQueue.size();
    }


	public MessageMonitoredQueue getNotSentMessageQueue() {
		return notSentMessageQueue;
	}

	/**
	 * Mensajes TCP SAFE enviados
	 * @return
	 */
	public int getNMessagesSent() {
		return nMessagesSent;
	}

	/**
	 * Mensajes TCP SAFE confirmados
	 * @return
	 */
	public int getNMessagesConfirmed() {
		return nMessagesConfirmed;
	}

	/**
	 * Mensajes TCP SAFE fallidos encolados para reenvío
	 * @return
	 */
	public int getNMessagesFailed() {
		return nMessagesFailed;
	}

	/**
	 * Mensajes TCP SAFE seleccionados para reenvio por no confirmación
	 * @return
	 */
	public int getNMessagesReplayed() {
		return nMessagesReplayed;
	}

	/**
	 * Mensajes TCP SAFE destruidos
	 * @return
	 */
	public int getNMessagesDestroyed() {
		return nMessagesDestroyed;
	}

	/**
	 * Mensajes TCP SAFE fallidos avisados al usuario
	 * @return
	 */
	public int getNMessagesDroped() {
		return nMessagesDroped;
	}

	/**
	 * Mensajes TCP SAFE recepcionados
	 */
	public int getNMessagesReceived() {
		return nMessagesReceived;
	}

	/**
	 * Mensajes TCP SAFE seleccionados para ruteo
	 * @return
	 */
	public int getNMessagesRouted() {
		return nMessagesRouted;
	}

	/**
	 * Encola un mensaje a la lista de mensajes para enviar
	 * @param message El mensaje a enviar
	 * @throws ArgumentOutOfRangeException
	 */
	public void queueMessageToSend(Message message) throws ArgumentOutOfRangeException
	{
		if (message.getMetaType() == MessageMetaType.MULTICAST)
		{
			messageIdCollection.add(message.getId());
			nMessagesCounted++;
			send(message);
		}
		else if (message.getMetaType() == MessageMetaType.SAFEMULTICAST)
		{
			messageIdCollection.add(message.getId());
			nMessagesCounted++;
			notSentMessageQueue.put(message);
		}
		else if (message.getMetaType() == MessageMetaType.FASTUNICAST)
		{
			send(message);
		}
		else
		{
			notSentMessageQueue.put(message);
		}
	}

	/**
	 * Identifica el tipo de mensaje a enviar y lo envia mediante un netHandler
	 * @param message El mensaje a enviar
	 */
	private void send(Message message) throws ArgumentOutOfRangeException
	{
		if (message.getMetaType() == MessageMetaType.MULTICAST)
		{
			MulticastMessage multicastMessage = (MulticastMessage)message;
			multicastMessage.send(netHandler);
		}
		else if (message.getMetaType() == MessageMetaType.SAFEMULTICAST)
		{
			SafeMulticastMessage safeMulticastMessage = (SafeMulticastMessage)message;
			safeMulticastMessage.send(netHandler);
		}
		else if (message.getMetaType() == MessageMetaType.UNICAST)
		{
			UnicastMessage unicastMessage = (UnicastMessage)message;
			// IP al que deberia enviar el mensaje
			InetAddress ip = pathNextIp(unicastMessage.getTargetNetUser());
			// usuario registrado en la lista con la IP dada
			NetUser listedNetUser = netUserList.getUser(unicastMessage.getTargetNetUser().getIp());
			if (ip != null)
			{
				if (!unicastMessage.send(netHandler, ip))
				{
					unicastMessage.setFailReason(MessageFailReason.TCPFAIL);
					failedMessageQueue.put(unicastMessage);
				}
			}
			else if (listedNetUser != null && listedNetUser.getId().equals(unicastMessage.getTargetNetUser().getId()))
			{
				unicastMessage.setFailReason(MessageFailReason.NOTROUTEBUTHOSTONNET);
				failedMessageQueue.put(unicastMessage);
			}
			else
			{
				unicastMessage.setFailReason(MessageFailReason.NOTROUTETOHOST);
				failedMessageQueue.put(unicastMessage);
			}
		}
		else if (message.getMetaType() == MessageMetaType.SAFEUNICAST)
		{
			SafeUnicastMessage safeMessage = (SafeUnicastMessage)message;
			InetAddress ip = pathNextIp(safeMessage.getTargetNetUser());
			NetUser listedNetUser = netUserList.getUser(safeMessage.getTargetNetUser().getIp());
			if (ip != null)
			{
				if (safeMessage.send(netHandler, ip))
				{
					//statics
					nMessagesSent++;
					//si el mensaje enviado fue de este usuario se espera confirmación
					if (safeMessage.getSenderNetUser().getId().equals(netUser.getId()))
					{
						safeMessage.setWaitTimeOut(waitForAck + 2 * safeMessage.getTargetNetUser().getJumpsAway());
						notConfirmedMessageList.add(safeMessage);
					}
				}
				else
				{
					//statics
					nMessagesFailed++;
					safeMessage.setFailReason(MessageFailReason.TCPFAIL);
					failedMessageQueue.put(safeMessage);
				}
			}
			else if (listedNetUser != null && listedNetUser.getId().equals(safeMessage.getTargetNetUser().getId()))
			{
				safeMessage.setFailReason(MessageFailReason.NOTROUTEBUTHOSTONNET);
				failedMessageQueue.put(safeMessage);
			}
			else
			{
				safeMessage.setFailReason(MessageFailReason.DESTROY);
				failedMessageQueue.put(safeMessage);
			}
		}
		else if (message.getMetaType() == MessageMetaType.FASTUNICAST)
		{
			FastUnicastMessage fastUnicastMessage = (FastUnicastMessage)message;
			InetAddress ip = pathNextIp(fastUnicastMessage.getTargetNetUser());
			//NetUser listedNetUser = netUserList.getUser(fastUnicastMessage.getTargetNetUser().getIp());
			if (ip != null)
			{
				fastUnicastMessage.send(netHandler, ip);
			}
		}
	}

	/**
	 * Identifica un mensaje de bajo nivel recibido y lo retorna como mensaje de alto nivel, y/o lo rutea segun corresponde
	 * @param netMessage El mensaje recibido de bajo nivel
	 * @return el mensaje de alto nivel recibido, o null si se ruteó hacia otra parte
	 */
	public Message attendMessage(NetMessage netMessage) throws ArgumentOutOfRangeException
	{
		int messageMetaType = messageFactory.getMessageMetaType(netMessage.getBody());
		if (messageMetaType == MessageMetaType.MULTICAST)
		{
			UUID messageId = messageFactory.getMessageId(netMessage.getBody());
			if (!messageIdCollection.contains(messageId))
			{
				Message message = messageFactory.makeMessage(netMessage.getBody());
				if (message != null)
				{
					message.jumpsAdd1();
					if (message.getJumps() > 1)
					{
						queueMessageToSend(message);
					}
					else
					{
						if (rand.nextDouble() < 0.5)
						{
							queueMessageToSend(message);
						}
						else
						{
							messageIdCollection.add(message.getId());
							nMessagesCounted++;
						}
					}
					return message;
				}
			}
		}
		else if (messageMetaType == MessageMetaType.SAFEMULTICAST)
		{
			this.netHandler.informationNetworkingHandler("ROUTER: attending SafeMulticast Message");
			UUID messageId = messageFactory.getMessageId(netMessage.getBody());
			if (!messageIdCollection.contains(messageId))
			{
				Message message = messageFactory.makeMessage(netMessage.getBody());
				if (message != null)
				{
					message.jumpsAdd1();
					queueMessageToSend(message);
					return message;
				}
			}
		}
		else if (messageMetaType == MessageMetaType.UNICAST)
		{
			Message message = messageFactory.makeMessage(netMessage.getBody());
			if (message != null)
			{
				message.jumpsAdd1();
				UnicastMessage unicastMessage = (UnicastMessage)message;
				if (unicastMessage.getTargetNetUser().getId().equals(netUser.getId()))
				{
					return message;
				}
				else
				{
					//Parametrizar
					if (notSentMessageQueue.size() > 50 || failedMessageQueue.size() > 50)
					{
						nMessagesFailed++;
						unicastMessage.setFailReason(MessageFailReason.DESTROY);
						failedMessageQueue.put(unicastMessage);
					}
					else
					{
						nMessagesRouted++;
						queueMessageToSend(unicastMessage);
					}
				}
			}
		}
		else if (messageMetaType == MessageMetaType.SAFEUNICAST)
		{
			//Enviamos el ack primero que todo
			NetUser targetNetUser = messageFactory.getTargetNetUser(netMessage.getBody());
			if (netUser.getId().equals(targetNetUser.getId()))
			{
				UUID messageId = messageFactory.getMessageId(netMessage.getBody());
				NetUser senderNetUser = messageFactory.getSenderNetUser(netMessage.getBody());
				if (senderNetUser != null)
				{
					AckMessage ackMessage = new AckMessage(senderNetUser, messageId);
					ackMessage.setSenderNetUser(netUser);
					queueMessageToSend(ackMessage);
				}
			}
			//Procesamos el mensaje
			Message message = messageFactory.makeMessage(netMessage.getBody());
			if (message != null)
			{
				message.jumpsAdd1();
				SafeUnicastMessage safeMessage = (SafeUnicastMessage)message;
				if (safeMessage.getTargetNetUser().getId().equals(netUser.getId()))
				{
					if (!messageIdCollection.contains(safeMessage.getId()))
					{
						//statics
						nMessagesReceived++;
						messageIdCollection.add(safeMessage.getId());
						nMessagesCounted++;
						return message;
					}
				}
				else
				{
					//Parametrizar
					if (notSentMessageQueue.size() > 50 || failedMessageQueue.size() > 50)
					{
						nMessagesFailed++;
						safeMessage.setFailReason(MessageFailReason.DESTROY);
						failedMessageQueue.put(safeMessage);
					}
					else
					{
						nMessagesRouted++;
						queueMessageToSend(safeMessage);
					}
				}
			}
		}
		else if (messageMetaType == MessageMetaType.FASTUNICAST)
		{
			Message message = messageFactory.makeMessage(netMessage.getBody());
			if (message != null)
			{
				message.jumpsAdd1();
				FastUnicastMessage fastUnicastMessage = (FastUnicastMessage)message;
				if (fastUnicastMessage.getTargetNetUser().getId().equals(netUser.getId()))
				{
					return message;
				}
				else
				{
					nMessagesRouted++;
					queueMessageToSend(fastUnicastMessage);
				}
			}
		}
		return null;
	}


	/**
	 * Procesa un mensaje de confirmación que se ha recibido
	 * @param ackMessage el mensaje de confirmación
	 */
    public void proccessAckMessage(AckMessage ackMessage)
    {
        if (notConfirmedMessageList.remove(ackMessage.getMessageId()))
        {
            //statics
            nMessagesConfirmed++;
        }
    }

  	/**
     * Procesa los mensajes de tipo Unicast que no han sido enviados aun
  	 * @throws ArgumentOutOfRangeException 
  	 * @throws InterruptedException 
     */
    public void proccessNotSentMessage() throws ArgumentOutOfRangeException, InterruptedException
    {
        Message message = notSentMessageQueue.draw();
        System.out.println("processNotSentMessage");
        switch(message.getMetaType())
        {
	        case MessageMetaType.MULTICAST:
//	        	System.out.println("multicast");
	        case MessageMetaType.SAFEMULTICAST:
//	        	System.out.println("safe multi");
	        case MessageMetaType.UNICAST:
//	        	System.out.println("uni");
	        case MessageMetaType.SAFEUNICAST:
//	        	System.out.println("safe multi");
	        case MessageMetaType.FASTUNICAST:
//	        	System.out.println("fast uni");
	        	send(message);
	        	break;
	        default: return;
        }
        
//        if (message.getMetaType() == MessageMetaType.MULTICAST)
//        {
//            send(message);
//        }
//        else if (message.getMetaType() == MessageMetaType.SAFEMULTICAST)
//        {
//            send(message);
//        }
//        else if (message.getMetaType() == MessageMetaType.UNICAST)
//        {
//            send(message);
//        }
//        else if (message.getMetaType() == MessageMetaType.SAFEUNICAST)
//        {
//            send(message);
//        }
//        else if (message.getMetaType() == MessageMetaType.FASTUNICAST)
//        {
//            send(message);
//        }
    }

    /**
     * Procesa los mensajes tipo Safe que no han sido confirmados
     */
    public void proccessNotConfirmedMessages() throws ArgumentOutOfRangeException
    {
        Message[] messages = notConfirmedMessageList.messageListToArray();
        int nMessages = messages.length;
        if (nMessages > netHandler.getNetData().getMaxMessagesProcess())
        {
            nMessages = netHandler.getNetData().getMaxMessagesProcess();
        }
        for (int i = 0; i < nMessages; i++)
        {
            SafeUnicastMessage safeMessage = (SafeUnicastMessage)messages[i];
            if (safeMessage.getWaitTimeOut() <= 0)
            {
                //statics
                nMessagesReplayed++;
                notConfirmedMessageList.remove(safeMessage.getId());
                queueMessageToSend(safeMessage);
            }
            else
            {
                safeMessage.WaitTimeOutDec1();
            }
        }
    }

    /**
     * Procesa los mensajes de tipo Unicast que no han sido enviados con exito
     * @throws ArgumentOutOfRangeException 
     */
    public void proccessFailedMessages() throws ArgumentOutOfRangeException
    {
        int nMessages = failedMessageQueue.size();
        if (nMessages > netHandler.getNetData().getMaxMessagesProcess())
        {
            nMessages = netHandler.getNetData().getMaxMessagesProcess();
        }
        for (int i = 0; i < nMessages; i++)
        {
            Message message = failedMessageQueue.draw();
            switch (message.getFailReason())
            {
                case MessageFailReason.TCPFAIL:
                    {
                        queueMessageToSend(message);
                        break;
                    }
                case MessageFailReason.NOTROUTEBUTHOSTONNET:
                    {
                        queueMessageToSend(message);
                        break;
                    }
                case MessageFailReason.NOTROUTETOHOST:
                    {
                        if (message.getSenderNetUser().getId().equals(netUser.getId()))
                        {
                            if (message.getMetaType() == MessageMetaType.MULTICAST)
                            {
                                messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.SAFEMULTICAST)
                            {
                            	messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.UNICAST)
                            {
                                //UnicastMessage unicastMessage = (UnicastMessage)message;
                                messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.SAFEUNICAST)
                            {
                                //statics
                                nMessagesDroped++;
                                //SafeUnicastMessage safeUnicastMessage = (SafeUnicastMessage)message;
                                messageError.messageError(message);
                            }
                        }
                        break;
                    }
                case MessageFailReason.DESTROY:
                    {
                        if (message.getSenderNetUser().getId().equals(netUser.getId()))
                        {
                            if (message.getMetaType() == MessageMetaType.MULTICAST)
                            {
                            	messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.SAFEMULTICAST)
                            {
                            	messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.UNICAST)
                            {
                                //UnicastMessage unicastMessage = (UnicastMessage)message;
                                messageError.messageError(message);
                            }
                            else if (message.getMetaType() == MessageMetaType.SAFEUNICAST)
                            {
                                //statics
                                nMessagesDroped++;
                                //SafeUnicastMessage safeUnicastMessage = (SafeUnicastMessage)message;
                                messageError.messageError(message);
                            }
                        }
                        else
                        {
                            if (message.getClass().equals(SafeUnicastMessage.class))
                            {
                                //statics
                                nMessagesDestroyed++;
                            }
                        }
                        break;
                    }
            }
        }
    }
    
    /**
     * Actualiza los parámetros del router
     * @param netHandler El manejador de la red
     * @param netUser Los datos del usuario local
     * @param netUserList La lista de usuarios de la red
     */
    public void updateRouterTable(NetHandler netHandler, NetUser netUser, NetUserList netUserList)
    {
        this.netHandler = netHandler;
        this.netUser = netUser;
        this.netUserList = netUserList;

        int newMessageNumber = nMessagesCounted;
        nMessagesCounted = 0;
        //int neighborhoodSize = netUser.getNeighborhoodIds().length;
        //se incrementa el numero de iteraciones
        iterations++;
        //actualizo el promedio de cada registro
        messageAverage = ((messageAverage * (iterations - 1) + newMessageNumber) / iterations);
        if (messageAverage > 1)
        {
            //calculo el nuevo tamaño optimo de la lista,
            int size = 1 + (int)(((Math.log(duplicationProbability) / (Math.log(messageAverage - 1) - Math.log(messageAverage)))));
            //si el nuevo tamaño optimo es mayor, se debe aumentar el tamaño de la lista
            if (size > messageIdCollection.getMaxSize())
            {
                messageIdCollection.setMaxSize(size);
            }
        }
    }
    
    /**
     * Encuentra el camino mas corto hacia el destinatario de un mensaje y retorna la ip del vecino que se encuentra en el camino
     * @param receiverNetUser El usuario destinatario del mensaje
     * @return Un InetAddress con la IP del remote machine vecino que se encuentra en el camino óptimo
     */
	private InetAddress pathNextIp(NetUser receiverNetUser) throws ArgumentOutOfRangeException
	{
		NetUser[] listedNodes = netUserList.userListToArray();
		//Se chequea que existan al menos dos usuarios mas
		if (listedNodes.length < 2)
		{
			for(int i = 0; i < netUser.getNeighborhoodIds().length; i++) {
				if (netUser.getNeighborhoodIds()[i].equals(receiverNetUser.getId()))
				{
					return receiverNetUser.getIp();
				}
			}
			//no existe conexion con ese usuario
			return null;
		}

		InetAddress ip = null; 
		//Se crea el array de nodos del grafo
		ArrayList<NetUser> nodes = new ArrayList<NetUser>();
		nodes.add(netUser);
		int receiberIndex = -1;
		for (int i = 0; i < listedNodes.length; i++)
		{
			nodes.add(listedNodes[i]);
			if(receiverNetUser.getId().equals(listedNodes[i].getId())) 
			{
				receiberIndex = i + 1;
			}
		}
		if (receiberIndex != -1)
		{
			//Se ejecuta el algoritmo de Dijkstra para encontrar el orden de los indices de los nodos con el camino optimo
			Dijkstra dijkstra = new Dijkstra(nodes, netHandler.getNetData());
			int[] nodeIndexes = dijkstra.GetMinimumPath(0, receiberIndex);
			if (nodeIndexes.length > 0)
			{
				ip = nodes.get(nodeIndexes[0]).getIp();
				if (ip.equals(receiverNetUser.getIp()))
				{
					for (int i = 0; i < netUser.getNeighborhoodIds().length; i++)
					{
						if (netUser.getNeighborhoodIds()[i].equals(receiverNetUser.getId()))
						{
							return receiverNetUser.getIp();
						}
					}
					return null;
				}
			}
		}
		return ip;
	}
}
