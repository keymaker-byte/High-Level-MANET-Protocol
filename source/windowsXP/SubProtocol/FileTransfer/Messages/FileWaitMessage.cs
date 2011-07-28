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
using CommLayer; using CommLayer.Messages;

namespace SubProtocol.FileTransfer.Messages
{
    /// <summary>
    /// Representa un mensaje de espera para descargar un archivo
    /// Protocolo Safe TCP
    /// </summary>
    public class FileWaitMessage : SafeUnicastMessage 
    {
        /// <summary>
        /// El id del archivo
        /// </summary>
        private Guid _fileHandlerId;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileWaitMessage() : base()
        {
            this.Type = SubProtocol.FileTransfer.Types.FILEWAITMESSAGE;
            this.ProtocolType = SubProtocol.FileTransfer.Types.FILETRANSFERPROTOCOL;
        }

        /// <summary>
        /// Constructor Parametrizado
        /// </summary>
        /// <param name="targetNetUser">El destinatario de este mensaje</param>
        /// <param name="fileHandlerId">El id del archivo</param>
        public FileWaitMessage(NetUser targetNetUser, Guid fileHandlerId) : this()
        {
            this.TargetNetUser = targetNetUser;
            this.FileHandlerId = fileHandlerId;
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
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] pack = FileHandlerId.ToByteArray();
            return pack;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            FileHandlerId = new Guid(messagePack);
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "FileWaitMessage: FileHandlerId=" + FileHandlerId;
        }
    }
}
