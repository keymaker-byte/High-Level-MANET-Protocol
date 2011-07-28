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
using System.Collections;
using System.IO;
using System.Threading;

namespace SubProtocol.FileTransfer
{
    /// <summary>
    /// Datos de un archivo
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        /// El nombre del archivo
        /// </summary>
        private String _name;

        /// <summary>
        /// La ruta
        /// </summary>
        private String _path;

        

        /// <summary>
        /// El tamaño en numero de bytes del archivo
        /// </summary>
        private Int64 _size;

        /// <summary>
        /// El Id del archivo
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Constructor 
        /// </summary>
        public FileInformation()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructor Parametrizado, crea un identificador de archivo nuevo
        /// </summary>
        /// <param name="name">El nombre del archivo</param>
        /// <param name="size">El tamaño, en numero de bytes</param>
        /// <param name="path">La ruta al archivo</param>
        public FileInformation(String name, Int64 size, String path)
            : this()
        {
            Name = name;
            Size = size;
            Path = path;
        }

        /// <summary>
        /// Constructor Parametrizad
        /// </summary>
        /// <param name="name">El nombre del archivo</param>
        /// <param name="size">El tamaño, en numero de bytes</param>
        /// <param name="id">El identificador del archivo</param>
        public FileInformation(String name, Int64 size, Guid id)
        {
            Name = name;
            Size = size;
            Id = id;
        }

        /// <summary>
        /// El nombre del archivo 
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// (ruta completa)
        /// </summary>
        public String Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// El tamaño en numero de bytes del archivo
        /// </summary>
        public Int64 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// El identificador del archivo
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    /// <summary>
    /// Lista de Objetos de información de archivos
    /// </summary>
    public class FileInformationList
    {
        /// <summary>
        /// La tabla de hashing de la colección
        /// </summary>
        private Hashtable collection;

        /// <summary>
        /// Candado para control de threading
        /// </summary>
        private Object thisLock;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FileInformationList()
        {
            collection = new Hashtable();
            thisLock = new Object();
        }

        /// <summary>
        /// Agrega a la lista
        /// </summary>
        /// <param name="fileInformation">La informacion de archivo a agregar</param>
        public void add(FileInformation fileInformation) 
        {
            lock (thisLock)
            {
                if (!collection.Contains(fileInformation.Id))
                {
                    collection.Add(fileInformation.Id, fileInformation);
                }
                else
                {
                    collection.Remove(fileInformation.Id);
                    collection.Add(fileInformation.Id, fileInformation);
                } 
            }
        }

        /// <summary>
        /// Remueve de la lista
        /// </summary>
        /// <param name="id">El id  a remover</param>
        /// <returns>true si existia y fue removido, false si no</returns>
        public bool remove(Guid id)
        {
            lock (thisLock)
            {
                if (collection.Contains(id))
                {
                    collection.Remove(id);
                    return true;
                }
                else
                {
                    return false;
                } 
            }
        }

        /// <summary>
        /// Obtiene un objeto de la lista con busqueda en orden constante
        /// </summary>
        /// <param name="id">el id a buscar</param>
        /// <returns>El objeto de la lista, null si no existía</returns>
        public FileInformation getFileInformation(Guid id)
        {
            lock (thisLock)
            {
                Object o = collection[id];
                if (o != null)
                {
                    return (FileInformation)o;
                }
                else
                {
                    return null;
                } 
            }
        }

        /// <summary>
        /// Calcula el tamaño de la coleccion
        /// </summary>
        /// <returns>El tamaño de la coleccion</returns>
        public Int32 size()
        {
            lock (thisLock)
            {
                return collection.Count;
            }
            
        }

        /// <summary>
        /// Genera un array con el contenido de la coleccion
        /// </summary>
        /// <returns>el array con el contenido de la coleccion</returns>
        public FileInformation[] toArray()
        {
            lock (thisLock)
            {
                FileInformation[] us = new FileInformation[collection.Count];
                IDictionaryEnumerator en = collection.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    us[i] = (FileInformation)en.Value;
                    i++;
                }
                return us;
            }
        }
    }

    /// <summary>
    /// Datos de configuración para descarga y transferencia de archivos
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// El tamaño de las partes en las que se divide un archivo para carga o descarga
        /// </summary>
        private Int32 _partSize;

        /// <summary>
        /// Ruta del directorio de descarga de archivos
        /// </summary>
        private String _downloadDir;

        /// <summary>
        /// tiempo de espera de respuesta de transferencia
        /// </summary>
        private Int32 _fileTimeOut;

        /// <summary>
        /// La lista de archivos que se comparten
        /// </summary>
        private FileInformationList fileList;

        /// <summary>
        /// Tiempo que se suma al recibir respuesta de transferencia
        /// </summary>
        private Int32 _fileRiseUp;

        /// <summary>
        /// Intervalo de interación de transferencia de archivos
        /// </summary>
        private Int32 _timeIntervalTimer;

        /// <summary>
        /// numero maximo de cargas activas
        /// </summary>
        private Int32 _simulteneusUpload;

        /// <summary>
        /// número maximo de descargas activas
        /// </summary>
        private Int32 _simulteneusDownload;

        /// <summary>
        /// Default Constructor, inicializa los valores por omisión de la configuración.
        /// </summary>
        public FileData()
        {
            PartSize = 1024 * 1024 / 8;
            DownloadDir = createDownloadDir();
            FileTimeOut = 10;
            FileRiseUp = 5;
            TimeIntervalTimer = 1000;
            SimulteneusUpload = 1;
            SimulteneusDownload = 1;
            fileList = new FileInformationList();
        }

        /// <summary>
        /// El tamaño de las partes de los archivos enviados, en numero de bytes
        /// </summary>
        public Int32 PartSize
        {
            get { return _partSize; }
            set { _partSize = value; }
        }

        /// <summary>
        /// La ruta completa del directorio de archivos descargados
        /// </summary>
        public String DownloadDir
        {
            get { return _downloadDir; }
            set { _downloadDir = value; }
        }

        /// <summary>
        /// Tiempo de espera de respuesta de transferencia
        /// </summary>
        public Int32 FileTimeOut
        {
            get { return _fileTimeOut; }
            set { _fileTimeOut = value; }
        }

        /// <summary>
        /// Tiempo que se suma al recibir respuesta de transferencia
        /// </summary>
        public Int32 FileRiseUp
        {
            get { return _fileRiseUp; }
            set { _fileRiseUp = value; }
        }

        /// <summary>
        /// Intervalo de interación de transferencia de archivos
        /// </summary>
        public Int32 TimeIntervalTimer
        {
            get { return _timeIntervalTimer; }
            set { _timeIntervalTimer = value; }
        }

        /// <summary>
        /// numero maximo de cargas activas
        /// </summary>
        public Int32 SimulteneusUpload
        {
            get { return _simulteneusUpload; }
            set { _simulteneusUpload = value; }
        }

        /// <summary>
        /// número maximo de descargas activas
        /// </summary>
        public Int32 SimulteneusDownload
        {
            get { return _simulteneusDownload; }
            set { _simulteneusDownload = value; }
        }

        /// <summary>
        /// Crea el directorio de archivos descargados
        /// </summary>
        /// <returns>La ruta completa a los archivos descargados</returns>
        private String createDownloadDir()
        {
            String actualDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String dir = Path.Combine(actualDir, "descargas");
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>
        /// Lista de archivos que comparte este usuario
        /// Lee del directorio compartido la lista de archivos cada vez que se llama a esta propiedad
        /// </summary>
        public FileInformationList FileList
        {
            get 
            {
                return fileList; 
            }
        }
    }
}
