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
using CommLayerCompact;
using CommLayerCompact.Messages;
using SubProtocolCompact;

namespace SubProtocolCompact.FileTransfer.Messages
{
    /// <summary>
    /// Representa un mensaje de solicitud de archivo
    /// Protocolo Safe TCP
    /// </summary>
    public class FileRequestMessage : SafeUnicastMessage 
    {
        /// <summary>
        /// El id del archivo
        /// </summary>
        private Guid _fileId;

        /// <summary>
        /// El id de la transferencia
        /// </summary>
        private Guid _fileHandlerId;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileRequestMessage() : base()
        {
            this.Type = SubProtocolCompact.FileTransfer.Types.FILEREQUESTMESSAGE;
            this.ProtocolType = SubProtocolCompact.FileTransfer.Types.FILETRANSFERPROTOCOL;
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="targetNetUser">El receptor de este mensaje</param>
        /// <param name="fileId">El id del archivo</param>
        /// <param name="fileHandlerId">El identificador de la transferencia</param>
        public FileRequestMessage(NetUser targetNetUser, Guid fileId, Guid fileHandlerId): this()
        {
            this.TargetNetUser = targetNetUser;
            this.FileId = fileId;
            this.FileHandlerId = fileHandlerId;
        }

        /// <summary>
        /// El id del archivo
        /// </summary>
        public Guid FileId
        {
            get { return _fileId; }
            set { _fileId = value; }
        }

        /// <summary>
        /// El Id de la transferencia
        /// </summary>
        public Guid FileHandlerId
        {
            get { return _fileHandlerId; }
            set { _fileHandlerId = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] fileID = FileId.ToByteArray();//16 (0 - 15)
            byte[] fileHandlerId = FileHandlerId.ToByteArray(); //16 (16 - 31)

            byte[] pack = new byte[32];
            fileID.CopyTo(pack, 0);
            fileHandlerId.CopyTo(pack, 16);
            return pack;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            byte[] fileID = new byte[16];
            Array.Copy(messagePack, 0, fileID, 0, fileID.Length);
            FileId = new Guid(fileID);

            byte[] fileHandlerID = new byte[16];
            Array.Copy(messagePack, 16, fileHandlerID, 0, fileHandlerID.Length);
            FileHandlerId = new Guid(fileHandlerID);
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "FileRequestMessage: FileId=" + FileId + " FileHandlerId=" + FileHandlerId;
        }

    }
}
