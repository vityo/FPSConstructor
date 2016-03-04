using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;






using IrrGame;






namespace IrrGame.IrrAi
{
    public class CCombatNPC: ICombatNPC
    {
        public static uint TimeJump;
        public static uint TimeCrouching;
        public static uint TimeFallToDeath;
        public static float JumpUpFactor;
        public static float JumpAroundFactor;
        private uint TimeSinceFallToDeath;
        private uint TimeSinceCrouching;
        private uint timeSinceMovingAround;
        private Vector3Df dirAround;
        private float up;

//        private IAIManager.OcclusionQueryCallbackPtr oqcp;
        public static void SetCombatNPCSettings(uint aTimeJump, uint aTimeCrouching, uint aTimeFallToDeath,
            float aJumpUpFactor, float aJumpAroundFactor)
        {
            TimeJump = aTimeJump;
            TimeCrouching = aTimeCrouching;
            TimeFallToDeath = aTimeFallToDeath;
            JumpUpFactor = aJumpUpFactor;
            JumpAroundFactor = aJumpAroundFactor;
        }

        public CCombatNPC(SCombatNPCDesc desc, IAIManager aimgr, SceneManager smgr, int id)//, IAIManager.OcclusionQueryCallbackPtr oq)//, AnimatedMeshSceneNode characterNodeAnimate) 
            : base(desc, aimgr, smgr, id)
        {
  //          oqcp = oq;
            //CharacterNodeAnimate = characterNodeAnimate;

            PathDestinationWaypoint = null;
	        FieldOfView = null;
	        DebugFOV = null;
	        CurrentWaypoint = null;
	        DestinationWaypoint = null;

	        // Try to get a pointer to the desired starting waypoint
	        if (WaypointGroup!=null && WaypointGroup.Waypoints.Count > 0)
            {
                    bool next = false;

		            if (desc.StartWaypointID == -1)
                        CurrentWaypoint = aimgr.getWaypointFromId(WaypointGroup, (new System.Random()).Next()%WaypointGroup.Waypoints.Count);
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
  
	        FieldOfView = AIManager.createConeFieldOfView(desc.FovDimensions, desc.FovOcclusionCheck);
	        if (FieldOfView!=null && Node!=null)
            {      
		        FieldOfView.setPosition(Node.AbsolutePosition);
		        FieldOfView.setRotation(Node.Rotation + (new Vector3Df(0,0,0)));
	        }
            else
                Console.WriteLine("Failed FOV creation");
  
	        DebugFOV = AIManager.createDebugConeFOVSceneNode(desc.FovDimensions);
	
            if (DebugFOV!=null && Node!=null)
            {
		        DebugFOV.Position = Node.AbsolutePosition;
		        DebugFOV.Rotation = Node.Rotation +  (new Vector3Df(0,0,0));
	        }
            else
                Console.WriteLine("Failed D-FOV-SN creation");
  
		        PathFinder = AIManager.createAStarPathFinder();
		        //PathFinder = AIManager->createBreadthFirstPathFinder();
	        if (PathFinder==null)
                Console.WriteLine("Failed PathFinder creation");

	        setStayPut(true,false);
        }

        ~CCombatNPC() 
        {
            if (FieldOfView != null)
            {
                AIManager.removeFieldOfView(FieldOfView);
                FieldOfView = null;
            }

            if (DebugFOV != null)
            {
                AIManager.removeDebugFOVScenNode(DebugFOV);
                DebugFOV = null;
            }

            if (PathFinder != null)
            {
                AIManager.removePathFinder(PathFinder);
                PathFinder = null;
            }
        }

        public override void setDestination(IWaypoint dest)
        {
            if (dest==null)
                return;
    
	        StayPut = false;
  
  
	        if (PathFinder.findPath(CurrentWaypoint, dest, PathToDestination))
            {
		        PathDestinationWaypoint = dest;
    
		
  
        		changeState(E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH);
	        }
//             else
//                 Console.WriteLine("NPC {0} FAILED TO FIND PATH\n", Node.ID);
        }

        public override void setDestination(int destIdx)
        {
            setDestination(AIManager.getWaypointFromId(WaypointGroup, destIdx));
        }

        public override void update(uint elapsedTime)
        {

            base.update(elapsedTime);

            
            if (FieldOfView != null)
            {
                FieldOfView.setPosition(Node.AbsolutePosition);
                FieldOfView.setRotation(Node.Rotation + (new Vector3Df(0, 0, 0)));
            }
  
	        if (DebugFOV!=null)
            {
		        DebugFOV.UpdateAbsolutePosition();
		        DebugFOV.Position = Node.AbsolutePosition;
		        DebugFOV.Rotation = Node.Rotation + (new Vector3Df(0,0,0));
	        }


            if (State != E_NPC_STATE_TYPE.ENST_JUMP && State != E_NPC_STATE_TYPE.ENST_DIE && 
                State != E_NPC_STATE_TYPE.ENST_DEATH)
            //&& State != E_NPC_STATE_TYPE.ENST_CROUCH)
	            checkFieldOfVision();

//             if (CharacterNodeAnimate.AnimatorList.Count>0)
//             {
//                 if (((CollisionResponseSceneNodeAnimator)(CharacterNodeAnimate.AnimatorList[0])).Falling)
//                     return;
//             }




	        switch (State) 
            {
                case E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT:
                case E_NPC_STATE_TYPE.ENST_STAND_ATTACK:
//                     if(State == E_NPC_STATE_TYPE.ENST_ATTACK)
//                         rotateToFace(DestinationWaypoint.getPosition() + NodeOffset);  
                    //changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);

                    if (StayPut)
 				        return;
 			        else
                    {
				        if (PathToDestination.Count == 0 && CurrentWaypoint == PathDestinationWaypoint)
                        {
					        sendEvent(E_NPC_EVENT_TYPE.ENET_AT_GOAL, null);
					        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
				        } 
                        else 
					        changeState(E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH);
			        }
			        break;

                case E_NPC_STATE_TYPE.ENST_CROUCH_ATTACK:
                case E_NPC_STATE_TYPE.ENST_CROUCH:
                    if (TimeSinceCrouching <= TimeCrouching)
                    {
                        TimeSinceCrouching += elapsedTime;
                    }
                    else
                    {
                        TimeSinceCrouching = 0;
                        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
                    }
                    break;

                case E_NPC_STATE_TYPE.ENST_DIE:
                    if (TimeSinceFallToDeath <= TimeFallToDeath)
                    {
                        TimeSinceFallToDeath += elapsedTime;
                    }
                    else
                    {
                        TimeSinceFallToDeath = 0;
                        changeState(E_NPC_STATE_TYPE.ENST_DEATH);
                    }
                    break;  

		        case E_NPC_STATE_TYPE.ENST_MOVING: 
            
			        if (DestinationWaypoint!=null && 
                        !(Node.AbsolutePosition.GetDistanceFrom(DestinationWaypoint.getPosition() + NodeOffset) <= AtDestinationThreshold))
                    {
				        Vector3Df dir = ((DestinationWaypoint.getPosition() + NodeOffset) - Node.AbsolutePosition).Normalize();
				
                        float factor = elapsedTime * MoveSpeed;


                        Node.Position = Node.AbsolutePosition + dir * factor;// +(new Vector3Df(0, -1, 0));
			        }
                    else
				        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);

			        break;

                case E_NPC_STATE_TYPE.ENST_JUMP:

                    if (timeSinceMovingAround <= TimeJump)
                    {
                        System.Random r = new System.Random();



                        float factorAround = elapsedTime * JumpAroundFactor;

                        if (dirAround == null)
                        {
                            Line3Df ray = new Line3Df();

                            do 
                            {

                                dirAround = (new Vector3Df((float)r.NextDouble() - 0.5f, 0, (float)r.NextDouble()- 0.5f)).Normalize();
                                up = Node.Position.Y;
                                ray = new Line3Df(Node.AbsolutePosition, Node.AbsolutePosition + dirAround * (TimeJump * MoveSpeed + Node.Scale.X) * 2);
                            } while (AIManager.occlusionQuery(ray));
                            
                        }



                        Vector3Df v3dfUp = new Vector3Df(0, timeSinceMovingAround > TimeJump / 2 ? -1 : 1, 0);



                        Node.Position = Node.AbsolutePosition + dirAround * factorAround + v3dfUp * JumpUpFactor;// +(new Vector3Df(0, -1, 0));

                        timeSinceMovingAround += elapsedTime;
                    }
                    else
                    {
                        dirAround = null;
                        Node.Position = new Vector3Df(Node.AbsolutePosition.X,up,Node.AbsolutePosition.Z);
                        timeSinceMovingAround = 0;
                        changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
                    }
                        
                    break;
		        case E_NPC_STATE_TYPE.ENST_FOLLOWING_PATH: 
            
			        if (DestinationWaypoint!=null && !(Node.Position.GetDistanceFrom(DestinationWaypoint.getPosition() + NodeOffset) <= AtDestinationThreshold))
                    {
                        DestinationWaypoint = CurrentWaypoint;
			        }
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
       
			        if (DestinationWaypoint!=null)
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

        public override bool isVisibleToNPC(IAIEntity entity)
        {
            if (FieldOfView==null || entity==null)
                return false;

            Vector3Df entityPos = entity.getAbsolutePosition();
         
            return FieldOfView.isInFOV(entity.getNode().BoundingBoxTransformed, entityPos);

        }

        public override void setPosition(IWaypoint w)
        {
            CurrentWaypoint = w;

	        base.setPosition(w.getPosition());

            Vector3Df rotVec = new Vector3Df(0, -90, 0);
	        if (FieldOfView!=null) 
            {      
		        FieldOfView.setPosition(Node.AbsolutePosition);
		        FieldOfView.setRotation(Node.Rotation +rotVec);
	        }
  
	        if (DebugFOV!=null)
            {
		        DebugFOV.UpdateAbsolutePosition();
		        DebugFOV.Position = Node.AbsolutePosition;
		        DebugFOV.Rotation = Node.Rotation + rotVec;
	        }  
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
	        names.Add("fovDimensions");
	        Vector3Df dim = FieldOfView != null? FieldOfView.getDimensions() : new Vector3Df(0,0,0);
	        strw = dim.X.ToString();
	        strw += ",";
	        strw += dim.Y.ToString();
	        strw += ",";
	        strw += dim.Z.ToString();
	        values.Add(strw);
	        names.Add("range");
	        strw = Range.ToString();    
	        values.Add(strw);
	        names.Add("moveSpeed");
	        strw = MoveSpeed.ToString();
	        values.Add(strw);
	        names.Add("atDestinationThreshold");
	        strw = AtDestinationThreshold.ToString();
	        values.Add(strw);
	        names.Add("fovOcclusionCheck");
	        strw = FovOcclusionCheck.ToString();
	        values.Add(strw);
	        names.Add("checkFovForEnemies");
	        strw = CheckFovForEnemies ? "1" : "0";
	        values.Add(strw);
	        names.Add("checkFovForAllies");
	        strw = CheckFovForAllies ? "1" : "0";
	        values.Add(strw);
        }

        private void checkFieldOfVision()
        {
            bool go = true;

            foreach (SEntityGroup EnemyGroup in EnemyGroups)
            {

                if (EnemyGroup != null && CheckFovForEnemies)
                {
                    for (int e = 0; e < EnemyGroup.Entities.Count; ++e)
                    {
                        IAIEntity enemy = EnemyGroup.Entities[e];

                        if (!enemy.bIsLive)
                            continue;

                        
                        
//                         if(enemy.getID()==3 && this.ID == 1)
//                             Program.ray1 = new Line3Df(this.Node.AbsolutePosition,enemy.getNode().AbsolutePosition);




                        if (enemy != this && isVisibleToNPC(enemy))
                        {
                            go = false;

                            enemy.setVisibleToOtherEntity(true);
                            Vector3Df enemyPos = enemy.getAbsolutePosition();

                            foreach (SEntityGroup AllyGroup in AllyGroups)
                            {
                                if (AllyGroup != null)
                                {
                                    for (int a = 0; a < AllyGroup.Entities.Count; ++a)
                                    {
                                        if (AllyGroup.Entities[a] != this && AllyGroup.Entities[a].getType() == E_AIENTITY_TYPE.EAIET_COMBATNPC ||
                                            AllyGroup.Entities[a] != this && AllyGroup.Entities[a].getType() == E_AIENTITY_TYPE.EAIET_PATHFINDINGNPC)
                                            ((INPC)AllyGroup.Entities[a]).sendEventToNPC(E_NPC_EVENT_TYPE.ENET_ENEMY_POSITION_KNOWN, enemyPos);
                                    }
                                }
                            }

                            if (Node.AbsolutePosition.GetDistanceFrom(enemyPos) <= Range)
                                sendEvent(E_NPC_EVENT_TYPE.ENET_ENEMY_IN_RANGE, enemy);
                            else
                                sendEvent(E_NPC_EVENT_TYPE.ENET_ENEMY_VISIBLE, enemy);
                        }
                    }

                }
            }

            if (go && TimeSinceCrouching>0)
            {
                TimeSinceCrouching = 0;
                changeState(E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT);
            }

            foreach (SEntityGroup AllyGroup in AllyGroups)
            {
                if (AllyGroup != null && CheckFovForAllies)
                {
                    for (int a = 0; a < AllyGroup.Entities.Count; ++a)
                    {
                        IAIEntity ally = AllyGroup.Entities[a];

                        if (ally != this && isVisibleToNPC(ally))
                        {
                            ally.setVisibleToOtherEntity(true);

                            if (Node.AbsolutePosition.GetDistanceFrom(ally.getAbsolutePosition()) <= Range)
                                sendEvent(E_NPC_EVENT_TYPE.ENET_ALLY_IN_RANGE, ally);
                            else
                                sendEvent(E_NPC_EVENT_TYPE.ENET_ALLY_VISIBLE, ally);
                        }
                    }
                }
            }
        }
    }
}
