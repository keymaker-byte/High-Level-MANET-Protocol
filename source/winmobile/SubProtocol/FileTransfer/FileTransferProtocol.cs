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
using CommLayer;
using CommLayer.Messages;
using System.Collections;
using SubProtocol.FileTransfer.Messages;
using SubProtocol.FileTransfer.ControlI;

namespace SubProtocol.FileTransfer
{
    /// <summary>
    /// Clase que enumera los tipos de mensajes usados en el protocolo
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// Constante para el tipo file transfer protocol
        /// </summary>
        public const Int32 FILETRANSFERPROTOCOL = 200;

        /// <summary>
        /// Constante para el tipo file request
        /// </summary>
        public const Int32 FILEREQUESTMESSAGE = 200;

        /// <summary>
        /// Constante para el tipo file part
        /// </summary>
        public const Int32 FILEPARTMESSAGE = 201;

        /// <summary>
        /// Constante para el tipo file complete
        /// </summary>
        public const Int32 FILECOMPLETEMESSAGE = 202;

        /// <summary>
        /// Constante para el tipo file download error
        /// </summary>
        public const Int32 FILEERRORMESSAGES = 203;

        /// <summary>
        /// Constante para el tipo file download error
        /// </summary>
        public const Int32 FILEWAITMESSAGE = 204;

        /// <summary>
        /// Constante para el tipo file list request
        /// </summary>
        public const Int32 FILELISTREQUESTMESSAGE = 205;

        /// <summary>
        /// Constante para el tipo file list
        /// </summary>
        public const Int32 FILELISTMESSAGE = 206;
    }

    /// <summary>
    /// Protocolo de transferencia de archivos
    /// </summary>
    public class FileTransferProtocol : SubProtocolI
    {

        /// <summary>
        /// Handler de eventos de transferencia de archivos
        /// </summary>
        public ControlFileHandlerI controlFileHandler;

        /// <summary>
        /// Handler de eventos de información de archivos compartidos
        /// </summary>
        public ControlFileListHandlerI controlFileListHandler;

        /// <summary>
        /// Evento que se gatilla cuando el protocolo quiere enviar un mensaje a la red
        /// </summary>
        public event Communication.MessageEvent sendMessageEvent;

        /// <summary>
        /// Cola de archivos para descargas
        /// </summary>
        private FileMessageHandlerQueue fileMessageDownloadQueue;

        /// <summary>
        /// Cola de archivos para cargas
        /// </summary>
        private FileMessageHandlerQueue fileMessageUploadQueue;

        /// <summary>
        /// Invoca threads de archivos
        /// </summary>
        private System.Threading.Timer timer;

        /// <summary>
        /// Punto de sincronización del timer
        /// </summary>
        private Int32 timerPoint;

        /// <summary>
        /// Thread de archivos
        /// </summary>
        private Thread timerThread;

        /// <summary>
        /// Punto de sincronización de transferencia de archivos
        /// </summary>
        private Int32 fileMessageHandlerPoint;

        /// <summary>
        /// Lock para transferencia de archivos
        /// </summary>
        private Object fileMessageHandlerLock;

        /// <summary>
        /// Configuracion de archivos
        /// </summary>
        private FileData fileData;

        /// <summary>
        /// Lista de downloads activos
        /// </summary>
        private Hashtable activeDownloads;

        /// <summary>
        /// Lista de uploads activos
        /// </summary>
        private Hashtable activeUploads;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controlFileHandler">Un manejador de eventos de archivos</param>
        /// <param name="controlFileListHandler">Un manejador de eventos de lista de archivos</param>
        /// <param name="fileData">Los parametros de configuración de archivos</param>
        public FileTransferProtocol(ControlFileHandlerI controlFileHandler, ControlFileListHandlerI controlFileListHandler, FileData fileData)
        {
            this.controlFileHandler = controlFileHandler;
            this.controlFileListHandler = controlFileListHandler;
            this.fileData = fileData;
            init();
        }

        /// <summary>
        /// Inicializa los componentes
        /// </summary>
        private void init()
        {
            fileMessageDownloadQueue = new FileMessageHandlerQueue();
            fileMessageUploadQueue = new FileMessageHandlerQueue();
            activeDownloads = new Hashtable();
            activeUploads = new Hashtable();
            fileMessageHandlerLock = new Object();
            timerPoint = 0;
        }

        /// <summary>
        /// Función invocada cada 1 segundos por el timer, invoca un thread de interación de timer
        /// </summary>
        /// <param name="objectState">Parámetro</param>
        private void communicationTimer(Object objectState)
        {
            if (Interlocked.CompareExchange(ref timerPoint, 1, 0) == 0)
            {
                timerThread = new Thread(new ThreadStart(processFiles));
                timerThread.Start();
                timerThread.Join();
                timerPoint = 0;
            }
        }

        /// <summary>
        /// Se gatilla cuando se recibe un mensaje
        /// </summary>
        /// <param name="message">El mensaje recibido</param>
        public void proccessMessage(Message message)
        {
            switch (message.Type)
            {
                case SubProtocol.FileTransfer.Types.FILEREQUESTMESSAGE:
                    {
                        FileRequestMessage fileRequestMessage = (FileRequestMessage)message;
                        FileInformation fileInformation = fileData.FileList.getFileInformation(fileRequestMessage.FileId);
                        if (fileInformation != null)
                        {
                            FileMessageSender fileMessageSender = new FileMessageSender(fileRequestMessage.SenderNetUser, fileRequestMessage.FileHandlerId, sendMessageDelegate, fileInformation, fileData);
                            lock (fileMessageHandlerLock)
                            {
                                if (!activeUploads.Contains(fileMessageSender.Id))
                                {
                                    if (fileMessageUploadQueue.put(fileMessageSender))
                                    {
                                        controlFileHandler.uploadFileQueued(fileRequestMessage.SenderNetUser, fileMessageSender.Id.ToString(), fileInformation.Name);
                                    }
                                }
                            }
                        }
                        else
                        {
                            FileErrorMessage fileErrorMessage = new FileErrorMessage(fileRequestMessage.SenderNetUser, fileRequestMessage.FileHandlerId);
                            sendMessageEvent(fileErrorMessage);
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEERRORMESSAGES:
                    {
                        FileErrorMessage fileErrorMessage = (FileErrorMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeUploads.Contains(fileErrorMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeUploads[fileErrorMessage.FileHandlerId];
                                controlFileHandler.uploadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeUploads.Remove(fileMessageHandler.Id);
                            }
                            else if (activeDownloads.Contains(fileErrorMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeDownloads[fileErrorMessage.FileHandlerId];
                                controlFileHandler.downloadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeDownloads.Remove(fileMessageHandler.Id);
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEPARTMESSAGE:
                    {
                        FilePartMessage filePartMessage = (FilePartMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeDownloads.Contains(filePartMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeDownloads[filePartMessage.FileHandlerId];
                                fileMessageHandler.attendMessage(filePartMessage);
                                fileMessageHandler.waitUp(fileData.FileRiseUp);
                            }
                            else
                            {
                                FileErrorMessage fileErrorMessage = new FileErrorMessage(filePartMessage.SenderNetUser, filePartMessage.FileHandlerId);
                                sendMessageEvent(fileErrorMessage);
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEWAITMESSAGE:
                    {
                        FileWaitMessage fileWaitMessage = (FileWaitMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeDownloads.Contains(fileWaitMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeDownloads[fileWaitMessage.FileHandlerId];
                                fileMessageHandler.waitUp(fileData.FileRiseUp);
                            }
                            else if (activeUploads.Contains(fileWaitMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeUploads[fileWaitMessage.FileHandlerId];
                                fileMessageHandler.waitUp(fileData.FileRiseUp);
                            }
                            else
                            {
                                if (!fileMessageDownloadQueue.contains(fileWaitMessage.FileHandlerId) && !fileMessageUploadQueue.contains(fileWaitMessage.FileHandlerId))
                                {
                                    FileErrorMessage fileErrorMessage = new FileErrorMessage(fileWaitMessage.SenderNetUser, fileWaitMessage.FileHandlerId);
                                    sendMessageEvent(fileErrorMessage);
                                }
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILECOMPLETEMESSAGE:
                    {
                        FileCompleteMessage fileCompleteMessage = (FileCompleteMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeUploads.Contains(fileCompleteMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeUploads[fileCompleteMessage.FileHandlerId];
                                fileMessageHandler.State = FileMessageHandlerState.COMPLETED;
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILELISTREQUESTMESSAGE:
                    {
                        FileListRequestMessage fileListRequestMessage = (FileListRequestMessage)message;
                        FileListMessage fileListMessage = new FileListMessage(fileListRequestMessage.SenderNetUser, fileData.FileList);
                        sendMessageEvent(fileListMessage);
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILELISTMESSAGE:
                    {
                        FileListMessage fileListMessage = (FileListMessage)message;
                        controlFileListHandler.addFileList(fileListMessage.SenderNetUser, fileListMessage.FileList);
                        break;
                    }
            }
        }

        /// <summary>
        /// Se gatilla cuando no es posible entregar un mensaje
        /// </summary>
        /// <param name="message">El mensaje no entregado</param>
        public void errorMessage(Message message)
        {
            switch (message.Type)
            {
                case SubProtocol.FileTransfer.Types.FILEREQUESTMESSAGE:
                    {
                        FileRequestMessage fileRequestMessage = (FileRequestMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeDownloads.Contains(fileRequestMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeDownloads[fileRequestMessage.FileHandlerId];
                                controlFileHandler.downloadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeDownloads.Remove(fileMessageHandler.Id);
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEERRORMESSAGES:
                    {
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEPARTMESSAGE:
                    {
                        FilePartMessage filePartMessage = (FilePartMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeUploads.Contains(filePartMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeUploads[filePartMessage.FileHandlerId];
                                controlFileHandler.uploadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeUploads.Remove(fileMessageHandler.Id);
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILEWAITMESSAGE:
                    {
                        FileWaitMessage fileWaitMessage = (FileWaitMessage)message;
                        lock (fileMessageHandlerLock)
                        {
                            if (activeDownloads.Contains(fileWaitMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeDownloads[fileWaitMessage.FileHandlerId];
                                controlFileHandler.downloadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeDownloads.Remove(fileMessageHandler.Id);
                            }
                            else if (activeUploads.Contains(fileWaitMessage.FileHandlerId))
                            {
                                FileMessageHandler fileMessageHandler = (FileMessageHandler)activeUploads[fileWaitMessage.FileHandlerId];
                                controlFileHandler.uploadFileFailed(fileMessageHandler.Id.ToString());
                                fileMessageHandler.close();
                                activeUploads.Remove(fileMessageHandler.Id);
                            }
                        }
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILECOMPLETEMESSAGE:
                    {
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILELISTREQUESTMESSAGE:
                    {
                        break;
                    }
                case SubProtocol.FileTransfer.Types.FILELISTMESSAGE:
                    {
                        break;
                    }
            }
        }
        
        /// <summary>
        /// Se gatilla cuando se quiere enviar una petición de archivo
        /// </summary>
        /// <param name="netUser">El usuario dueño del archivo</param>
        /// <param name="fileInformation">los datos del archivo</param>
        public void sendFileRequest(NetUser netUser, FileInformation fileInformation)
        {
            FileMessageReceiver fileMessageReceiber = new FileMessageReceiver(netUser, sendMessageDelegate, fileInformation, fileData);
            if (fileMessageDownloadQueue.put(fileMessageReceiber))
            {
                controlFileHandler.downloadFileQueued(netUser, fileMessageReceiber.Id.ToString(), fileInformation.Name);
            }
        }

        /// <summary>
        /// Se gatilla cuando se quiere enviar una petición de lista de archivos
        /// </summary>
        /// <param name="netUser">El usuario dueño de los archivos</param>
        public void sendFileListRequest(NetUser netUser)
        {
            FileListRequestMessage fileListRequestMessage = new FileListRequestMessage(netUser);
            sendMessageEvent(fileListRequestMessage);
        }

        /// <summary>
        /// Se gatilla cuando se quiere enviar la lista de archivos
        /// </summary>
        /// <param name="netUser">El usuario al cual enviarle la lista de archvos</param>
        public void sendFileList(NetUser netUser)
        {
            FileListMessage fileListMessage = new FileListMessage(netUser, fileData.FileList);
            sendMessageEvent(fileListMessage);
        }

        /// <summary>
        /// Detiene cualquier transferencia activa y termina el protocolo
        /// </summary>
        public void stop()
        {
            try
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {
                timerThread.Abort();
                timerThread.Join();
            }
            catch (Exception)
            {
            }
            init();
        }

        /// <summary>
        /// Activa el protocolo
        /// </summary>
        public void start()
        {
            timer = new System.Threading.Timer(communicationTimer, null, 0, fileData.TimeIntervalTimer);
        }

        /// <summary>
        /// Se gatilla cuando se quiere envíar un mensaje
        /// Este parametro es para asignar a los handlers
        /// </summary>
        /// <param name="message">El mensaje a enviar</param>
        internal void sendMessageDelegate(Message message)
        {
            sendMessageEvent(message);
        }

        /// <summary>
        /// Obtiene la lista de tipos de mensajes usados por este protocolo
        /// </summary>
        /// <returns>Una tabla con valores y tipos de mensajes usados en el protocolo</returns>
        public MessageTypeList getMessageTypes()
        {
            MessageTypeList typeCollection = new MessageTypeList();
            typeCollection.add(SubProtocol.FileTransfer.Types.FILECOMPLETEMESSAGE, typeof(FileCompleteMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILEERRORMESSAGES, typeof(FileErrorMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILEPARTMESSAGE, typeof(FilePartMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILEREQUESTMESSAGE, typeof(FileRequestMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILEWAITMESSAGE, typeof(FileWaitMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILELISTREQUESTMESSAGE, typeof(FileListRequestMessage));
            typeCollection.add(SubProtocol.FileTransfer.Types.FILELISTMESSAGE, typeof(FileListMessage));
            return typeCollection;
        }

        /// <summary>
        /// Procesa el archivos en proceso de descarga
        /// </summary>
        private void processFiles()
        {
            if (Interlocked.CompareExchange(ref fileMessageHandlerPoint, 1, 0) == 0)
            {
                lock (fileMessageHandlerLock)
                {
                    FileMessageHandler[] handlers = new FileMessageHandler[activeDownloads.Count];
                    IDictionaryEnumerator en = activeDownloads.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        handlers[i] = (FileMessageHandler)en.Value;
                        i++;
                    }
                    foreach (FileMessageHandler fileMessageHandler in handlers)
                    {
                        switch (fileMessageHandler.State)
                        {
                            case FileMessageHandlerState.WAITING:
                                {
                                    fileMessageHandler.open();
                                    controlFileHandler.downloadFileOpened(fileMessageHandler.Id.ToString());
                                    break;
                                }
                            case FileMessageHandlerState.OPEN:
                                {
                                    fileMessageHandler.TimeOut--;
                                    if (fileMessageHandler.TimeOut <= 0)
                                    {
                                        fileMessageHandler.resetTimeOut();
                                        sendMessageEvent(new FileWaitMessage(fileMessageHandler.RemoteNetUser, fileMessageHandler.Id));
                                    }
                                    break;
                                }
                            case FileMessageHandlerState.ACTIVE:
                                {
                                    fileMessageHandler.execute();
                                    controlFileHandler.downloadFileTransfer(fileMessageHandler.Id.ToString(), fileMessageHandler.completed());
                                    fileMessageHandler.TimeOut--;
                                    if (fileMessageHandler.TimeOut <= 0)
                                    {
                                        fileMessageHandler.resetTimeOut();
                                        sendMessageEvent(new FileWaitMessage(fileMessageHandler.RemoteNetUser, fileMessageHandler.Id));
                                    }
                                    break;
                                }
                            case FileMessageHandlerState.ERROR:
                                {
                                    controlFileHandler.downloadFileFailed(fileMessageHandler.Id.ToString());
                                    fileMessageHandler.close();
                                    activeDownloads.Remove(fileMessageHandler.Id);
                                    break;
                                }
                            case FileMessageHandlerState.COMPLETED:
                                {
                                    controlFileHandler.downloadFileComplete(fileMessageHandler.Id.ToString(), fileMessageHandler.FileName);
                                    fileMessageHandler.close();
                                    activeDownloads.Remove(fileMessageHandler.Id);
                                    break;
                                }
                        }
                    }
                }
                lock (fileMessageHandlerLock)
                {
                    FileMessageHandler[] handlers = new FileMessageHandler[activeUploads.Count];
                    IDictionaryEnumerator en = activeUploads.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        handlers[i] = (FileMessageHandler)en.Value;
                        i++;
                    }
                    foreach (FileMessageHandler fileMessageHandler in handlers)
                    {
                        switch (fileMessageHandler.State)
                        {
                            case FileMessageHandlerState.WAITING:
                                {
                                    fileMessageHandler.open();
                                    controlFileHandler.uploadFileOpened(fileMessageHandler.Id.ToString());
                                    break;
                                }
                            case FileMessageHandlerState.OPEN:
                                {
                                    fileMessageHandler.TimeOut--;
                                    if (fileMessageHandler.TimeOut <= 0)
                                    {
                                        fileMessageHandler.resetTimeOut();
                                        sendMessageEvent(new FileWaitMessage(fileMessageHandler.RemoteNetUser, fileMessageHandler.Id));
                                    }
                                    break;
                                }
                            case FileMessageHandlerState.ACTIVE:
                                {
                                    fileMessageHandler.execute();
                                    controlFileHandler.uploadFileTransfer(fileMessageHandler.Id.ToString(), fileMessageHandler.completed());
                                    break;
                                }
                            case FileMessageHandlerState.ERROR:
                                {
                                    controlFileHandler.uploadFileFailed(fileMessageHandler.Id.ToString());
                                    fileMessageHandler.close();
                                    activeUploads.Remove(fileMessageHandler.Id);
                                    break;
                                }
                            case FileMessageHandlerState.COMPLETED:
                                {
                                    controlFileHandler.uploadFileComplete(fileMessageHandler.Id.ToString());
                                    fileMessageHandler.close();
                                    activeUploads.Remove(fileMessageHandler.Id);
                                    break;
                                }
                        }
                    }
                }
                lock (fileMessageHandlerLock)
                {
                    if (activeDownloads.Count < fileData.SimulteneusDownload)
                    {
                        FileMessageHandler fileMessageHandler = fileMessageDownloadQueue.draw();
                        if (fileMessageHandler != null)
                        {
                            activeDownloads.Add(fileMessageHandler.Id, fileMessageHandler);
                        }
                    }
                }
                lock (fileMessageHandlerLock)
                {
                    if (activeUploads.Count < fileData.SimulteneusUpload)
                    {
                        FileMessageHandler fileMessageHandler = fileMessageUploadQueue.draw();
                        if (fileMessageHandler != null)
                        {
                            activeUploads.Add(fileMessageHandler.Id, fileMessageHandler);
                        }
                    }
                }
                fileMessageHandlerPoint = 0;
            }
        }
    }
}
