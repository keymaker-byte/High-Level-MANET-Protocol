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
using System.IO;
using SubProtocol.FileTransfer.Messages;
using System.Threading;
using CommLayer;
using CommLayer.Messages;

namespace SubProtocol.FileTransfer
{
    /// <summary>
    /// Esta clase implementa funciones para enviar un archivo particionado mediante mensajes de texto
    /// </summary>
    internal class FileMessageSender : FileMessageHandler
    {
        /// <summary>
        /// El indice de la parte que marca el límite aún no enviado
        /// </summary>
        private Int32 currentPart;
        
        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="remoteNetUser">El usuario con quien se intercambia el archivo</param>
        /// <param name="remoteFileHandlerId">El id de la transferencia de archivo</param>
        /// <param name="sendMessageDelegate">Una función por la que se pueda enviar un mensaje</param>
        /// <param name="fileInformation">Información del archivo</param>
        /// <param name="fileData">Datos de configuración de archivos</param>
        public FileMessageSender(NetUser remoteNetUser, Guid remoteFileHandlerId, SendMessageDelegate sendMessageDelegate, FileInformation fileInformation, FileData fileData) : base(remoteNetUser, sendMessageDelegate, fileInformation, fileData)
        {
            FileName = fileInformation.Path;
            currentPart = 0;
            this.Type = FileMessageHandlerType.UPLOAD;
            this.Id = remoteFileHandlerId;
        }

        /// <summary>
        /// Abre el archivo solicitado para descarga y marca el estado del este manejador de archivos como abierto
        /// </summary>
        public override void open()
        {
            loadFile();
        }

        /// <summary>
        /// Atiende una solicitud de envío de parte
        /// </summary>
        public override void attendMessage(Message message)
        {
            if (message.Type == SubProtocol.FileTransfer.Types.FILECOMPLETEMESSAGE)
            {
                State = FileMessageHandlerState.COMPLETED;
                close();
            }
        }

        /// <summary>
        /// Envia un trozo del archivo
        /// </summary>
        public override void execute()
        {
            sendPartMessage();
        }

        /// <summary>
        /// Cierra posibles conexiones a disco que puedan quedar abiertos
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
                Int32 percent = (int)(currentPart * 100 / PartsNumber);
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
        /// Carga el lector del archivo solicitado y envía un mensaje con información del archivo
        /// Si ocurre algun error de lectura envia un mensaje de error al usuario remoto
        /// </summary>
        private void loadFile()
        {
            try
            {
                FileHandlerStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                PartsNumber = getPartsNumber(FileInformation.Size, PartSize);
                State = FileMessageHandlerState.ACTIVE;
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

        /// <summary>
        /// Atiende una solicitud de envío de parte
        /// </summary>
        public void sendPartMessage( )
        {
            try
            {
                long pointer = currentPart * PartSize;
                if (pointer < FileHandlerStream.Length)
                {
                    int dataSize;
                    if (currentPart == PartsNumber - 1)
                    {
                        dataSize = (int)(FileHandlerStream.Length - PartSize * (PartsNumber - 1));
                    }
                    else
                    {
                        dataSize = PartSize;
                    }
                    byte[] fileData = new byte[dataSize];
                    FileHandlerStream.Seek(pointer, SeekOrigin.Begin);
                    int n = FileHandlerStream.Read(fileData, 0, fileData.Length);
                    while (n < fileData.Length && n != 0)
                    {
                        int m = FileHandlerStream.Read(fileData, n, fileData.Length - n);
                        if (m == 0)
                        {
                            break;
                        }
                        else
                        {
                            n += m;
                        }
                    }
                    sendMessage(new FilePartMessage(RemoteNetUser, Id, currentPart, fileData));
                    currentPart++;
                    this.State = FileMessageHandlerState.ACTIVE;
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

        /// <summary>
        /// Calcula el número total de particiones del archivo a transferir
        /// </summary>
        /// <param name="fileSize">El tamaño total del archivo</param>
        /// <param name="partSize">El tamaño de las particiones</param>
        /// <returns>El número total de particiones del archivo a transferir</returns>
        private Int32 getPartsNumber(Int64 fileSize, Int32 partSize)
        {
            Int32 partsNumber = 0;

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
