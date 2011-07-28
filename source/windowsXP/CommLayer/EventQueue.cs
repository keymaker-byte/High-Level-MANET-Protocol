using System;
using System.Collections.Generic;
using System.Text;
using CommLayer.Messages;
using System.Threading;

namespace CommLayer
{
    /// <summary>
    /// Cola de objetos de eventos
    /// </summary>
    internal class EventQueuePC
    {
        /// <summary>
        /// Cola de prioridad FIFO
        /// </summary>
        private Queue<Event> queue;

        /// <summary>
        /// Cantidad en la cola
        /// </summary>
        int itemCount;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EventQueuePC()
        {
            queue = new Queue<Event>();
            itemCount = 0;
        }

        /// <summary>
        /// Desencola el primer objeto de la lista, se bloquea hasta que alguien inserte un elemento
        /// </summary>
        /// <returns>El primer objeto de la cola, null si está vacía</returns>
        public Event draw()
        {
            lock (this)
            {
                while (itemCount == 0)
                {
                    Monitor.Wait(this);
                }

                Event eventHandler = queue.Dequeue();
                itemCount--;
                return eventHandler;
            }
        }

        /// <summary>
        /// Coloca un objeto en la cola
        /// </summary>
        /// <param name="m">El objeto a colocar en la cola</param>
        public void put(Event m)
        {
            lock (this)
            {
                queue.Enqueue(m);
                itemCount++;
                Monitor.Pulse(this);
            }
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
