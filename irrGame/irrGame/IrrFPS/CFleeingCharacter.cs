using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime.Core;
using IrrlichtLime.Scene;

namespace IrrGame.IrrFPS
{
    public class CFleeingCharacter : CNPCharacter 
    {
        private bool Fleeing;
		private bool Scanning;
	
        public static void stateChangedCallback(E_NPC_STATE_TYPE state, object userData) 
        {
  
	        CFleeingCharacter character = (CFleeingCharacter)userData;
	
            if (character == null)
                return;
  
	        switch (state)
            {  
		        case E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT: 
		        case E_NPC_STATE_TYPE.ENST_MOVING:
			        character.setAnimation(state);
			        break;
	        }
        }

        static void eventCallback(E_NPC_EVENT_TYPE evnt, object userData, object eventData) 
        {
	        CFleeingCharacter character = (CFleeingCharacter)userData;
	
            if (character == null)
                return;
  
	        switch (evnt)
            {
		        case E_NPC_EVENT_TYPE.ENET_AT_GOAL: 
			        character.makeNewPlan();
			        break;

                case E_NPC_EVENT_TYPE.ENET_ENEMY_VISIBLE:
                case E_NPC_EVENT_TYPE.ENET_ENEMY_IN_RANGE:
                case E_NPC_EVENT_TYPE.ENET_UNDER_ATTACK:
                case E_NPC_EVENT_TYPE.ENET_ENEMY_POSITION_KNOWN:
			        character.setFleeing();
			        break;
	        }
      
        }

		public CFleeingCharacter(SCharacterDesc desc, string meshPath, string texturePath):
            base(desc, E_CHARACTER_TYPE.ECT_FLEEING, meshPath, texturePath)
        {

            Fleeing = false;
            Scanning = false;

            ((INPC)AIEntity).setStateChangedCallback(stateChangedCallback);
            ((INPC)AIEntity).setEventCallback(eventCallback);

            makeNewPlan();
        }
        
        public void StopAnimation()
        {
//             if (AIEntity.bIsLive)
//             {
                CharacterNode.RemoveAnimators();

                CharacterNode.AnimationSpeed = 0;
//            }
        }

        public override bool update(uint elapsedTime)
        {
            if (base.update(elapsedTime)) 
                return true;

	        if (Scanning)
                AIEntity.getNode().Rotation = AIEntity.getNode().Rotation + (new Vector3Df(0,1,0));

	        return false;
        }

        public void setFleeing()
        {
            if (Fleeing) 
                return;

            SWaypointGroup wyptGroup = ((INPC)AIEntity).getWaypointGroup();

            ((INPC)AIEntity).setDestination((new System.Random()).Next() % wyptGroup.Waypoints.Count);
            Fleeing = true;
            //EnemyInteractText.Visible = true;
            Scanning = false;
        }

        public void makeNewPlan()
        {
            Fleeing = false;
            //EnemyInteractText.Visible = false;

            ((INPC)AIEntity).setStayPut(true);
            Scanning = true;
        }

        public override void setAnimation(E_NPC_STATE_TYPE state)
        {
            AnimationTypeMD2 animation;
  
	        switch (state) 
            {
		        case E_NPC_STATE_TYPE.ENST_MOVING:
			        if (Fleeing)
                        animation = AnimationTypeMD2.Crouch_Walk;
			        else 
                        animation = AnimationTypeMD2.Run;
			        break;

		        case E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT:
			        animation = AnimationTypeMD2.Stand;
			        break;
        
                default:
                    animation = (AnimationTypeMD2)(-1);
                    break;
	        }
  
	        //if (CurrAnimation != animation && bIsLive) 
            if (CurrAnimation != animation) 
            {
		        CharacterNode.SetMD2Animation(animation);
		        CurrAnimation = animation;
            }
        }
    
		public bool isFleeing()
        {
            return Fleeing; 
        }
    }
}
