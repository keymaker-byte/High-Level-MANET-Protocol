using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
        /// Constructor vacío
        /// </summary>
        public NetMessageQueue()
        {
            queue = new Queue<NetMessage>();
            itemCount = 0;
        }

        /// <summary>
        /// Obtiene el primer mensaje en la cola, null si esta vacía
        /// </summary>
        /// <returns>el primer mensaje de la cola o null si está vacía</returns>
        public NetMessage draw()
        {
            lock (this)
            {
                while (itemCount == 0)
                {
                    Monitor.Wait(this);
                }

                NetMessage netMessage = queue.Dequeue();
                itemCount--;
                return netMessage;
            } 
        }

        /// <summary>
        /// Coloca un mensaje en la cola
        /// </summary>
        /// <param name="m">el mensaje a colocar en la cola</param>
        public void put(NetMessage m)
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
