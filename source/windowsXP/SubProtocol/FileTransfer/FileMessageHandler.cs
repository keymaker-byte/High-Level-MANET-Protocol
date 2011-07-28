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
using CommLayer.Messages;
using CommLayer;

namespace SubProtocol.FileTransfer
{

    /// <summary>
    /// Clase que enumera los estados posibles del manejador de archivos
    /// </summary>
    internal static class FileMessageHandlerState
    {
        /// <summary>
        /// Constante para el estado esperando en la cola
        /// </summary>
        public const Int32 WAITING = 0;

        /// <summary>
        /// Constante para el estado abierto
        /// </summary>
        public const Int32 OPEN = 1;

        /// <summary>
        /// Constante para el estado completado
        /// </summary>
        public const Int32 COMPLETED = 2;

        /// <summary>
        /// Constante para el estado fallido
        /// </summary>
        public const Int32 ERROR = 3;

        /// <summary>
        /// Constante para el estado solicitado
        /// </summary>
        public const Int32 ACTIVE = 4;
    }

    /// <summary>
    /// Clase que enumera los tipos posibles del manejador de archivos
    /// </summary>
    internal static class FileMessageHandlerType
    {
        /// <summary>
        /// Constante para el estado esperando en la cola
        /// </summary>
        public const Int32 DOWNLOAD = 0;

        /// <summary>
        /// Constante para el estado abierto
        /// </summary>
        public const Int32 UPLOAD = 1;
    }

    /// <summary>
    /// Esta clase representa un enviador y/o receptor de archivos abstracto
    /// </summary>
    internal abstract class FileMessageHandler
    {
        /// <summary>
        /// El id de la transferencia
        /// </summary>
        private Guid _id;

        /// <summary>
        /// El usuario remoto con quien se está intercambiando el archivo
        /// </summary>
        private NetUser _remoteNetUser;

        /// <summary>
        /// La información del archivo
        /// </summary>
        private FileInformation _fileInformation;
        
        /// <summary>
        /// El tamaño de las partes en las que se divide el archivo para la transferencia
        /// </summary>
        private Int32 _partSize;

        /// <summary>
        /// El número de partes en las que se divide el archivo
        /// </summary>
        private Int64 _partsNumber;

        /// <summary>
        /// La ruta del archivo usado para la transferencia
        /// </summary>
        private String _fileName;

        /// <summary>
        /// El stream usado para leer o escribir en el archivo
        /// </summary>
        private FileStream _fileHandlerStream;

        /// <summary>
        /// El estado del manejador, un valor de FileMessageHandlerState
        /// </summary>
        private Int32 _state;

        /// <summary>
        /// El mensaje de error en caso de que ocurriese
        /// </summary>
        private String _error;

        /// <summary>
        /// El tiempo de espera que ha transcurrido
        /// </summary>
        private Int32 _timeOut;

        /// <summary>
        /// El maximo tiempo de espera que se toma en cuenta antes de notificar un fallo en la transferencia
        /// </summary>
        private Int32 _maxTimeOut;

        /// <summary>
        /// El tipo del manejador
        /// </summary>
        private Int32 _type;

        /// <summary>
        /// Tipo para la funcionalidad de envío de mensaje
        /// </summary>
        /// <param name="message">El mensaje a envíar</param>
        public delegate void SendMessageDelegate(Message message);

        /// <summary>
        /// Se gatilla cuando el manejador de archivos debe envíar un mensaje a la MANET
        /// </summary>
        public SendMessageDelegate sendMessage;

        public FileMessageHandler(NetUser remoteNetUser, SendMessageDelegate sendMessage, FileInformation fileInformation, FileData fileData)
        {
            this.Id = Guid.NewGuid();
            this.RemoteNetUser = remoteNetUser;
            this.sendMessage = sendMessage;
            this.FileInformation = fileInformation;
            this.PartSize = fileData.PartSize;
            State = FileMessageHandlerState.WAITING;
            MaxTimeOut = fileData.FileTimeOut;
            resetTimeOut();
        }

        /// <summary>
        /// El id de la transferencia
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// El tamaño de las particiones
        /// </summary>
        public Int32 PartSize
        {
            get { return _partSize; }
            set { _partSize = value; }
        }

        /// <summary>
        /// El FileStream usado para leer y/o escribir del archivo
        /// </summary>
        public FileStream FileHandlerStream
        {
          get { return _fileHandlerStream; }
          set { _fileHandlerStream = value; }
        }

        /// <summary>
        /// El usuario remoto con quien se esta intercambiando el archivo
        /// </summary>
        public NetUser RemoteNetUser
        {
            get { return _remoteNetUser; }
            set { _remoteNetUser = value; }
        }

        /// <summary>
        /// El estado de este manejador de archivos
        /// </summary>
        public Int32 State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// La causa de error, en caso de falla
        /// </summary>
        public String Error
        {
            get { return _error; }
            set { _error = value; }
        }

        /// <summary>
        /// El número de partes en que se divide el archivo
        /// </summary>
        public Int64 PartsNumber
        {
            get { return _partsNumber; }
            set { _partsNumber = value; }
        }

        /// <summary>
        /// El nombre del archivo que se usa para la transferencia
        /// </summary>
        public String FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// El tiempo de espera que ha transcurrido para el fileHandler en modo Open
        /// </summary>
        public Int32 TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// La información del archivo
        /// </summary>
        public FileInformation FileInformation
        {
            get { return _fileInformation; }
            set { _fileInformation = value; }
        }

        /// <summary>
        /// El tiempo de espera maximo que se espera por una respuesta
        /// </summary>
        public Int32 MaxTimeOut
        {
            get { return _maxTimeOut; }
            set { _maxTimeOut = value; }
        }

        /// <summary>
        /// Aumenta el tiempo de espera
        /// </summary>
        /// <param name="n">La cantidad de tiempo sumar al TimeOut</param>
        public void waitUp(Int32 n)
        {
            TimeOut = (TimeOut + n);
            if (TimeOut > MaxTimeOut)
            {
                resetTimeOut();
            }
        }

        /// <summary>
        /// Resetea el tiempo de espera
        /// </summary>
        public void resetTimeOut()
        {
            TimeOut = MaxTimeOut;
        }

        /// <summary>
        /// El tipo del manejador
        /// </summary>
        public Int32 Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Cierra el manejador de transmisión
        /// </summary>
        public abstract void close();

        /// <summary>
        /// Abre el manejador de transmisión
        /// </summary>
        public abstract void open();

        /// <summary>
        /// Ejecuta un paso en la iteración del transmisor
        /// </summary>
        public abstract void attendMessage(Message message);

        /// <summary>
        /// Ejecuta un paso en la iteración del transmisor
        /// </summary>
        public abstract void execute();

        /// <summary>
        /// Calcula cuanto se ha transferido del archivo con un valor entre 0 y 100 %
        /// </summary>
        /// <returns>el porcentaje transmitido</returns>
        public abstract Int32 completed();
    }
}
