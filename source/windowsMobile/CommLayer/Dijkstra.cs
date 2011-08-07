using System;
using System.Collections.Generic;
using System.Text;

namespace CommLayerCompact
{
    /// <summary> 
    /// Implements a generalized Dijkstra's algorithm to calculate 
    /// both minimum distance and minimum path. 
    /// </summary> 
    /// <remarks> 
    /// For this algorithm, all nodes should be provided, and handled 
    /// in the delegate methods, including the startNetworkingHandler and finish nodes. 
    /// </remarks> 
    internal class Dijkstra
    {
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
        public delegate IEnumerable<int> NearbyNodesHint(int startingNode);
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
        public delegate int InternodeTraversalCost(int start, int finish);

        /// <summary> 
        /// Creates an instance of the <see cref="Dijkstra"/> class. 
        /// </summary> 
        /// <param name="totalNodeCount"> 
        /// The total number of nodes in the graph. 
        /// </param> 
        /// <param name="traversalCost"> 
        /// The delegate that can provide the cost of a transition between 
        /// any two nodes. 
        /// </param> 
        /// <param name="hint"> 
        /// An optional delegate that can provide a small subset of nodes 
        /// that a given nodeIndex may be connected to. 
        /// </param> 
        public Dijkstra(int totalNodeCount, InternodeTraversalCost traversalCost, NearbyNodesHint hint)
        {
            if (totalNodeCount < 3) throw new ArgumentOutOfRangeException("totalNodeCount " + totalNodeCount + " Expected a minimum of 3.");
            if (traversalCost == null) throw new ArgumentNullException("traversalCost");
            Hint = hint;
            TraversalCost = traversalCost;
            TotalNodeCount = totalNodeCount;
        }



        //ESTO ES IMPLEMENTACION CAMBIADA
        private List<NetUser> netUserList;
        private NetLayerCompact.NetData netData;

        public Dijkstra(List<NetUser> netUserList, NetLayerCompact.NetData _netData)
        {
            if (netUserList.Count < 3) throw new ArgumentOutOfRangeException("totalNodeCount " + netUserList.Count + "Expected a minimum of 3.");
            this.netUserList = netUserList;
            Hint = getConnectedNodesNetuSer;
            TraversalCost = getCostNetUser;
            TotalNodeCount = this.netUserList.Count;
            netData = _netData;
        }

        private int getCostNetUser(int aNodeIndex, int bNodeIndex)
        {
            for (int i = 0; i < netUserList[aNodeIndex].NeighborhoodIds.Length; i++)
            {
                if (netUserList[aNodeIndex].NeighborhoodIds[i].Equals(netUserList[bNodeIndex].Id))
                {
                    int cost = netData.StatePathNN;
                    switch (netUserList[bNodeIndex].State)
                    {
                        case CommunicationQuality.NORMAL:
                            {
                                switch (netUserList[bNodeIndex].SignalQuality)
                                {
                                    case NetUserQuality.NORMAL:
                                        {
                                            cost = netData.StatePathNN;
                                            break;
                                        }
                                    case NetUserQuality.LOW:
                                        {
                                            cost = netData.StatePathNL;
                                            break;
                                        }
                                    case NetUserQuality.CRITICAL:
                                        {
                                            cost = netData.StatePathNC;
                                            break;
                                        }
                                }
                                break;
                            }
                        case CommunicationQuality.OVERLOADED:
                            {
                                switch (netUserList[bNodeIndex].SignalQuality)
                                {
                                    case NetUserQuality.NORMAL:
                                        {
                                            cost = netData.StatePathON;
                                            break;
                                        }
                                    case NetUserQuality.LOW:
                                        {
                                            cost = netData.StatePathOL;
                                            break;
                                        }
                                    case NetUserQuality.CRITICAL:
                                        {
                                            cost = netData.StatePathOC;
                                            break;
                                        }
                                }
                                break;
                            }
                        case CommunicationQuality.CRITICAL:
                            {
                                switch (netUserList[bNodeIndex].SignalQuality)
                                {
                                    case NetUserQuality.NORMAL:
                                        {
                                            cost = netData.StatePathCN;
                                            break;
                                        }
                                    case NetUserQuality.LOW:
                                        {
                                            cost = netData.StatePathCL;
                                            break;
                                        }
                                    case NetUserQuality.CRITICAL:
                                        {
                                            cost = netData.StatePathCC;
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    return cost;
                }
            }

            return netData.StatePathNotFound;
        }

        private IEnumerable<int> getConnectedNodesNetuSer(int nodeIndex)
        {
            List<int> nodeIndexes = new List<int>();
            for (int i = 0; i < netUserList[nodeIndex].NeighborhoodIds.Length; i++)
            {
                for (int j = 0; j < netUserList.Count; j++)
                {
                    if (netUserList[j].Id.Equals(netUserList[nodeIndex].NeighborhoodIds[i]))
                    {
                        nodeIndexes.Add(j);
                        break;
                    }
                }
            }
            return nodeIndexes;
        }


        protected readonly NearbyNodesHint Hint;
        protected readonly InternodeTraversalCost TraversalCost;
        protected readonly int TotalNodeCount;

        /// <summary> 
        /// The composite product of a Dijkstra algorithm. 
        /// </summary> 
        public struct Results
        {
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
                MinimumDistance = minimumDistance;
                MinimumPath = minimumPath;
            }

            /// <summary> 
            /// The minimum path array, where each array element index corresponds  
            /// to a nodeIndex designation, and the array element value is a pointer to 
            /// the nodeIndex that should be used to travel to this one. 
            /// </summary> 
            public readonly int[] MinimumPath;
            /// <summary> 
            /// The minimum distance from the connecting nodeIndex to the given nodeIndex. 
            /// </summary> 
            public readonly int[] MinimumDistance;
        }

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
        public virtual Results Perform(int start)
        {
            // Initialize the distance to every nodeIndex from the connecting nodeIndex. 
            int[] d = GetStartingTraversalCost(start);
            // Initialize best path to every nodeIndex as from the connecting nodeIndex. 
            int[] p = GetStartingBestPath(start);
            ICollection<int> c = GetChoices();

            c.Remove(start); // take connecting nodeIndex out of the list of choices 

            //Debug.WriteLine("Step v C D P"); 
            //Debug.WriteLine(string.Format("init - {{{0}}} [{1}] [{2}]", 
            // ArrayToString<int>(",", c), ArrayToString<int>(",", d), ArrayToString<int>(",", p))); 
            //int step = 0; 

            // begin greedy loop 
            while (c.Count > 1)
            {
                // Find element v in c, that minimizes d[v] 
                int v = FindMinimizingDinC(d, c);
                c.Remove(v); // remove v from the list of future solutions 
                // Consider all unselected nodes and consider their cost from v. 
                foreach (int w in (Hint != null ? Hint(v) : c))
                {
                    if (!c.Contains(w)) continue; // discard pixels not in c 
                    // At this point, relative(Index) points to a candidate pixel,  
                    // that has not yet been selected, and lies within our area of interest. 
                    // Consider whether it is now within closer reach. 
                    int cost = TraversalCost(v, w);
                    if (cost < int.MaxValue && d[v] + cost < d[w]) // don't let wrap-around negatives slip by 
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
        public virtual int[] GetMinimumPath(int start, int finish)
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
        protected virtual int[] GetMinimumPath(int start, int finish, int[] shortestPath)
        {
            Stack<int> path = new Stack<int>();
            do
            {
                path.Push(finish);
                finish = shortestPath[finish]; // step back one step toward the startNetworkingHandler point 
            }
            while (finish != start);
            return path.ToArray();
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
        protected virtual int[] GetStartingBestPath(int startingNode)
        {
            int[] p = new int[TotalNodeCount];
            for (int i = 0; i < p.Length; i++)
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
        protected virtual int FindMinimizingDinC(int[] d, ICollection<int> c)
        {
            int bestIndex = -1;
            foreach (int ci in c)
                if (bestIndex == -1 || d[ci] < d[bestIndex])
                    bestIndex = ci;
            return bestIndex;
        }

        /// <summary> 
        /// Initializes an collection of all nodes not yet considered. 
        /// </summary> 
        /// <returns> 
        /// The initialized collection. 
        /// </returns> 
        protected virtual ICollection<int> GetChoices()
        {
            ICollection<int> choices = new List<int>(TotalNodeCount);
            for (int i = 0; i < TotalNodeCount; i++)
                choices.Add(i);
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
        protected virtual int[] GetStartingTraversalCost(int start)
        {
            int[] subset = new int[TotalNodeCount];
            for (int i = 0; i < subset.Length; i++)
                subset[i] = int.MaxValue; // all are unreachable 
            subset[start] = 0; // zero cost from startNetworkingHandler to startNetworkingHandler 
            foreach (int nearby in Hint(start))
                subset[nearby] = TraversalCost(start, nearby);
            return subset;
        }
    } 
}
