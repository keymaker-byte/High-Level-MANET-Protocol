using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetLayer
{
    /// <summary>
    /// Clase que representa un mensaje recibido o enviado por la RED
    /// </summary>
    public class NetMessage
    {
        /// <summary>
        /// El cuerpo del mensaje
        /// </summary>
        private byte[] _body;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="body">El contenido del mensaje</param>
        public NetMessage(byte[] body)
        {
            Body = body;
        }
        
        /// <summary>
        /// El contenido del mensaje
        /// </summary>
        public byte[] Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <summary>
        /// El largo de este mensaje, en número de bytes
        /// </summary>
        /// <returns></returns>
        public Int32 getSize()
        {
            return Body.Length;
        }
    }
}
