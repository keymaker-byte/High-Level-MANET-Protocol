package hlmp.commlayer;
import java.io.BufferedReader;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.PrintWriter;

import javax.xml.bind.annotation.XmlRootElement;

import hlmp.netlayer.*;

@XmlRootElement
public class Configuration {
	/// <summary>
    /// Los datos del usuario local
    /// </summary>
    private NetUser netUser;

    /// <summary>
    /// Los datos de la red
    /// </summary>
    private NetData netData;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Configuration()
    {
        this.netData = new NetData();
        this.netUser = new NetUser();
    }

    /// <summary>
    /// Constructor Parametrizado
    /// </summary>
    /// <param name="netUser">Datos del usuario</param>
    /// <param name="netData">Datos de configuración de red</param>
    public Configuration(NetUser netUser, NetData netData)
    {
        this.netUser = netUser;
        this.netData = netData;
    }

    public NetUser getNetUser() {
		return netUser;
	}

	public void setNetUser(NetUser netUser) {
		this.netUser = netUser;
	}

	public NetData getNetData() {
		return netData;
	}

	public void setNetData(NetData netData) {
		this.netData = netData;
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
				ObjectSerializer mySerializer = new ObjectSerializer(configuration);
				String serialized = mySerializer.serialize(configuration);
				PrintWriter pw = new PrintWriter(new FileWriter(directory + "CommLayer.conf"));
				pw.print(serialized);
				pw.close();
			}
		}
		//        catch (ThreadAbortException e)
		//        {
		//            throw e;
		//        }
		catch (Exception e)
		{
			//throw;
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
        	ObjectSerializer mySerializer = new ObjectSerializer(new Configuration());
        	BufferedReader bf = new BufferedReader(new FileReader(directory + "CommLayer.conf"));
        	String line = bf.readLine();
            Configuration conf = (Configuration)mySerializer.unserialize(line);
            bf.close();
            return conf;
        }
//        catch (ThreadAbortException e)
//        {
//            throw e;
//        }
        catch (Exception e)
        {
            return null;
        }
    }
}
