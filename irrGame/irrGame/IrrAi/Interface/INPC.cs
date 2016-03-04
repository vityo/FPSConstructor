using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;

namespace IrrGame.IrrAi.Interface
{
    
    public enum E_NPC_STATE_TYPE 
    {
        ENST_STAND_ATTACK,
        ENST_CROUCH_ATTACK,
	    ENST_FIND_WAYPOINT,
        ENST_MOVING,
        ENST_JUMP,
        ENST_FOLLOWING_PATH,
        ENST_CROUCH,
        ENST_DIE,
        ENST_DEATH,
        NUM_STATE_TYPES   
    }
 
    public enum E_NPC_EVENT_TYPE 
    {
	    ENET_ENEMY_VISIBLE,
        ENET_ENEMY_IN_RANGE,
        ENET_ALLY_VISIBLE,
        ENET_ALLY_IN_RANGE,
        ENET_UNDER_ATTACK,
        ENET_ENEMY_POSITION_KNOWN,
        ENET_AT_GOAL,
        //ENET_MUST_MOVE_AROUND,
        ENET_DIE,
        NUM_EVENT_TYPES     
    }


    public class SNPCDesc
    {
        public string WaypointGroupName;
	    public int WaypointGroupIdx;
        public int StartWaypointID;    
        public float MoveSpeed;
        public float AtDestinationThreshold;
	    
        public SNPCDesc() : base() 
        {
		    WaypointGroupName = "";
            WaypointGroupIdx = 0;
            StartWaypointID = 0;
            MoveSpeed = 1;
            AtDestinationThreshold = 1;// 5.0f;
        }
	
        public SNPCDesc(string scale, string name, string offset, string position, string rotation, 
            string waypointGroupName, string startWaypointID, string moveSpeed, string atDestinationThreshold)
            : base(scale, name, offset, position, rotation) 
        {
			WaypointGroupName = waypointGroupName;
			StartWaypointID = int.Parse(startWaypointID);
			MoveSpeed = float.Parse(moveSpeed);
			AtDestinationThreshold = float.Parse(atDestinationThreshold);
			WaypointGroupIdx = 0;
		}
        
	    

    }



    public abstract class INPC : IAIEntity 
    {
		protected SWaypointGroup WaypointGroup;
		protected List<IWaypoint> PathToDestination;
		protected IWaypoint PathDestinationWaypoint;
		protected IWaypoint CurrentWaypoint;
		protected IWaypoint DestinationWaypoint;
		protected IPathFinder PathFinder;
		protected float MoveSpeed, AtDestinationThreshold;
		protected bool StayPut;
		protected E_NPC_STATE_TYPE State;

		public delegate void StateChangedCallbackPtr(E_NPC_STATE_TYPE eNST, object variable);
        public StateChangedCallbackPtr sccPtr;
		public delegate void EventCallbackPtr(E_NPC_EVENT_TYPE eNST, object variable1, object variable2);
        public EventCallbackPtr ecPtr;

		protected void changeState(E_NPC_STATE_TYPE state) 
        {
			State = state; 
			if (sccPtr!=null)
				sccPtr(State, UserData);  
		}

		protected void sendEvent(E_NPC_EVENT_TYPE evnt, object eventData) 
        {
			if (ecPtr!=null)
				ecPtr(evnt, UserData, eventData);   
		}

		public INPC(SNPCDesc desc, IAIManager aimgr, SceneManager smgr, E_AIENTITY_TYPE type, int id) : base(desc, aimgr, smgr, type, id) 
        {
            State = E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT;
            PathToDestination = new List<IWaypoint>();

			sccPtr = null;
			ecPtr = null;

			if (desc.WaypointGroupName.Length == 0)
                WaypointGroup = aimgr.getWaypointGroupFromIndex(desc.WaypointGroupIdx);
			else
                WaypointGroup = aimgr.getWaypointGroupFromName(desc.WaypointGroupName);

			MoveSpeed = desc.MoveSpeed;
			AtDestinationThreshold = desc.AtDestinationThreshold;
		}

		~INPC() {}

		public void setStayPut(bool sp, bool crouch) 
        {
			StayPut = sp;

  			if (StayPut)
                if(crouch)
  				    changeState(E_NPC_STATE_TYPE.ENST_CROUCH_ATTACK); 
                else
                    changeState(E_NPC_STATE_TYPE.ENST_STAND_ATTACK); 
		}
//         public void RotateToEntity(IAIEntity entity)
//         {
//             Node.Rotation = new Vector3Df(0,90,0);
// 
//         }

		public bool isStayingPut() 
        {
			return StayPut;
		}

		public SWaypointGroup getWaypointGroup() 
        {
			return WaypointGroup; 
		}

		public void setWaypointGroup(SWaypointGroup group) 
        {
			WaypointGroup = group;
		}

		public void setStateChangedCallback(StateChangedCallbackPtr cb) 
        {
			sccPtr = cb;
		}

		public void setEventCallback(EventCallbackPtr cb) 
        {
			ecPtr = cb;
		}

		public IWaypoint getDestinationWaypoint() 
        { 
			return PathDestinationWaypoint;
		}

		public IWaypoint getCurrentWaypoint() 
        {
			return CurrentWaypoint;
		}

		public void sendEventToNPC(E_NPC_EVENT_TYPE evnt, object eventData) 
        {
			switch (evnt) 
            {
				case E_NPC_EVENT_TYPE.ENET_UNDER_ATTACK:
                    sendEvent(evnt, null);
                    bool enemyVisible = (bool)eventData;
                    
                    if(!enemyVisible)
                        changeState(E_NPC_STATE_TYPE.ENST_JUMP);
                    else
                        if((new System.Random()).Next(2)==0)
                             changeState(E_NPC_STATE_TYPE.ENST_JUMP);
                        else
                            changeState(E_NPC_STATE_TYPE.ENST_CROUCH);
                   // sendEvent(evnt, null);

					break;
                case E_NPC_EVENT_TYPE.ENET_DIE:
                    sendEvent(evnt, null);
                    changeState(E_NPC_STATE_TYPE.ENST_DIE);
//                 case E_NPC_EVENT_TYPE.ENET_MUST_MOVE_AROUND:
//                     changeState(E_NPC_STATE_TYPE.ENST_JUMP);
// 
                     break;
				case E_NPC_EVENT_TYPE.ENET_ENEMY_POSITION_KNOWN:
					sendEvent(evnt, eventData);
					break;


				default:
					break;       
			}
		}

		public float getMoveSpeed() 
        {
            return MoveSpeed; 
        }

		public void setMoveSpeed(float speed) 
        {
            MoveSpeed = speed; 
        }

		public float getAtDestinationThreshold() 
        {
            return AtDestinationThreshold; 
        }

		public void setAtDestinationThreshold(float threshold) 
        {
            AtDestinationThreshold = threshold; 
        }

		public abstract void setDestination(IWaypoint dest);

		public abstract void setDestination(int destIdx);

        public override void update(uint elapsedTime)
        {
            base.update(elapsedTime);
        }

		public abstract void setPosition(IWaypoint w);

		public static bool contains(List<INPC> vec, INPC npc) 
        {
            foreach (INPC iter in vec)
            {
                if (npc == iter) 
                    return true;
            }

			return false; 
		}
    }
}
