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
using CommLayer;
using CommLayer.Messages;
using SubProtocol.FileTransfer.Messages;
using SubProtocol.FileTransfer.ControlI;
using CommControl.Util;

namespace CommControl
{
    /// <summary>
    /// Control gráfico para la transferencia de archivos
    /// </summary>
    public partial class FileControlCompact : UserControl, ControlFileHandlerI
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="netUser">Un usuario de red</param>
        /// <param name="s1">Un string</param>
        /// <param name="s2">Un string</param>
        delegate void NetUserCallback(NetUser netUser, String s1, String s2);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un string</param>
        delegate void StringCallback(String s);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un string</param>
        /// <param name="number">Un número entero</param>
        delegate void StringValueCallback(String s, Int32 number);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un string</param>
        /// <param name="number">Un número entero</param>
        delegate void StringStringCallback(String s1, String s2);

        /// <summary>
        /// Lista de archivos encolados o procesados
        /// </summary>
        private List<FileItem> fileList;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileControlCompact()
        {
            InitializeComponent();
            fileList = new List<FileItem>();
        }

        /// <summary>
        /// El tamaño de la imágen de los íconos de los usuarios
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
            set { treeView.Indent = value; }
        }

        /// <summary>
        /// Borra la información de la ventana de transferencia, a excepción del historial de archivos
        /// </summary>
        public void clear()
        {
            foreach (FileItem fileItem in fileList)
            {
                if (fileItem.State.Equals(FileControlState.LOADINGDATA) || fileItem.State.Equals(FileControlState.QUEUED) || fileItem.State.Equals(FileControlState.WAITING))
                {
                    fileItem.State = FileControlState.FILEFAIL;
                    refreshList(fileItem);
                }
            }
        }

        /// <summary>
        /// Agrega un archivo para descarga a la cola
        /// </summary>
        /// <param name="netUser">El usuario de la red dueño del archivo</param>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="fileName">El nombre del archivo</param>
        public void downloadFileQueued(NetUser netUser, String fileHandlerId, String fileName)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(downloadFileQueued);
                this.Invoke(d, new object[] { netUser, fileHandlerId, fileName });
            }
            else
            {
                FileItem fileItem = new FileItem();
                fileItem.NetUserName = netUser.Name;
                fileItem.FileName = fileName;
                fileItem.Percent = 0;
                fileItem.State = FileControlState.QUEUED;
                fileItem.Type = FileControlStateType.DOWNLOAD;
                fileItem.FileHandlerId = fileHandlerId;
                fileList.Add(fileItem);
                refreshList(fileItem);
            }
        }

        /// <summary>
        /// Marca como atendido a un archivo previamente encolado para descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void downloadFileOpened(String fileHandlerId)
        {
            if (treeView.InvokeRequired)
            {
                StringCallback d = new StringCallback(downloadFileOpened);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.QUEUED;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Aumenta la barra de porcentaje del archivo en proceso de descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="percent">el porcentaje total que se ha transferido</param>
        public void downloadFileTransfer(String fileHandlerId, Int32 percent)
        {
            if (treeView.InvokeRequired)
            {
                StringValueCallback d = new StringValueCallback(downloadFileTransfer);
                this.Invoke(d, new object[] { fileHandlerId, percent });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.LOADINGDATA;
                        fileItem.Percent = percent;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Marca como completado al archivo en proceso de descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void downloadFileComplete(String fileHandlerId, String path)
        {
            if (treeView.InvokeRequired)
            {
                StringStringCallback d = new StringStringCallback(downloadFileComplete);
                this.Invoke(d, new object[] { fileHandlerId, path });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.FILECOMPLETE;
                        fileItem.Percent = 100;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Marca como fallido al archivo en proceso de descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void downloadFileFailed(String fileHandlerId)
        {
            if (treeView.InvokeRequired)
            {
                StringCallback d = new StringCallback(downloadFileFailed);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.FILEFAIL;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Encola a un archivo para transferencia (upload)
        /// </summary>
        /// <param name="netUser">El usuario de la red al cual se le debe transferir el archivo</param>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="fileName">El nombre del archivo</param>
        public void uploadFileQueued(NetUser netUser, String fileHandlerId, String fileName)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(uploadFileQueued);
                this.Invoke(d, new object[] { netUser, fileHandlerId, fileName });
            }
            else
            {
                FileItem fileItem = new FileItem();
                fileItem.NetUserName = netUser.Name;
                fileItem.FileName = fileName;
                fileItem.Percent = 0;
                fileItem.State = FileControlState.QUEUED;
                fileItem.Type = FileControlStateType.UPLOAD;
                fileItem.FileHandlerId = fileHandlerId;
                fileList.Add(fileItem);
                refreshList(fileItem);
            }
        }

        /// <summary>
        /// Marca como atendido a un archivo previamente encolado para transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void uploadFileOpened(String fileHandlerId)
        {
            if (treeView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileOpened);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.LOADINGDATA;
                        fileItem.Percent = 0;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Aumenta la barra de progreso del archivo en proceso de transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="percent">porcentaje total que se ha enviado del archivo</param>
        public void uploadFileTransfer(String fileHandlerId, Int32 percent)
        {
            if (treeView.InvokeRequired)
            {
                StringValueCallback d = new StringValueCallback(uploadFileTransfer);
                this.Invoke(d, new object[] { fileHandlerId, percent });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.LOADINGDATA;
                        fileItem.Percent = percent;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Marca como completado al archivo en proceso de transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void uploadFileComplete(String fileHandlerId)
        {
            if (treeView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileComplete);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.FILECOMPLETE;
                        fileItem.Percent = 100;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Marca como fallido al archivo en proceso de transferencia.
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void uploadFileFailed(String fileHandlerId)
        {
            if (treeView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileFailed);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (FileItem fileItem in fileList)
                {
                    if (fileItem.FileHandlerId.Equals(fileHandlerId))
                    {
                        fileItem.State = FileControlState.FILEFAIL;
                        refreshList(fileItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Recarga el historial de archivos encolados para transferencias y/o descargas
        /// </summary>
        private void refreshList(FileItem fileItem)
        {
            bool exist = false;
            foreach (TreeNode node in treeView.Nodes)
            {
                String filehandlerID = (String)node.Tag;
                if (filehandlerID.Equals(fileItem.FileHandlerId))
                {
                    node.Text = fileItem.ToString();
                    exist = true;
                    break;
                }
            }
            if (!exist)
            {
                TreeNode fileNode = new TreeNode(fileItem.ToString());
                fileNode.ImageIndex = 0;
                fileNode.SelectedImageIndex = 0;
                fileNode.Tag = fileItem.FileHandlerId;
                treeView.Nodes.Add(fileNode);
            }
        }
    }

    /// <summary>
    /// Clase que enumero los posibles estados del control gráfico de archivos
    /// </summary>
    internal class FileControlState
    {
        /// <summary>
        /// Constante para el estado esperando
        /// </summary>
        public const String WAITING = "Waiting";

        /// <summary>
        /// Constante para el estado activo
        /// </summary>
        public const String LOADINGDATA = "Transfering";

        /// <summary>
        /// Constante para el estado en transferencia
        /// </summary>
        public const String FILECOMPLETE = "Completed";

        /// <summary>
        /// Constante para el estado fallido
        /// </summary>
        public const String FILEFAIL = "Failed";

        /// <summary>
        /// Constante para el estado en cola de espera
        /// </summary>
        public const String QUEUED = "Queued";
    }

    /// <summary>
    /// Clase que enumero los posibles tipos de transferencia en el control gráfico de archivos
    /// </summary>
    internal class FileControlStateType
    {
        /// <summary>
        /// Constante para el tipo transferencia  - upload
        /// </summary>
        public const String UPLOAD = "Upload";

        /// <summary>
        /// Constante para el tipo descarga
        /// </summary>
        public const String DOWNLOAD = "Download";
    }
}
