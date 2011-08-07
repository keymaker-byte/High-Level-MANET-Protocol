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

namespace CommControl.Util
{
    internal class FileItem
    {

        public FileItem()
        {
        }

        private String _netUserName;

        public String NetUserName
        {
            get { return _netUserName; }
            set { _netUserName = value; }
        }
        private String _fileName;

        public String FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        private String _fileHandlerId;

        public String FileHandlerId
        {
            get { return _fileHandlerId; }
            set { _fileHandlerId = value; }
        }
        private String _state;

        public String State
        {
            get { return _state; }
            set { _state = value; }
        }
        private String _type;

        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Int32 _percent;

        public Int32 Percent
        {
            get { return _percent; }
            set { _percent = value; }
        }


        public override string ToString()
        {
            return NetUserName + "|" + FileName + "|" + Percent + "|" + State + "|" + Type + "|" + FileHandlerId;
        }
    }
}
