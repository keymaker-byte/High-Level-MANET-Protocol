using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommLayerCompact
{
    /// <summary>
    /// Lista de Tipos de Mensajes
    /// </summary>
    public class MessageTypeList
    {
        /// <summary>
        /// La tabla de hashing de la colección
        /// </summary>
        internal Hashtable Collection;

        /// <summary>
        /// Candado para control de threading
        /// </summary>
        private Object thisLock;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MessageTypeList()
        {
            Collection = new Hashtable();
            thisLock = new Object();
        }

        /// <summary>
        /// Agrega un tipo de mensaje a la lista
        /// </summary>
        /// <param name="messageType">El tipo del mensaje, los tipos entre 0 y 1000 esta reservados para el sistema</param>
        /// <param name="messageTypeObject">El tipo del mensaje, como objeto tipo, puede ser obtenido mediante typeof</param>
        public void add(Int32 messageType, Type messageTypeObject) 
        {
            lock (thisLock)
            {
                //Si no tengo el ip, entonces agrego al usuario como alguien nuevo
                if (!Collection.Contains(messageType))
                {
                    Collection.Add(messageType, messageTypeObject);
                }
                //Si ya la tengo, actualizo el objeto usuario
                else
                {
                    Collection.Remove(messageType);
                    Collection.Add(messageType, messageTypeObject);
                }
            }
        }
    }
}
