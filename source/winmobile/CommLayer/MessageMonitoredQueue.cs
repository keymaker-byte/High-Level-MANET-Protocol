using System;
using System.Collections.Generic;
using System.Text;
using CommLayer.Messages;
using System.Threading;
using OpenNETCF.Threading;

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
        /// El monitor
        /// </summary>
        Monitor2 monitor;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MessageMonitoredQueue()
        {
            queue = new Queue<Message>();
            itemCount = 0;
            monitor = new Monitor2();
        }

        /// <summary>
        /// Obtiene el primer mensaje en la cola, null si esta vacía
        /// </summary>
        /// <returns>el primer mensaje de la cola o null si está vacía</returns>
        public Message draw()
        {
            monitor.Enter();
            while (itemCount == 0)
            {
                monitor.Wait();
            }

            Message message = queue.Dequeue();
            itemCount--;
            monitor.Exit();
            return message;
        }

        /// <summary>
        /// Coloca un mensaje en la cola
        /// </summary>
        /// <param name="m">el mensaje a colocar en la cola</param>
        public void put(Message m)
        {
            monitor.Enter();
            queue.Enqueue(m);
            itemCount++;
            monitor.Pulse();
            monitor.Exit();
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
            monitor.Enter();
            monitor.Pulse();
            monitor.Exit();
        }
    }
}
