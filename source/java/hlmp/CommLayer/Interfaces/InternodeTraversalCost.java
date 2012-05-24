package hlmp.CommLayer.Interfaces;

public interface InternodeTraversalCost {

	/**
	 * Determines the cost of moving from a given nodeIndex to another given nodeIndex. 
	 * @param start The nodeIndex being moved away from.
	 * @param finish The nodeIndex that may be moved to. 
	 * @return The cost of the transition from {@code start}
	 * to {@code finish} or {@code Integer.MaxValue} if the transition is impossible
	 * (i.e. there is no edge between the two nodes). 
	 */
	public int traversalCost(int start, int finish);
}
