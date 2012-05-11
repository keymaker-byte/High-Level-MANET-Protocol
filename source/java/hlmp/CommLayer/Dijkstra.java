package hlmp.CommLayer;

import hlmp.CommLayer.Constants.*;
import hlmp.CommLayer.Exceptions.*;
import hlmp.CommLayer.Interfaces.*;
import hlmp.NetLayer.NetData;

import java.util.AbstractCollection;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Stack;

/**
 * Implements a generalized Dijkstra's algorithm to calculate 
 * both minimum distance and minimum path.
 * 
 * For this algorithm, all nodes should be provided, and handled
 * in the delegate methods, including the startNetworkingHandler and finish nodes.
 */
public class Dijkstra implements InternodeTraversalCost, NearbyNodesHint{

	protected InternodeTraversalCost traversalCost;
	protected NearbyNodesHint hint;

	/**
	 * Creates an instance of the Dijkstra class.
	 * @param totalNodeCount The total number of nodes in the graph.
	 * @param delegate
	 * @throws ArgumentOutOfRangeException
	 * @throws ArgumentNullException
	 */
	
	/**
	 * Creates an instance of the Dijkstra class.
	 * @param totalNodeCount The total number of nodes in the graph.
	 * @param traversalCost The interface that can provide the cost of a transition between any two nodes. 
	 * @param hint An optional interface that can provide a small subset of nodes that a given nodeIndex may be connected to. 
	 * @throws ArgumentOutOfRangeException
	 * @throws ArgumentNullException
	 */
	public Dijkstra(int totalNodeCount, InternodeTraversalCost traversalCost, NearbyNodesHint hint) throws ArgumentOutOfRangeException, ArgumentNullException
	{
		if (totalNodeCount < 3) throw new ArgumentOutOfRangeException("totalNodeCount: "+totalNodeCount+ ". Expected a minimum of 3.");
		if (traversalCost == null) throw new ArgumentNullException("traversalCost");
		this.TotalNodeCount = totalNodeCount;
		this.hint = hint;
		this.traversalCost = traversalCost;
	}


	/**
	 * La lista de usuarios de la red
	 */
	private ArrayList<NetUser> netUserList;

	/**
	 * Los datos de configuración de la red
	 */
	private NetData netData;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="netUserList">La lista de usuarios de la red</param>
	/// <param name="_netData">Los datos de configuración de red</param>
	
	/**
	 * Constructor Parametrizado
	 * @param netUserList La lista de usuarios de la red
	 * @param netData Los datos de configuración de red
	 * @throws ArgumentOutOfRangeException
	 */
	public Dijkstra(ArrayList<NetUser> netUserList, NetData netData) throws ArgumentOutOfRangeException
	{
		if (netUserList.size() < 3) throw new ArgumentOutOfRangeException("totalNodeCount: "+netUserList.size()+". Expected a minimum of 3.");
		this.netUserList = netUserList;
		this.hint = this;
		this.traversalCost = this;
		TotalNodeCount = this.netUserList.size();
		this.netData = netData;
	}


	/// <summary>
	/// Obtiene el costo de un camino entre dos usuarios
	/// </summary>
	/// <param name="aNodeIndex">El indice del nodo del primer usuario</param>
	/// <param name="bNodeIndex">El indice del nodo del segundo usuario</param>
	/// <returns></returns>
	private int getCostNetUser(int aNodeIndex, int bNodeIndex)
	{
		for (int i = 0; i < netUserList.get(aNodeIndex).getNeighborhoodIds().length; i++)
		{
			if (netUserList.get(aNodeIndex).getNeighborhoodIds()[i].equals(netUserList.get(bNodeIndex).getId()))
			{
				int cost = netData.getStatePathNN();
				switch (netUserList.get(bNodeIndex).getState())
				{
				case CommunicationQuality.NORMAL:
				{
					switch (netUserList.get(bNodeIndex).getSignalQuality())
					{
					case NetUserQuality.NORMAL:
					{
						cost = netData.getStatePathNN();
						break;
					}
					case NetUserQuality.LOW:
					{
						cost = netData.getStatePathNL();
						break;
					}
					case NetUserQuality.CRITICAL:
					{
						cost = netData.getStatePathNC();
						break;
					}
					}
					break;
				}
				case CommunicationQuality.OVERLOADED:
				{
					switch (netUserList.get(bNodeIndex).getSignalQuality())
					{
					case NetUserQuality.NORMAL:
					{
						cost = netData.getStatePathON();
						break;
					}
					case NetUserQuality.LOW:
					{
						cost = netData.getStatePathOL();
						break;
					}
					case NetUserQuality.CRITICAL:
					{
						cost = netData.getStatePathOC();
						break;
					}
					}
					break;
				}
				case CommunicationQuality.CRITICAL:
				{
					switch (netUserList.get(bNodeIndex).getSignalQuality())
					{
					case NetUserQuality.NORMAL:
					{
						cost = netData.getStatePathCN();
						break;
					}
					case NetUserQuality.LOW:
					{
						cost = netData.getStatePathCL();
						break;
					}
					case NetUserQuality.CRITICAL:
					{
						cost = netData.getStatePathCC();
						break;
					}
					}
					break;
				}
				}
				return cost;
			}
		}

		return netData.getStatePathNotFound();
	}


	/// <summary>
	/// Obtiene una lista con los nodos directamente conectados a un usuario
	/// </summary>
	/// <param name="nodeIndex">El índice del nodo a consutar</param>
	/// <returns>Una lista con los nodos conectados</returns>
	private AbstractCollection<Integer> getConnectedNodesNetUser(int nodeIndex)
	{
		ArrayList<Integer> nodeIndexes = new ArrayList<Integer>();
		for (int i = 0; i < netUserList.get(nodeIndex).getNeighborhoodIds().length; i++)
		{
			for (int j = 0; j < netUserList.size(); j++)
			{
				if (netUserList.get(j).getId().equals(netUserList.get(nodeIndex).getNeighborhoodIds()[i]))
				{
					nodeIndexes.add(j);
					break;
				}
			}
		}
		return nodeIndexes;
	}

	/// <summary>
	/// TotalNodeCount
	/// </summary>
	protected /*readonly */final int TotalNodeCount;


	/// <summary> 
	/// Performs the Dijkstra algorithm on the data provided when the  
	/// <see cref="Dijkstra"/> object was instantiated. 
	/// </summary> 
	/// <param name="start"> 
	/// The nodeIndex to use as a connecting location. 
	/// </param> 
	/// <returns> 
	/// A struct containing both the minimum distance and minimum path 
	/// to every nodeIndex from the given <paramref name="startNetworkingHandler"/> nodeIndex. 
	/// </returns> 
	public Results Perform(int start)
	{
		// Initialize the distance to every nodeIndex from the connecting nodeIndex. 
		int[] d = GetStartingTraversalCost(start);
		// Initialize best path to every nodeIndex as from the connecting nodeIndex. 
		int[] p = GetStartingBestPath(start);
		Collection<Integer> c = GetChoices();

		c.remove(start); // take connecting nodeIndex out of the list of choices 

		//Debug.WriteLine("Step v C D P"); 
		//Debug.WriteLine(string.Format("init - {{{0}}} [{1}] [{2}]", 
		// ArrayToString<int>(",", c), ArrayToString<int>(",", d), ArrayToString<int>(",", p))); 
		//int step = 0; 

		// begin greedy loop 
		while (c.size() > 1)
		{
			// Find element v in c, that minimizes d[v] 
			int v = FindMinimizingDinC(d, c);
			c.remove(v); // remove v from the list of future solutions 
			// Consider all unselected nodes and consider their cost from v.

			Collection<Integer> temp = (this.hint != null ? hint.hint(v) : c); 

			for(Integer w : temp)
			{
				if (!c.contains(w)) continue; // discard pixels not in c 
				// At this point, relative(Index) points to a candidate pixel,  
				// that has not yet been selected, and lies within our area of interest. 
				// Consider whether it is now within closer reach. 
				int cost = traversalCost.traversalCost(v, w);
				if (cost < Integer.MAX_VALUE && d[v] + cost < d[w]) // don't let wrap-around negatives slip by 
				{
					// We have found a better way to get at relative 
					d[w] = d[v] + cost; // record new distance 
					// Record how we came to this new pixel 
					p[w] = v;
				}
			}
			//Debug.WriteLine(string.Format("{4} {3} {{{0}}} [{1}] [{2}]", 
			// ArrayToString<int>(",", c), ArrayToString<int>(",", d), ArrayToString<int>(",", p), v + 1, ++step)); 
		}

		return new Results(p, d);
	}


	/// <summary> 
	/// Uses the Dijkstra algorithhm to find the minimum path 
	/// from one nodeIndex to another. 
	/// </summary> 
	/// <param name="start"> 
	/// The nodeIndex to use as a connecting location. 
	/// </param> 
	/// <param name="finish"> 
	/// The nodeIndex to use as a finishing location. 
	/// </param> 
	/// <returns> 
	/// A struct containing both the minimum distance and minimum path 
	/// to every nodeIndex from the given <paramref name="start"/> nodeIndex. 
	/// </returns> 
	public int[] GetMinimumPath(int start, int finish)
	{
		Results results = Perform(start);
		return GetMinimumPath(start, finish, results.MinimumPath);
	}

	/// <summary> 
	/// Finds an array of nodes that provide the shortest path 
	/// from one given nodeIndex to another. 
	/// </summary> 
	/// <param name="start"> 
	/// The connecting nodeIndex. 
	/// </param> 
	/// <param name="finish"> 
	/// The finishing nodeIndex. 
	/// </param> 
	/// <param name="shortestPath"> 
	/// The P array of the completed algorithm. 
	/// </param> 
	/// <returns> 
	/// The list of nodes that provide the one step at a time path  
	/// from <paramref name="startNetworkingHandler"/> to <paramref name="finish"/> nodes. 
	/// </returns> 
	protected int[] GetMinimumPath(int start, int finish, int[] shortestPath)
	{
		Stack<Integer> path = new Stack<Integer>();
		do
		{
			path.push(finish);
			finish = shortestPath[finish]; // step back one step toward the startNetworkingHandler point 
		}
		while (finish != start);
		
		Integer[] aux= path.toArray(new Integer[path.size()]);
		int[] aux2 = new int[aux.length];
		for(int i=0; i< aux.length; i++)
			aux2[i] = aux[i].intValue();
		return aux2;
	}


	/// <summary> 
	/// Initializes the P array for the algorithm. 
	/// </summary> 
	/// <param name="startingNode"> 
	/// The nodeIndex that has been designated the connecting nodeIndex for the entire algorithm. 
	/// </param> 
	/// <returns> 
	/// The new P array. 
	/// </returns> 
	/// <remarks> 
	/// A fresh P array will set every single nodeIndex's source nodeIndex to be  
	/// the connecting nodeIndex, including the connecting nodeIndex itself. 
	/// </remarks> 
	protected int[] GetStartingBestPath(int startingNode)
	{
		int[] p = new int[TotalNodeCount];
		for (int i = 0; i < p.length; i++)
			p[i] = startingNode;
		return p;
	}

	/// <summary> 
	/// Finds the yet-unconsidered nodeIndex that has the least cost to reach. 
	/// </summary> 
	/// <param name="d"> 
	/// The cost of reaching any nodeIndex. 
	/// </param> 
	/// <param name="c"> 
	/// The nodes that are still available for picking. 
	/// </param> 
	/// <returns> 
	/// The nodeIndex that is closest (has the shortest special path). 
	/// </returns> 
	protected int FindMinimizingDinC(int[] d, Collection<Integer> c)
	{
		int bestIndex = -1;
		for(int ci: c)
			if (bestIndex == -1 || d[ci] < d[bestIndex])
				bestIndex = ci;
		return bestIndex;
	}

	/// <summary> 
	/// Initializes an Collection of all nodes not yet considered. 
	/// </summary> 
	/// <returns> 
	/// The initialized Collection. 
	/// </returns> 
	protected Collection<Integer> GetChoices()
	{
		Collection<Integer> choices = new ArrayList<Integer>(TotalNodeCount);
		for (int i = 0; i < TotalNodeCount; i++)
			choices.add(i);
		return choices;
	}

	/// <summary> 
	/// Initializes the D array for the startNetworkingHandler of the algorithm. 
	/// </summary> 
	/// <param name="start"> 
	/// The connecting nodeIndex. 
	/// </param> 
	/// <returns> 
	/// The contents of the new D array. 
	/// </returns> 
	/// <remarks> 
	/// The traversal cost for every nodeIndex will be set to impossible 
	/// (int.MaxValue) unless a connecting edge is found between the 
	/// <paramref name="startNetworkingHandler"/>ing nodeIndex and the nodeIndex in question. 
	/// </remarks> 
	protected int[] GetStartingTraversalCost(int start)
	{
		int[] subset = new int[TotalNodeCount];
		for (int i = 0; i < subset.length; i++)
			subset[i] = Integer.MAX_VALUE; // all are unreachable 
		subset[start] = 0; // zero cost from startNetworkingHandler to startNetworkingHandler 
		for(int nearby : hint.hint(start))
			subset[nearby] = traversalCost.traversalCost(start, nearby);
		return subset;
	}


	public int traversalCost(int start, int finish) {
		return getCostNetUser(start,finish);
	}


	public AbstractCollection<Integer> hint(int startingNode) {
		return getConnectedNodesNetUser(startingNode);
	}

	/// <summary> 
	/// Joins the elements of an array into a string, using 
	/// a given separator. 
	/// </summary> 
	/// <typeparam name="T">The type of element in the array.</typeparam> 
	/// <param name="separator">The seperator to insert between each element.</param> 
	/// <param name="array">The array.</param> 
	/// <returns>The resulting string.</returns> 
	/**protected string ArrayToString<T>(string separator, IEnumerable<int> array)
    {
        StringBuilder sb = new StringBuilder();
        foreach (int t in array)
            sb.AppendFormat("{0}{1}", t < int.MaxValue ? t + 1 : t, separator);
        sb.Length -= separator.Length;
        return sb.ToString();
    }*/
}
