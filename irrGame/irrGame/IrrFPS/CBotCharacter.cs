using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

using IrrGame.IrrAi.Interface;
using IrrGame;

namespace IrrGame.IrrFPS
{
    class CBotCharacter : CNPCharacter 
    {
        

        private bool Chasing, EnemyVisibleUpdate, EnemyVisible;
        private uint TimeSinceLastShot;
        
        private MetaTriangleSelector MetaSelector;
        

        public void StopAnimation()
        {

            if (AIEntity.bIsLive)
            {



                CharacterNode.AnimationSpeed = 0;
                CharacterNode.RemoveAnimators();
            }

            foreach (CProjectile proj in Projectiles)
            {
                proj.GetNode().RemoveAnimators();
            }
        }

        static void stateChangedCallback(E_NPC_STATE_TYPE state, object userData)
        {
            CBotCharacter character = (CBotCharacter)userData;
            
            if (character == null) 
                return;

//              switch (state)
//              {
//                  case E_NPC_STATE_TYPE.ENST_FIND_WAYPOINT:
//                  case E_NPC_STATE_TYPE.ENST_MOVING:
//                  case E_NPC_STATE_TYPE.ENST_MOVING_AROUND:
                     character.setAnimation(state);
//                      break;
//              }

        }

        public static void eventCallback(E_NPC_EVENT_TYPE evnt, object userData, object eventData) 
        {
	        CBotCharacter character = (CBotCharacter)userData;
	        
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

//                 case E_NPC_EVENT_TYPE.ENET_UNDER_ATTACK:
// 
//                     character.jumpAround();
// 
//                     break;
	        }
      
        }
        
        public CBotCharacter(SCharacterDesc desc, string meshPath, string texturePath, MetaTriangleSelector selector,float Range,
            Vector3Df FieldOfViewDimensions, float MoveSpeed, float AtDestinationThreshold) :
            //base(desc, E_CHARACTER_TYPE.ECT_CHASING, meshPath, texturePath,selector) 
            base(desc, meshPath, texturePath, selector, Range, FieldOfViewDimensions, MoveSpeed, AtDestinationThreshold) 
        {

            Projectiles = new List<CProjectile>();

            Chasing = false;
            EnemyVisibleUpdate = false;
            TimeSinceLastShot = ShotDelayTime + 1;
            MetaSelector = selector;

            if (AIEntity != null)
            {
                ((INPC)AIEntity).setStayPut(false,false);
                ((INPC)AIEntity).setStateChangedCallback(stateChangedCallback);
                ((INPC)AIEntity).setEventCallback(eventCallback);
            }
        }

        ~CBotCharacter()
        {
            //garbage collector
	        Projectiles.Clear();
        }

        public virtual void setAnimation(E_NPC_STATE_TYPE state)
        {
            AnimationTypeMD2 animation = CurrAnimation;
            float animationSpeed = 0;
            bool shot = false;

            switch (state)
            {
                case E_NPC_STATE_TYPE.ENST_MOVING:
                    animation = AnimationTypeMD2.Run;
                    animationSpeed = AnimationSpeedRun;
                    break;
                case E_NPC_STATE_TYPE.ENST_STAND_ATTACK:
                case E_NPC_STATE_TYPE.ENST_CROUCH_ATTACK:
                    shot = true;
                    //animation = AnimationTypeMD2.Attack;
                    //if(CurrAnimation != AnimationTypeMD2.Jump)
                    if (CurrAnimation == AnimationTypeMD2.Crouch_Stand || CurrAnimation == AnimationTypeMD2.Crouch_Attack)
                    {
                        animation = AnimationTypeMD2.Crouch_Attack;
                        animationSpeed = AnimationSpeedCrouchAttack;
                    }
                    else
                    {
                        animation = AnimationTypeMD2.Attack;
                        animationSpeed = AnimationSpeedAttack;
                    }
                    //animation = AnimationTypeMD2.Jump;
                    break;
                case E_NPC_STATE_TYPE.ENST_JUMP:
                    if (CurrAnimation != AnimationTypeMD2.Crouch_Stand &&
                        CurrAnimation != AnimationTypeMD2.Crouch_Attack)
                    {
                        animation = AnimationTypeMD2.Jump;
                        animationSpeed = AnimationSpeedJump;
                    }
                    break;
                case E_NPC_STATE_TYPE.ENST_CROUCH:
                    if (CurrAnimation != AnimationTypeMD2.Jump)
                        if (EnemyVisible)
                        {
                            shot = true;
                            animation = AnimationTypeMD2.Crouch_Attack;
                            animationSpeed = AnimationSpeedCrouchAttack;
                        }
                        else
                        {
                            animation = AnimationTypeMD2.Crouch_Stand;
                            animationSpeed = AnimationSpeedCrouchStand; 
                        }
                    break;
                case E_NPC_STATE_TYPE.ENST_DIE:
                    if (CurrAnimation != AnimationTypeMD2.Death_Fall_Back &&
                        CurrAnimation != AnimationTypeMD2.Death_Fall_Forward)
                    {
                        if (EnemyVisible)
                            animation = AnimationTypeMD2.Death_Fall_Back;
                        else
                            animation = AnimationTypeMD2.Death_Fall_Forward;

                        animationSpeed = AnimationSpeedDeath;
                    }
                    break;
                case E_NPC_STATE_TYPE.ENST_DEATH:
                    animation = AnimationTypeMD2.Boom;
                    animationSpeed = AnimationSpeedBoom;
                    break;
                //                 case E_NPC_STATE_TYPE.ENST_MOVING_AROUND:
                //                     //if((new System.Random()).Next(2)==0)
                //                       //  animation = AnimationTypeMD2.Stand;
                //                     //else
                //                     animation = AnimationTypeMD2Attack;
                //                     break;

                default:
                    animation = (AnimationTypeMD2)(-1);
                    break;
            }


            //if (CurrAnimation != animation && bIsLive)
            if (CurrAnimation != animation || (shot && TimeSinceLastShot >= ShotDelayTime))
            {
                CharacterNode.SetMD2Animation(animation);
                CharacterNode.AnimationSpeed = animationSpeed;
                CurrAnimation = animation;
                
            }

        }
        public override void deleteNodes()
        {
            base.deleteNodes();
            
//             if (AIEntity.bIsLive)
//             {
                CharacterNode.Remove();
           // }
        }
//         public void jumpAround()
//         {
//             if (!jumpAnimator.Falling)
//             {
//                 
// 
// 
// 
// 
//                 //                  Vector3Df gravity = jumpAnimator.Gravity;
//                 //                  System.Random r = new System.Random();
//                 //                  int sizeJump = (int)Math.Abs(gravity.Y);
//                 //                  jumpAnimator.Gravity = (new Vector3Df(r.Next(-sizeJump, sizeJump),
//                 //                      sizeJump, r.Next(-sizeJump, sizeJump))).Normalize();
//                 
//                 jumpAnimator.Jump(20);
//                 
//                 //  jumpAnimator.Gravity = gravity;
//                 
//                 
//                 //System.Random r = new System.Random();
//                 //((INPC)AIEntity).setPosition(((INPC)AIEntity).getAbsolutePosition()+new Vector3Df(r.Next(-30,30),-30,r.Next(-30,30)));
// 
//                 /*System.Random r = new System.Random();
//                 jumpAnimator.Gravity = (new Vector3Df(r.Next(-100,100),
//                     r.Next(-100, 100), r.Next(-100, 100))).Normalize();
//                 jumpAnimator.Jump(200);
//                 
//                 jumpAnimator.Gravity = new Vector3Df(0,-100, 0);
//                 sceneManager.anim*/
//             }
//         }

        public void chase(IAIEntity chasee)
        {

            if (chasee == null || Chasing) 
                return;

            ((INPC)AIEntity).setDestination(AIManager.getNearestWaypoint(((INPC)AIEntity).getWaypointGroup(), chasee.getAbsolutePosition()));
            Chasing = true;
            EnemyVisibleUpdate = true;
            //EnemyInteractText.Visible = true;
        }

        public void attack(IAIEntity attackee)
        {
            bool crouch = CurrAnimation == AnimationTypeMD2.Crouch_Attack ? true: false;
            //EnemyVisibleUpdate = true;
            EnemyVisible = true;

            if (attackee == null)
                return;

            EnemyVisibleUpdate = true;
           // EnemyInteractText.Visible = true;

            
            
            
            
            
            
            ((INPC)AIEntity).setStayPut(true,crouch);







            //((INPC)AIEntity).RotateToEntity(attackee);

            if (TimeSinceLastShot >= ShotDelayTime && Ammo > 0)
            {
//                 Vector3Df projStartPos = projectileSettings.StartPositionBotStand;
//                 // new Vector3Df(0, 20, 0);
// 
//                 if(CurrAnimation == AnimationTypeMD2.Crouch_Attack)
//                     projStartPos = projectileSettings.StartPositionBotCrouch;
//                 //new Vector3Df(0, 0, 0);
                
                CProjectile proj = new CProjectile(getAbsolutePosition(), crouch,
                    true,attackee.getAbsolutePosition(), sceneManager,new Matrix(),false);
                //,projectileSettings);

                if (proj != null)
                    Projectiles.Add(proj);

                TimeSinceLastShot = 0;
                Ammo--;
                
                //TimeSinceLastRefill = 0;
            }
        }

        public void makeNewPlan()
        {
            /*EnemyVisibleUpdate = false;*/
            EnemyVisible = false;
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

//         protected void die()
//         {
//             base.die();
// 
//             foreach (CProjectile proj in Projectiles)
//             {
//                 proj.GetNode().RemoveAnimators();
//             }
// 
//         }

        public void AnalyzeProjectiles()
        {
            List<SEntityGroup> EnemyGroups = AIEntity.getEnemyGroups();
            CCharacter enemy = null;

            Vector3Df outVec;
            Triangle3Df outTri;
            SceneNode outNode;

            for (int p = 0; p < Projectiles.Count; ++p)
            {
                Line3Df ray = new Line3Df(Projectiles[p].getPreviousPosition(), Projectiles[p].getPosition());

                if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, MetaSelector, out outVec, out outTri, out outNode))
                {
                    //collision = true;

                    Projectiles[p].remove();

                    Projectiles.RemoveAt(p);
                    p--;
                }
            }


            foreach (SEntityGroup EnemyGroup in EnemyGroups)
            {
                List<IAIEntity> enemies = EnemyGroup.Entities;

                for (int i = 0; i < enemies.Count; ++i)
                {
                    bool enHit = false;

                    enemy = (CCharacter)(enemies[i].getUserData());

                    if (!enemy.getAIEntity().bIsLive)
                        continue;

                    //TriangleSelector trSel = sceneManager.CreateTriangleSelector(enemy.getAIEntity().CharacterNodeAnimate);

                                                 TriangleSelector trSel = sceneManager.CreateTriangleSelector(((MeshSceneNode)enemy.getAIEntity().getNode()).Mesh,
                                                   enemy.getAIEntity().getNode());

                    if(trSel == null)
                        trSel = sceneManager.CreateTriangleSelector(((MeshSceneNode)enemy.getAIEntity().getNode()).Mesh,
                                                    enemy.getAIEntity().getNode());
                    //enemy.getAIEntity().getNode().TriangleSelector = trSel;
                    for (int p = 0; p < Projectiles.Count; ++p)
                    {
                        Line3Df ray = new Line3Df(Projectiles[p].getPreviousPosition(), Projectiles[p].getPosition());

                        if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, trSel, out outVec, out outTri, out outNode))
                        {
                            // trSel.Drop();

                            enemy.registerHit();
                            //collision = true;

                            Projectiles[p].remove();

                            Projectiles.RemoveAt(p);
                            p--;

                            break;
                        }


                    }


                    trSel.Drop();
                }
            }

            for (int p = 0; p < Projectiles.Count; ++p)
            {
                if ((Projectiles[p]).update())
                {
                    Projectiles[p].remove();

                    Projectiles.RemoveAt(p);
                    p--;
                }
            }



/*            for (int p = 0; p < Projectiles.Count; ++p)
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

                            //enemy.getAIEntity().getNode().TriangleSelector = trSel;
//                            (MeshSceneNode)enemy.getAIEntity().getNode()).Mesh,
  //                              enemy.getAIEntity().getNode());


                            if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, trSel, out outVec, out outTri, out outNode))
                            {
                              //  trSel.Drop();

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

                    if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, MetaSelector, out outVec, out outTri, out outNode) && !enHit)
                    {
                        Projectiles[p].remove();

                        Projectiles.RemoveAt(p);
                        p--;
                    }
                }
            }
 */
        }

        public override bool update(uint elapsedTime)
        {
            if (!AIEntity.bIsLive)
            {
                AnalyzeProjectiles();

                return true;
            }

            if (Health <= 0)
            {
                die();
                //CurrAnimation = AnimationTypeMD2.Death_Fall_Back;
                //bIsLive = false;

                //sceneManager.AddToDeletionQueue(CharacterNode);
                //if(CharacterNode != null)
                //  CharacterNode.Remove();



                return true;
            }

            if (base.update(elapsedTime)) 
                return true;

            AnalyzeProjectiles();
  
	        TimeSinceLastShot += elapsedTime;
  
	        if (!EnemyVisibleUpdate) 
                ((INPC)AIEntity).setStayPut(false,false);
  
	        EnemyVisibleUpdate = false;

	        return false;
        }
    
		public bool isChasing() 
        {
            return Chasing; 
        }

        public override void registerHit()
        {
            base.registerHit();
            //((INPC)AIEntity).sendEventToNPC(E_NPC_EVENT_TYPE.ENET_UNDER_ATTACK, null);
            ((INPC)AIEntity).sendEventToNPC(E_NPC_EVENT_TYPE.ENET_UNDER_ATTACK, EnemyVisible);

        }
    }
}
