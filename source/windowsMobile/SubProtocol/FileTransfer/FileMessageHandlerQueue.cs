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

namespace SubProtocolCompact.FileTransfer
{
    /// <summary>
    /// Cola de manejadores de archivos
    /// </summary>
    internal class FileMessageHandlerQueue
    {
        /// <summary>
        /// Objeto para el control de concurrencia
        /// </summary>
        private Object thisLock = new Object();

        /// <summary>
        /// Cola de manejadores de archivos
        /// </summary>
        private Queue<FileMessageHandler> queue;

        /// <summary>
        /// Default Constructor 
        /// </summary>
        public FileMessageHandlerQueue()
        {
            queue = new Queue<FileMessageHandler>();
        }

        /// <summary>
        /// Obtiene el primer manejador en la cola, null si esta vacía
        /// </summary>
        /// <returns>El primer mensaje de la cola fileInformation null si está vacía</returns>
        public FileMessageHandler draw()
        {
            lock (thisLock)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Chequea si existe un file handler con el id  correspondiente
        /// </summary>
        /// <param name="id">el id a buscar</param>
        /// <returns>true si existe, false si no</returns>
        public bool contains(Guid id)
        {
            lock (thisLock)
            {
                FileMessageHandler[] fileHandlers = toArray();
                for (int i = 0; i < fileHandlers.Length; i++)
                {
                    if (fileHandlers[i].Id.Equals(id))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Coloca un manejador en la cola
        /// </summary>
        /// <param name="fileMessageHandler">El manejador a colocar en la cola</param>
        public bool put(FileMessageHandler fileMessageHandler)
        {
            lock (thisLock)
            {
                if (!contains(fileMessageHandler.Id))
                {
                    queue.Enqueue(fileMessageHandler);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Retorna el tamaño de la cola
        /// </summary>
        /// <returns>El tamaño de la cola</returns>
        public int size()
        {
            lock (thisLock)
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// Convierte los elementos de la cola a un array
        /// </summary>
        /// <returns>Un array con los elementos de la cola</returns>
        public FileMessageHandler[] toArray()
        {
            lock (thisLock)
            {
                return queue.ToArray(); 
            }
        }
    }
}
