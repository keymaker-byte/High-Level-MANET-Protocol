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
using CommLayerCompact.Messages;
using CommLayerCompact;

namespace SubProtocolCompact.Ping.Messages
{
    /// <summary>
    /// Envía una respuesta de mensaje de Ping a la maquina remota destino para medir tiempo de envio + respuesta
    /// </summary>
    public class PingResponseMessage : FastUnicastMessage
    {

        private Int64 _timestamp;

        private byte[] data;

        /// <summary>
        /// Constructor
        /// </summary>
        public PingResponseMessage() : base()
        {
            this.Type = SubProtocolCompact.Ping.Types.PINGRESPONSEMESSAGE;
            this.ProtocolType = SubProtocolCompact.Ping.Types.PINGPROTOCOL;
            data = new byte[16];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="targetNetUser"></param>
        public PingResponseMessage(Int64 timeStamp, NetUser targetNetUser) : this()
        {
            Timestamp = timeStamp;
            TargetNetUser = targetNetUser;
        }

        /// <summary>
        /// Tiempo original
        /// </summary>
        public Int64 Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            byte[] timestamp = BitConverter.GetBytes(Timestamp);
            Array.Copy(timestamp, data, 8);
            return data;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            Timestamp = BitConverter.ToInt64(messagePack, 0);
        }

    }
}
