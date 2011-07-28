/***************************************************************************
----------------------------------------------------------------------------
  This file is part of the HLMP API - Plug-ins Source Code.
  http://hlmprotocol.bicubic.cl
 
  Copyright (C) Bicubic TMG.  All rights reserved.
 
  This source code is intended only as a supplement to HLMP API 
  and/or on-line documentation.  
 
  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      
  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        
  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       
  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  
  AND PERMISSION FROM BICUBIC TMG.   

  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
----------------------------------------------------------------------------
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using CommLayer; 
using CommLayer.Messages;

namespace SubProtocol.FileTransfer.Messages
{
    /// <summary>
    /// Representa un mensaje que contiene la parte de un archivo
    /// Protocolo Safe TCP
    /// </summary>
    public class FilePartMessage : SafeUnicastMessage 
    {
        /// <summary>
        /// El id del archivo
        /// </summary>
        private Guid _fileHandlerId;

        /// <summary>
        /// El id de la parte del archivo
        /// </summary>
        private Int64 _partId;

        /// <summary>
        /// el conjunto de bytes de la parte del archivo
        /// </summary>
        private byte[] _filePart;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FilePartMessage() : base()
        {
            this.Type = SubProtocol.FileTransfer.Types.FILEPARTMESSAGE;
            this.ProtocolType = SubProtocol.FileTransfer.Types.FILETRANSFERPROTOCOL;
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="targetNetUser">El receptor de este mensaje</param>
        /// <param name="fileHandlerId">El id del archivo</param>
        /// <param name="partId">El id de la parte del archivo</param>
        /// <param name="filePart">El conjunto de bytes de la parte del archivo en formato string base 64</param>
        public FilePartMessage(NetUser targetNetUser, Guid fileHandlerId, Int64 partId, byte[] filePart) : this()
        {
            this.TargetNetUser = targetNetUser;
            this.FileHandlerId = fileHandlerId;
            this.PartId = partId;
            this.FilePart = filePart;
        }

        /// <summary>
        /// El id del archivo
        /// </summary>
        public Guid FileHandlerId
        {
            get { return _fileHandlerId; }
            set { _fileHandlerId = value; }
        }

        /// <summary>
        /// El id de la parte del archivo
        /// </summary>
        public Int64 PartId
        {
            get { return _partId; }
            set { _partId = value; }
        }

        /// <summary>
        /// El conjunto de bytes de la parte del archivo en formato string base 64
        /// </summary>
        public byte[] FilePart
        {
            get { return _filePart; }
            set { _filePart = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] packFileID = FileHandlerId.ToByteArray(); //16 (0 - 15)
            byte[] packPartID = BitConverter.GetBytes(PartId); //8 (16 - 23)
            
            byte[] pack = new byte[24 + FilePart.Length];
            packFileID.CopyTo(pack, 0);
            packPartID.CopyTo(pack, 16);
            FilePart.CopyTo(pack, 24);

            return pack;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            byte[] packFileID = new byte[16];
            Array.Copy(messagePack, 0, packFileID, 0, packFileID.Length);
            FileHandlerId = new Guid(packFileID);

            PartId = BitConverter.ToInt64(messagePack, 16);

            FilePart = new byte[messagePack.Length - 24];
            Array.Copy(messagePack, 24, FilePart, 0, FilePart.Length);
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "FilePartMessage: FileHandlerId=" + FileHandlerId + " PartId=" + PartId;
        }
    }
}
