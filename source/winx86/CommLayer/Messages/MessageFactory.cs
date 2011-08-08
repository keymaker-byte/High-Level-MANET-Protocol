using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CommLayer;
using System.Threading;
using System.Reflection;
using System.Net;

namespace CommLayer.Messages
{
    /// <summary>
    /// Fabricador de mensajes provenientes de la red
    /// </summary>
    internal class MessageFactory
    {
        /// <summary>
        /// Tipos adicionalaes de mensajes
        /// </summary>
        private Hashtable messageTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageTypes">Tabla de hash con valores y tipos de mensajes adicionales</param>
        public MessageFactory(Hashtable messageTypes)
        {
            this.messageTypes = messageTypes;
        }

        /// <summary>
        /// Fabrica un mensaje que ha provenido de la red
        /// </summary>
        /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
        /// <returns>Un mensaje de alto nivel</returns>
        public Message makeMessage(byte[] messageData) 
        {
            Message message = null;
            try
            {
                Int32 messageType = BitConverter.ToInt32(messageData, 4);
                if (messageType < 100)
                {
                    switch (messageType)
                    {
                        case MessageType.ACK:
                            {
                                message = new AckMessage();
                                message.byteArrayToProperties(messageData);
                                break;
                            }
                        case MessageType.IMALIVE:
                            {
                                message = new ImAliveMessage();
                                message.byteArrayToProperties(messageData);
                                break;
                            }
                    }
                }
                else
                {
                    Type classType = (Type)messageTypes[messageType];
                    message = (Message)Activator.CreateInstance(classType);
                    message.byteArrayToProperties(messageData);
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
            return message;
        }

        /// <summary>
        /// Obtiene el id del mensaje
        /// </summary>
        /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
        /// <returns>El id del mensage</returns>
        public Guid getMessageId(byte[] messageData)
        {
            try
            {
                byte[] messageId = new byte[16];
                Array.Copy(messageData, 32, messageId, 0, 16);
                Guid Id = new Guid(messageId);
                return Id;
            }
            catch (Exception)
            {
            }
            return new Guid();
        }

        /// <summary>
        /// Obtiene el target user solo si es de tipo unicast
        /// </summary>
        /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
        /// <returns>El destinatario</returns>
        public NetUser getTargetNetUser(byte[] messageData)
        {
            try
            {
                NetUser targetNetUser = new NetUser();
                byte[] targetId = new byte[16];
                Array.Copy(messageData, 52, targetId, 0, 16);
                targetNetUser.Id = new Guid(targetId);
                byte[] targetIP = new byte[4];
                Array.Copy(messageData, 68, targetIP, 0, 4);
                targetNetUser.Ip = new IPAddress(targetIP);
                return targetNetUser;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Obtiene el sender user
        /// </summary>
        /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
        /// <returns>El sender</returns>
        public NetUser getSenderNetUser(byte[] messageData)
        {
            try
            {
                NetUser senderNetUser = new NetUser();
                byte[] userId = new byte[16];
                Array.Copy(messageData, 12, userId, 0, 16);
                senderNetUser.Id = new Guid(userId);
                byte[] userIP = new byte[4];
                Array.Copy(messageData, 28, userIP, 0, 4);
                senderNetUser.Ip = new IPAddress(userIP);
                return senderNetUser;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Obtiene el metatipo del mensaje
        /// </summary>
        /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
        /// <returns>El metatipo del mensage</returns>
        public Int32 getMessageMetaType(byte[] messageData)
        {
            try
            {
                Int32 MetaType = BitConverter.ToInt32(messageData, 0);
                return MetaType;
            }
            catch (Exception)
            {
            }
            return 0;
        }
    }
}
