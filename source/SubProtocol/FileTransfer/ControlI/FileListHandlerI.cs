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
using CommLayer;
using CommLayer.Messages;

namespace SubProtocol.FileTransfer.ControlI
{
    /// <summary>
    /// Interfaz para eventos relacionados con la información de archivos compartidos en la red
    /// </summary>
    public interface FileListHandlerI
    {
        /// <summary>
        /// Se gatilla cuando un usuario a compartido una nueva lista de archivos
        /// </summary>
        /// <param name="netUser">El usuario dueño de los archivos</param>
        /// <param name="fileList">La lista de archivos</param>
        void addFileList(NetUser netUser, FileInformationList fileList);
        
        /// <summary>
        /// Se gatilla cuando un usuario ha dejado de compartir archivos en la red
        /// </summary>
        /// <param name="netUser">El usuario de la red que ya no comparte archivos</param>
        void removeFileList(NetUser netUser);
    }
}
