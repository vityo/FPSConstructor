using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrAi
{
    public class CBreadthFirstPathFinder : IPathFinder 
    {

		public CBreadthFirstPathFinder() {}
        public override bool findPath(IWaypoint startNode, IWaypoint goalNode, List<IWaypoint> path)
        {
	        if (startNode == null || goalNode == null)
                return false;
  
	        List<SSearchNode> visited = new List<SSearchNode>();
	        List<SSearchNode> queue = new List<SSearchNode>();
	        bool found = false;
  
	        queue.Add(new SSearchNode(null, startNode));

            while (queue.Count != 0)
            {
                SSearchNode sNode = queue[0];
                queue.RemoveAt(0);
                visited.Add(sNode);

                if (sNode.Waypoint.equals(goalNode))
                {
                    found = getPath(sNode, ref path);

                    break;
                }

                foreach (SNeighbour iter in sNode.Waypoint.getNeighbours())
                {
                    if (!base.contains(visited, iter.Waypoint) && !base.contains(queue, iter.Waypoint))
                        queue.Add(new SSearchNode(sNode, iter.Waypoint));
                }
            }

	        deleteSearchNodes(visited);
	        deleteSearchNodes(queue);
          
	        return found; 
        }
    }
}
