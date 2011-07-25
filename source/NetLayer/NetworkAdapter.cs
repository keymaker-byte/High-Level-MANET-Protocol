using System;
using System.Collections.Generic;

namespace NetLayer
{
    /// <summary>
    /// Clase que representa un dispositivo de red del sistema
    /// </summary>
    public class NetworkAdapter
    {
        private UInt32 _index;
        private String _id;
        private String _description;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public NetworkAdapter()
        {
        }

        /// <summary>
        /// El Index del dispositivo, en la lista de dispositivos del sistema operativo
        /// </summary>
        public UInt32 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// El SettingID del dispositivo en la lista de dispositivos del sistema operativo
        /// </summary>
        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// El Description del dispositivo en la lista de dispositivos del sistema operativo
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
            if (Description.Length > 30)
            {
                return Description.Substring(0, 30);
            }
            else if (Description.Length <= 1)
            {
                return "device";
            }
            return Description;
        }

        /// <summary>
        /// Sobreescribe equals
        /// </summary>
        /// <param name="obj">otro objeto</param>
        /// <returns>true si tienen el mismo Index, false si no</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(NetworkAdapter)))
            {
                NetworkAdapter aobj = (NetworkAdapter)obj;
                if (this.Index.Equals(aobj.Index))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// HasChode
        /// </summary>
        /// <returns>el hashcode del padre</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
