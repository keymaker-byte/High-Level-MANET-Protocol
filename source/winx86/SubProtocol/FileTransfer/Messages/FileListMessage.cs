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

namespace SubProtocol.FileTransfer.Messages
{
    /// <summary>
    /// Representa un mensaje que contiene la lista de archivos que comparte un usuario
    /// Protocolo Safe TCP
    /// </summary>
    public class FileListMessage : SafeUnicastMessage 
    {
        /// <summary>
        /// La lista de archivos
        /// </summary>
        private FileInformationList _fileList;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileListMessage() : base()
        {
            this.Type = SubProtocol.FileTransfer.Types.FILELISTMESSAGE;
            this.ProtocolType = SubProtocol.FileTransfer.Types.FILETRANSFERPROTOCOL;
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="targetNetUser">El receptor de este mensaje</param>
        /// <param name="fileList">La lista de archivos compartidos</param>
        public FileListMessage(NetUser targetNetUser, FileInformationList fileList) : this()
        {
            this.TargetNetUser = targetNetUser;
            this.FileList = fileList;
        }

        /// <summary>
        /// La lista de archivos compartidos
        /// </summary>
        public FileInformationList FileList
        {
            get { return _fileList; }
            set { _fileList = value; }
        }

        /// <summary>
        /// Convierte las propiedades del mensaje en un paquete de bytes
        /// </summary>
        /// <returns>un paquete de bytes con las propiedades del mensaje</returns>
        public override byte[] makePack()
        {
            FileInformation[] fileInformations = FileList.toArray();
            byte[] listSize = BitConverter.GetBytes(fileInformations.Length); //4 (0-3)
            List<byte[]> fileInformationBytes = new List<byte[]>();
            int i = 4;
            foreach (FileInformation fileInformation in fileInformations)
            {
                byte[] id = fileInformation.Id.ToByteArray(); //16 (0 - 15)
                byte[] size = BitConverter.GetBytes(fileInformation.Size); //8 (16 - 23)
                byte[] name = Encoding.Unicode.GetBytes(fileInformation.Name); //nameSize (28 - 27 + nameSize)
                byte[] nameSize = BitConverter.GetBytes(name.Length); //4 (24 - 27)

                byte[] fileInformationPack = new byte[28 + name.Length];
                id.CopyTo(fileInformationPack, 0);
                size.CopyTo(fileInformationPack, 16);
                nameSize.CopyTo(fileInformationPack, 24);
                name.CopyTo(fileInformationPack, 28);
                fileInformationBytes.Add(fileInformationPack);
                i += fileInformationPack.Length;
            }
            int n = i;
            byte[] pack = new byte[n];
            listSize.CopyTo(pack, 0);
            i = listSize.Length;
            foreach (byte[] fileInformationPack in fileInformationBytes)
            {
                fileInformationPack.CopyTo(pack, i);
                i += fileInformationPack.Length;
            }
            return pack;
        }

        /// <summary>
        /// Convierte un paquete de bytes en las propiedades del mensaje
        /// </summary>
        /// <param name="messagePack">El paquete de bytes</param>
        public override void unPack(byte[] messagePack)
        {
            FileList = new FileInformationList();
            int listSize = BitConverter.ToInt32(messagePack, 0);
            int i = 4;
            for (int n = 0; n < listSize; n++)
            {
                FileInformation fileInformation = new FileInformation();
                byte[] id = new byte[16];
                Array.Copy(messagePack, i, id, 0, 16);
                fileInformation.Id = new Guid(id);
                i += 16;
                fileInformation.Size = BitConverter.ToInt64(messagePack, i);
                i += 8;
                int nameSize = BitConverter.ToInt32(messagePack, i);
                i += 4;
                fileInformation.Name = Encoding.Unicode.GetString(messagePack, i, nameSize);
                i += nameSize;
                FileList.add(fileInformation);
            }
        }

        /// <summary>
        /// Sobreescribe el metodo toString
        /// </summary>
        /// <returns>El string que representa este objeto</returns>
        public override string ToString()
        {
            return base.ToString() + "FileListMessage:";
        }
    }
}
