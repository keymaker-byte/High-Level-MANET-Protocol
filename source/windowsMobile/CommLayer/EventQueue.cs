using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenNETCF.Threading;

namespace CommLayerCompact
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
        /// El monitor
        /// </summary>
        Monitor2 monitor;
        

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EventQueuePC()
        {
            queue = new Queue<Event>();
            itemCount = 0;
            monitor = new Monitor2();
        }

        /// <summary>
        /// Desencola el primer objeto de la lista, se bloquea hasta que alguien inserte un elemento
        /// </summary>
        /// <returns>El primer objeto de la cola, null si está vacía</returns>
        public Event draw()
        {
            monitor.Enter();
            while (itemCount == 0)
            {
                monitor.Wait();
            }

            Event eventHandler = queue.Dequeue();
            itemCount--;
            monitor.Exit();
            return eventHandler;
        }

        /// <summary>
        /// Coloca un objeto en la cola
        /// </summary>
        /// <param name="m">El objeto a colocar en la cola</param>
        public void put(Event m)
        {
            monitor.Enter();
            queue.Enqueue(m);
            itemCount++;
            monitor.Pulse();
            monitor.Exit();
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
