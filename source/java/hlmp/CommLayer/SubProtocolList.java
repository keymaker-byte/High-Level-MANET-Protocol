package hlmp.CommLayer;

import java.util.HashMap;

public class SubProtocolList {
	
	/**
     * La tabla de hashing de la colección
     */
    private HashMap<Integer, SubProtocol> Collection;


    /**
     * Default Constructor
     */
    public SubProtocolList()
    {
        Collection = new HashMap<Integer, SubProtocol>();
    }
    
    /**
     * Agrega un sub protocolo a la lista
     * @param subProtocolType El tipo del subprotocolo (los numeros entre 0 y 1000 esta reservados para el sistema)
     * @param subProtocol un objeto de sub protocolo inicializado
     */
    public synchronized void add(int subProtocolType, SubProtocol subProtocol) 
    {
            //Si no tengo el ip, entonces agrego al usuario como alguien nuevo
            if (!Collection.containsKey(subProtocolType))
            {
                Collection.put(subProtocolType, subProtocol);
            }
            //Si ya la tengo, actualizo el objeto usuario
            else
            {
                Collection.remove(subProtocolType);
                Collection.put(subProtocolType, subProtocol);
            }
    }

	public HashMap<Integer, SubProtocol> getCollection() {
		return Collection;
	}

}
