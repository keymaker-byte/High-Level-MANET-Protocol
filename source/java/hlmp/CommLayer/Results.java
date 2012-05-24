package hlmp.CommLayer;

/** 
 * The composite product of a Dijkstra algorithm. 
 */
public class Results {

    /**
     * Prepares a Dijkstra results package.
     * @param minimumPath The minimum path array, where each array element index corresponds
     * 		to a nodeIndex designation, and the array element value is a pointer to
     * 		the nodeIndex that should be used to travel to this one.
     * @param minimumDistance The minimum distance from the connecting nodeIndex to the given nodeIndex. 
     */
    public Results(int[] minimumPath, int[] minimumDistance)
    {
        this.MinimumDistance = minimumDistance;
        this.MinimumPath = minimumPath;
    }

    /** 
     * The minimum path array, where each array element index corresponds  
     * to a nodeIndex designation, and the array element value is a pointer to 
     * the nodeIndex that should be used to travel to this one. 
     */ 
    public final int[] MinimumPath;
    /** 
     * The minimum distance from the connecting nodeIndex to the given nodeIndex. 
     */ 
    public final int[] MinimumDistance;
}
