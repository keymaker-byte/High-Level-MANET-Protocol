package hlmp.CommLayer;

import java.net.InetAddress;
//import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;

/**
 *  Lista de usuarios de la MANET, La llave de los usuarios es su número IP
 */
public class NetUserList {

	/**
	 * Una tabla de hashing para búsqueda en orden constante
	 */	
	private HashMap<InetAddress, NetUser> usersCollection;

	/**
	 * Default Constructor
	 */
	public NetUserList()
	{
		usersCollection = new HashMap<InetAddress, NetUser>();
	}

	/**
	 * Agrega un usuario a la colección (Asocia una IP a un usuario)
	 * @param ip La ip del usuario
	 * @param newUser El usuario
	 * @return true si efectivamente era un nuevo usuario, false si tuvo que borrar a uno ya existente
	 */
	public synchronized boolean add(InetAddress ip, NetUser newUser) 
	{
		//Si no tengo el ip, entonces agrego al usuario como alguien nuevo
		if (!usersCollection.containsKey(ip))
		{
			usersCollection.put(ip, newUser);
			return true;
		}
		//Si ya la tengo, actualizo el objeto usuario
		else
		{
//			NetUser oldUser = getUser(ip);
			usersCollection.remove(ip);
			usersCollection.put(ip, newUser);
			return false;
		} 
	}

	/**
	 * Remueve a un usuario de la colección
	 * @param ip La ip del usuario a remover
	 * @return true si existía y false si no
	 */
	public synchronized boolean remove(InetAddress ip)
	{
		if (usersCollection.containsKey(ip))
		{
			usersCollection.remove(ip);
			return true;
		}
		else
		{
			return false;
		} 
	}

	/**
	 * Obtiene a un usuario de la colección
	 * @param ip La ip del usuario a obtener
	 * @return El usuario, null si el usuario no existía en la colección
	 */
	public synchronized NetUser getUser(InetAddress ip)
	{

		NetUser o = usersCollection.get(ip);
		if (o != null)
		{
			return o;
		}
		else
		{
			return null;
		} 
	}
	
	/**
     * Fabrica un array con la colección de usuarios
     * @return Un array de usuarios
     */
    public synchronized NetUser[] userListToArray()
    {
            NetUser[] us = new NetUser[usersCollection.size()];
            Iterator<NetUser> iterator = usersCollection.values().iterator();
            int i = 0;
            while (iterator.hasNext())
            {
                us[i] = iterator.next();
                i++;
            }
            return us; 
    }

    /**
     * Calcula el tamaño de la colección de usuarios
     * @return El tamaño de la colección
     */
    public synchronized int size()
    {
    	return usersCollection.size(); 
    }

	@Override
	public synchronized String toString() {
		NetUser[] us=this.userListToArray();
		String t="NetUserList:\n";
		for(NetUser u: us){
			t+="\t"+u.getName()+"\n";
		}
		return t;
	}

    
}
