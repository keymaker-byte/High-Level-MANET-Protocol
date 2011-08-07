using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using NetLayerCompact;
using System.Threading;

namespace CommLayerCompact
{
    /// <summary>
    /// Datos de configuración necesarios para establecer la estructura de comunicación
    /// </summary>
    public class Configuration 
    {

        /// <summary>
        /// Los datos del usuario local
        /// </summary>
        private NetUser _netUser;

        /// <summary>
        /// Los datos de la red
        /// </summary>
        private NetData _netData;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configuration()
        {
            this.NetData = new NetData();
            this.NetUser = new NetUser();
        }

        /// <summary>
        /// Constructor Parametrizado
        /// </summary>
        /// <param name="netUser">Datos del usuario</param>
        /// <param name="netData">Datos de configuración de red</param>
        public Configuration(NetUser netUser, NetData netData)
        {
            this.NetUser = netUser;
            this.NetData = netData;
        }

        /// <summary>
        /// El usuario que ejecuta esta estructura de comunicación
        /// </summary>
        public NetUser NetUser
        {
            get { return _netUser; }
            set { _netUser = value; }
        }

        /// <summary>
        /// Los datos de red
        /// </summary>
        public NetData NetData
        {
            get { return _netData; }
            set { _netData = value; }
        }

        /// <summary>
        /// Serializa un objeto de este tipo
        /// </summary>
        /// <param name="directory">El directorio donde serializarlo debe terminar con el simbolo "/"</param>
        /// <param name="configuration">El objeto configuración a serializar</param>
        public static void save(String directory, Configuration configuration)
        {
            try
            {
                if (configuration != null)
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
                    StreamWriter myWriter = new StreamWriter(File.Open(directory + "CommLayer.conf", FileMode.Create, FileAccess.Write));
                    mySerializer.Serialize(myWriter, configuration);
                    myWriter.Close();
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Carga un objeto serializado de este tipo
        /// </summary>
        /// <param name="directory">El directorio donde se encuentra el archivo serializado debe terminar con el simbolo "/"</param>
        /// <returns>Un objeto de tipo Configuration con los datos que estaban serializados</returns>
        public static Configuration load(String directory)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
                FileStream myFileStream = new FileStream(directory + "CommLayer.conf", FileMode.Open);
                Configuration conf = (Configuration)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                return conf;
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
