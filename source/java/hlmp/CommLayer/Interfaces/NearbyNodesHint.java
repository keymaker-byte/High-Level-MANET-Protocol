package hlmp.CommLayer.Interfaces;

import java.util.AbstractCollection;

public interface NearbyNodesHint {

	/**
	 * An optional delegate that can help optimize the algorithm
	 * by showing it a subset of nodes to consider. Very useful 
	 * for limited connectivity graphs. (like pixels on a screen!) 
	 * @param startingNode The nodeIndex that is being traveled away FROM. 
	 * @return An array of nodes that might be reached from the  {@code startingNode}.
	 */
	public AbstractCollection<Integer> hint(int startingNode);
}
