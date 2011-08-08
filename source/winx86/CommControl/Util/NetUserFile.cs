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
using SubProtocol.FileTransfer;

namespace CommControl.Util
{
    /// <summary>
    /// Representa a un archivo asociado a un usuario de red, para uso de la lista de archivos compartidos
    /// </summary>
    internal class NetUserFile
    {
        /// <summary>
        /// El usuario de red
        /// </summary>
        private NetUser _netUser;

        /// <summary>
        /// El nombre del archivo
        /// </summary>
        private FileInformation _fileInformation;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetUserFile()
        {
        }

        /// <summary>
        /// El usuario de red
        /// </summary>
        public NetUser NetUser
        {
            get { return _netUser; }
            set { _netUser = value; }
        }

        /// <summary>
        /// El nombre del archivo
        /// </summary>
        public FileInformation FileInformation
        {
            get { return _fileInformation; }
            set { _fileInformation = value; }
        }
    }
}
