using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommLayer.Messages
{
    /// <summary>
    /// Clase que enumera las acciones y errores posibles al tratar de enviar un mensaje
    /// </summary>
    internal static class MessageFailReason
    {
        /// <summary>
        /// El mensaje no ha fallado
        /// </summary>
        public const Int32 NOTFAIL = 1;

        /// <summary>
        /// El mensaje ha fallado en la entrega al vecino elegido
        /// </summary>
        public const Int32 TCPFAIL = 2;

        /// <summary>
        /// No existe una ruta al destinatario, pero el usuario si se encuentra en la MANET
        /// </summary>
        public const Int32 NOTROUTETOHOST = 3;

        /// <summary>
        /// El mensaje debe ser destruido
        /// </summary>
        public const Int32 DESTROY = 4;

        /// <summary>
        /// El usuario destino no existe en la red
        /// </summary>
        public const Int32 NOTROUTEBUTHOSTONNET = 5;
    }

    /// <summary>
    /// Clase que enumera lso tipos de mete mensajes
    /// </summary>
    internal static class MessageMetaType
    {
        /// <summary>
        /// Constante para sin tipo
        /// </summary>
        public const Int32 NOMETATYPE = 0;
        
        /// <summary>
        /// Constante para el tipo multicast
        /// </summary>
        public const Int32 MULTICAST = 1;

        /// <summary>
        /// Constante para el tipo unicast
        /// </summary>
        public const Int32 UNICAST = 2;

        /// <summary>
        /// Constante para el tipo safe multicast
        /// </summary>
        public const Int32 SAFEMULTICAST = 3;

        /// <summary>
        /// Constante para el tipo safe unicast
        /// </summary>
        public const Int32 SAFEUNICAST = 4;

        /// <summary>
        /// Constante para el tipo safe unicast
        /// </summary>
        public const Int32 FASTUNICAST = 5;
    }

    /// <summary>
    /// Clase que enumera los tipos de mensajes derivados conocidos por el sistema
    /// </summary>
    internal static class MessageType
    {
        /// <summary>
        /// Constante para sin tipo
        /// </summary>
        public const Int32 NOTYPE = 0;
        
        /// <summary>
        /// Constante para el tipo im alive
        /// </summary>
        public const Int32 IMALIVE = 1;

        /// <summary>
        /// Constante para ack
        /// </summary>
        public const Int32 ACK = 2;
    }

    /// <summary>
    /// Clase que enumera los tipos de protocolos a los que pertenecen los mensajes
    /// </summary>
    internal static class MessageProtocolType
    {
        /// <summary>
        /// Constante para sin tipo
        /// </summary>
        public const Int32 NOTYPE = 0;

        /// <summary>
        /// Constante para el tipo HLMP
        /// </summary>
        public const Int32 HLMP = 1;
    }

    /// <summary>
    /// Representa un mensaje de alto nivel que puede ser envíado a la MANET
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// El autor de este mensaje
        /// </summary>
        private NetUser _senderNetUser;

        /// <summary>
        /// El id unico de este mensaje
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Los saltos de host que ha dado este mensaje
        /// </summary>
        private Int32 _jumps;

        /// <summary>
        /// El motivo de fallo en la entrega (un valor de MessageFailReason)
        /// </summary>
        private Int32 _failReason;

        /// <summary>
        /// El tipo de dato abstracto de este mensaje
        /// </summary>
        private Int32 _metaType;

        /// <summary>
        /// El tipo de este mensaje
        /// </summary>
        private Int32 _type;

        /// <summary>
        /// El tipo de protocolo de este mensaje
        /// </summary>
        private Int32 _protocolType;

        /// <summary>
        /// El autor de este mensaje
        /// </summary>
        public NetUser SenderNetUser
        {
            get { return _senderNetUser; }
            set { _senderNetUser = value; }
        }

        /// <summary>
        /// El id unico de este mensaje
        /// </summary>
        internal Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Los saltos de host que ha dado este mensaje
        /// </summary>
        public Int32 Jumps
        {
            get { return _jumps; }
            set { _jumps = value; }
        }

        /// <summary>
        /// El motivo de fallo en la entrega (un valor de MessageFailReason)
        /// </summary>
        internal Int32 FailReason
        {
            get { return _failReason; }
            set { _failReason = value; }
        }

        /// <summary>
        /// El tipo de dato abstracto de este mensaje
        /// </summary>
        public Int32 MetaType
        {
            get { return _metaType; }
            set { _metaType = value; }
        }

        /// <summary>
        /// El tipo de este mensaje
        /// </summary>
        public Int32 Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// El tipo de este mensaje
        /// </summary>
        public Int32 ProtocolType
        {
            get { return _protocolType; }
            set { _protocolType = value; }
        }

        /// <summary>
        /// Convierte a este mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un array de bytes con todos los datos del mensaje</returns>
        internal abstract byte[] toByteArray();

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messageData">un array de bytes con todos los datos del mensaje</param>
        internal abstract void byteArrayToProperties(byte[] messageData);
    }
}
