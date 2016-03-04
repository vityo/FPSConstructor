using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;
using IrrlichtLime.Core;

using IrrGame.IrrAi.Interface;
using IrrGame;

namespace IrrGame.IrrFPS
{
    class CChasingCharacter : CNPCharacter 
    {
        public static uint SHOT_DELAY_TIME = 80;

        private bool Chasing, EnemyVisible;
        private uint TimeSinceLastShot;
        private List<CProjectile> Projectiles;
        private MetaTriangleSelector MetaSelector;

        public void StopAnimation()
        {
            foreach (CProjectile proj in Projectiles)
            {
                proj.GetNode().RemoveAnimators();
            }

            CharacterNode.AnimationSpeed = 0;
        }

        static void stateChangedCallback(E_NPC_STATE_TYPE state, object userData)
        {
            CChasingCharacter character = (CChasingCharacter)userData;
            
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

        public static void eventCallback(E_NPC_EVENT_TYPE evnt, object userData, object eventData) 
        {
	        CChasingCharacter character = (CChasingCharacter)userData;
	        
            if (character == null)
                return;
  
	        switch (evnt) 
            {
		        case E_NPC_EVENT_TYPE.ENET_AT_GOAL:
			        character.makeNewPlan();
			        break;  

		        case E_NPC_EVENT_TYPE.ENET_ENEMY_VISIBLE: 
			        character.chase((IAIEntity)eventData);
			        break;

		        case E_NPC_EVENT_TYPE.ENET_ENEMY_IN_RANGE: 
			        character.attack((IAIEntity)eventData);
			        break;

		        case E_NPC_EVENT_TYPE.ENET_ENEMY_POSITION_KNOWN:
			        character.goTo((Vector3Df)eventData);
			        break;
	        }
      
        }

        public CChasingCharacter(SCharacterDesc desc, string meshPath, string texturePath, MetaTriangleSelector selector):
            base(desc, E_CHARACTER_TYPE.ECT_CHASING, meshPath, texturePath) 
        {
            Projectiles = new List<CProjectile>();

            Chasing = false;
            EnemyVisible = false;
            TimeSinceLastShot = SHOT_DELAY_TIME + 1;
            MetaSelector = selector;

            if (AIEntity != null)
            {
                ((INPC)AIEntity).setStayPut(false);
                ((INPC)AIEntity).setStateChangedCallback(stateChangedCallback);
                ((INPC)AIEntity).setEventCallback(eventCallback);
            }
        }

        ~CChasingCharacter()
        {
            //garbage collector
	        Projectiles.Clear();
        }

        public void chase(IAIEntity chasee)
        {

            if (chasee == null || Chasing) 
                return;

            ((INPC)AIEntity).setDestination(AIManager.getNearestWaypoint(((INPC)AIEntity).getWaypointGroup(), chasee.getAbsolutePosition()));
            Chasing = true;
            EnemyVisible = true;
            //EnemyInteractText.Visible = true;
        }

        public void attack(IAIEntity attackee)
        {
            if (attackee == null)
                return;

            EnemyVisible = true;
           // EnemyInteractText.Visible = true;

            ((INPC)AIEntity).setStayPut(true);

            if (TimeSinceLastShot >= SHOT_DELAY_TIME && Ammo > 0)
            {
                CProjectile proj = new CProjectile(getAbsolutePosition(), attackee.getAbsolutePosition() , sceneManager);

                if (proj != null)
                    Projectiles.Add(proj);

                TimeSinceLastShot = 0;
                Ammo--;
                TimeSinceLastRefill = 0;
            }
        }

        public void makeNewPlan()
        {
            Chasing = false;
            //EnemyInteractText.Visible = false;

            SWaypointGroup waypointGroup = ((INPC)AIEntity).getWaypointGroup();
            ((INPC)AIEntity).setDestination((new System.Random()).Next() % waypointGroup.Waypoints.Count);   
        }

        public void goTo(Vector3Df pos)
        {
            if (Chasing || pos == null)
                return;

            IWaypoint wypt = AIManager.getNearestWaypoint(((INPC)AIEntity).getWaypointGroup(), pos);
            ((INPC)AIEntity).setDestination(wypt);
            Chasing = true;
        }

        public override bool update(uint elapsedTime)
        {
            if (base.update(elapsedTime)) 
                return true;
 
	        for (int p = 0 ; p < Projectiles.Count ; ++p) 
            {
		        if (Projectiles[p].update())
                {
                    Projectiles[p].remove();

			        Projectiles.RemoveAt(p);
			        p--;
		        }
                else
                {
			        CCharacter enemy = null;
			        Line3Df ray = new Line3Df(Projectiles[p].getPreviousPosition(), Projectiles[p].getPosition());
			        Vector3Df outVec;
			        Triangle3Df outTri;
			        SceneNode outNode;
			        

                    bool enHit = false;

                    List<SEntityGroup> EnemyGroups = AIEntity.getEnemyGroups();


                    foreach (SEntityGroup EnemyGroup in EnemyGroups)
                    {
                        List<IAIEntity> enemies = EnemyGroup.Entities;

                        for (int e = 0; e < enemies.Count; ++e)
                        {
                            enemy = (CCharacter)(enemies[e].getUserData());
                            
                            if (!enemy.getAIEntity().bIsLive)
                                continue;
                            //AABBox box = enemy.getAIEntity().getNode().BoundingBoxTransformed;

                            TriangleSelector trSel = sceneManager.CreateTriangleSelector(((MeshSceneNode)enemy.getAIEntity().getNode()).Mesh,
                                enemy.getAIEntity().getNode());


                            if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, trSel, out outVec, out outTri, out outNode))
                            {
                                enemy.registerHit();

                                Projectiles[p].remove();

                                Projectiles.RemoveAt(p);
                                p--;

                                enHit = true;

                                break;
                            }
                        }

                        if (enHit)
                            break;
                    }

			        if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, MetaSelector,out outVec, out outTri,out outNode) && !enHit) 
                    {
                        Projectiles[p].remove();

				        Projectiles.RemoveAt(p);
				        p--;
			        } 
		        }
	        }
  
	        TimeSinceLastShot += elapsedTime;
  
	        if (!EnemyVisible) 
                ((INPC)AIEntity).setStayPut(false);
  
	        EnemyVisible = false;

	        return false;
        }
    
		public bool isChasing() 
        {
            return Chasing; 
        }
    }
}
