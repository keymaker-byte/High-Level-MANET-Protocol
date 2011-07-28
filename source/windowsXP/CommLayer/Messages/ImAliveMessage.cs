using System;
using System.Collections.Generic;
using System.Text;

namespace CommLayer.Messages
{
    /// <summary>
    /// Representa un mensaje para informar a los demas usuarios la existencia y la información de este
    /// Protocolo UDP
    /// </summary>
    internal class ImAliveMessage : MulticastMessage 
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImAliveMessage() : base()
        {
            this.Type = MessageType.IMALIVE;
            this.ProtocolType = MessageProtocolType.HLMP;
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] userNameSize = BitConverter.GetBytes(SenderNetUser.Name.Length); //4 (0 - 3)
            byte[] userName = Encoding.UTF8.GetBytes(SenderNetUser.Name);//userNameSize (4 - userNameSize + 3)
            byte[] userNeighborhoodSize = BitConverter.GetBytes(SenderNetUser.NeighborhoodIds.Length);//4 (userNameSize + 4 - userNameSize + 7)
            byte[] userNeighborhood = new byte[SenderNetUser.NeighborhoodIds.Length * 16];//userNeighborhoodSize*16 (userNameSize + 8 - userNameSize + 7 + userNeighborhoodSize*16)
            for (int i = 0; i < SenderNetUser.NeighborhoodIds.Length; i++)
            {
                byte[] neighborId = SenderNetUser.NeighborhoodIds[i].ToByteArray();
                neighborId.CopyTo(userNeighborhood, i * 16);
            }
            byte[] userState = BitConverter.GetBytes(SenderNetUser.State);//4 (userNameSize + 8 + userNeighborhoodSize*16  --  userNameSize + 11 + userNeighborhoodSize*16)
            //upLayerDataSize (userNameSize + 12 + userNeighborhoodSize*16  --  userNameSize + 11 + userNeighborhoodSize*16 + upLayerDataSize)

            byte[] pack = new byte[SenderNetUser.Name.Length + 12 + SenderNetUser.NeighborhoodIds.Length * 16 + SenderNetUser.UpLayerData.Length];
            userNameSize.CopyTo(pack, 0);
            userName.CopyTo(pack, 4);
            userNeighborhoodSize.CopyTo(pack, SenderNetUser.Name.Length + 4);
            userNeighborhood.CopyTo(pack, SenderNetUser.Name.Length + 8);
            userState.CopyTo(pack, SenderNetUser.Name.Length + 8 + SenderNetUser.NeighborhoodIds.Length * 16);
            SenderNetUser.UpLayerData.CopyTo(pack, SenderNetUser.Name.Length + 12 + SenderNetUser.NeighborhoodIds.Length * 16);
            return pack;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            Int32 userNameSize = BitConverter.ToInt32(messagePack, 0);
            SenderNetUser.Name = Encoding.UTF8.GetString(messagePack, 4, userNameSize);
            Int32 userNeighborhoodSize = BitConverter.ToInt32(messagePack, userNameSize + 4);
            SenderNetUser.NeighborhoodIds = new Guid[userNeighborhoodSize];
            for (int i = 0; i < userNeighborhoodSize; i++)
            {
                byte[] neighborId = new byte[16];
                Array.Copy(messagePack, userNameSize + 8 + i * 16, neighborId, 0, 16);
                SenderNetUser.NeighborhoodIds[i] = new Guid(neighborId);
            }
            SenderNetUser.State = BitConverter.ToInt32(messagePack, userNameSize + 8 + userNeighborhoodSize * 16);
            int upLayerDataSize = messagePack.Length - (userNameSize + 12 + userNeighborhoodSize * 16);
            if (upLayerDataSize > 0)
            {
                SenderNetUser.UpLayerData = new byte[upLayerDataSize];
                Array.Copy(messagePack, userNameSize + 12 + userNeighborhoodSize * 16, SenderNetUser.UpLayerData, 0, upLayerDataSize);
            }
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "ImAliveMessage";
        }
    }
}
