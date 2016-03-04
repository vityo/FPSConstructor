using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;
using IrrGame.IrrAi;

using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace IrrGame.IrrFPS
{
    public class CCharacter 
    {
        //public static uint SHOT_DELAY_TIME = 300;
        
        public static int MaxHealth;
        public static int MaxAmmo;
        public static uint ShotDelayTime;
        public static bool RegenerateHealth;
        public static int RefillPeriodHealth;
        public static bool RegenerateAmmo;
        public static int RefillPeriodAmmo;
        public static int DrawnHealth;

        public static float AnimationSpeedDeath;
        public static float AnimationSpeedBoom;
        public static float AnimationSpeedCrouchAttack;
        public static float AnimationSpeedAttack;
        public static float AnimationSpeedJump;
        public static float AnimationSpeedCrouchWalk;
        public static float AnimationSpeedRun;
        public static float AnimationSpeedCrouchStand;
        public static float AnimationSpeedStand;


        public static Vector3Df offsetStand;
        public static Vector3Df offsetCrouch;
        public static uint TimeFallToDeath;

        protected IAIEntity AIEntity;
        protected IAIManager AIManager;
        protected SceneManager sceneManager;
        public Vector3Df Pos;
        protected int Health;
        protected int Ammo;
        protected uint TimeSinceLastRefillHealth;
        protected uint TimeSinceLastRefillAmmo;
       // protected E_CHARACTER_TYPE CharacterType;
        //protected bool RegenerateHealth;
        protected AnimatedMeshSceneNode CharacterNode;
       //public bool bIsLive;

        public int startWaypointID;

        public List<CProjectile> Projectiles;

        public bool Crouch;

		public class SCharacterDesc 
        {
            public SceneManager sceneManager;
			public CAIManager AIManager;
			public int StartWaypointID;
			public string WaypointGroupName; 
			public int WaypointGroupIdx; 
			public float NPCRange;    
			public string Name; 
			public bool RegenerateHealth;

			public SCharacterDesc() 
            {
				sceneManager = null;
				AIManager = null;
				StartWaypointID = 1;
				WaypointGroupName = "";   
				WaypointGroupIdx = 0;
				NPCRange = 0;   
				Name = "";
				RegenerateHealth = false;
			}
			
		}
    
// 		public static int MAX_AMMO = 50;
// 		public static int MAX_HEALTH = 100;
// 		public static int REFILL_PERIOD = 500;

        public CCharacter(SCharacterDesc desc)
            //, Program.Projectile aProjectileSettings)
            //, Program.Character aCharacterSettings)//, E_CHARACTER_TYPE characterType)
        {
//             characterSettings = aCharacterSettings;
//             projectileSettings = aProjectileSettings;
            Pos = new Vector3Df();
            //bIsLive = true;
                
            sceneManager = desc.sceneManager;
            AIManager = desc.AIManager;
           // RegenerateHealth = desc.RegenerateHealth;
            startWaypointID = desc.StartWaypointID;

          //  CharacterType = characterType;
            Health = MaxHealth;
            Ammo = MaxAmmo;
            TimeSinceLastRefillHealth = 0;
            TimeSinceLastRefillAmmo = 0;
            AIEntity = null;
        }

        ~CCharacter()
        {
            if (AIEntity != null)
            {
                AIManager.removeAIEntity(AIEntity);
                AIEntity = null;
            }
        }

        public static void SetCharacterSettings(int aMaxHealth,int aMaxAmmo,uint aShotDelayTime,
            bool aRegenerateHealth, int aRefillPeriodHealth, bool aRegenerateAmmo, int aRefillPeriodAmmo,
            int aDrawnHealth, float aAnimationSpeedDeath, float aAnimationSpeedBoom, float aAnimationSpeedCrouchAttack,
            float aAnimationSpeedAttack, float aAnimationSpeedJump, float aAnimationSpeedCrouchWalk, float aAnimationSpeedRun,
            float aAnimationSpeedCrouchStand, float aAnimationSpeedStand, Vector3Df aOffsetStand, Vector3Df aOffsetCrouch,
            uint aTimeFallToDeath)
        {
            MaxHealth = aMaxHealth;
            MaxAmmo = aMaxAmmo;
            ShotDelayTime = aShotDelayTime;
            RegenerateHealth = aRegenerateHealth;
            RefillPeriodHealth = aRefillPeriodHealth;
            RegenerateAmmo = aRegenerateAmmo;
            RefillPeriodAmmo = aRefillPeriodAmmo;
            DrawnHealth = aDrawnHealth;

            AnimationSpeedDeath = aAnimationSpeedDeath;
            AnimationSpeedBoom = aAnimationSpeedBoom;
            AnimationSpeedCrouchAttack = aAnimationSpeedCrouchAttack;
            AnimationSpeedAttack = aAnimationSpeedAttack;
            AnimationSpeedJump = aAnimationSpeedJump;
            AnimationSpeedCrouchWalk = aAnimationSpeedCrouchWalk;
            AnimationSpeedRun = aAnimationSpeedRun;
            AnimationSpeedCrouchStand = aAnimationSpeedCrouchStand;
            AnimationSpeedStand = aAnimationSpeedStand;
            
            offsetStand = aOffsetStand;
            offsetCrouch = aOffsetCrouch;

            TimeFallToDeath = aTimeFallToDeath;
        }

        public virtual void deleteNodes()
        {
            
            for (int p = 0; p < Projectiles.Count; ++p)
            {
                    Projectiles[p].remove();

                    Projectiles.RemoveAt(p);
                    p--;
            }
            
        }

        public void die()
        {

//             float FRAME_TIME = 0.05f;
//             SceneNodeAnimator anim = null;
// 
//             string sPath = "media/Explosion/";
//             List<Texture> textures = new List<Texture>();
// 
//             for (int g = 1; g <= 11; ++g)
//             {
//                 string sTextureFilePath = sPath + g.ToString() + ".jpg";
// 
//                 textures.Add(sceneManager.VideoDriver.GetTexture(sTextureFilePath));
//             }
// 
//             anim = sceneManager.CreateTextureAnimator(textures, FRAME_TIME, false);
// 
//             SceneNode bill = sceneManager.AddBillboardSceneNode(null, new Dimension2Df(70, 70), Pos + (new Vector3Df(0, 10, 0)), -1);
//             bill.SetMaterialFlag(MaterialFlag.Lighting, false);
//             bill.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture("media/Explosion/1.jpg"));
//             bill.SetMaterialType(MaterialType.TransparentAddColor);
//             bill.AddAnimator(anim);
//             anim.Drop();
// 
//             anim = sceneManager.CreateDeleteAnimator(FRAME_TIME * 11);
//             bill.AddAnimator(anim);
//             anim.Drop();


            AIEntity.bIsLive = false;


//             if (CharacterNode != null)
//                  CharacterNode.Remove();
            
            //CharacterNode.Remove();            
            ((INPC)AIEntity).sendEventToNPC(E_NPC_EVENT_TYPE.ENET_DIE, null);
        }


        public virtual bool update(uint elapsedTime)
        {

            if (RegenerateHealth && TimeSinceLastRefillHealth > RefillPeriodHealth)
            {
                if (Health < MaxHealth)
                    ++Health;

                TimeSinceLastRefillHealth = 0;
            }
            else
            {
                TimeSinceLastRefillHealth += elapsedTime;
            }

            if (RegenerateAmmo && TimeSinceLastRefillAmmo > RefillPeriodAmmo)
            {
                if (Ammo < MaxAmmo)
                    ++Ammo;

                TimeSinceLastRefillAmmo = 0;
            }
            else
            {
                TimeSinceLastRefillAmmo += elapsedTime;
            }
// 
//             if (RegenerateHealth && TimeSinceLastRefill > RefillPeriod)
//             {
//                 if (Health < MaxHealth)
//                     ++Health;
// 
//                 if (Ammo < MaxAmmo) 
//                     ++Ammo;
// 
//                 TimeSinceLastRefill = 0;
//             }
//             else
//             {
//                 TimeSinceLastRefill += elapsedTime;
//             }

            return false;
        }

		public virtual void registerHit()
        {
            Health -= DrawnHealth;

            if (Health < 0)
                Health = 0;

            //TimeSinceLastRefill = 0;


        }

		public IAIEntity getAIEntity() 
        {
            return AIEntity; 
        }

		public Vector3Df getAbsolutePosition() 
        {
            return AIEntity.getAbsolutePosition(); 
        }

//         public E_CHARACTER_TYPE getCharacterType() 
//         {
//             return CharacterType; 
//         }

		public int getHealth() 
        {
            return Health; 
        }

		public int getAmmo()
        {
            return Ammo; 
        }

		public void addHealth(int amount) 
        {
			Health += amount;
			if (Health > 100) Health = 100;
			if (Health < 0) Health = 0;
		}  
    }
}
