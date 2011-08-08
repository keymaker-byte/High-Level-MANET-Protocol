using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommLayer
{
    /// <summary>
    /// Lista de SubProtocolos
    /// </summary>
    public class SubProtocolList
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
        public SubProtocolList()
        {
            Collection = new Hashtable();
            thisLock = new Object();
        }

        /// <summary>
        /// Agrega un sub protocolo a la lista
        /// </summary>
        /// <param name="subProtocolType">El tipo del subprotocolo (los numeros entre 0 y 1000 esta reservados para el sistema)</param>
        /// <param name="subProtocol">un objeto de sub protocolo inicializado</param>
        public void add(Int32 subProtocolType, SubProtocolI subProtocol) 
        {
            lock (thisLock)
            {
                //Si no tengo el ip, entonces agrego al usuario como alguien nuevo
                if (!Collection.Contains(subProtocolType))
                {
                    Collection.Add(subProtocolType, subProtocol);
                }
                //Si ya la tengo, actualizo el objeto usuario
                else
                {
                    Collection.Remove(subProtocolType);
                    Collection.Add(subProtocolType, subProtocol);
                }
            }
        }
    }
}
