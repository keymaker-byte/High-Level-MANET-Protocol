using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenNETCF.Threading;

namespace NetLayer
{
    /// <summary>
    /// Clase para la cola de mensajes que llegan por la RED
    /// </summary>
    public class NetMessageQueue
    {
        private Queue<NetMessage> queue;

        /// <summary>
        /// Cantidad en la cola
        /// </summary>
        int itemCount;

        /// <summary>
        /// El monitor
        /// </summary>
        Monitor2 monitor;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public NetMessageQueue()
        {
            queue = new Queue<NetMessage>();
            itemCount = 0;
            monitor = new Monitor2();
        }

        /// <summary>
        /// Obtiene el primer mensaje en la cola, null si esta vacía
        /// </summary>
        /// <returns>el primer mensaje de la cola o null si está vacía</returns>
        public NetMessage draw()
        {
            monitor.Enter();
            while (itemCount == 0)
            {
                monitor.Wait();
            }

            NetMessage netMessage = queue.Dequeue();
            itemCount--;
            monitor.Exit();
            return netMessage;
        }

        /// <summary>
        /// Coloca un mensaje en la cola
        /// </summary>
        /// <param name="m">el mensaje a colocar en la cola</param>
        public void put(NetMessage m)
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
