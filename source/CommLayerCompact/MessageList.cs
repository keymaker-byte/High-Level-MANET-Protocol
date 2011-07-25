using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CommLayerCompact.Messages;

namespace CommLayerCompact
{
    /// <summary>
    /// Lista de mensajes que asegura busqueda en orden constante
    /// </summary>
    internal class MessageList
    {
        /// <summary>
        /// La tabla de hashing de la colección
        /// </summary>
        private Hashtable messageCollection;
        /// <summary>
        /// Candado para control de threading
        /// </summary>
        private Object thisLock;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public MessageList()
        {
            messageCollection = new Hashtable();
            thisLock = new Object();
        }

        /// <summary>
        /// Agrega un mensaje a la lista
        /// </summary>
        /// <param name="message">el mensaje a agregar</param>
        public void add(Message message) 
        {
            lock (thisLock)
            {
                //Si no tengo el ip, entonces agrego al usuario como alguien nuevo
                if (!messageCollection.Contains(message.Id))
                {
                    messageCollection.Add(message.Id, message);
                }
                //Si ya la tengo, actualizo el objeto usuario
                else
                {
                    messageCollection.Remove(message.Id);
                    messageCollection.Add(message.Id, message);
                } 
            }
        }

        /// <summary>
        /// Remueve un mensaje de la lista
        /// </summary>
        /// <param name="id">el fileID del mensaje a remover</param>
        /// <returns>true si existia y fue removido, false si no</returns>
        public bool remove(Guid id)
        {
            lock (thisLock)
            {
                if (messageCollection.Contains(id))
                {
                    messageCollection.Remove(id);
                    return true;
                }
                else
                {
                    return false;
                } 
            }
        }

        /// <summary>
        /// Obtiene un mensaje de la lista con busqueda en orden constante
        /// </summary>
        /// <param name="id">el fileID del mensaje</param>
        /// <returns>el mensaje de la lista, message null si no existía</returns>
        public Message getMessage(Guid id)
        {
            lock (thisLock)
            {
                Object o = messageCollection[id];
                if (o != null)
                {
                    return (Message)o;
                }
                else
                {
                    return null;
                } 
            }
        }

        /// <summary>
        /// retorna un array con la lista de mensajes
        /// </summary>
        /// <returns>un array de los mensajes listados</returns>
        public Message[] messageListToArray()
        {
            lock (thisLock)
            {
                Message[] us = new Message[messageCollection.Count];
                IDictionaryEnumerator en = messageCollection.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    us[i] = (Message)en.Value;
                    i++;
                }
                return us; 
            }
        }

        /// <summary>
        /// calcula el tamaño de la lista
        /// </summary>
        /// <returns>el tamaño de la lista</returns>
        public int size()
        {
            lock (thisLock)
            {
                return messageCollection.Count; 
            }
        }
    }
}
