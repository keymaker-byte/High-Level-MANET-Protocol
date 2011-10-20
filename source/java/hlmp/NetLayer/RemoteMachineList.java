package hlmp.NetLayer;

import java.net.InetAddress;
import java.util.HashMap;
import java.util.Iterator;

public class RemoteMachineList {

	private HashMap<InetAddress, RemoteMachine> remoteMachineCollection;

	/**
	 * Constructor vacío
	 */
	public RemoteMachineList()
	{
		remoteMachineCollection = new HashMap<InetAddress, RemoteMachine>();
	}

	/**
	 * Agrega una maquina a la colección, asocia una IP a la maquina como llave de la colección
	 * @param ip la ip de la máquina
	 * @param newRemoteMachine la maquina a agregar 
	 */
	public synchronized void add(InetAddress ip, RemoteMachine newRemoteMachine) 
	{
		remoteMachineCollection.put(ip, newRemoteMachine);
	}

	/**
	 * Remueve a una maquina de la colección
	 * @param remoteMachine la maquina a remover 
	 * @return true si la maquina existía, false si no
	 */
	public synchronized boolean remove(RemoteMachine remoteMachine)
	{
		RemoteMachine listedRemoteMachine = getRemoteMachine(remoteMachine.getIp());
		if (listedRemoteMachine != null && remoteMachine.getId().equals(listedRemoteMachine.getId()))
		{
			remoteMachineCollection.remove(listedRemoteMachine.getIp());
			return true;
		}
		return false;
	}

	/**
	 * Obtiene a una maquina de la colección segun la Ip
	 * @param ip 
	 */
	public synchronized RemoteMachine getRemoteMachine(InetAddress ip)
	{
		RemoteMachine rm = remoteMachineCollection.get(ip);
		if (rm != null)
		{
			return rm;
		}
		else
		{
			return null;
		} 
	}

	/**
	 * Construye un array con los elementos de la lista
	 * @return Un array con los elementos de la lista
	 */
	public synchronized RemoteMachine[] toObjectArray()
	{
		RemoteMachine[] us = new RemoteMachine[remoteMachineCollection.size()];
		Iterator<RemoteMachine> iterator = remoteMachineCollection.values().iterator();
		int i = 0;
		while (iterator.hasNext())
		{
			us[i] = iterator.next();
			i++;
		}
		return us; 
	}

	/**
	 * Calcula el tamaño de la lista
	 * @return El tamaño de la lista
	 */
	public synchronized int size()
	{
		return remoteMachineCollection.size(); 
	}
}
