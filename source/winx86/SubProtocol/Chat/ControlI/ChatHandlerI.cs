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

namespace SubProtocol.Chat.ControlI
{
    /// <summary>
    /// Interfaz para eventos de mensajes de texto globales
    /// </summary>
    public interface ChatHandlerI
    {        
        /// <summary>
        /// Se gatilla cuando se ha recibido un nuevo mensaje de texto de un usuario
        /// </summary>
        /// <param name="netUser">El autor del mensaje</param>
        /// <param name="message">El mensaje de texto</param>
        void chatMessageReceived(NetUser netUser, String message);

        /// <summary>
        /// Se gatilla cuando se ha recibido un nuevo mensaje de texto a nivel grupal
        /// </summary>
        /// <param name="netUser">El autor del mensaje</param>
        /// <param name="message">El mensaje de texto</param>
        void groupChatMessageReceived(NetUser netUser, String message);

        /// <summary>
        /// Se gatilla cuando se recibe un mensaje de warning del protocolo
        /// </summary>
        /// <param name="text">el texto de información</param>
        void chatWarninglInformation(String text);
    }
}
