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

namespace SubProtocolCompact.FileTransfer.Messages
{
    /// <summary>
    /// Representa un mensaje de petición de lista de archivos que comparte un usuario
    /// Protocolo Safe TCP
    /// </summary>
    public class FileListRequestMessage : SafeUnicastMessage 
    {

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileListRequestMessage() : base()
        {
            this.Type = SubProtocolCompact.FileTransfer.Types.FILELISTREQUESTMESSAGE;
            this.ProtocolType = SubProtocolCompact.FileTransfer.Types.FILETRANSFERPROTOCOL;
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="targetNetUser">El receptor de este mensaje</param>
        public FileListRequestMessage(NetUser targetNetUser) : this()
        {
            this.TargetNetUser = targetNetUser;
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            return new byte[0];
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "FileListRequestMessage:";
        }
    }
}
