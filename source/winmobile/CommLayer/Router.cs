using System;
using System.Collections.Generic;
using System.Text;
using NetLayer;
using CommLayer.Messages;
using System.Net;
using System.Collections;

namespace CommLayer
{
    /// <summary>
    /// Clase que se encarga de la logica de entrega y recepción de mensajes
    /// </summary>
    internal class Router
    {
        /// <summary>
        /// La colección de ids de mensajes
        /// </summary>
        private MessageIdCollection messageIdCollection;

        /// <summary>
        /// Promedio de mensajes que se reciben en cada iteracion
        /// </summary>
        private Double messageAverage;

        /// <summary>
        /// Número de iteraciones realizadas para el calculo del promedio
        /// </summary>
        private Int64 iterations;

        /// <summary>
        /// Probabilidad de que se duplique un mensaje en la red
        /// </summary>
        private Double duplicationProbability;

        /// <summary>
        /// Cola de mensajes para envíar
        /// </summary>
        private MessageMonitoredQueue notSentMessageQueue;

        /// <summary>
        /// Cola de mensajes que han fallado en la entrega
        /// </summary>
        private MessageQueue failedMessageQueue;

        /// <summary>
        /// Cola de mensajes tipo Safe no confirmados con ACK
        /// </summary>
        private MessageList notConfirmedMessageList;

        /// <summary>
        /// Delegado para avisar mensajes fallidos
        /// </summary>
        /// <param name="message">El mensaje que ha fallado</param>
        public delegate void MessagErrorDelegate(Message message);

        /// <summary>
        /// Se gatilla cada vez que un mensaje a fallado
        /// </summary>
        private MessagErrorDelegate messageError;

        /// <summary>
        /// Veces que se espera para el reenvio de un mensaje tipo Safe
        /// </summary>
        private Int32 waitForAck;

        /// <summary>
        /// El manejador de la red
        /// </summary>
        private NetHandler netHandler;

        /// <summary>
        /// El netUser dueño de este router
        /// </summary>
        private NetUser netUser;

        /// <summary>
        /// La lista de usuarios de la MANET
        /// </summary>
        private NetUserList netUserList;

        /// <summary>
        /// Statics para número de mensajes enviados
        /// </summary>
        private Int64 nMessagesSent;

        /// <summary>
        /// Statics para número de mensajes confirmados
        /// </summary>
        private Int64 nMessagesConfirmed;

        /// <summary>
        /// Statics para número de mensajes fallidos
        /// </summary>
        private Int64 nMessagesFailed;

        /// <summary>
        /// Statics para número de mensajes reenviados
        /// </summary>
        private Int64 nMessagesReplayed;

        /// <summary>
        /// Statics para número de mensajes destruidos
        /// </summary>
        private Int64 nMessagesDestroyed;

        /// <summary>
        /// Statics para número de mensajes avisados de no entrega
        /// </summary>
        private Int64 nMessagesDroped;

        /// <summary>
        /// Statics para numero de mensajes recibidos
        /// </summary>
        private Int64 nMessagesReceived;

        /// <summary>
        /// Statics para numero de mensajes ruteados
        /// </summary>
        private Int64 nMessagesRouted;

        /// <summary>
        /// Statics para numero de mensajes recibidos en el ultimo update del router
        /// </summary>
        private Int32 nMessagesCounted;

        /// <summary>
        /// Fabricador de mensajes
        /// </summary>
        private MessageFactory messageFactory;

        /// <summary>
        /// Generador de numeros aleatorios
        /// </summary>
        private Random rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Router(MessagErrorDelegate routerInformation, Int32 waitForAck, Hashtable messageTypes)
        {
            this.messageError = routerInformation;
            this.waitForAck = waitForAck;
            this.messageFactory = new MessageFactory(messageTypes);
            duplicationProbability = Math.Pow(10, -100);
            messageAverage = 0;
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

        /// <summary>
        /// Tamaño máximo de la lista de ids de mensajes
        /// </summary>
        public Int32 MaxListSize
        {
            get { return messageIdCollection.MaxSize; }
        }

        /// <summary>
        /// Tamaño actual de la lista de ids de mensajes
        /// </summary>
        public Int32 ListSize
        {
            get { return messageIdCollection.size(); }
        }

        /// <summary>
        /// Promedio de mensajes que se reciben en cada iteracion
        /// </summary>
        public double MessageAverage
        {
            get { return messageAverage; }
        }

        /// <summary>
        /// Tamaño de la lista de mensajes fallidos
        /// </summary>
        public Int32 FailedMessagesSize
        {
            get { return failedMessageQueue.size(); }
        }

        /// <summary>
        /// Tamaño de la lista de mensajes no confirmados
        /// </summary>
        public Int32 NotConfirmedSize
        {
            get { return notConfirmedMessageList.size(); }
        }

        /// <summary>
        /// Tamaño de la lista de mensajes no confirmados
        /// </summary>
        public Int32 NotSentSize
        {
            get { return notSentMessageQueue.size(); }
        }

        /// <summary>
        /// Mensajes TCP SAFE enviados
        /// </summary>
        public Int64 NMessagesSent
        {
            get { return nMessagesSent; }
        }

        /// <summary>
        /// Mensajes TCP SAFE confirmados
        /// </summary>
        public Int64 NMessagesConfirmed
        {
            get { return nMessagesConfirmed; }
        }

        /// <summary>
        /// Mensajes TCP SAFE fallidos encolados para reenvío
        /// </summary>
        public Int64 NMessagesFailed
        {
            get { return nMessagesFailed; }
        }

        /// <summary>
        /// Mensajes TCP SAFE seleccionados para reenvio por no confirmación
        /// </summary>
        public Int64 NMessagesReplayed
        {
            get { return nMessagesReplayed; }
        }

        /// <summary>
        /// Mensajes TCP SAFE destruidos
        /// </summary>
        public Int64 NMessagesDestroyed
        {
            get { return nMessagesDestroyed; }
        }

        /// <summary>
        /// Mensajes TCP SAFE recepcionados
        /// </summary>
        public Int64 NMessagesReceived
        {
            get { return nMessagesReceived; }
        }

        /// <summary>
        /// Mensajes TCP SAFE seleccionados para ruteo
        /// </summary>
        public Int64 NMessagesRouted
        {
            get { return nMessagesRouted; }
        }

        /// <summary>
        /// Mensajes TCP SAFE fallidos avisados al usuario 
        /// </summary>
        public Int64 NMessagesDroped
        {
            get { return nMessagesDroped; }
        }

        internal MessageMonitoredQueue NotSentMessageQueue
        {
            get { return notSentMessageQueue; }
        }

        /// <summary>
        /// Encola un mensaje a la lista de mensajes para enviar
        /// </summary>
        /// <param name="message">El mensaje a enviar</param>
        public void queueMessageToSend(Message message)
        {
            if (message.MetaType == MessageMetaType.MULTICAST)
            {
                messageIdCollection.add(message.Id);
                nMessagesCounted++;
                send(message);
            }
            else if (message.MetaType == MessageMetaType.SAFEMULTICAST)
            {
                messageIdCollection.add(message.Id);
                nMessagesCounted++;
                notSentMessageQueue.put(message);
            }
            else if (message.MetaType == MessageMetaType.FASTUNICAST)
            {
                send(message);
            }
            else
            {
                notSentMessageQueue.put(message);
            }
        }

        /// <summary>
        /// Identifica el tipo de mensaje a enviar y lo envia mediante un netHandler
        /// </summary>
        /// <param name="message">El mensaje e enviar</param>
        private void send(Message message)
        {
            if (message.MetaType == MessageMetaType.MULTICAST)
            {
                MulticastMessage multicastMessage = (MulticastMessage)message;
                multicastMessage.send(netHandler);
            }
            else if (message.MetaType == MessageMetaType.SAFEMULTICAST)
            {
                SafeMulticastMessage safeMulticastMessage = (SafeMulticastMessage)message;
                safeMulticastMessage.send(netHandler);
            }
            else if (message.MetaType == MessageMetaType.UNICAST)
            {
                UnicastMessage unicastMessage = (UnicastMessage)message;
                IPAddress ip = pathNextIp(unicastMessage.TargetNetUser);
                NetUser listedNetUSer = netUserList.getUser(unicastMessage.TargetNetUser.Ip);
                if (ip != null)
                {
                    if (!unicastMessage.send(netHandler, ip))
                    {
                        unicastMessage.FailReason = MessageFailReason.TCPFAIL;
                        failedMessageQueue.put(unicastMessage);
                    }
                }
                else if (listedNetUSer != null && listedNetUSer.Id.Equals(unicastMessage.TargetNetUser.Id))
                {
                    unicastMessage.FailReason = MessageFailReason.NOTROUTEBUTHOSTONNET;
                    failedMessageQueue.put(unicastMessage);
                }
                else
                {
                    unicastMessage.FailReason = MessageFailReason.NOTROUTETOHOST;
                    failedMessageQueue.put(unicastMessage);
                }
            }
            else if (message.MetaType == MessageMetaType.SAFEUNICAST)
            {
                SafeUnicastMessage safeMessage = (SafeUnicastMessage)message;
                IPAddress ip = pathNextIp(safeMessage.TargetNetUser);
                NetUser listedNetUser = netUserList.getUser(safeMessage.TargetNetUser.Ip);
                if (ip != null)
                {
                    if (safeMessage.send(netHandler, ip))
                    {
                        //statics
                        nMessagesSent++;
                        //si el mensaje enviado fue de este usuario se espera confirmación
                        if (safeMessage.SenderNetUser.Id.Equals(netUser.Id))
                        {
                            safeMessage.WaitTimeOut = waitForAck + 2 * safeMessage.TargetNetUser.JumpsAway;
                            notConfirmedMessageList.add(safeMessage);
                        }
                    }
                    else
                    {
                        //statics
                        nMessagesFailed++;
                        safeMessage.FailReason = MessageFailReason.TCPFAIL;
                        failedMessageQueue.put(safeMessage);
                    }
                }
                else if (listedNetUser != null && listedNetUser.Id.Equals(safeMessage.TargetNetUser.Id))
                {
                    safeMessage.FailReason = MessageFailReason.NOTROUTEBUTHOSTONNET;
                    failedMessageQueue.put(safeMessage);
                }
                else
                {
                    safeMessage.FailReason = MessageFailReason.DESTROY;
                    failedMessageQueue.put(safeMessage);
                }
            }
            else if (message.MetaType == MessageMetaType.FASTUNICAST)
            {
                FastUnicastMessage fastUnicastMessage = (FastUnicastMessage)message;
                IPAddress ip = pathNextIp(fastUnicastMessage.TargetNetUser);
                NetUser listedNetUSer = netUserList.getUser(fastUnicastMessage.TargetNetUser.Ip);
                if (ip != null)
                {
                    fastUnicastMessage.send(netHandler, ip);
                }
            }
        }

        /// <summary>
        /// Identifica un mensaje de bajo nivel recibido y lo retorna como mensajede alto nivel, y/o lo rutea segun corresponde
        /// </summary>
        /// <param name="netMessage">El mensaje recibido de bajo nivel</param>
        /// <returns>el mensaje de alto nivel recibido, o null si se ruteó hacia otra parte</returns>
        public Message attendMessage(NetMessage netMessage)
        {
            Int32 messageMetaType = messageFactory.getMessageMetaType(netMessage.Body);
            if (messageMetaType == MessageMetaType.MULTICAST)
            {
                Guid messageId = messageFactory.getMessageId(netMessage.Body);
                if (!messageIdCollection.contains(messageId))
                {
                    Message message = messageFactory.makeMessage(netMessage.Body);
                    if (message != null)
                    {
                        message.Jumps++;
                        if (message.Jumps > 1)
                        {
                            queueMessageToSend(message);
                        }
                        else
                        {
                            if (rand.NextDouble() < 0.5)
                            {
                                queueMessageToSend(message);
                            }
                            else
                            {
                                messageIdCollection.add(message.Id);
                                nMessagesCounted++;
                            }
                        }
                        return message;
                    }
                }
            }
            else if (messageMetaType == MessageMetaType.SAFEMULTICAST)
            {
                Guid messageId = messageFactory.getMessageId(netMessage.Body);
                if (!messageIdCollection.contains(messageId))
                {
                    Message message = messageFactory.makeMessage(netMessage.Body);
                    if (message != null)
                    {
                        message.Jumps++;
                        queueMessageToSend(message);
                        return message;
                    }
                }
            }
            else if (messageMetaType == MessageMetaType.UNICAST)
            {
                Message message = messageFactory.makeMessage(netMessage.Body);
                if (message != null)
                {
                    message.Jumps++;
                    UnicastMessage unicastMessage = (UnicastMessage)message;
                    if (unicastMessage.TargetNetUser.Id.Equals(netUser.Id))
                    {
                        return message;
                    }
                    else
                    {
                        //Parametrizar
                        if (notSentMessageQueue.size() > 50 || failedMessageQueue.size() > 50)
                        {
                            nMessagesFailed++;
                            unicastMessage.FailReason = MessageFailReason.DESTROY;
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
                NetUser targetNetUser = messageFactory.getTargetNetUser(netMessage.Body);
                if (netUser.Id.Equals(targetNetUser.Id))
                {
                    Guid messageId = messageFactory.getMessageId(netMessage.Body);
                    NetUser senderNetUser = messageFactory.getSenderNetUser(netMessage.Body);
                    if (senderNetUser != null)
                    {
                        AckMessage ackMessage = new AckMessage(senderNetUser, messageId);
                        ackMessage.SenderNetUser = netUser;
                        queueMessageToSend(ackMessage);
                    }
                }
                //PRocesamos en mensaje
                Message message = messageFactory.makeMessage(netMessage.Body);
                if (message != null)
                {
                    message.Jumps++;
                    SafeUnicastMessage safeMessage = (SafeUnicastMessage)message;
                    if (safeMessage.TargetNetUser.Id.Equals(netUser.Id))
                    {
                        if (!messageIdCollection.contains(safeMessage.Id))
                        {
                            //statics
                            nMessagesReceived++;
                            messageIdCollection.add(safeMessage.Id);
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
                            safeMessage.FailReason = MessageFailReason.DESTROY;
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
                Message message = messageFactory.makeMessage(netMessage.Body);
                if (message != null)
                {
                    message.Jumps++;
                    FastUnicastMessage fastUnicastMessage = (FastUnicastMessage)message;
                    if (fastUnicastMessage.TargetNetUser.Id.Equals(netUser.Id))
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

        /// <summary>
        /// Procesa un mensaje de confirmación que se ha recibido
        /// </summary>
        /// <param name="ackMessage">el mensaje de confirmación</param>
        public void proccessAckMessage(AckMessage ackMessage)
        {
            if (notConfirmedMessageList.remove(ackMessage.MessageId))
            {
                //statics
                nMessagesConfirmed++;
            }
        }

        /// <summary>
        /// Procesa los mensajes de tipo Unicast que no han sido enviados aun
        /// </summary>
        public void proccessNotSentMessage()
        {
            Message message = notSentMessageQueue.draw();
            if (message.MetaType == MessageMetaType.MULTICAST)
            {
                send(message);
            }
            else if (message.MetaType == MessageMetaType.SAFEMULTICAST)
            {
                send(message);
            }
            else if (message.MetaType == MessageMetaType.UNICAST)
            {
                send(message);
            }
            else if (message.MetaType == MessageMetaType.SAFEUNICAST)
            {
                send(message);
            }
            else if (message.MetaType == MessageMetaType.FASTUNICAST)
            {
                send(message);
            }
        }

        /// <summary>
        /// Procesa los mensajes tipo Safe que no han sido confirmados
        /// </summary>
        public void proccessNotConfirmedMessages()
        {
            Message[] messages = notConfirmedMessageList.messageListToArray();
            int nMessages = messages.Length;
            if (nMessages > netHandler.NetData.MaxMessagesProcess)
            {
                nMessages = netHandler.NetData.MaxMessagesProcess;
            }
            for (int i = 0; i < nMessages; i++)
            {
                SafeUnicastMessage safeMessage = (SafeUnicastMessage)messages[i];
                if (safeMessage.WaitTimeOut <= 0)
                {
                    //statics
                    nMessagesReplayed++;
                    notConfirmedMessageList.remove(safeMessage.Id);
                    queueMessageToSend(safeMessage);
                }
                else
                {
                    safeMessage.WaitTimeOut--;
                }
            }
        }

        /// <summary>
        /// Procesa los mensajes de tipo Unicast que no han sido enviados con exito
        /// </summary>
        public void proccessFailedMessages()
        {
            int nMessages = failedMessageQueue.size();
            if (nMessages > netHandler.NetData.MaxMessagesProcess)
            {
                nMessages = netHandler.NetData.MaxMessagesProcess;
            }
            for (int i = 0; i < nMessages; i++)
            {
                Message message = failedMessageQueue.draw();
                switch (message.FailReason)
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
                            if (message.SenderNetUser.Id.Equals(netUser.Id))
                            {
                                if (message.MetaType == MessageMetaType.MULTICAST)
                                {
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.SAFEMULTICAST)
                                {
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.UNICAST)
                                {
                                    UnicastMessage unicastMessage = (UnicastMessage)message;
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.SAFEUNICAST)
                                {
                                    //statics
                                    nMessagesDroped++;
                                    SafeUnicastMessage safeUnicastMessage = (SafeUnicastMessage)message;
                                    messageError(message);
                                }
                            }
                            break;
                        }
                    case MessageFailReason.DESTROY:
                        {
                            if (message.SenderNetUser.Id.Equals(netUser.Id))
                            {
                                if (message.MetaType == MessageMetaType.MULTICAST)
                                {
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.SAFEMULTICAST)
                                {
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.UNICAST)
                                {
                                    UnicastMessage unicastMessage = (UnicastMessage)message;
                                    messageError(message);
                                }
                                else if (message.MetaType == MessageMetaType.SAFEUNICAST)
                                {
                                    //statics
                                    nMessagesDroped++;
                                    SafeUnicastMessage safeUnicastMessage = (SafeUnicastMessage)message;
                                    messageError(message);
                                }
                            }
                            else
                            {
                                if (message.GetType().BaseType.Equals(typeof(SafeUnicastMessage)))
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

        /// <summary>
        /// Actualiza los parámetros del router
        /// </summary>
        /// <param name="netHandler">El manejador de la red</param>
        /// <param name="netUser">Los datos del usuario local</param>
        /// <param name="netUserList">La lista de usuarios de la red</param>
        public void updateRouterTable(NetHandler netHandler, NetUser netUser, NetUserList netUserList)
        {
            this.netHandler = netHandler;
            this.netUser = netUser;
            this.netUserList = netUserList;

            int newMessageNumber = nMessagesCounted;
            nMessagesCounted = 0;
            int neighborhoodSize = netUser.NeighborhoodIds.Length;
            //se incrementa el numero de iteraciones
            iterations++;
            //actualizo el promedio de cada registro
            messageAverage = ((messageAverage * (iterations - 1) + newMessageNumber) / iterations);
            if (messageAverage > 1)
            {
                //calculo el nuevo tamaño optimo de la lista,
                int size = 1 + (int)(((Math.Log(duplicationProbability) / (Math.Log(messageAverage - 1) - Math.Log(messageAverage)))));
                //si el nuevo tamaño optimo es mayor, se debe aumentar el tamaño de la lista
                if (size > messageIdCollection.MaxSize)
                {
                    messageIdCollection.MaxSize = size;
                }
            }
        }

        /// <summary>
        /// Encuentra el camino mas corto hacia el destinatario de un mensaje y retorna la ip del vecino que se encuentra en el camino
        /// </summary>
        /// <param name="receiverNetUser">El usuario destinatario del mensaje</param>
        /// <returns>Un string con la IP del remote machine vecino que se encuentra en el camino óptimo</returns>
        private IPAddress pathNextIp(NetUser receiverNetUser)
        {
            NetUser[] listedNodes = netUserList.userListToArray();
            //Se chequea que existan al menos dos usuarios mas
            if (listedNodes.Length < 2)
            {
                for (int i = 0; i < netUser.NeighborhoodIds.Length; i++)
                {
                    if (netUser.NeighborhoodIds[i].Equals(receiverNetUser.Id))
                    {
                        return receiverNetUser.Ip;
                    }
                }
                //no existe conexion con ese usuario
                return null;
            }

            IPAddress ip = null;
            //Se crea el array de nodos del grafo
            List<NetUser> nodes = new List<NetUser>();
            nodes.Add(netUser);
            int receiberIndex = -1;
            for (int i = 0; i < listedNodes.Length; i++)
            {
                nodes.Add(listedNodes[i]);
                if (receiverNetUser.Id.Equals(listedNodes[i].Id))
                {
                    receiberIndex = i + 1;
                }
            }
            if (receiberIndex != -1)
            {
                //Se ejecuta el algoritmo de Dijkstra para encontrar el orden de los indices de los nodos con el camino optimo
                Dijkstra dijkstra = new Dijkstra(nodes, netHandler.NetData);
                int[] nodeIndexes = dijkstra.GetMinimumPath(0, receiberIndex);
                if (nodeIndexes.Length > 0)
                {
                    ip = nodes[nodeIndexes[0]].Ip;
                    if (ip.Equals(receiverNetUser.Ip))
                    {
                        for (int i = 0; i < netUser.NeighborhoodIds.Length; i++)
                        {
                            if (netUser.NeighborhoodIds[i].Equals(receiverNetUser.Id))
                            {
                                return receiverNetUser.Ip;
                            }
                        }
                        return null;
                    }
                }
            }
            return ip;
        }
    }
}
