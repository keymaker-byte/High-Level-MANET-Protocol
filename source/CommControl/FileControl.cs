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
using SubProtocol.FileTransfer.Messages;
using SubProtocol.FileTransfer.ControlI;

namespace CommControl
{
    /// <summary>
    /// Control gráfico para la transferencia de archivos
    /// </summary>
    public partial class FileControl : UserControl, FileHandlerI
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
        /// <param name="s">Un guid</param>
        delegate void StringCallback(String s);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un guid</param>
        delegate void StringStringCallback(String s1, String s2);
        
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="s">Un string</param>
        /// <param name="number">Un número entero</param>
        delegate void StringValueCallback(String s, Int32 number);

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Borra la información de la ventana de transferencia, a excepción del historial de archivos
        /// </summary>
        public void clear()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                String status = (String)(row.Cells[3].Value);
                if (status.Equals(FileControlState.LOADINGDATA) || status.Equals(FileControlState.QUEUED) || status.Equals(FileControlState.WAITING))
                {
                    row.Cells[3].Value = FileControlState.FILEFAIL;
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
            if (dataGridView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(downloadFileQueued);
                this.Invoke(d, new object[] { netUser, fileHandlerId, fileName });
            }
            else
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridView.Rows.Add(new Object[] { netUser.Name, fileName, 0, FileControlState.QUEUED, FileControlStateType.DOWNLOAD, fileHandlerId });
            } 
        }

        /// <summary>
        /// Marca como atendido a un archivo previamente encolado para descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void downloadFileOpened(String fileHandlerId)
        {
            if (dataGridView.InvokeRequired)
            {
                StringCallback d = new StringCallback(downloadFileOpened);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.QUEUED;
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
            if (dataGridView.InvokeRequired)
            {
                StringValueCallback d = new StringValueCallback(downloadFileTransfer);
                this.Invoke(d, new object[] { fileHandlerId, percent });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.LOADINGDATA;
                        row.Cells[2].Value = percent;
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
            if (dataGridView.InvokeRequired)
            {
                StringStringCallback d = new StringStringCallback(downloadFileComplete);
                this.Invoke(d, new object[] { fileHandlerId, path });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[2].Value = 100;
                        row.Cells[3].Value = FileControlState.FILECOMPLETE;
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
            if (dataGridView.InvokeRequired)
            {
                StringCallback d = new StringCallback(downloadFileFailed);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.FILEFAIL;
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
            if (dataGridView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(uploadFileQueued);
                this.Invoke(d, new object[] { netUser, fileHandlerId, fileName });
            }
            else
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridView.Rows.Add(new Object[] { netUser.Name, fileName, 0, FileControlState.QUEUED, FileControlStateType.UPLOAD, fileHandlerId });
            } 
        }

        /// <summary>
        /// Marca como atendido a un archivo previamente encolado para transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        public void uploadFileOpened(String fileHandlerId)
        {
            if (dataGridView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileOpened);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.LOADINGDATA;
                        row.Cells[2].Value = 0;
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
            if (dataGridView.InvokeRequired)
            {
                StringValueCallback d = new StringValueCallback(uploadFileTransfer);
                this.Invoke(d, new object[] { fileHandlerId, percent });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.LOADINGDATA;
                        row.Cells[2].Value = percent;
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
            if (dataGridView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileComplete);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.FILECOMPLETE;
                        row.Cells[2].Value = 100;
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
            if (dataGridView.InvokeRequired)
            {
                StringCallback d = new StringCallback(uploadFileFailed);
                this.Invoke(d, new object[] { fileHandlerId });
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (((String)(row.Cells[5].Value)).Equals(fileHandlerId))
                    {
                        row.Cells[3].Value = FileControlState.FILEFAIL;
                        break;
                    }
                } 
            }
        }
    }

    /// <summary>
    /// Clase que enumera y describe los posibles estados del control gráfico de archivos
    /// </summary>
    internal class FileControlState
    {
        /// <summary>
        /// Constante para el estado de esperar respuesta
        /// </summary>
        public const String WAITING = "Waiting";
        
        /// <summary>
        /// Constante para el estado de transferencia activa
        /// </summary>
        public const String LOADINGDATA = "Transfering";

        /// <summary>
        /// Constante para el estado de completado
        /// </summary>
        public const String FILECOMPLETE = "Completed";
        
        /// <summary>
        /// Constante para el estado de fallido
        /// </summary>
        public const String FILEFAIL = "Failed";

        /// <summary>
        /// Constante para el estado de encolamiento y espera
        /// </summary>
        public const String QUEUED = "Queued";
    }

    /// <summary>
    /// Clase que enumera y describe los posibles tipos de transferencia en el control gráfico de archivos
    /// </summary>
    internal class FileControlStateType
    {
        /// <summary>
        /// Constante para el tipo de transferencia (upload)
        /// </summary>
        public const String UPLOAD = "Upload";

        /// <summary>
        /// Constante para el tipo de descarga
        /// </summary>
        public const String DOWNLOAD = "Download";
    }
}
