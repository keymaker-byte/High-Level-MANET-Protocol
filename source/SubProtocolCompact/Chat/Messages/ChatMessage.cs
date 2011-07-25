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

namespace SubProtocolCompact.Chat.Messages
{
    /// <summary>
    /// Representa un mensaje de texto global
    /// Protocolo Safe TCP
    /// </summary>
    public class ChatMessage : SafeUnicastMessage 
    {
        /// <summary>
        /// El texto de este mensaje
        /// </summary>
        private String _text;

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        public ChatMessage() : base()
        {
            this.Type = SubProtocolCompact.Chat.Types.CHATMESSAGE;
            this.ProtocolType = SubProtocolCompact.Chat.Types.CHATPROTOCOL;
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="targetNetUser">El usuario destinatario de este mensaje</param>
        /// <param name="text">El texto del mensaje</param>
        public ChatMessage(NetUser targetNetUser, String text) : this()
        {
            this.TargetNetUser = targetNetUser;
            this.Text = text;
        }

        /// <summary>
        /// El texto del mensaje
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[]  makePack()
        {
            byte[] text = Encoding.Unicode.GetBytes(Text);
            return text;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            Text = Encoding.Unicode.GetString(messagePack, 0, messagePack.Length);
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "TextMessage: Text=" + Text;
        }
    }
}
