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
using System.Text;
using System.Windows.Forms;
using CommLayer;
using SubProtocol.FileTransfer.ControlI;
using SubProtocol.FileTransfer;
using CommControl.Util;

namespace CommControl
{
    /// <summary>
    /// Control gráfico para la lista de archivos compartidos en la red que se pueden descargar
    /// Eventos que gatilla:
    /// fileDownloadNotification cuando el usuario quiere descargar un archivo de la red (clic sobre el la opcion descargar de un archivo)
    /// </summary>
    public partial class FileListControl : UserControl, FileListHandlerI
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
        /// Subprotocol
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
        public FileListControl()
        {
            InitializeComponent();
            thisLock = new Object();
        }

        /// <summary>
        /// Limpia la lista de archivos (elimina todos los archivos de la lista)
        /// </summary>
        public void clear()
        {
            if (treeView.InvokeRequired )
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
                        assignMenuStrip(ref fileNode, netUserFile);
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
        /// Asigna un menú con opción de descarga a un nodos del arbol de archivos
        /// </summary>
        /// <param name="treeNode">El nodo del árbol de archivos</param>
        /// <param name="netUserFile">El archivo asociado a un usuario de red</param>
        private void assignMenuStrip(ref TreeNode treeNode, NetUserFile netUserFile)
        {
            ContextMenuStrip contextMenucontextMenu = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "Download";
            toolStripMenuItem.Click += toolStripEvent;
            toolStripMenuItem.Tag = netUserFile;
            contextMenucontextMenu.Items.Add(toolStripMenuItem);
            treeNode.ContextMenuStrip = contextMenucontextMenu;
        }

        /// <summary>
        /// Se gatilla cuando el usuario hace clic sobre un menu con opción de descarga
        /// </summary>
        /// <param name="sender">El sender</param>
        /// <param name="e">Los argumentos de evento</param>
        private void toolStripEvent(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            NetUserFile netUserFile = (NetUserFile)toolStripMenuItem.Tag;
            if (netUserFile != null)
            {
                FileTransferProtocol.sendFileRequest(netUserFile.NetUser, netUserFile.FileInformation);
            }
        }
    }
}
