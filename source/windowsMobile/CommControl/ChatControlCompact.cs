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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommLayerCompact;
using CommLayerCompact.Messages;
using OpenNETCF;
using SubProtocolCompact.Chat.ControlI;
using SubProtocolCompact.Chat;

namespace CommControlCompact
{
    /// <summary>
    /// Control gráfico para envío y recepción de mensajes de texto globales
    /// Eventos que gatilla:
    /// messageNotification cuando el usuario quiere enviar un mensaje de texto (clic sobre el boton envíar)
    /// </summary>
    public partial class ChatControlCompact : UserControl, ChatHandlerI
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="netUser">Un usuario de red</param>
        /// <param name="s">Un string</param>
        delegate void NetUserStringCallback(NetUser netUser, String s);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un string</param>
        delegate void StringCallback(String s);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        private delegate void VoidArgumentCallBak();
        
        /// <summary>
        /// El usuario local 
        /// </summary>
        private NetUser netUser;

        private ChatProtocol _chatProtocol;

        /// <summary>
        /// Protocolo que maneja este handler
        /// </summary>
        public ChatProtocol ChatProtocol
        {
            get { return _chatProtocol; }
            set { _chatProtocol = value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ChatControlCompact()
        {
            InitializeComponent();
        }

        /// <summary>
        /// El usuario dueño de esta ventana de Chat, su nombre se muestra al escribir un mensaje
        /// </summary>
        public NetUser NetUser
        {
            get { return netUser; }
            set { netUser = value; }
        }

        /// <summary>
        /// Agrega un mensaje de texto a la ventana
        /// </summary>
        /// <param name="netUser">El usuario autor del mensaje</param>
        /// <param name="message">El mensaje de texto</param>
        public void chatMessageReceived(NetUser netUser, String message)
        {
            if (textBoxText.InvokeRequired)
            {
                NetUserStringCallback d = new NetUserStringCallback(chatMessageReceived);
                this.Invoke(d, new object[] { netUser, message });
            }
            else
            {
                 textBoxText.Text = ("[" + (netUser != null ? netUser.Name : "unknown") + "] say: " + message) + Environment2.NewLine + textBoxText.Text;
            } 
        }

        /// <summary>
        /// Agrega un mensaje de texto a la ventana grupal
        /// </summary>
        /// <param name="netUser">El usuario autor del mensaje</param>
        /// <param name="message">El mensaje de texto</param>
        public void groupChatMessageReceived(NetUser netUser, String message)
        {
            if (textBoxText.InvokeRequired)
            {
                NetUserStringCallback d = new NetUserStringCallback(groupChatMessageReceived);
                this.Invoke(d, new object[] { netUser, message });
            }
            else
            {
                textBoxText.Text = ("[" + (netUser != null ? netUser.Name : "unknown") + "] say: " + message) + Environment2.NewLine + textBoxText.Text;
            }
        }

        /// <summary>
        /// Agrega un mensaje del sistema a la ventana
        /// <param name="text">el texto de información</param>
        /// </summary>
        public void chatWarninglInformation(String text)
        {
            if (textBoxText.InvokeRequired)
            {
                StringCallback d = new StringCallback(chatWarninglInformation);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                textBoxText.Text = ("WARNING: " + text) + Environment2.NewLine + textBoxText.Text;
            }
        }

        /// <summary>
        /// Borra todo el texto de la ventana de chat
        /// </summary>
        public void clear()
        {
            if (textBoxText.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d);
            }
            else
            {
                textBoxText.Text = "";
            }
        }

        /// <summary>
        /// Método gatillado al hacer clic en el boton enviar
        /// </summary>
        /// <param name="sender">El sender del evento</param>
        /// <param name="e">Los parametros del evento</param>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxMessage.Text.Length > 0)
            {
                ChatProtocol.sendTextMessage(null, textBoxMessage.Text);
                chatMessageReceived(netUser, textBoxMessage.Text);
                textBoxMessage.Text = "";
            }
        }

        /// <summary>
        /// Método gatillado al presionar una tecla teniendo foco del textBox mensaje
        /// </summary>
        /// <param name="sender">quien envia el evento</param>
        /// <param name="e">los parametros del evento</param>
        private void textBoxMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('\r'))
            {
                if (textBoxMessage.Text.Length > 0)
                {
                    ChatProtocol.sendTextMessage(null, textBoxMessage.Text);
                    chatMessageReceived(netUser, textBoxMessage.Text);
                    textBoxMessage.Text = "";
                }
            }
        }
    }
}
