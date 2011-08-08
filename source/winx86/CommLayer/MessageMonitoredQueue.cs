using System;
using System.Collections.Generic;
using System.Text;
using CommLayer.Messages;
using System.Threading;

namespace CommLayer
{
    /// <summary>
    /// Cola de mensajes
    /// </summary>
    internal class MessageMonitoredQueue
    {

        /// <summary>
        /// Cola de prioridad FIFO
        /// </summary>
        private Queue<Message> queue;

        /// <summary>
        /// Cantidad en la cola
        /// </summary>
        int itemCount;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MessageMonitoredQueue()
        {
            queue = new Queue<Message>();
            itemCount = 0;
        }

        /// <summary>
        /// Obtiene el primer mensaje en la cola, null si esta vacía
        /// </summary>
        /// <returns>el primer mensaje de la cola o null si está vacía</returns>
        public Message draw()
        {
            lock (this)
            {
                while (itemCount == 0)
                {
                    Monitor.Wait(this);
                }

                Message message = queue.Dequeue();
                itemCount--;
                return message;
            }
        }

        /// <summary>
        /// Coloca un mensaje en la cola
        /// </summary>
        /// <param name="m">el mensaje a colocar en la cola</param>
        public void put(Message m)
        {
            lock (this)
            {
                queue.Enqueue(m);
                itemCount++;
                Monitor.Pulse(this);
            }
        }

        /// <summary>
        /// Retorna el tamaño de la cola
        /// </summary>
        /// <returns>el tamaño de la cola</returns>
        public int size()
        {
            return queue.Count;
        }

        /// <summary>
        /// Desbloquea forzosamente el bloquedo de draw
        /// </summary>
        public void unblok()
        {
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
    }
}
