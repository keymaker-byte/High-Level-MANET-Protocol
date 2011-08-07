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
using CommLayerCompact;
using CommLayerCompact.Messages;

namespace SubProtocolCompact.FileTransfer.ControlI
{
    /// <summary>
    /// Interfaz para eventos relacionados con descarga y transferencia de archivos en la red
    /// </summary>
    public interface ControlFileHandlerI
    {
        /// <summary>
        /// Se gatilla cuando un archivo ha sido encolado para descarga
        /// </summary>
        /// <param name="netUser">El usuario de la red dueño del archivo</param>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="fileName">El nombre del archivo</param>
        void downloadFileQueued(NetUser netUser, String fileHandlerId, String fileName);
        
        /// <summary>
        /// Se gatilla cuando un archivo ha sido atendido para descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void downloadFileOpened(String fileHandlerId);
        
        /// <summary>
        /// Se gatilla cuando se ha recibido una parte del archivo en proceso de descarga
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="percent">el porcentaje total que se ha transferido</param>
        void downloadFileTransfer(String fileHandlerId, Int32 percent);
        
        /// <summary>
        /// Se gatilla cuando el archivo en proceso de descarga ha sido completado
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void downloadFileComplete(String fileHandlerId, String path);
        
        /// <summary>
        /// Se gatilla cuando el archivo en proceso de descarga ha fallado
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void downloadFileFailed(String fileHandlerId);
        
        /// <summary>
        /// Se gatilla cuando un archivo es encolado para transferencia (upload)
        /// </summary>
        /// <param name="netUser">El usuario de la red al cual se le debe transferir el archivo</param>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="fileName">El nombre del archivo</param>
        void uploadFileQueued(NetUser netUser, String fileHandlerId, String fileName);
        
        /// <summary>
        /// Se gatilla cuando un archivo encolado para transferencia es atendido
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void uploadFileOpened(String fileHandlerId);
        
        /// <summary>
        /// Se gatilla cuando se ha envíado una parte del archivo en proceso de transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        /// <param name="percent">porcentaje total que se ha enviado del archivo</param>
        void uploadFileTransfer(String fileHandlerId, Int32 percent);
        
        /// <summary>
        /// Se gatilla cuando se ha completado la transferencia de un archivo
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void uploadFileComplete(String fileHandlerId);
        
        /// <summary>
        /// Se gatilla cuando ha fallado el archivo en proceso de transferencia
        /// </summary>
        /// <param name="fileHandlerId">El id de la transferencia</param>
        void uploadFileFailed(String fileHandlerId);
    }
}
