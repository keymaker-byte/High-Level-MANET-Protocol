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
using System.Threading;
using System.Windows.Forms;
using CommLayerCompact;
using SubProtocolCompact.FileTransfer.ControlI;
using SubProtocolCompact.FileTransfer;
using CommControlCompact.Util;

namespace CommControlCompact
{
    /// <summary>
    /// Control gráfico para la lista de archivos compartidos en la red que se pueden descargar
    /// Eventos que gatilla:
    /// fileDownloadNotification cuando el usuario quiere descargar un archivo de la red (clic sobre el la opcion descargar de un archivo)
    /// </summary>
    public partial class FileListControlCompact : UserControl, ControlFileListHandlerI
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="netUser">un usuario de la red</param>
        private delegate void NetUserCallback(NetUser netUser);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="netUser">Un usuario de la red</param>
        /// <param name="fileList">Una lista de archivos</param>
        private delegate void NetUserFilesCallback(NetUser netUser, FileInformationList fileList);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        private delegate void VoidArgumentCallBak();

        /// <summary>
        /// Protocol
        /// </summary>
        private FileTransferProtocol _fileTransferProtocol;

        /// <summary>
        /// Lock
        /// </summary>
        private Object thisLock;

        /// <summary>
        /// Protocolo que maneja este handler
        /// </summary>
        public FileTransferProtocol FileTransferProtocol
        {
            get { return _fileTransferProtocol; }
            set { _fileTransferProtocol = value; }
        }

        /// <summary>
        /// Dafault Constructor
        /// </summary>
        public FileListControlCompact()
        {
            InitializeComponent();
            thisLock = new Object();
        }

        /// <summary>
        /// Tamaño de los íconos de los nodos de cada archivo
        /// </summary>
        public Size ImageSize
        {
            get { return iconList.ImageSize; }
            set { iconList.ImageSize = value; }
        }

        /// <summary>
        /// Tamaño de la indentación de los nodos en el arbol
        /// </summary>
        public Int32 Indent
        {
            get { return treeView.Indent; }
            set { treeView.Indent= value; }
        }

        /// <summary>
        /// Limpia la lista de archivos (elimina todos los archivos de la lista)
        /// </summary>
        public void clear()
        {
            if (treeView.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d);
            }
            else
            {
                treeView.Nodes.Clear();
            }
        }

        /// <summary>
        /// Agrega una lista de archivos de un usuario de la red a la ventana 
        /// </summary>
        /// <param name="netUser">El usuario dueño de la lista de archivo</param>
        /// <param name="fileList">La lista de archivos</param>
        public void addFileList(NetUser netUser, FileInformationList fileList)
        {
            if (treeView.InvokeRequired)
            {
                NetUserFilesCallback d = new NetUserFilesCallback(addFileList);
                this.Invoke(d, new object[] { netUser, fileList });
            }
            else
            {
                removeFileList(netUser);
                FileInformation[] fileListArray = fileList.toArray();
                lock (thisLock)
                {
                    foreach (FileInformation file in fileListArray)
                    {
                        TreeNode fileNode = new TreeNode(file.Name + " (" + (file.Size / 1024) + " KB) " + " [" + netUser.Name + "]");
                        fileNode.ImageIndex = 0;
                        fileNode.SelectedImageIndex = 0;
                        NetUserFile netUserFile = new NetUserFile();
                        netUserFile.NetUser = netUser;
                        netUserFile.FileInformation = file;
                        fileNode.Tag = netUserFile;
                        treeView.Nodes.Add(fileNode);
                    } 
                }
            }
        }

        /// <summary>
        /// Elimina la lista de archivos de un usuario de la red de la ventana
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        public void removeFileList(NetUser netUser)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(removeFileList);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                lock (thisLock)
                {
                    for (int i = treeView.Nodes.Count - 1; i >= 0; i--)
                    {
                        TreeNode node = treeView.Nodes[i];
                        NetUserFile netUserFile = (NetUserFile)node.Tag;
                        if (netUserFile.NetUser.Name.Equals(netUser.Name))
                        {
                            treeView.Nodes.RemoveAt(i);
                        }
                    } 
                }
            }
        }

        /// <summary>
        /// Acción para el evento de clic sobre el menu de cada nodo
        /// </summary>
        /// <param name="sender">El sender</param>
        /// <param name="e">Los parametros del evento</param>
        private void menuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node != null)
            {
                try
                {
                    NetUserFile netUserFile = (NetUserFile)node.Tag;
                    FileTransferProtocol.sendFileRequest(netUserFile.NetUser, netUserFile.FileInformation);
                }
                catch (ThreadAbortException ex)
                {
                    throw ex;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
