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

namespace SubProtocolCompact.FileTransfer
{
    /// <summary>
    /// Clase que enumera los estados posibles de las partes de archivos
    /// </summary>
    internal static class FilePartStatus
    {
        /// <summary>
        /// Constante para el estado no recibida
        /// </summary>
        public const Int32 NOTRECEIVED = 0;

        /// <summary>
        /// Constante para el estado recibida
        /// </summary>
        public const Int32 RECEIVED = 1;
    }

    /// <summary>
    /// Clase que representa un indicador de trozo de archivo
    /// </summary>
    internal class FilePartIndicator
    {
        /// <summary>
        /// El estado de recepción de la parte
        /// </summary>
        private Int32 _status;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FilePartIndicator()
        {
            this.Status = FilePartStatus.NOTRECEIVED;
        }

        /// <summary>
        /// El estado en que se encuentra este trozo de archivo
        /// </summary>
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
