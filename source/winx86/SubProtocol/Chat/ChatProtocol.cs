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
using SubProtocol.Chat.Messages;
using SubProtocol.Chat.ControlI;
using System.Collections;

namespace SubProtocol.Chat
{

    /// <summary>
    /// Clase que enumera los tipos de mensajes usados en el protocolo
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// Constante para tipo chat
        /// </summary>
        public const Int32 CHATPROTOCOL = 100;
        
        /// <summary>
        /// Constante para el tipo texto
        /// </summary>
        public const Int32 CHATMESSAGE = 100;

        /// <summary>
        /// Constante para el tipo texto grupal (multicast)
        /// </summary>
        public const Int32 GROUPCHATMESSAGE = 101;
    }

    /// <summary>
    /// Protocolo de envio y recepcion de mensajes de texto
    /// </summary>
    public class ChatProtocol : SubProtocolI
    {
        /// <summary>
        /// Handler de eventos de mensajes de texto
        /// </summary>
        private ChatHandlerI controlChatHandler;

        /// <summary>
        /// Se gatilla cuando el protocolo quiere enviar un mensaje a la red
        /// </summary>
        public event Communication.MessageEvent sendMessageEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controlChatHandler">Un manejador de eventos de chat</param>
        public ChatProtocol(ChatHandlerI controlChatHandler)
        {
            this.controlChatHandler = controlChatHandler;
        }

        /// <summary>
        /// Obtiene la lista de tipos de mensajes usados por este protocolo
        /// </summary>
        /// <returns>Una tabla con valores y tipos de mensajes usados en el protocolo</returns>
        public MessageTypeList getMessageTypes()
        {
            MessageTypeList typeCollection = new MessageTypeList();
            typeCollection.add(SubProtocol.Chat.Types.CHATMESSAGE, typeof(ChatMessage));
            typeCollection.add(SubProtocol.Chat.Types.GROUPCHATMESSAGE, typeof(GroupChatMessage));
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
                case SubProtocol.Chat.Types.CHATMESSAGE:
                    {
                        ChatMessage textMessage = (ChatMessage)message;
                        controlChatHandler.chatMessageReceived(textMessage.SenderNetUser, textMessage.Text);
                        break;
                    }
                case SubProtocol.Chat.Types.GROUPCHATMESSAGE:
                    {
                        GroupChatMessage textMessage = (GroupChatMessage)message;
                        controlChatHandler.groupChatMessageReceived(textMessage.SenderNetUser, textMessage.Text);
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
            switch (message.Type)
            {
                case SubProtocol.Chat.Types.CHATMESSAGE:
                    {
                        ChatMessage textMessage = (ChatMessage)message;
                        controlChatHandler.chatWarninglInformation("No se ha podido entregar el mensage: " + textMessage.Text + ". Al usuario: " + textMessage.TargetNetUser.Name);
                        break;
                    }
                case SubProtocol.Chat.Types.GROUPCHATMESSAGE:
                    {
                        GroupChatMessage textMessage = (GroupChatMessage)message;
                        controlChatHandler.chatWarninglInformation("No se ha podido enviar el mensage: " + textMessage.Text);
                        break;
                    }
            }
        }

        /// <summary>
        /// Envía un mensaje de texto a la red
        /// </summary>
        /// <param name="netUser">El usuario destino del mensaje, null si es para todos</param>
        /// <param name="text">El texto del mensaje, si es de largo 0 no se envía</param>
        public void sendTextMessage(NetUser netUser, String text)
        {
            if (text.Length > 0)
            {
                if (netUser != null)
                {
                    ChatMessage textMessage = new ChatMessage(netUser, text);
                    sendMessageEvent(textMessage);
                }
                else
                {
                    GroupChatMessage textMessage = new GroupChatMessage(text);
                    sendMessageEvent(textMessage);
                }
            }
        }
    }
}
