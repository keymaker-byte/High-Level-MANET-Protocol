using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace NetLayer
{
    /// <summary>
    /// Lista de maquinas remotas conectadas directamente por TCP
    /// </summary>
    public class RemoteMachineList
    {
        private Hashtable remoteMachineCollection;
        private Object thisLock = new Object();

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public RemoteMachineList()
        {
            remoteMachineCollection = new Hashtable();
        }

        /// <summary>
        /// Agrega una maquina a la colección, asocia una IP a la maquina como llave de la colección
        /// </summary>
        /// <param name="ip">la ip de la máquina</param>
        /// <param name="newRemoteMachine">la maquina a agregar</param>
        public void add(IPAddress ip, RemoteMachine newRemoteMachine) 
        {
            lock (thisLock)
            {
                remoteMachineCollection.Add(ip, newRemoteMachine);
            }
        }

        /// <summary>
        /// Remueve a una maquina de la colección
        /// </summary>
        /// <param name="remoteMachine">la maquina a remover</param>
        /// <returns>true si la maquina existía, false si no</returns>
        public bool remove(RemoteMachine remoteMachine)
        {
            lock (thisLock)
            {
                RemoteMachine listedRemoteMachine = getRemoteMachine(remoteMachine.Ip);
                if (listedRemoteMachine != null && remoteMachine.Id.Equals(listedRemoteMachine.Id))
                {
                    remoteMachineCollection.Remove(listedRemoteMachine.Ip);

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Obtiene a una maquina de la colección segun la Ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public RemoteMachine getRemoteMachine(IPAddress ip)
        {
            lock (thisLock)
            {
                Object o = remoteMachineCollection[ip];
                if (o != null)
                {
                    return (RemoteMachine)o;
                }
                else
                {
                    return null;
                } 
            }
        }

        /// <summary>
        /// Crea un array con los objetos de la coleccion
        /// </summary>
        /// <returns>un array de maquinas remotas</returns>
        public RemoteMachine[] toObjectArray()
        {
            lock (thisLock)
            {
                RemoteMachine[] us = new RemoteMachine[remoteMachineCollection.Count];
                IDictionaryEnumerator en = remoteMachineCollection.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    us[i] = (RemoteMachine)en.Value;
                    i++;
                }
                return us; 
            }
        }

        /// <summary>
        /// Calcula el tamaño de la colección
        /// </summary>
        /// <returns>el tamaño de la colección</returns>
        public int size()
        {
            lock (thisLock)
            {
                return remoteMachineCollection.Count; 
            }
        }
    }
}
