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
using SubProtocolCompact.Ping.Messages;
using SubProtocolCompact.Ping.ControlI;
using System.Collections;

namespace SubProtocolCompact.Ping
{

    /// <summary>
    /// Clase que enumera los tipos de mensajes usados en el protocolo
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// Constante para tipo ping
        /// </summary>
        public const Int32 PINGPROTOCOL = 300;
        
        /// <summary>
        /// Constante para el tipo ping
        /// </summary>
        public const Int32 PINGMESSAGE = 300;

        /// <summary>
        /// Constante para el tipo ping response
        /// </summary>
        public const Int32 PINGRESPONSEMESSAGE = 301;
    }

    /// <summary>
    /// Protocolo de envio y recepcion de pings
    /// </summary>
    public class PingProtocol : SubProtocolI
    {
        /// <summary>
        /// Handler de eventos de ping
        /// </summary>
        private PingHandlerI controlPingHandler;

        /// <summary>
        /// Se gatilla cuando el protocolo quiere enviar un mensaje a la red
        /// </summary>
        public event Communication.MessageEvent sendMessageEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controlPingHandler">Un manejador de eventos de ping</param>
        public PingProtocol(PingHandlerI controlPingHandler)
        {
            this.controlPingHandler = controlPingHandler;
        }

        /// <summary>
        /// Obtiene la lista de tipos de mensajes usados por este protocolo
        /// </summary>
        /// <returns>Una tabla con valores y tipos de mensajes usados en el protocolo</returns>
        public MessageTypeList getMessageTypes()
        {
            MessageTypeList typeCollection = new MessageTypeList();
            typeCollection.add(SubProtocolCompact.Ping.Types.PINGMESSAGE, typeof(PingMessage));
            typeCollection.add(SubProtocolCompact.Ping.Types.PINGRESPONSEMESSAGE, typeof(PingResponseMessage));
            return typeCollection;
        }

        /// <summary>
        /// Procesa un mensaje recibido de la red
        /// </summary>
        /// <param name="message">El mensaje recibido</param>
        public void proccessMessage(Message message)
        {
            switch (message.Type)
            {
                case SubProtocolCompact.Ping.Types.PINGMESSAGE:
                    {
                        PingMessage pingMessage = (PingMessage)message;
                        PingResponseMessage pingResponseMessage = new PingResponseMessage(pingMessage.Timestamp, message.SenderNetUser);
                        sendMessageEvent(pingResponseMessage);
                        break;
                    }
                case SubProtocolCompact.Ping.Types.PINGRESPONSEMESSAGE:
                    {
                        PingResponseMessage pingResponseMessage = (PingResponseMessage)message;
                        controlPingHandler.pingResponseMessageReceived(pingResponseMessage.SenderNetUser, (new TimeSpan(DateTime.Now.Ticks - pingResponseMessage.Timestamp)).TotalMilliseconds);
                        break;
                    }
            }
        }

        /// <summary>
        /// Procesa un mensaje que no se ha podido enviar
        /// </summary>
        /// <param name="message">El mensaje que no se ha podido enviar</param>
        public void errorMessage(Message message)
        {
        }

        /// <summary>
        /// Envía un mensaje de ping a la red
        /// </summary>
        /// <param name="netUser">El usuario destino del mensaje</param>
        public void sendPingMessage(NetUser netUser)
        {
            PingMessage pingMessage = new PingMessage(netUser);
            sendMessageEvent(pingMessage);
        }
    }
}
