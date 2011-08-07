using System;
using System.Collections.Generic;
using System.Text;
using CommLayerCompact.Messages;

namespace CommLayerCompact
{
    /// <summary>
    /// Clase para la cola de mensajes
    /// </summary>
    internal class MessageQueue
    {
        private Object thisLock = new Object();
        private Queue<Message> queue;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public MessageQueue()
        {
            queue = new Queue<Message>();
        }

        /// <summary>
        /// Obtiene el primer mensaje en la cola, null si esta vacía
        /// </summary>
        /// <returns>el primer mensaje de la cola message null si está vacía</returns>
        public Message draw()
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
        /// Coloca un mensaje en la cola
        /// </summary>
        /// <param name="m">el mensaje a colocar en la cola</param>
        public void put(Message m)
        {
            lock (thisLock)
            {
                queue.Enqueue(m);
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
                return queue.Count;
            }
        }
        
    }
}
