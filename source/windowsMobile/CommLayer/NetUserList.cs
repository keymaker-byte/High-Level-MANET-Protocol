using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;

namespace CommLayer
{
    /// <summary>
    ///  Lista de usuarios remotos observados en la RED
    /// </summary>
    public class NetUserList
    {
        private Hashtable usersCollection;
        private Object thisLock;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        internal NetUserList()
        {
            usersCollection = new Hashtable();
            thisLock = new Object();
        }

        /// <summary>
        /// Agrega un usuario a la colección (Asocia una IP a un usuario)
        /// </summary>
        /// <param name="ip">la ip del usuario</param>
        /// <param name="newUser">el usuario</param>
        /// <returns>true si efectivamente era un nuevo usuario, false si tuvo que borrar al ya existente</returns>
        internal bool add(IPAddress ip, NetUser newUser) 
        {

            lock (thisLock)
            {
                //Si no tengo el ip, entonces agrego al usuario como alguien nuevo
                if (!usersCollection.Contains(ip))
                {
                    usersCollection.Add(ip, newUser);
                    return true;
                }
                //Si ya la tengo, actualizo el objeto usuario
                else
                {
                    NetUser oldUser = getUser(ip);
                    usersCollection.Remove(ip);
                    usersCollection.Add(ip, newUser);
                    return false;
                } 
            }
        }

        /// <summary>
        /// Remueve a un usuario de la colección
        /// </summary>
        /// <param name="ip">La ip del usuario a remover</param>
        /// <returns>true si existía y false si no</returns>
        internal bool remove(IPAddress ip)
        {
            lock (thisLock)
            {
                if (usersCollection.Contains(ip))
                {
                    usersCollection.Remove(ip);
                    return true;
                }
                else
                {
                    return false;
                } 
            }
        }

        /// <summary>
        /// Obtiene a un usuario de la colección
        /// </summary>
        /// <param name="ip">La ip del usuario a obtener</param>
        /// <returns>El usuario</returns>
        internal NetUser getUser(IPAddress ip)
        {
            lock (thisLock)
            {
                Object o = usersCollection[ip];
                if (o != null)
                {
                    return (NetUser)o;
                }
                else
                {
                    return null;
                } 
            }
        }

        /// <summary>
        /// Retorna la colección de usuarios como un array simple de usuarios
        /// </summary>
        /// <returns>un array de usuarios</returns>
        public NetUser[] userListToArray()
        {
            lock (thisLock)
            {
                NetUser[] us = new NetUser[usersCollection.Count];
                IDictionaryEnumerator en = usersCollection.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    us[i] = (NetUser)en.Value;
                    i++;
                }
                return us; 
            }
        }

        /// <summary>
        /// Calcula el tamaño de la colección de usuarios
        /// </summary>
        /// <returns>el tamaño de la colección</returns>
        public int size()
        {

            lock (thisLock)
            {
                return usersCollection.Count; 
            }
        }
    }
}
