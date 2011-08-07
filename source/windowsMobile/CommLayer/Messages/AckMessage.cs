using System;
using System.Collections.Generic;
using System.Text;

namespace CommLayerCompact.Messages
{
    /// <summary>
    /// Representa un mensaje de confirmación para los mensajes de tipo Safe
    /// Protocolo TCP
    /// </summary>
    internal class AckMessage : UnicastMessage 
    {
        /// <summary>
        /// El id del mensaje
        /// </summary>
        private Guid _messageId;

        /// <summary>
        /// Constructor
        /// </summary>
        public AckMessage() : base()
        {
            this.Type = MessageType.ACK;
            this.ProtocolType = MessageProtocolType.HLMP;
        }

        /// <summary>
        /// Constructor Parametrizado
        /// </summary>
        /// <param name="targetNetUser">El destinatario de este mensaje</param>
        /// <param name="messageId">El id del mensaje a confirmar</param>
        public AckMessage(NetUser targetNetUser, Guid messageId) : this()
        {
            this.TargetNetUser = targetNetUser;
            this.MessageId = messageId;
        }

        /// <summary>
        /// El id del mensaje a confirmar
        /// </summary>
        public Guid MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] messageId = MessageId.ToByteArray(); //16 (0 - 15)
            return messageId;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            MessageId = new Guid(messagePack);
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "AckMessage: MessageId=" + MessageId.ToString();
        }

    }
}
