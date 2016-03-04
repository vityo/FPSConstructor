using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace IrrGame.IrrFPS
{
    public class CProjectile 
    {
//         public static float Speed;
//         public static float MaxDistanceTravelled;
//         public static Vector3Df StartPositionBotStand;
//         public static Vector3Df StartPositionBotCrouch;
//         
//         public static float LiveBillBoardTextureTimePerFrame;
//         public static Dimension2Df LiveBillBoardDimension;
//         public static List<string> LibeBillBoardTexturePaths;
// 
        ///private static Program.Projectile projectileSettings;
        
        public static Vector3Df StartPositionBotStand;
        public static Vector3Df StartPositionBotCrouch;
        public static Vector3Df StartPositionPlayerStand;
        public static Vector3Df StartPositionPlayerCrouch;
        public static float Speed;
        public static float MaxDistanceTravelled;

        public static float LiveTextureTimePerFrame;
        public static Dimension2Df LiveDimension;
        public static List<string> LiveTexturePaths;

        public static float DieTextureTimePerFrame;
        public static Dimension2Df DieDimension;
        public static List<string> DieTexturePaths;

        private SceneManager sceneManager;
		private Vector3Df Direction;
		private Vector3Df PrevPos;
		private SceneNode Bill;
		private float SqDistTravelled;
        
        //public bool bRemove;

        public CProjectile(Vector3Df pos, bool crouchProjectile, bool botProjectile, Vector3Df goal, SceneManager smgr,
            Matrix mat, bool FPS)
            //,Program.Projectile aProjectileSettings)
        {
            //projectileSettings. = aProjectileSettings;


            if (botProjectile)
            {
//                 if (!FPS)
//                     goal += StartPositionBotStand;

                if (crouchProjectile)
                    pos += StartPositionBotCrouch;
                else
                    pos += StartPositionBotStand;
            }
            
      /*      else
                if (FPS)
                {
                    Vector3Df offset = new Vector3Df(0,0,0);
                    
                    
                    //                     //                          mat.TransformVector(ref offset);
                    if (crouchProjectile)
                        offset = new Vector3Df(StartPositionPlayerCrouch.X,
                            StartPositionPlayerCrouch.Y, StartPositionPlayerCrouch.Z);
                    else
                        offset = new Vector3Df(StartPositionPlayerStand.X,
                            StartPositionPlayerStand.Y, StartPositionPlayerStand.Z);

                    mat.TransformVector(ref offset);
                    pos = offset;
// 
//                                               offset = new Vector3Df(CProjectile.StartPositionPlayerStand.X,
//                                                   CProjectile.StartPositionPlayerStand.Y, CProjectile.StartPositionPlayerStand.Z);
//                     // 
//                     //                          //Matrix mat = Camera.AbsoluteTransformation;
//                     //                          mat.TransformVector(ref offset);
//                     //                          proj = new CProjectile(offset, Crouch, false,
//                     //                             (Camera.Target - Camera.AbsolutePosition).Normalize(), sceneManager, mat);
//                     if (crouchProjectile)
//                         pos += StartPositionPlayerCrouch;
//                     else
//                         pos += StartPositionPlayerStand;
                }
                else
                    if (crouchProjectile)
                        pos += StartPositionPlayerCrouch;
                    else
                        pos += StartPositionPlayerStand;
//             else
            //                 pos += projectileSettings.StartPositionPlayer;
            
            */
            Direction = (goal - pos).Normalize();

	        PrevPos = pos;
	        SqDistTravelled = 0;
	        sceneManager = smgr;
  
	        List<Texture> textures = new List<Texture>();

            foreach (string texturePath in LiveTexturePaths)
            {
                Texture t = sceneManager.VideoDriver.GetTexture(texturePath);
                textures.Add(t);
            }
// 	        for (int g=1; g<=7; ++g) 
//             {
// 		        string tmp = "media/Projectile/portal" + g.ToString() + ".jpg";
// 		        Texture t = sceneManager.VideoDriver.GetTexture(tmp);
// 		        textures.Add(t);
// 	        }

            SceneNodeAnimator anim = smgr.CreateTextureAnimator(textures, LiveTextureTimePerFrame, true);

            Bill = smgr.AddBillboardSceneNode(sceneManager.RootNode, LiveDimension, pos, -1);
	        Bill.SetMaterialFlag(MaterialFlag.Lighting, false);
	        //Bill.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture("media/Projectile/portal1.jpg"));
            Bill.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(LiveTexturePaths[0]));
            //"media/Plasmaball/1.jpg"));
            
	        Bill.SetMaterialType(MaterialType.TransparentAddColor);

           
	        Bill.AddAnimator(anim);
	        //anim.Drop();
        }

        ~CProjectile()
        {
            //remove();
            
        }
        public static void SetProjectileSettings(Vector3Df aStartPositionBotStand,Vector3Df aStartPositionBotCrouch,
            Vector3Df aStartPositionPlayerStand, Vector3Df aStartPositionPlayerCrouch, 
            float aSpeed, float aMaxDistanceTravelled, float aLiveTextureTimePerFrame,
            Dimension2Df aLiveDimension,List<string> aLiveTexturePaths,float aDieTextureTimePerFrame,
            Dimension2Df aDieDimension,List<string> aDieTexturePaths)
        {
            StartPositionBotStand = aStartPositionBotStand;
            StartPositionBotCrouch = aStartPositionBotCrouch;
            StartPositionPlayerStand = aStartPositionPlayerStand;
            StartPositionPlayerCrouch = aStartPositionPlayerCrouch;
            Speed = aSpeed;
            MaxDistanceTravelled = aMaxDistanceTravelled;

            LiveTextureTimePerFrame = aLiveTextureTimePerFrame;
            LiveDimension = aLiveDimension;
            LiveTexturePaths = aLiveTexturePaths;

            DieTextureTimePerFrame = aDieTextureTimePerFrame;
            DieDimension = aDieDimension;
            DieTexturePaths = aDieTexturePaths;
        }
        public SceneNode GetNode()
        {
            return Bill;
        }
         public void remove()
         {
             //bRemove = true;
             //Bill.Remove();
             
             Vector3Df pos = Bill.Position;
             Bill.Remove();
             Bill = null;

             float frameTime = DieTextureTimePerFrame;

             SceneNodeAnimator anim = null;

             List<Texture> textures = new List<Texture>();

             foreach (string texturePath in DieTexturePaths)
             {
                 Texture t = sceneManager.VideoDriver.GetTexture(texturePath);
                 textures.Add(t);
             }

//              for (int g = 1; g <= 6; ++g)
//              {
//                  string tmp = "Media/Plasmaball/" + g.ToString() + ".jpg";
//                  textures.Add(sceneManager.VideoDriver.GetTexture(tmp));
//              }

             anim = sceneManager.CreateTextureAnimator(textures, frameTime, true);

             Bill = sceneManager.AddBillboardSceneNode(sceneManager.RootNode, DieDimension,
                 pos - Direction*Speed/2, -1);
             //Bill.AbsolutePosition = colPoint;
             Bill.SetMaterialFlag(MaterialFlag.Lighting, false);
             Bill.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(DieTexturePaths[0]));
             Bill.SetMaterialType(MaterialType.TransparentAddColor);
             Bill.AddAnimator(anim);
             anim.Drop();

             anim = sceneManager.CreateDeleteAnimator(frameTime * DieTexturePaths.Count);
             Bill.AddAnimator(anim);
             anim.Drop();
              
         }

        public bool update()
        {
            if (SqDistTravelled > MaxDistanceTravelled) 
                return true;

            PrevPos = Bill.Position;

            Vector3Df distance = Direction * Speed;

            if (Bill != null)
                Bill.Position = PrevPos + distance;

            SqDistTravelled += distance.LengthSQ;

            return false;
        }

		public Vector3Df getPosition()
        {
            return Bill.Position; 
        }
		
        public Vector3Df getPreviousPosition() 
        {
            return PrevPos; 
        }
    }
}
