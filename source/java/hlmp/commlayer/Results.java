package hlmp.commlayer;

/// <summary> 
/// The composite product of a Dijkstra algorithm. 
/// </summary>
public class Results {

	/// <summary> 
    /// Prepares a Dijkstra results package. 
    /// </summary> 
    /// <param name="minimumPath"> 
    /// The minimum path array, where each array element index corresponds  
    /// to a nodeIndex designation, and the array element value is a pointer to 
    /// the nodeIndex that should be used to travel to this one. 
    /// </param> 
    /// <param name="minimumDistance"> 
    /// The minimum distance from the connecting nodeIndex to the given nodeIndex. 
    /// </param> 
    public Results(int[] minimumPath, int[] minimumDistance)
    {
        this.MinimumDistance = minimumDistance;
        this.MinimumPath = minimumPath;
    }

    /// <summary> 
    /// The minimum path array, where each array element index corresponds  
    /// to a nodeIndex designation, and the array element value is a pointer to 
    /// the nodeIndex that should be used to travel to this one. 
    /// </summary> 
    public final int[] MinimumPath;
    /// <summary> 
    /// The minimum distance from the connecting nodeIndex to the given nodeIndex. 
    /// </summary> 
    public final int[] MinimumDistance;
}
