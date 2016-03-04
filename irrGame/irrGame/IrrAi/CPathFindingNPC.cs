using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;

namespace IrrGame.IrrAi
{
    public class CPathFindingNPC : INPC
    {

		public CPathFindingNPC(SNPCDesc desc, IAIManager aimgr, SceneManager smgr, int id) :
            base(desc, aimgr, smgr, E_AIENTITY_TYPE.EAIET_PATHFINDINGNPC, id)
        {
	        sccPtr = null;
	        ecPtr = null;
	        PathDestinationWaypoint = null;
	        CurrentWaypoint = null;
	        DestinationWaypoint = null;

	        if (WaypointGroup!=null && WaypointGroup.Waypoints.Count > 0)
            {
		        if (desc.StartWaypointID == -1)
                    CurrentWaypoint = aimgr.getWaypointFromId(WaypointGroup,(new System.Random()).Next()%WaypointGroup.Waypoints.Count);
		        else
                    CurrentWaypoint = AIManager.getWaypointFromId(WaypointGroup, desc.StartWaypointID);
		
                if (CurrentWaypoint!=null)
                {
			        setPosition(CurrentWaypoint);
			        PathDestinationWaypoint = CurrentWaypoint;
		        }
                else
                    Console.WriteLine("Couldn't place NPC {0} at desired waypoint {1}", id, desc.StartWaypointID);
	        }
            else
                Console.WriteLine("Couldn't place NPC {0} in desired waypoint group {1}", id, desc.WaypointGroupName); 
    

		    PathFinder = AIManager.createAStarPathFinder();
    //		PathFinder = AIManager->createBreadthFirstPathFinder();
	
	        if (PathFinder==null)
                Console.WriteLine("Failed PathFinder creation");

	        setStayPut(true, false);
        }

        ~CPathFindingNPC()
        {
            if (PathFinder!=null)
            {
                AIManager.removePathFinder(PathFinder);
                PathFinder = null;
            }
        }

        public override void setDestination(IWaypoint dest)
        {
            if (dest == null)
                return;
    
	        StayPut = false;

            if (PathFinder.findPath(CurrentWaypoint, dest, PathToDestination))
            {

                PathDestinationWaypoint = dest;

                changeState(E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH);
            }
            else
                Console.WriteLine("NPC {0} FAILED TO FIND PATH\n", Node.ID);
        }

        public override void setDestination(int destIdx)
        {
            setDestination(AIManager.getWaypointFromId(WaypointGroup, destIdx));
        }

        public override void update(uint elapsedTime)
        {
           switch (State) 
           {
               case E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT: 
//     			    if (StayPut) 
//         			    return;
//                     else
//                     {
				        if (PathToDestination.Count == 0 && CurrentWaypoint == PathDestinationWaypoint) 
                        {
    					    sendEvent(E_NPC_EVENT_TYPE.ENET_AT_GOAL, null);
	    				    changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
				        }
                        else 
					        changeState(E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH);
			  //  }
			    break;    
		
		        case E_NPC_STATE_TYPE.ENST_MOVING: 
			        if (DestinationWaypoint!=null && 
                        !(Node.AbsolutePosition.GetDistanceFrom(DestinationWaypoint.getPosition() + NodeOffset) <= AtDestinationThreshold))
                    {
				        Vector3Df dir = ((DestinationWaypoint.getPosition() + NodeOffset) - Node.AbsolutePosition).Normalize();
				        float factor = elapsedTime * MoveSpeed;
				        Node.Position = Node.AbsolutePosition + dir * factor;
				
                        break; 
			        } 
                    else
				        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
			        break;    

		        case E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH: 
			        if (DestinationWaypoint != null && 
                        !(Node.Position.GetDistanceFrom(DestinationWaypoint.getPosition() + NodeOffset) <= AtDestinationThreshold))
				        DestinationWaypoint = CurrentWaypoint;
			        else
                    {
				        if (PathToDestination.Count == 0)
                        {
					        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);

					        break;
				        }
				
				        DestinationWaypoint = PathToDestination[PathToDestination.Count-1];
				
				        PathToDestination.RemoveAt(PathToDestination.Count-1); 
			        }
       
			        if (DestinationWaypoint != null)
                    {
				        rotateToFace(DestinationWaypoint.getPosition() + NodeOffset);     
				        CurrentWaypoint = DestinationWaypoint;
				        changeState(E_NPC_STATE_TYPE.ENST_MOVING);
			        }
                    else
                        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);        
      
			        break;    
	        }
        }

        public override void setPosition(IWaypoint w)
        {
            CurrentWaypoint = w;
	        base.setPosition(w.getPosition());
        }

        public void writeOutXMLDescription(ref List<string> names, ref List<string> values)
        {
            base.writeOutXMLDescription(ref names,ref values);

            string strw = "";
            names.Add("waypointGroupName");
            values.Add(WaypointGroup.getName());
            names.Add("startWaypointID");
            strw = CurrentWaypoint != null ? CurrentWaypoint.getID().ToString() : "";
            values.Add(strw);
            names.Add("moveSpeed");
            strw = MoveSpeed.ToString();
            values.Add(strw);
            names.Add("atDestinationThreshold");
            strw = AtDestinationThreshold.ToString();
            values.Add(strw);
        }
    }
}
