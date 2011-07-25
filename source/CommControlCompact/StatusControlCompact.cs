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

namespace CommControlCompact
{
    /// <summary>
    /// Control grafico que permite administrar el estado de conexion a la red
    /// Eventos que gatilla:
    /// - connectNotification cuando el usuario quiere conectarse a la red (clic sobre el boton conectar)
    /// - disConnectNotification cuando el usuario quiere desconectarse d ela red (clic sobre el boton desconectar)
    /// - clearNotification cuando el sistema ha reseteado todos los datos relacionados con la MANET
    /// </summary>
    public partial class StatusControlCompact : UserControl
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        delegate void VoidArgumentCallBak();

        /// <summary>
        /// Modelo de evento de conexion
        /// </summary>
        public delegate void ConnectNotificationHandler();

        /// <summary>
        /// Se gatilla cuando el usuario quiere conectarse a la red
        /// </summary>
        public event ConnectNotificationHandler connectNotification;

        /// <summary>
        /// Modelo de Evento de desconección
        /// </summary>
        public delegate void DisConnectNotificationHandler();

        /// <summary>
        /// Se gatilla cuando el usuario quiere desconectarse de la red
        /// </summary>
        public event DisConnectNotificationHandler disConnectNotification;

        /// <summary>
        /// Guarda el estado actual de la conexion
        /// </summary>
        private Int32 state;

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusControlCompact()
        {
            InitializeComponent();
            state = CommunicationState.STOPPED;
        }

        /// <summary>
        /// El tamaño de los íconos que muestran el estado de la conexión
        /// </summary>
        public Size ImageSize
        {
            get { return imageList.ImageSize; }
            set { imageList.ImageSize = value; }
        }

        /// <summary>
        /// El estado actual de conexión a la MANET (un valor de CommunicationState)
        /// </summary>
        public int State
        {
            get { return state; }
        }

        /// <summary>
        /// Cambia el estado a conectado
        /// </summary>
        public void connectHandler()
        {
            if (pictureBox.InvokeRequired || labelStatus.InvokeRequired || buttonConnect.InvokeRequired || buttonDisconnect.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(connectHandler);
                this.Invoke(d);
            }
            else
            {
                state = CommunicationState.STARTED;
                pictureBox.Image = imageList.Images[2];
                labelStatus.Text = "Connected";
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
            }
        }

        /// <summary>
        /// Cambia el estado a desconectado
        /// </summary>
        public void disconnectHandler()
        {
            if (pictureBox.InvokeRequired || labelStatus.InvokeRequired || buttonConnect.InvokeRequired || buttonDisconnect.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(disconnectHandler);
                this.Invoke(d);
            }
            else
            {
                state = CommunicationState.STOPPED;
                pictureBox.Image = imageList.Images[0];
                labelStatus.Text = "Disconnected";
                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
            }
        }

        /// <summary>
        /// Cambia el estado a conectando
        /// </summary>
        public void connectingHandler()
        {
            if (pictureBox.InvokeRequired || labelStatus.InvokeRequired || buttonConnect.InvokeRequired || buttonDisconnect.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(connectingHandler);
                this.Invoke(d);
            }
            else
            {
                state = CommunicationState.STARTING;
                pictureBox.Image = imageList.Images[1];
                labelStatus.Text = "Connecting...";
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
            }
        }

        /// <summary>
        /// Cambia el estado a desconectando
        /// </summary>
        public void disconnectingHandler()
        {
            if (pictureBox.InvokeRequired || labelStatus.InvokeRequired || buttonConnect.InvokeRequired || buttonDisconnect.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(disconnectingHandler);
                this.Invoke(d);
            }
            else
            {
                state = CommunicationState.STOPPING;
                pictureBox.Image = imageList.Images[1];
                labelStatus.Text = "Disconnecting...";
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = false;
            }
        }

        /// <summary>
        /// Cambia el estado a reconectando
        /// </summary>
        public void reconnectingHandler()
        {
            if (pictureBox.InvokeRequired || labelStatus.InvokeRequired || buttonConnect.InvokeRequired || buttonDisconnect.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(reconnectingHandler);
                this.Invoke(d);
            }
            else
            {
                state = CommunicationState.STARTING;
                pictureBox.Image = imageList.Images[1];
                labelStatus.Text = "Reconnecting...";
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
            }
        }

        /// <summary>
        /// Evento para el botón de conección
        /// </summary>
        /// <param name="sender">El sender del evento</param>
        /// <param name="e">Los argumentos del evento</param>
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            connectNotification();
        }

        /// <summary>
        /// Evento para el botón de desconección
        /// </summary>
        /// <param name="sender">El sender del evento</param>
        /// <param name="e">Los argumentos del evento</param>
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            disConnectNotification();
        }
    }
}
