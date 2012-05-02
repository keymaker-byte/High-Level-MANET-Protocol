package hlmp.CommLayer;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.PrintWriter;

import org.simpleframework.xml.Default;

import hlmp.NetLayer.*;


@Default
public class Configuration {
	/**
     * Los datos del usuario local
     */
    private NetUser netUser;

    /**
     * Los datos de la red
     */
    private NetData netData;

    /**
     * Default Constructor
     */
    public Configuration()
    {
        this.netData = new NetData();
        this.netUser = new NetUser();
    }

    /**
     * Constructor Parametrizado
     * @param netUser Datos del usuario
     * @param netData Datos de configuración de red
     */
    public Configuration(NetUser netUser, NetData netData)
    {
        this.netUser = netUser;
        this.netData = netData;
    }

    /**
     * 
     * @return Los datos del usuario local
     */
    public NetUser getNetUser() {
		return netUser;
	}

	public void setNetUser(NetUser netUser) {
		this.netUser = netUser;
	}

	/**
	 * 
	 * @return Los datos de la red
	 */
	public NetData getNetData() {
		return netData;
	}

	public void setNetData(NetData netData) {
		this.netData = netData;
	}


    /**
     * Serializa un objeto de este tipo
     * @param directory El directorio donde serializarlo debe terminar con el simbolo "/"
     * @param configuration El objeto configuración a serializar
     */
	public static void save(String directory, Configuration configuration)
    {
		try
		{
			if (configuration != null)
			{
				ObjectSerializer mySerializer = new ObjectSerializer();
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

    /**
     * Carga un objeto serializado de este tipo
     * @param directory El directorio donde se encuentra el archivo serializado debe terminar con el simbolo "/"
     * @return Un objeto de tipo Configuration con los datos que estaban serializados
     */
    public static Configuration load(String directory)
    {
        try
        {
        	ObjectSerializer mySerializer = new ObjectSerializer();
        	BufferedReader bf = new BufferedReader(new FileReader(directory + "CommLayer.conf"));
        	String line = bf.readLine();
            Configuration conf = (Configuration)mySerializer.unserialize(line, Configuration.class);
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
