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
using System.IO;
using CommControl;
using CommLayer;
using CommLayer.Messages;
using SubProtocol;
using SubProtocol.Chat;
using SubProtocol.FileTransfer;
using SubProtocol.Ping;

namespace HLMPFileSharing
{
    /// <summary>
    /// Programa de Ejemplo de transferencia de archivos
    /// </summary>
    public partial class MainWindow : Form, SubProtocol.Ping.ControlI.PingHandlerI
    {
        
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        delegate void VoidArgumentCallBak();
        
        /// <summary>
        /// CallBacks para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="message">un string</param>
        delegate void StringCallback(String message);

        /// <summary>
        /// CallBacks para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="message">un string</param>
        delegate void NetUserDoubleCallback(NetUser netUser, Double d);

        /// <summary>
        /// CallBacks para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="message">un mensage</param>
        delegate void MessageCallback(CommLayer.Messages.Message message);

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
        /// Escritor de archivo para log
        /// </summary>
        private StreamWriter Tex;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            try
            {
                //incializamos una configuracion
                configuration = new Configuration();

                //configuramos los controles
                statusControl.connectNotification += connect;
                statusControl.disConnectNotification += disconnect;
                chatControl.NetUser = configuration.NetUser;

                //configuramos los sub protocolos
                SubProtocolList subProtocols = new SubProtocolList();
                chatProtocol = new ChatProtocol(chatControl);
                subProtocols.add(SubProtocol.Chat.Types.CHATPROTOCOL, chatProtocol);
                fileTransferProtocol = new FileTransferProtocol(fileControl, fileListControl, createFileData());
                subProtocols.add(SubProtocol.FileTransfer.Types.FILETRANSFERPROTOCOL, fileTransferProtocol);
                pingProtocol = new PingProtocol(this);
                subProtocols.add(SubProtocol.Ping.Types.PINGPROTOCOL, pingProtocol);

                chatControl.ChatProtocol = chatProtocol;
                fileListControl.FileTransferProtocol = fileTransferProtocol;

                //Configuramos los tipos de mensajes que no pertenecen a subprotocolos
                MessageTypeList messageTypeList = new MessageTypeList();

                //se crea el objeto de comunicación
                communication = new Communication(configuration, subProtocols, messageTypeList);

                //Se setean los handlers de los eventos de usuarios
                communication.addUserEvent += netUserControl.addUserHandler;
                communication.addUserEvent += fileTransferProtocol.sendFileListRequest;
                communication.addUserEvent += networkGraphControl.addUserHandler;
                communication.refreshUserEvent += netUserControl.refreshUserHandler;
                communication.refreshUserEvent += networkGraphControl.refreshUserHandler;
                communication.refreshLocalUserEvent += netUserControl.refreshLocalUserHandler;
                communication.refreshLocalUserEvent += networkGraphControl.refreshLocalUserHandler;
                communication.removeUserEvent += netUserControl.removeUserHandler;
                communication.removeUserEvent += fileListControl.removeFileList;
                communication.removeUserEvent += networkGraphControl.removeUserHandler;

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

                //Se setean los handlers de los eventos del sistems
                communication.exceptionEvent += exceptionHandler;
                communication.netInformationEvent += netInformationHandler;

                //iniciamos el consumidor de eventos
                communication.startEventConsumer();

                //Se abre el archivo para log
                FileInfo t = new FileInfo("CommLayer.log");
                Tex = t.AppendText();
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
            if (Tex != null)
            {
                Tex.Close();
            }
            Hide();
            base.OnClosing(e);
        }

        /// <summary>
        /// Recibe eventos del sistema
        /// Los eventos recibidos son agregados al archivo de log
        /// </summary>
        /// <param name="message">El mensaje que ha enviado el sistema de red</param>
        public void netInformationHandler(String message)
        {
            String log = DateTime.Now.ToString() + " - " + message + Environment.NewLine;
            Tex.Write(log);
            Tex.Flush();
        }

        /// <summary>
        /// Recibe una excepción del sistema
        /// El error es mostrado en una ventana modal
        /// </summary>
        /// <param name="e">la excepción generada</param>
        public void exceptionHandler(Exception e)
        {
            MessageBox.Show(e.Message);
            netInformationHandler(e.StackTrace);
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
            if (communication != null)
            {
                communication.disconnectAsync();
            }
        }

        /// <summary>
        /// Lo gatillamos cuando se desconecta asì limpiamos los controles
        /// </summary>
        public void clear()
        {
            if (netUserControl.InvokeRequired || fileControl.InvokeRequired || fileListControl.InvokeRequired || networkGraphControl.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d, new object[] { });
            }
            else
            {
                netUserControl.clear();
                fileListControl.clear();
                fileControl.clear();
                networkGraphControl.clear();
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
            String actualDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
                TreeNode fileNode = new TreeNode(fileInfo.Name + " (" + (fileInfo.Length / 1024) + " KB) ");
                fileNode.ImageIndex = 0;
                fileNode.SelectedImageIndex = 0;
                treeViewMyFiles.Nodes.Add(fileNode);
            }

            return fileData;
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            textBoxPing.AppendText("Enviando Ping a todos los usuarios..." + Environment.NewLine);
            ///Envio un ping a todos los usuarios conocidos (se los pedimos a communication)
            NetUser[] netUserList = communication.NetUserList.userListToArray();
            foreach (NetUser netUser in netUserList)
            {
                pingProtocol.sendPingMessage(netUser);
            }
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
                textBoxPing.AppendText("respuesta desde..." + netUser.Name + Environment.NewLine);
                textBoxPing.AppendText("tiempo de mensaje..." + (milliseconds / 1000.0) + " segundos" + Environment.NewLine);
            }
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog().Equals(DialogResult.OK))
            {
                if (openFileDialog.FileName != null)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                    Int64 size = fs.Length;
                    fs.Close();
                    fileTransferProtocol.FileData.FileList.add(new FileInformation(fileInfo.Name, size, fileInfo.FullName));
                    TreeNode fileNode = new TreeNode(fileInfo.Name + " (" + (fileInfo.Length / 1024) + " KB) ");
                    fileNode.ImageIndex = 0;
                    fileNode.SelectedImageIndex = 0;
                    treeViewMyFiles.Nodes.Add(fileNode);
                    NetUser[] netUserList = communication.NetUserList.userListToArray();
                    foreach (NetUser netUser in netUserList)
                    {
                        fileTransferProtocol.sendFileList(netUser);
                    }
                }
            }
        }
    }
}