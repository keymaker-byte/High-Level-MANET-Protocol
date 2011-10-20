package hlmp.CommLayer.Interfaces;

import java.util.AbstractCollection;

public interface IDijkstra {

	/// <summary> 
    /// Determines the cost of moving from a given nodeIndex to another given nodeIndex. 
    /// </summary> 
    /// <param name="start"> 
    /// The nodeIndex being moved away from. 
    /// </param> 
    /// <param name="finish"> 
    /// The nodeIndex that may be moved to. 
    /// </param> 
    /// <returns> 
    /// The cost of the transition from <paramref name="startNetworkingHandler"/> to 
    /// <paramref name="finish"/>, or <see cref="Int32.MaxValue"/> 
    /// if the transition is impossible (i.e. there is no edge between  
    /// the two nodes). 
    /// </returns> 
	public int InternodeTraversalCost(int start, int finish);
	
	/// <summary> 
    /// An optional delegate that can help optimize the algorithm 
    /// by showing it a subset of nodes to consider. Very useful 
    /// for limited connectivity graphs. (like pixels on a screen!) 
    /// </summary> 
    /// <param name="startingNode"> 
    /// The nodeIndex that is being traveled away FROM. 
    /// </param> 
    /// <returns> 
    /// An array of nodes that might be reached from the  
    /// <paramref name="startingNode"/>. 
    /// </returns> 
	public AbstractCollection<Integer> NearbyNodesHint(int startingNode);
}
