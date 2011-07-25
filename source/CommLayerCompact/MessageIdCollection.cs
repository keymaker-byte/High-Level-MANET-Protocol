using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommLayerCompact
{
    /// <summary>
    /// Colección de fileID's de mensajes. Estructura usada por el Router
    /// Los ids son unicos en la colección
    /// Si se agrega un fileID cuando el tamaño es igual a MaxSize, se borra el primero en la cola y se agrega el afinal de la cola
    /// </summary>
    internal class MessageIdCollection
    {
        /// <summary>
        /// Tabla de hashing para busqueda de orden constante
        /// </summary>
        private Hashtable messageIdList;
        /// <summary>
        /// Cola de prioridad para entrada y salida en orden constante
        /// </summary>
        private Queue<Guid> messageIdqueue;
        /// <summary>
        /// Control de threeading
        /// </summary>
        private Object thisLock;
        /// <summary>
        /// El tamaño máximo de la colección
        /// </summary>
        private Int32 maxSize;        

        /// <summary>
        /// Constructor
        /// </summary>
        public MessageIdCollection()
        {
            thisLock = new Object();
            messageIdList = new Hashtable();
            messageIdqueue = new Queue<Guid>();
            MaxSize = 100;
        }

        /// <summary>
        /// El tamaño máximo de la colección
        /// </summary>
        public Int32 MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void add(Guid id)
        {
            lock (thisLock)
            {
                if (!messageIdList.Contains(id))
                {
                    if (messageIdqueue.Count >= MaxSize)
                    {
                        Guid deadId = messageIdqueue.Dequeue();
                        messageIdList.Remove(deadId);
                    }
                    messageIdList.Add(id, id);
                    messageIdqueue.Enqueue(id);
                }
            }
        }

        /// <summary>
        /// Retorna el tamaño de la cola
        /// </summary>
        /// <returns>el tamaño de la cola</returns>
        public int size()
        {
            lock (thisLock)
            {
                return messageIdqueue.Count;
            }
        }

        /// <summary>
        /// Retorna true si existe el fileID en la colección
        /// </summary>
        public bool contains(Guid id)
        {
            lock (thisLock)
            {
                return messageIdList.Contains(id);
            }
        }
    }
}
