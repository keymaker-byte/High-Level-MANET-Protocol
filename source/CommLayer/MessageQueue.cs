using System;
using System.Collections.Generic;
using System.Text;
using CommLayer.Messages;

namespace CommLayer
{
    /// <summary>
    /// Cola de mensajes
    /// </summary>
    internal class MessageQueue
    {
        /// <summary>
        /// Objeto para control de threading
        /// </summary>
        private Object thisLock = new Object();

        /// <summary>
        /// Cola de prioridad FIFO
        /// </summary>
        private Queue<Message> queue;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MessageQueue()
        {
            queue = new Queue<Message>();
        }

        /// <summary>
        /// Desencola el primer mensaje de la lista
        /// </summary>
        /// <returns>El primer mensaje de la cola, null si está vacía</returns>
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
        /// <param name="m">El mensaje a colocar en la cola</param>
        public void put(Message m)
        {
            lock (thisLock)
            {
                queue.Enqueue(m);
            }
        }

        /// <summary>
        /// Calcula el tamaño de la cola
        /// </summary>
        /// <returns>El tamaño de la cola</returns>
        public int size()
        {
            lock (thisLock)
            {
                return queue.Count;
            }
        }
    }
}
