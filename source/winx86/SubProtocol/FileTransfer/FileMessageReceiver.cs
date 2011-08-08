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
using System.Threading;
using System.IO;
using SubProtocol.FileTransfer.Messages;
using CommLayer;
using CommLayer.Messages;

namespace SubProtocol.FileTransfer
{
    /// <summary>
    /// Esta clase implementa funciones para recepcionar un archivo particionado que se envía mediante mensajes de texto
    /// </summary>
    internal class FileMessageReceiver : FileMessageHandler
    {
        /// <summary>
        /// Un array con las particiones del archivo
        /// </summary>
        private FilePartIndicator[] parts;
        
        /// <summary>
        /// Objeto para control de threading
        /// </summary>
        private Object thisLock;

        /// <summary>
        /// Numero de partes que se han recibido
        /// </summary>
        private Int32 partsLoaded;

        /// <summary>
        /// Directorio de descarga
        /// </summary>
        private String downloadDir;

        /// <summary>
        /// Constructor Parametrizado
        /// </summary>
        /// <param name="remoteNetUser">El usuario remoto con el cual se intercambiará el archivo</param>
        /// <param name="sendMessageDelegate">Un método con el cual se puedan envíar mensajes a la MANET</param>
        /// <param name="fileInformation">La información del archivo</param>
        /// <param name="FileData">Los parámetros de configuración</param>
        public FileMessageReceiver(NetUser remoteNetUser, SendMessageDelegate sendMessageDelegate, FileInformation fileInformation, FileData fileData) : base(remoteNetUser, sendMessageDelegate, fileInformation, fileData)
        {
            thisLock = new Object();
            this.Type = FileMessageHandlerType.DOWNLOAD;
            this.downloadDir = fileData.DownloadDir;
        }

        /// <summary>
        /// Envía el mensaje al usuario remoto de petición de archivo y marca el estado del manejador como abierto
        /// </summary>
        public override void open()
        {
            createFile();
            FileRequestMessage fileRequestMessage = new FileRequestMessage(RemoteNetUser, FileInformation.Id, Id);
            sendMessage(fileRequestMessage);
        }

        /// <summary>
        /// Envía un mensaje de petición de partes del archivo al usuario remoto y marca el estado del manejador como esperando
        /// </summary>
        public override void attendMessage(Message message)
        {
            if (message.Type == SubProtocol.FileTransfer.Types.FILEPARTMESSAGE)
            {
                FilePartMessage filePartMessage = (FilePartMessage)message;
                receivePartMessage(filePartMessage);
            }
        }

        /// <summary>
        /// No hace nada
        /// </summary>
        public override void execute()
        {
        }

        /// <summary>
        /// Cierra posibles conexiones a disco creadas
        /// </summary>
        public override void close()
        {
            try
            {
                FileHandlerStream.Close();
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception)
            {
            } 
        }

        /// <summary>
        /// Calcula cuanto se ha transferido del archivo con un valor entre 0 y 100 %
        /// </summary>
        /// <returns>el porcentaje transmitido</returns>
        public override Int32 completed()
        {
            try
            {
                Int32 percent = (int)(partsLoaded * 100 / PartsNumber);
                if (percent > 100)
                {
                    percent = 100;
                }
                return percent;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Crea el archivo temporal para la recepción de las partes
        /// </summary>
        private void createFile()
        {
            lock (thisLock)
            {
                try
                {
                    PartsNumber = getPartsNumber(FileInformation.Size, PartSize);
                    parts = new FilePartIndicator[PartsNumber];
                    for (long i = 0; i < parts.LongLength; i++)
                    {
                        parts[i] = new FilePartIndicator();
                    }

                    //crea e inicializa el archivo temporal
                    bool exists = true;
                    int j = 0;
                    while (exists)
                    {
                        FileName = downloadDir + "/" + RemoteNetUser.Name + "." + j + "." + FileInformation.Name;
                        FileInfo fileInfo = new FileInfo(FileName);
                        exists = fileInfo.Exists;
                        j++;
                    }
                    Int64 currentPart = 0;
                    long pointer = currentPart * PartSize;
                    FileHandlerStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                    while (pointer < FileInformation.Size)
                    {
                        int dataSize;
                        if (currentPart == PartsNumber - 1)
                        {
                            dataSize = (int)(FileInformation.Size - PartSize * (PartsNumber - 1));
                        }
                        else
                        {
                            dataSize = PartSize;
                        }
                        byte[] fileData = new byte[dataSize];
                        FileHandlerStream.Seek(pointer, SeekOrigin.Begin);
                        FileHandlerStream.Write(fileData, 0, fileData.Length);
                        currentPart++;
                        pointer = currentPart * PartSize;
                    }
                    partsLoaded = 0;
                    if (FileInformation.Size <= 0)
                    {
                        State = FileMessageHandlerState.COMPLETED;
                    }
                    else
                    {
                        State = FileMessageHandlerState.OPEN;
                    }
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    State = FileMessageHandlerState.ERROR;
                    Error = e.Message;
                    close();
                } 
            }
        }

        /// <summary>
        /// Atiende un mensaje que contiene una parte del archivo y escribe dicha parte a disco
        /// </summary>
        /// <param name="message">El mensaje con la parte del archivo</param>
        private void receivePartMessage(FilePartMessage message)
        {
            lock (thisLock)
            {
                try
                {
                    long pointer = message.PartId * PartSize;
                    if (pointer < FileInformation.Size && parts[message.PartId].Status == FilePartStatus.NOTRECEIVED)
                    {
                        FileHandlerStream.Seek(pointer, SeekOrigin.Begin);
                        FileHandlerStream.Write(message.FilePart, 0, message.FilePart.Length);
                        parts[message.PartId].Status = FilePartStatus.RECEIVED;
                        partsLoaded++;
                    }

                    bool completed = true;
                    for (long i = 0; i < parts.LongLength; i++)
                    {
                        if (parts[i].Status == FilePartStatus.NOTRECEIVED)
                        {
                            completed = false;
                            break;
                        }
                    }
                    if (completed)
                    {
                        sendMessage(new FileCompleteMessage(RemoteNetUser, Id));
                        this.State = FileMessageHandlerState.COMPLETED;
                    }
                    else
                    {
                        State = FileMessageHandlerState.ACTIVE;
                    }
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    sendMessage(new FileErrorMessage(RemoteNetUser, Id));
                    this.State = FileMessageHandlerState.ERROR;
                    this.Error = e.Message;
                    close();
                } 
            }
        }

        /// <summary>
        /// Obtiene el numero total de particiones del archivo
        /// </summary>
        /// <param name="fileSize">el tamaño total del archivo</param>
        /// <param name="partSize">el tamaño de las particiones</param>
        /// <returns>el numero total de particiones del archivo</returns>
        private Int64 getPartsNumber(Int64 fileSize, Int32 partSize)
        {
            Int64 partsNumber = 0;

            Int64 pointer = 0;
            while (pointer < fileSize)
            {
                partsNumber++;
                pointer += partSize;
            }
            return partsNumber;
        }
    }
}
