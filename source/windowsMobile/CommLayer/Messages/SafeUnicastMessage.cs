using System;
using System.Collections.Generic;
using System.Text;
using NetLayerCompact;
using System.Net;

namespace CommLayerCompact.Messages
{

    /// <summary>
    /// representa un Mensaje Unicast TCP con protocolo de seguridad en la entrega
    /// </summary>
    public abstract class SafeUnicastMessage : Message
    {
        /// <summary>
        /// El usuario receptor de este mensaje
        /// </summary>
        private NetUser _targetNetUser;

        /// <summary>
        /// El tiempo de espera restante para reenvíar este mensaje mientras espera el ACK
        /// </summary>
        private Int32 _waitTimeOut;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SafeUnicastMessage()
        {
            this.Id = Guid.NewGuid();
            this.Jumps = 0;
            this.FailReason = MessageFailReason.NOTFAIL;
            this.MetaType = MessageMetaType.SAFEUNICAST;
            this.Type = MessageType.NOTYPE;
            this.ProtocolType = MessageProtocolType.NOTYPE;
            this.WaitTimeOut = 0;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messageData">un array de bytes con todos los datos del mensaje</param>
        internal override void byteArrayToProperties(byte[] messageData)
        {
            byte[] metaPack = new byte[72];
            byte[] pack = new byte[messageData.Length - 72];
            Array.Copy(messageData, 0, metaPack, 0, metaPack.Length);
            Array.Copy(messageData, 72, pack, 0, pack.Length);
            metaUnPack(metaPack);
            unPack(pack);
        }

        /// <summary>
        /// El usuario receptor de este mensaje
        /// </summary>
        public NetUser TargetNetUser
        {
            get { return _targetNetUser; }
            set { _targetNetUser = value; }
        }

        /// <summary>
        /// El tiempo de espera restante para reeenvíar este mensaje mientras espera el ACK
        /// </summary>
        internal Int32 WaitTimeOut
        {
            get { return _waitTimeOut; }
            set { _waitTimeOut = value; }
        }

        /// <summary>
        /// Convierte a este mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un array de bytes con todos los datos del mensaje</returns>
        internal override byte[] toByteArray()
        {
            byte[] metaPack = makeMetaPack();
            byte[] pack = makePack();
            byte[] messageData = new byte[metaPack.Length + pack.Length];
            Array.Copy(metaPack, 0, messageData, 0, metaPack.Length);
            Array.Copy(pack, 0, messageData, metaPack.Length, pack.Length);
            return messageData;
        }

        /// <summary>
        /// Envía el mensaje a la MANET
        /// </summary>
        /// <param name="netHandler">El manejador de la red</param>
        /// <param name="ip">la ip de la maquina remota destino</param>
        internal bool send(NetHandler netHandler, IPAddress ip)
        {
            return netHandler.sendTcpMessage(new NetMessage(toByteArray()), ip);
        }

        /// <summary>
        /// Convierte la meta data de este mensaje en una estructura de bytes
        /// </summary>
        /// <returns>el array de bytes con la meta data</returns>
        private byte[] makeMetaPack()
        {
            byte[] messageMetaType = BitConverter.GetBytes(MetaType); //4 (0 - 3)
            byte[] messageType = BitConverter.GetBytes(Type); //4 (4 - 7)
            byte[] messageProtocolType = BitConverter.GetBytes(ProtocolType); //4 (8 - 11)
            byte[] userId = SenderNetUser.Id.ToByteArray(); //16 (12 - 27)
            byte[] userIp = SenderNetUser.Ip.GetAddressBytes(); //4 (28-31)
            byte[] messageId = Id.ToByteArray(); //16 (32 - 47)
            byte[] messageJumps = BitConverter.GetBytes(Jumps); //4 (48 - 51)
            byte[] targetId = TargetNetUser.Id.ToByteArray(); //16 (52 - 67)
            byte[] targetIp = TargetNetUser.Ip.GetAddressBytes(); //4 (68-71)

            byte[] pack = new byte[72];
            messageMetaType.CopyTo(pack, 0);
            messageType.CopyTo(pack, 4);
            messageProtocolType.CopyTo(pack, 8);
            userId.CopyTo(pack, 12);
            userIp.CopyTo(pack, 28);
            messageId.CopyTo(pack, 32);
            messageJumps.CopyTo(pack, 48);
            targetId.CopyTo(pack, 52);
            targetIp.CopyTo(pack, 68);
            return pack;
        }

        /// <summary>
        /// Convierte una estructura de bytes en la meta data de este mensaje
        /// </summary>
        /// <param name="messageMetaPack">un array de bytes con la meta data</param>
        private void metaUnPack(byte[] messageMetaPack)
        {
            MetaType = BitConverter.ToInt32(messageMetaPack, 0);
            Type = BitConverter.ToInt32(messageMetaPack, 4);
            ProtocolType = BitConverter.ToInt32(messageMetaPack, 8);
            SenderNetUser = new NetUser();
            byte[] userId = new byte[16];
            Array.Copy(messageMetaPack, 12, userId, 0, 16);
            SenderNetUser.Id = new Guid(userId);
            byte[] userIP = new byte[4];
            Array.Copy(messageMetaPack, 28, userIP, 0, 4);
            SenderNetUser.Ip = new IPAddress(userIP);
            byte[] messageId = new byte[16];
            Array.Copy(messageMetaPack, 32, messageId, 0, 16);
            Id = new Guid(messageId);
            Jumps = BitConverter.ToInt32(messageMetaPack, 48);
            TargetNetUser = new NetUser();
            byte[] targetId = new byte[16];
            Array.Copy(messageMetaPack, 52, targetId, 0, 16);
            TargetNetUser.Id = new Guid(targetId);
            byte[] targetIP = new byte[4];
            Array.Copy(messageMetaPack, 68, targetIP, 0, 4);
            TargetNetUser.Ip = new IPAddress(targetIP);
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public abstract byte[] makePack();

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public abstract void unPack(byte[] messagePack);

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return "SafeUnicastMessage : ";
        }
    }
}
