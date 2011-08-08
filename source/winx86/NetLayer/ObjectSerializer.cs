using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace NetLayer
{
    /// <summary>
    /// Clase para serializar objetos y hacerlos eviables por la red
    /// </summary>
    public class ObjectSerializer
    {
        private XmlSerializer serializer;

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="type">El tipo de objeto a serializar</param>
        /// <param name="extraTypes">tipos adicionales a serializar</param>
        public ObjectSerializer(Type type, Type[] extraTypes) 
        {
            if (extraTypes != null)
                serializer = new XmlSerializer(type, extraTypes);
            else
                serializer = new XmlSerializer(type);
        }

        /// <summary>
        /// Convierte un objeto en un string
        /// </summary>
        /// <param name="obj">el objeto a convertir</param>
        /// <returns>el string que representa al objeto</returns>
        public string serialize(Object obj) 
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new System.Xml.XmlTextWriter(sw);
            serializer.Serialize(writer, obj);
            writer.Flush();
            writer.Close();
            return sw.ToString();
        }

        /// <summary>
        /// Convierte un string creado con serialize en un objeto
        /// </summary>
        /// <param name="obj">el objeto serializado</param>
        /// <returns>El objeto construido a partir de la serialización</returns>
        public Object unserialize(string obj) 
        {
            StringReader strxml = new StringReader(obj);
            XmlTextReader reader = new XmlTextReader(strxml);
            if(serializer.CanDeserialize(reader))
            {
                return serializer.Deserialize(reader);
            }
            else 
            {
                return null;
            }
        }
    }
}
