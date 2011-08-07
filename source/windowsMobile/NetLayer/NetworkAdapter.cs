using System;
using System.Collections.Generic;
using OpenNETCF.Net.NetworkInformation;
using OpenNETCF.Net;

namespace NetLayerCompact
{
    /// <summary>
    /// Clase que representa un dispositivo de red del sistema
    /// </summary>
    public class NetworkAdapter
    {
        private String _name;
        private String _description;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public NetworkAdapter()
        {
        }

        /// <summary>
        /// El nombre del adaptador
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// La descripción o nombre comercial del adaptador
        /// </summary>
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Metodo toString sobreescrito
        /// </summary>
        /// <returns>la representación en String</returns>
        public override string ToString()
        {
            if (Description.Length > 20)
            {
                return Name + " " + Description.Substring(0, 20);
            }
            else if (Description.Length <= 1)
            {
                return "device";
            }
            return Name + " " + Description;
        }

        /// <summary>
        /// Sobreescribe el metodo para comparar igualdad,
        /// </summary>
        /// <param name="obj">el objeto con el cual comparar igualdad</param>
        /// <returns>true si ambos tienen el mismo nombre, false si no</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(NetworkAdapter)))
            {
                NetworkAdapter aobj = (NetworkAdapter)obj;
                if (this.Name.Equals(aobj.Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sobreescribe el hash
        /// </summary>
        /// <returns>el hash del padre</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
