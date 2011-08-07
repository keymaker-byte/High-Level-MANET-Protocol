/***************************************************************************
----------------------------------------------------------------------------
  This file is part of the HLMP API - File Sharing Sample Application.
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommControl;
using CommLayer;
using CommLayer.Messages;
using SubProtocol;
using SubProtocol.Chat;
using SubProtocol.FileTransfer;
using SubProtocol.Ping;
using OpenNETCF;
using System.IO;

namespace HLMP
{
    /// <summary>
    /// Programa de ejemplo de transferencia de archivos
    /// </summary>
    public partial class MainForm : Form, SubProtocol.Ping.ControlI.PingHandlerI
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        delegate void VoidArgumentCallBak();

        /// <summary>
        /// CallBacks para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="message">un string</param>
        delegate void NetUserDoubleCallback(NetUser netUser, Double d);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        delegate void MessageCallback(Message Message);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        delegate void StringCallback(String message);

        /// <summary>
        /// objeto de comunicación a la MANET
        /// </summary>
        private Communication communication;

        /// <summary>
        /// Objeto de configuración de la MANET
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Protocolo de chat
        /// </summary>
        private ChatProtocol chatProtocol;

        /// <summary>
        /// Protocolo de transferencia de archivos
        /// </summary>
        private FileTransferProtocol fileTransferProtocol;

        /// <summary>
        /// Protocolo de ping
        /// </summary>
        private PingProtocol pingProtocol;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            try
            {
                //se inicializa la configuración
                configuration = new Configuration();
                configuration.NetData.OpSystem = NetLayer.OpSystemType.SAMSUNGOMNIAII;

                //Configuración de controles
                statusControl.connectNotification += connect;
                statusControl.disConnectNotification += disconnect;
                chatControl.NetUser = configuration.NetUser;

                //Configuración de subprotocolos
                SubProtocolList subProtocols = new SubProtocolList();
                chatProtocol = new ChatProtocol(chatControl);
                subProtocols.add(SubProtocol.Chat.Types.CHATPROTOCOL, chatProtocol);
                fileTransferProtocol = new FileTransferProtocol(fileControl, fileListControl, createFileData());
                subProtocols.add(SubProtocol.FileTransfer.Types.FILETRANSFERPROTOCOL, fileTransferProtocol);
                pingProtocol = new PingProtocol(this);
                subProtocols.add(SubProtocol.Ping.Types.PINGPROTOCOL, pingProtocol);

                chatControl.ChatProtocol = chatProtocol;
                fileListControl.FileTransferProtocol = fileTransferProtocol;

                //se crea el objeto de comunicación
                communication = new Communication(configuration, subProtocols, null);

                //Se setean los handlers de los eventos de usuarios
                communication.addUserEvent += netUserControl.addUserHandler;
                communication.addUserEvent += fileTransferProtocol.sendFileListRequest;
                communication.refreshUserEvent += netUserControl.refreshUserHandler;
                communication.refreshLocalUserEvent += netUserControl.refreshLocalUserHandler;
                communication.removeUserEvent += netUserControl.removeUserHandler;
                communication.removeUserEvent += fileListControl.removeFileList;

                //Se setean los handlers de los eventos de conexion
                communication.connectEvent += statusControl.connectHandler;
                communication.connectEvent += fileTransferProtocol.start;
                communication.connectingEvent += statusControl.connectingHandler;
                communication.disconnectEvent += statusControl.disconnectHandler;
                communication.disconnectEvent += fileTransferProtocol.stop;
                communication.disconnectEvent += clear;
                communication.disconnectingEvent += statusControl.disconnectingHandler;
                communication.reconnectingEvent += statusControl.reconnectingHandler;
                communication.reconnectingEvent += fileTransferProtocol.stop;
                communication.reconnectingEvent += clear;

                //se setean los handlers de los eventos del sistema
                communication.exceptionEvent += exceptionHandler;
                communication.netInformationEvent += log;

                //iniciamos el consumidor de eventos
                communication.startEventConsumer();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ups!: " + e.Message);
                Close();
            }
        }

        /// <summary>
        /// Maneja el evento de cierre de la ventana
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (communication != null)
            {
                communication.disconnect();
                communication.stopEventConsumer();
            }
            Hide();
            Close();
            base.OnClosing(e);
        }

        /// <summary>
        /// Recibe una excepción del sistema
        /// </summary>
        /// <param name="e">la excepción generada</param>
        public void exceptionHandler(Exception e)
        {
            MessageBox.Show(e.Message);
        }

        /// <summary>
        /// Conecta a la MANET
        /// </summary>
        private void connect()
        {
            communication.connect();
        }

        /// <summary>
        /// Desconecta de la MANET
        /// </summary>
        private void disconnect()
        {
            communication.disconnectAsync();
        }

        /// <summary>
        /// Limpia las ventanas
        /// </summary>
        public void clear()
        {
            if (netUserControl.InvokeRequired || chatControl.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d, new object[] { });
            }
            else
            {
                netUserControl.clear();
                fileControl.clear();
                fileListControl.clear();
            }
        }

        /// <summary>
        /// Acción para el botón Salir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Escribe el log en el cuadro de texto
        /// </summary>
        /// <param name="logInformation">la informacion de log</param>
        private void log(String logInformation)
        {
            if (textBoxLog.InvokeRequired)
            {
                StringCallback d = new StringCallback(log);
                this.Invoke(d, new object[] { logInformation });
            }
            else
            {
                textBoxLog.Text = (logInformation) + Environment2.NewLine + textBoxLog.Text;
            }
        }

        /// <summary>
        /// Crea el archivo de informacion de recursos compartidos
        /// </summary>
        /// <returns>el archivo de file data para el protocolo de transferencia de archivos</returns>
        private FileData createFileData()
        {
            FileData fileData = new FileData();
            //crea el directorio en caso de que no exista
            String actualDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            String dir = Path.Combine(actualDir, "compartida");
            Directory.CreateDirectory(dir);

            //lee los archivos contenidos en el directorio
            FileInfo[] files = new DirectoryInfo(dir).GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                Int64 size = fs.Length;
                fs.Close();
                fileData.FileList.add(new FileInformation(fileInfo.Name, size, fileInfo.FullName));
            }
            fileData.TimeIntervalTimer = 100;
            return fileData;
        }

        /// <summary>
        /// Se gatilla cuando se ha recibido la confirmacion de un ping
        /// </summary>
        /// <param name="netUser">El autor de la respuesta de ping</param>
        /// <param name="milliseconds">Milisegundos totales</param>
        public void pingResponseMessageReceived(NetUser netUser, Double milliseconds)
        {
            if (textBoxPing.InvokeRequired)
            {
                NetUserDoubleCallback d = new NetUserDoubleCallback(pingResponseMessageReceived);
                this.Invoke(d, new object[] { netUser, milliseconds });
            }
            else
            {
                textBoxPing.Text = "respuesta desde..." + netUser.Name + Environment2.NewLine + textBoxPing.Text;
                textBoxPing.Text = "tiempo de mensaje..." + (milliseconds / 1000.0) + " segundos" + Environment2.NewLine + textBoxPing.Text;
            } 
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            textBoxPing.Text = "Enviando Ping a todos los usuarios..." + Environment2.NewLine + textBoxPing.Text;
            ///Envio un ping a todos los usuarios conocidos (se los pedimos a communication)
            NetUser[] netUserList = communication.NetUserList.userListToArray();
            foreach (NetUser netUser in netUserList)
            {
                pingProtocol.sendPingMessage(netUser);
            }
        }

    }
}