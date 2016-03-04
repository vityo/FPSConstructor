using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrAi
{
    class SAStarSearchNode : SSearchNode 
    {
        public float CostG;
        public float CostH;
        public float CostF;

        public SAStarSearchNode (SSearchNode p, IWaypoint w) : base(p, w) 
        {
		    CostG = 0;
            CostH = 0;
            CostF = 0;
        }     
    }

    public class CAStarPathFinder : IPathFinder 
    {
		public CAStarPathFinder() {}

        public override bool findPath(IWaypoint startNode, IWaypoint goalNode, List<IWaypoint> path)
        {
            if (startNode == null || goalNode == null)
                return false;
    
	        List<SSearchNode> lstOpen = new List<SSearchNode>(), lstClosed = new List<SSearchNode>();
	        SAStarSearchNode sNode = null;

	        lstOpen.Add(new SAStarSearchNode(null, startNode));
  
	        bool pFound = false;

	        while ((lstOpen.Count > 0) && !pFound) 
            {
		        sNode = (SAStarSearchNode)lstOpen[0];

		        if (sNode.Waypoint == goalNode) 
                {
			        pFound = getPath(sNode, ref path);

			        break;
		        }

		        lstClosed.Insert(0, sNode);
		        lstOpen.RemoveAt(0);

                foreach (SNeighbour iter in sNode.Waypoint.getNeighbours())
                {
                    SAStarSearchNode newSNode = new SAStarSearchNode(sNode, iter.Waypoint);
                    newSNode.CostH = iter.Distance;
                    newSNode.CostG = sNode.CostG + iter.Distance;
                    newSNode.CostF = newSNode.CostG + newSNode.CostH;
            
                    int pInsert = 0, pExist = -1;
            
                    for (int r = 0; r < lstClosed.Count; ++r)
                    {
                        if (lstClosed[r].Waypoint == newSNode.Waypoint)
                        {
                            pInsert = -1;

                            break;
                        }
                    }

                    if (pInsert >= 0)
                    {
                        for (int r = 0; r < lstOpen.Count; ++r)
                        {
                            if (lstOpen[r].Waypoint == newSNode.Waypoint) 
                                pExist = r;
                    
                            if (((SAStarSearchNode)lstOpen[r]).CostF < newSNode.CostF)
                                pInsert = r + 1;
                        }

                        if (pExist >= 0)
                        {
                            if (((SAStarSearchNode)lstOpen[pExist]).CostF > newSNode.CostF)
                            {
                                lstOpen.RemoveAt(pExist);
                            }
                            else
                            {
                                pInsert = -1;
                            }
                        }

                        if (pInsert >= 0)
                        {
                            if (newSNode.Waypoint == goalNode)
                            {
                                pFound = getPath(newSNode,ref path);
                                lstClosed.Insert(0,newSNode);

                                break;
                            }
                            else
                            {
                                if (pInsert >= lstOpen.Count)
                                {
                                    lstOpen.Add(newSNode);
                                }
                                else
                                {
                                    lstOpen.Insert(pInsert, newSNode);
                                }
                            }
                        }
                    }
                }
	        }
  
	        base.deleteSearchNodes(lstOpen);
	        base.deleteSearchNodes(lstClosed);
          
	        return pFound; 
        }
    }
}
