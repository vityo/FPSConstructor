using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;

namespace IrrGame.IrrAi.Interface
{

    public class SSearchNode 
    {
	
        public SSearchNode Parent;
        public IWaypoint Waypoint;

        public SSearchNode()
        {
            Parent = null;
            Waypoint = null;
        }
        public SSearchNode (SSearchNode p, IWaypoint w)
        {
		    Parent = p;
            Waypoint = w;
        } 
       
    }

    

    public abstract class IPathFinder 
    {		
		public IPathFinder() {}
		
		~IPathFinder() {}

        public virtual bool findPath(IWaypoint startNode, IWaypoint goalNode, List<IWaypoint> path)
        {
            return true;
        }
    

		protected virtual void deleteSearchNodes(List<SSearchNode> arr)
        {
            //garbage collector
        }
		
		protected virtual bool contains(List<SSearchNode> arr, IWaypoint node)
        {       
			for (int i = 0 ; i < arr.Count ; ++i)
				if (node == arr[i].Waypoint)
                    return true;

			return false; 
		}
		
		protected virtual  bool getPath(SSearchNode sNode, ref List<IWaypoint> path) 
        {
            
			path.Clear(); // clear out any previous paths, we want a fresh one now!
			
            SSearchNode pSNode = sNode;

			// Create path (backwards)
			while (pSNode != null)
            {
				path.Add(pSNode.Waypoint);
				pSNode = pSNode.Parent;
			}   

			return pSNode == null; // the current node should not have a parent, it should be the start node
		}
    
    }

}