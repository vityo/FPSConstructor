using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrFPS
{
    public class CPlayerCharacter: CCharacter 
    {

        private uint TimeSinceFallToDeath;

        private CameraSceneNode Camera;
        private AnimatedMeshSceneNode ShotNode;
        private uint TimeSinceLastShot;
        //private List<CProjectile> Projectiles;
        private MetaTriangleSelector MetaSelector;

        //private Vector3Df offsetProjectile;
        private bool FPS;
        private Vector3Df cameraTargetRelativeParent;
        private Vector3Df cameraPositionRelativeParent;
        //private Vector3Df cameraDirectionVector;
        //private Vector3Df cameraAbsolutePositionLast;

        //public static uint SHOT_DELAY_TIME = 300;
      //  public float crouchFactor = 0.5f;
        
        //public bool playerCrouch = false;
        private AnimationTypeMD2 CurrAnimation;
        private float MoveSpeed;
        private float RotateSpeed;
        private float JumpSpeed;

        private Vector3Df move;
        //private SceneNode Bill;

        private CollisionResponseSceneNodeAnimator jumpAnimator;
        private Vector3Df EllipsoidRadius;
        bool secondMethodCrouch = false;
        //private Vector3Df cameraPositionAbsolute;

        //private bool Crouch;

        public CPlayerCharacter(SCharacterDesc desc, CameraSceneNode camera, MetaTriangleSelector selector,
            //Program.Gun gun,
            //Program.Projectile aProjectileSettings, Program.Character aCharacterSettings, 
            Vector3Df scale, Vector3Df ScaleShotNode, Vector3Df PositionShotNode, Vector3Df RotationShotNode, string ModelPath, string TexturePath,
            bool aFPS, Vector3Df aCameraTargetRelativeParent, Vector3Df aCameraPositionRelativeParent, float aMoveSpeed,
            Vector3Df aEllipsoidRadius, Vector3Df GravityPerSecond, Vector3Df EllipsoidTranslation, float aRotateSpeed,
            float aJumpSpeed):
            //, uint aTimeFallToDeath):
            //, Vector3Df aCameraAbsolutePositionLast) ://, Vector3Df aCameraDirectionVector) :
            //, E_CHARACTER_TYPE type):
            base(desc)
            //,aProjectileSettings, aCharacterSettings)
            //, type) 
        {
            //cameraPositionAbsolute = aCameraPositionRelativeParent;

            EllipsoidRadius = aEllipsoidRadius;
            JumpSpeed = aJumpSpeed;
            RotateSpeed = aRotateSpeed;
/*            List<Texture> textures = new List<Texture>();

            for (int g = 1; g <= 7; ++g)
            {
                string tmp = "Media/Projectile/LiveBillBoard/" + g.ToString() + ".jpg";
                Texture t = sceneManager.VideoDriver.GetTexture(tmp);
                textures.Add(t);
            }

            SceneNodeAnimator animP = sceneManager.CreateTextureAnimator(textures, 0.03f, true);

            Bill = sceneManager.AddBillboardSceneNode(sceneManager.RootNode, new Dimension2Df(15, 15),
                camera.Position,
                -3);
            Bill.SetMaterialFlag(MaterialFlag.Lighting, false);
            Bill.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture("Media/Projectile/LiveBillBoard/1.jpg"));
            //Bill.SetMaterialTexture(0, smgr.VideoDriver.GetTexture("media/Plasmaball/1.jpg"));

            Bill.SetMaterialType(MaterialType.TransparentAddColor);


            Bill.AddAnimator(animP);
            //SceneNodeAnimator animD = smgr.CreateDeleteAnimator(5);
            //Bill.AddAnimator(animD);


            animP.Drop();
            //animD.Drop();

*/


















            MoveSpeed = aMoveSpeed;
            FPS = aFPS;
            Camera = camera;
            cameraTargetRelativeParent = aCameraTargetRelativeParent;
            cameraPositionRelativeParent = aCameraPositionRelativeParent;
            //cameraDirectionVector = aCameraDirectionVector;
            //cameraAbsolutePositionLast = aCameraAbsolutePositionLast;
//             //переделать свитч:!!!!!
// 	        switch (type) 
//             {
// 		        case E_CHARACTER_TYPE.ECT_CHASING:
                    AnimatedMesh mesh = sceneManager.GetMesh(ModelPath);
            //"Media/blaster.3ds");

			        if (mesh == null) 
                    {
				        Console.WriteLine("ShotMesh load failed");
				        return;
			        }
            
			        ShotNode = sceneManager.AddAnimatedMeshSceneNode(mesh);

			        if (ShotNode == null)
                    {
                        Console.WriteLine("ShotNode creation failed");
				        return;
			        }

                    ShotNode.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(TexturePath));
            
            //"Media/blaster.jpg"));

//                     if (Camera != null)
//                         ShotNode.Parent = Camera;
            
            //AIEntity.getNode();// Camera;


                    
                    
                    ShotNode.Scale = ScaleShotNode;// new Vector3Df(0.25f, 0.25f, 0.25f);
                    ShotNode.Position = PositionShotNode; //new Vector3Df(4.5f, -5, 9);
                    ShotNode.Rotation = RotationShotNode;// new Vector3Df(0, -90, 0);
			        ShotNode.SetMaterialFlag(MaterialFlag.Lighting, false);


                    if (!FPS)
                    {
                        ShotNode.SetMD2Animation(AnimationTypeMD2.Stand);
                        CurrAnimation = AnimationTypeMD2.Stand;
                        ShotNode.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(TexturePath));

                        ShotNode.Animate(0);
                    }

                    if (FPS)
                        ShotNode.Parent = Camera;

                    //offsetProjectile = aOffsetProjectile;
                    Projectiles = new List<CProjectile>();

                    
                    TimeSinceLastShot = ShotDelayTime;

                    //SHOT_DELAY_TIME;

                    MetaSelector = selector;

                    SAIEntityDesc pDesc = new SAIEntityDesc();
                    pDesc.UserData = this;
                    pDesc.Scale = scale;

                    
                    pDesc.Offset = offsetStand;
                    //AABBox box = ShotNode.BoundingBoxTransformed;
                    //pDesc.Offset = new Vector3Df(0, (box.MaxEdge.Y - box.MinEdge.Y) / 2.0f, 0);

                    pDesc.Name = "";
                    AIEntity = AIManager.createPlayerAIEntity(pDesc);

                    if (AIEntity == null)
                        Console.WriteLine("Failed PAIE creation");
            //	        }


                         

// 			        break;
// 
// 		        case E_CHARACTER_TYPE.ECT_FLEEING:
//                 case E_CHARACTER_TYPE.ECT_SPECTATING:
// 			        ShotNode = null;
// 			        break;
	//        }

//             jumpAnimator = desc.sceneManager.CreateCollisionResponseAnimator(selector,camera,
//                                         camera.BoundingBox.MaxEdge - camera.BoundingBox.Center, new Vector3Df(0, 0, 0));
//             camera.AddAnimator(jumpAnimator);



                    CreateAnimator(sceneManager, selector, EllipsoidRadius,
                           GravityPerSecond, EllipsoidTranslation);
        }

        private void MPovorot(Vector3Df v, double costetta, double sintetta, ref Vector3Df p)
        {
            double[,] M = new double[,]{{costetta +(1-costetta)*v.X*v.X, (1-costetta)*v.X*v.Y - sintetta*v.Z,(1-costetta)*v.X*v.Z+ sintetta*v.Y},
                                        {(1-costetta)*v.X*v.Y + sintetta*v.Z,costetta +(1-costetta)*v.Y*v.Y,(1-costetta)*v.Y*v.Z- sintetta*v.X},
                                        {(1-costetta)*v.X*v.Z- sintetta*v.Y,(1-costetta)*v.Y*v.Z+ sintetta*v.X,costetta +(1-costetta)*v.Z*v.Z}};
            p = new Vector3Df((float)(p.X * M[0, 0] + p.Y * M[1, 0] + p.Z * M[2, 0]),
                (float)(p.X * M[0, 1] + p.Y * M[1, 1] + p.Z * M[2, 1]),
                (float)(p.X * M[0, 2] + p.Y * M[1, 2] + p.Z * M[2, 2]));
        }

        public void RotateVectorForwardToUpVector(ref Vector3Df vector, Vector3Df upVector, double degrees)
        {
            Vector3Df vectorNormalize = new Vector3Df(vector.X / vector.Length, vector.Y / vector.Length, vector.Z / vector.Length);
            double degTNow = Math.Acos(vectorNormalize.DotProduct(upVector)) * 180d / Math.PI;
 
             

             if (degTNow + degrees < 1)
                degrees = 1 - degTNow;

             if (degTNow + degrees > 179)
                 degrees = 179 - degTNow;

            
//              if (degrees < 0)
//                  return;
//              double degTNow = Math.Acos(vector.DotProduct(new Vector3Df(0, 1, 0)) / (vector.Length)) * 180d / Math.PI;
//  
//              if (degTNow < 0)
//                  degrees = - degTNow;
//  
//              if (degTNow > 180)
//                  degrees = - 180 + degTNow;


            double degVecRad = degrees * Math.PI / 180d;
            double cos = Math.Cos(degVecRad);

            MPovorot(vector.CrossProduct(upVector).Normalize(), cos, Math.Sin(degVecRad),ref vector);

//             vectorCross
// 
// 
// 
//             Vector3Df vec = new Vector3Df(10, 50, 20);
//             //             Vector3Df vecXY = new Vector3Df(10, 50, 0);
//             Vector3Df vecYZ = new Vector3Df(0, 50, 20);
//             //             Vector3Df vecXZ = new Vector3Df(10, 0, 20);
// 
//             //Vector3Df vecX = new Vector3Df(vec.X, (float)Math.Sqrt(vec.Length * vec.Length - vec.X * vec.X), 0);
// 
//             double degX = Math.Acos(vec.X / vec.Length) * 180d / Math.PI;
//             double degZ = Math.Acos(vec.Z / vecYZ.Length) * 180d / Math.PI;
// 
//             Vector3Df vecNew = new Vector3Df(vec.Length, 0, 0);
// 
//             vecNew.RotateXZby(degX);
//             vecNew.RotateYZby(-degZ);
//  
//  
//    
// 
// 
//             double lengthXZ = Math.Sqrt(vector.X*vector.X + vector.Z*vector.Z);
// 
// 
//             double height = vector.Y;
// 
//             double lengthVec = Math.Sqrt(lengthXZ * lengthXZ + height * height);
// 
//             double acos = Math.Acos(lengthXZ/lengthVec);
// 
//             double degVec = acos * 180d / Math.PI;
// 
// 
//             
// 
// 
// 
//             
//             double degVecNew = degVec + degrees;
// 
//             if (degVecNew > 89)
//                 degVecNew = 89;
//             else
//                 if (degVecNew < -89)
//                     degVecNew = -89;
// 
//             double degVecNewRad = degVecNew * Math.PI / 180d;
// 
//             double vecNewY = lengthVec * Math.Sin(degVecNewRad);
// 
//             double lengthXZNew = lengthVec * Math.Cos(degVecNewRad);
// 
//             double vecNewX = vector.X *lengthXZNew / lengthXZ;
//             double vecNewZ = vector.Z *lengthXZNew / lengthXZ;
// 
// 
//             //Vector3Df vec = vector.HorizontalAngle;
// 
//              vector.X = (float)vecNewX;
//              vector.Y = (float)vecNewY;
//              vector.Z = (float)vecNewZ;

        }
        public void CreateAnimator(SceneManager smgr, MetaTriangleSelector metaTriangleSelector,
            Vector3Df EllipsoidRadius, Vector3Df GravityPerSecond, Vector3Df EllipsoidTranslation)
        {
            if (FPS)
            {
                 jumpAnimator = smgr.CreateCollisionResponseAnimator(metaTriangleSelector, Camera, EllipsoidRadius,
                 GravityPerSecond, EllipsoidTranslation + offsetStand);
                 Camera.AddAnimator(jumpAnimator);
            }
            else
            {
                jumpAnimator = smgr.CreateCollisionResponseAnimator(metaTriangleSelector, ShotNode, EllipsoidRadius,
                GravityPerSecond, EllipsoidTranslation + offsetStand);
                ShotNode.AddAnimator(jumpAnimator);
            }

            //jumpAnim.Jump();
            //jumpAnim.Drop();
        }
        public void Jump()
        {
            if(!jumpAnimator.Falling)
                jumpAnimator.Jump(JumpSpeed);
        }
        public void Turn(bool turnForward, bool turnBack, bool turnLeft, bool turnRight)
        {
            if (AIEntity.bIsLive||FPS)
            {

                Vector3Df forwardDir = new Vector3Df(0, 0, 0);

                if(FPS)
                    forwardDir= Camera.Target - Camera.Position;
                else
                    forwardDir = cameraTargetRelativeParent - cameraPositionRelativeParent;

                Vector3Df forwardDirPlane = (new Vector3Df(forwardDir.X, 0, forwardDir.Z)).Normalize();

                move = new Vector3Df(0,0,0);

                if (turnForward && !turnBack)
                    move+= new Vector3Df(forwardDirPlane.X * MoveSpeed, 0, forwardDirPlane.Z * MoveSpeed);

                if (turnBack && !turnForward)
                    move += new Vector3Df(-MoveSpeed * forwardDirPlane.X, 0, -MoveSpeed * forwardDirPlane.Z);

                if (turnLeft && !turnRight)
                    move += new Vector3Df(-forwardDirPlane.Z * MoveSpeed, 0, forwardDirPlane.X * MoveSpeed);

                if (turnRight && !turnLeft)
                    move += new Vector3Df(-MoveSpeed * (-forwardDirPlane.Z), 0, -MoveSpeed * forwardDirPlane.X);

                if (FPS)
                {
                    Camera.Position += move;
                    Camera.Target = Camera.AbsolutePosition + cameraTargetRelativeParent;
                }
                else
                    ShotNode.Position += move;
            }
        }

        public void RotateCamera(Vector2Df changeXYRelative)
        {
//             if (changeXYRelative.X != 0 || changeXYRelative.Y != 0)
//             {
//             } 

//             Vector3Df cameraPosition = new Vector3Df(-100, 100, 0);
//             Vector3Df cameraTarget = new Vector3Df(0, 0, 0);
//            float speedRotate = 200;

                //Matrix mat = ShotNode.AbsoluteTransformation;
                //mat.TransformVector(ref cameraTargetRelativeParent);
               
                //ShotNode.AbsolutePosition + cameraTargetRelativeParent;

            //Vector3Df cameraTargetRelativeParent = new Vector3Df(50, 50, 0);

//             Vector3Df cameraPositionRelativeParent = Camera.Position;
//             cameraPositionRelativeParent -= cameraTargetRelativeParent;
            
            
            //Vector3Df cameraPositionRelativeParentVerticalRotation = Camera.Position;//new Vector3Df(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
//             Matrix mat = ShotNode.AbsoluteTransformation;
//             mat.TransformVector(ref cameraPositionRelativeParent);

            //Vector3Df cameraTargetRelativeParent = Camera.Target;//new Vector3Df(0, 0, 0);
            

                

                //Camera.Position = cameraPosition;
                //Camera.Target = ShotNode.AbsolutePosition;

                //Vector 
            
            
            
            
            //cameraPositionRelativeParent.RotateXZby(-changeXYRelative.X * speedRotate, Camera.Target);





//            Matrix mat = Camera.AbsoluteTransformation;

//             Vector3Df a = new Vector3Df(10, 20, 30);
//             Vector3Df b = new Vector3Df(0, 10, 0);

            //Vector3Df upVector = new Vector3Df(Camera.UpVector);

            //RotateVectorForwardToUpVector(ref cameraDirectionVector, /*ref upVector,*/ changeXYRelative.Y * speedRotate);
            
            //cameraPositionRelativeParent.RotateXYby(-changeXYRelative.Y * speedRotate, Camera.Target);


            //Camera.Position = cameraTargetRelativeParent + cameraDirectionVector;


            //Camera.Position += cameraTargetRelativeParent;

           // cameraTargetRelativeParent.RotateXZby(changeXYRelative.X * speedRotate);//,ShotNode.AbsolutePosition);
            //Camera.Target = cameraTargetRelativeParent;

            //Vector3Df cameraRot = (new Vector3Df(Camera.Position.X,0,Camera.Position.Z))
            //Vector3Df shotNodeRotation = ShotNode.Rotation; 

            //Vector3Df cameraRot = (new Vector3Df(Camera.Position.X, Camera.Position.Y, Camera.Position.Z)).Normalize();
            


//             if (cameraAbsolutePositionLast != Camera.AbsolutePosition)
//             {
//                 Vector3Df move = Camera.AbsolutePosition - cameraAbsolutePositionLast;
// 
//                 Camera.Target += move;
// 
//                 cameraAbsolutePositionLast = new Vector3Df(Camera.AbsolutePosition.X,
//                     Camera.AbsolutePosition.Y, Camera.AbsolutePosition.Z);
//             }
 
//             Vector3Df vec = new Vector3Df(100, 20, 50);
//             vec.RotateXZby(30);
            
            //...






            if (!FPS)
            {
                if (AIEntity.bIsLive)
                {
                    ShotNode.Rotation = new Vector3Df(0, ShotNode.Rotation.Y + changeXYRelative.X * RotateSpeed, 0);
                }
                // 
                cameraTargetRelativeParent.RotateXZby(-changeXYRelative.X * RotateSpeed);

                cameraPositionRelativeParent.RotateXZby(-changeXYRelative.X * RotateSpeed);







                Camera.Target = ShotNode.AbsolutePosition + cameraTargetRelativeParent;// +new Vector3Df(0, 20, 0);// +cameraTargetRelativeParent + ;



                //Vector3Df cameraDirectionRelativeParent = cameraTargetRelativeParent - Camera.Position;
                Vector3Df cameraDirectionRelativeParent = cameraTargetRelativeParent - cameraPositionRelativeParent;

                RotateVectorForwardToUpVector(ref cameraDirectionRelativeParent, Camera.UpVector, changeXYRelative.Y * RotateSpeed);

                cameraPositionRelativeParent = cameraTargetRelativeParent - cameraDirectionRelativeParent;

                Camera.Position = ShotNode.AbsolutePosition + cameraPositionRelativeParent;


            }
            else
            {
                 cameraTargetRelativeParent.RotateXZby(-changeXYRelative.X * RotateSpeed);
                 RotateVectorForwardToUpVector(ref cameraTargetRelativeParent, Camera.UpVector, changeXYRelative.Y * RotateSpeed);



                 
            }





            





            //, ShotNode.Position);



            //Bill.Position = Camera.AbsolutePosition;

             

	            
               











//             Vector3Df cameraTargetAbsolute = new Vector3Df(cameraTargetRelativeParent.X,
//                 cameraTargetRelativeParent.Y,cameraTargetRelativeParent.Z);
//             cameraTargetAbsolute.RotateXZby(changeXYRelative.X * speedRotate);
//             cameraTargetAbsolute += ShotNode.AbsolutePosition;
//             Camera.Target = cameraTargetAbsolute;
            
//            ShotNode.Rotation = new Vector3Df((cameraRot.X * 180d / Math.PI), 0, (float)Math.Acos(cameraRot.Z * 180d / Math.PI));
            //= new Vector3Df(Camera.Position.X, 0, Camera.Position.Z);



               // Camera.Target = cameraTargetRelativeParent;
         //   }
             //   += new Vector3Df(changeXYRelative.X * speedRotate,0,0);
        }

//         public CPlayerCharacter(SCharacterDesc desc, CameraSceneNode camera, MetaTriangleSelector selector,
//             //Program.Gun gun,
//             //Program.Projectile aProjectileSettings, Program.Character aCharacterSettings, 
//             Vector3Df scale, Vector3Df offset, Vector3Df aOffsetProjectile,
//             string meshPath, string texturePath,
//             Vector3Df ScaleBody, Vector3Df PositionBody, Vector3Df RotationBody) :
//             //, E_CHARACTER_TYPE type):
//             base(desc)
//         //,aProjectileSettings, aCharacterSettings)
//         //, type) 
//         {
//             FPS = false;
//             offsetProjectile = aOffsetProjectile;
//             Projectiles = new List<CProjectile>();
// 
//             Camera = camera;
//             TimeSinceLastShot = ShotDelayTime;
// 
//             //SHOT_DELAY_TIME;
// 
//             MetaSelector = selector;
// 
//             //  	        if (type != E_CHARACTER_TYPE.ECT_SPECTATING) 
//             //              {
//             SAIEntityDesc pDesc = new SAIEntityDesc();
//             pDesc.UserData = this;
//             pDesc.Scale = scale;
//             //Vector3Df(15, 60, 15);
//             pDesc.Offset = offset;
//             //new Vector3Df(0, -30, 0);
//             pDesc.Name = "";
//             AIEntity = AIManager.createPlayerAIEntity(pDesc);
// 
//             if (AIEntity == null)
//                 Console.WriteLine("Failed PAIE creation");
//             //	        }
// 
// 
//             //             //переделать свитч:!!!!!
//             // 	        switch (type) 
//             //             {
//             // 		        case E_CHARACTER_TYPE.ECT_CHASING:
//             AnimatedMesh mesh = sceneManager.GetMesh(meshPath);
//             //"Media/blaster.3ds");
// 
//             if (mesh == null)
//             {
//                 Console.WriteLine("Gun Mesh load failed");
//                 return;
//             }
// 
//             ShotNode = sceneManager.AddAnimatedMeshSceneNode(mesh);
// 
//             if (ShotNode == null)
//             {
//                 Console.WriteLine("Gun Node creation failed");
//                 return;
//             }
// 
//             if (Camera != null)
//                 ShotNode.Parent = Camera;
// 
//             ShotNode.Scale = ScaleBody;// new Vector3Df(0.25f, 0.25f, 0.25f);
//             ShotNode.Position = PositionBody; //new Vector3Df(4.5f, -5, 9);
//             ShotNode.Rotation = RotationBody;// new Vector3Df(0, -90, 0);
// 
//             ShotNode.SetMaterialFlag(MaterialFlag.Lighting, false);
//             ShotNode.SetMD2Animation(AnimationTypeMD2.Stand);
//             CurrAnimation = AnimationTypeMD2.Stand;
//             ShotNode.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(texturePath));
// 
//             ShotNode.Animate(0);
//             AABBox box = ShotNode.BoundingBoxTransformed;
// 
//             //EnemyInteractText = sceneManager.AddTextSceneNode(sceneManager.GUIEnvironment.Skin.GetFont(IrrlichtLime.GUI.GUIDefaultFont.Default),  "!!! !!! !!!", new Color(255,255,0,0), CharacterNode);
//             //EnemyInteractText.Position = new Vector3Df(0, (box.MaxEdge.Y - box.MinEdge.Y)/2.0f, 0);
//             //EnemyInteractText.Visible = false;
// 
// //             SCombatNPCDesc npcDesc = new SCombatNPCDesc();
// //             npcDesc.StartWaypointID = desc.StartWaypointID;
// //             npcDesc.WaypointGroupName = desc.WaypointGroupName;
// //             npcDesc.Range = 700.0f;
// //             npcDesc.FovDimensions = new Vector3Df(600, 600, 0);
// //             npcDesc.UserData = this;
// //             npcDesc.Scale = box.MaxEdge - box.MinEdge;
// //             npcDesc.MoveSpeed = 0.15f;
// //             npcDesc.Offset = new Vector3Df(0, (box.MaxEdge.Y - box.MinEdge.Y) / 2.0f, 0);
// //             npcDesc.AtDestinationThreshold = 20.0f;
// //             AIEntity = AIManager.createCombatNPC(npcDesc);//, CharacterNode);
// // 
// //             if (AIEntity == null)
// //                 Console.WriteLine("Failed NPC creation");
// 
//             /*
//             ShotNode.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(TexturePathGun));
//             //"Media/blaster.jpg"));
// 
//             if (Camera != null)
//                 ShotNode.Parent = Camera;
// 
//             ShotNode.Scale = ScaleGun;// new Vector3Df(0.25f, 0.25f, 0.25f);
//             ShotNode.Position = PositionGun; //new Vector3Df(4.5f, -5, 9);
//             ShotNode.Rotation = RotationGun;// new Vector3Df(0, -90, 0);
//             ShotNode.SetMaterialFlag(MaterialFlag.Lighting, false);
//             // 			        break;
//             // 
//             // 		        case E_CHARACTER_TYPE.ECT_FLEEING:
//             //                 case E_CHARACTER_TYPE.ECT_SPECTATING:
//             // 			        ShotNode = null;
//             // 			        break;
//             //        }
// 
//             //             jumpAnimator = desc.sceneManager.CreateCollisionResponseAnimator(selector,camera,
//             //                                         camera.BoundingBox.MaxEdge - camera.BoundingBox.Center, new Vector3Df(0, 0, 0));
//             //             camera.AddAnimator(jumpAnimator);
//              * 
//              */
//         }

        ~CPlayerCharacter() { }

//         public override void registerHit()
//         {
//             base.registerHit();
// 
//                 
//             //jumpAnimator.Jump(10);
//         }
         //{
//             if (ShotNode != null) 
//             {
// 		        ShotNode.Remove();
// 		        ShotNode = null;
// 	        }
//   
// 	        Projectiles.Clear();
        //}
//         protected void die()
//         {
//             base.die();
// 
// //             foreach (CProjectile proj in Projectiles)
// //             {
// //                 proj.GetNode().RemoveAnimators();
// //             }
// 
//             //RemoveGun();
//         }

        public override void deleteNodes()
        {
            base.deleteNodes();

            ShotNode.Remove();
        }

        
        public void RemoveGun()
        {
            //if (ShotNode != null)
            //if (ShotNode.Visible)
            //{
                
              //  ShotNode.Remove();
            //}
        }

      /*   public void StopAnimation()
        {
             foreach (CProjectile proj in Projectiles)
             {
                 proj.GetNode().RemoveAnimators();
             }
            if(FPS)
                ShotNode.Visible = false;
        }
       */
//         public void Crouch(bool bPlayerCrouch)
//         {
//             if(playerCrouch == bPlayerCrouch)
//                 return;
// 
//             playerCrouch = bPlayerCrouch;
// 
//             if (playerCrouch)
//             {
//                 AIEntity.getNode().Scale = new Vector3Df(AIEntity.getNode().Scale.X, AIEntity.getNode().Scale.Y * crouchFactor, AIEntity.getNode().Scale.Z);
//                 AIEntity.NodeOffset = new Vector3Df(AIEntity.NodeOffset.X, AIEntity.NodeOffset.Y * crouchFactor, AIEntity.NodeOffset.Z);
//                 //Camera.//Camera.AnimatorList[0].
//             }
//             else
//             {
//                 AIEntity.getNode().Scale = new Vector3Df(AIEntity.getNode().Scale.X, AIEntity.getNode().Scale.Y / crouchFactor, AIEntity.getNode().Scale.Z);
//                 AIEntity.NodeOffset = new Vector3Df(AIEntity.NodeOffset.X, AIEntity.NodeOffset.Y / crouchFactor, AIEntity.NodeOffset.Z);
//             }
// 
//             
//         }
        public void AnalyzeProjectiles()
        {
            
            List<SEntityGroup> EnemyGroups = ((IPlayerAIEntity)AIEntity).getEnemyGroups();
            CCharacter enemy = null;
            
            Vector3Df outVec;
            Triangle3Df outTri;
            SceneNode outNode;

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



                    //enemy.getAIEntity().getNode().TriangleSelector = trSel;
                    for(int p = 0; p < Projectiles.Count; ++p)
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

                        if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, MetaSelector, out outVec, out outTri, out outNode))
                        {
                            //collision = true;

                            Projectiles[p].remove();

                            Projectiles.RemoveAt(p);
                            p--;
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







/*




            for (int p = 0; p < Projectiles.Count; ++p)
            {
                if ((Projectiles[p]).update())
                {
                    Projectiles[p].remove();

                    Projectiles.RemoveAt(p);
                    p--;
                }
                else
                {
                    CCharacter enemy = null;
                    //bool collision = false;
                    Line3Df ray = new Line3Df(Projectiles[p].getPreviousPosition(), Projectiles[p].getPosition());
                    Vector3Df outVec;
                    Triangle3Df outTri;
                    SceneNode outNode;

                    bool enHit = false;

                    List<SEntityGroup> EnemyGroups = ((IPlayerAIEntity)AIEntity).getEnemyGroups();

                    foreach (SEntityGroup EnemyGroup in EnemyGroups)
                    {
                        List<IAIEntity> enemies = EnemyGroup.Entities;

                        for (int i = 0; i < enemies.Count; ++i)
                        {
                            enemy = (CCharacter)(enemies[i].getUserData());

                            if (!enemy.getAIEntity().bIsLive)
                                continue;

                            TriangleSelector trSel = sceneManager.CreateTriangleSelector(enemy.getAIEntity().CharacterNodeAnimate);

//                             TriangleSelector trSel = sceneManager.CreateTriangleSelector(((MeshSceneNode)enemy.getAIEntity().getNode()).Mesh,
//                                enemy.getAIEntity().getNode());



                            //enemy.getAIEntity().getNode().TriangleSelector = trSel;

                            if (sceneManager.SceneCollisionManager.GetCollisionPoint(ray, trSel, out outVec, out outTri, out outNode))
                            {
                               // trSel.Drop();

                                enemy.registerHit();
                                //collision = true;

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
                        //collision = true;

                        Projectiles[p].remove();

                        Projectiles.RemoveAt(p);
                        p--;
                    }

                    // 			if (!collision) 
                    //                 projIter++;   
                }

            }
 */
        }
        public void SetAnimation(bool TurnForward, bool TurnBack, bool TurnLeft, bool TurnRight, bool shot, bool crouch)
        {
            if (secondMethodCrouch)
            {
                jumpAnimator.EllipsoidTranslation -= offsetCrouch;

                jumpAnimator.EllipsoidTranslation += offsetStand;
                ShotNode.Visible = true;

                secondMethodCrouch = false;
            }

            if (Crouch != crouch && FPS)
            {
                
                if (crouch)
                {
                    //jumpAnimator.TargetNode.Position -= offsetStand;

                    
                    Camera.Position -= offsetStand;
                    Camera.Target -= offsetStand;
                    jumpAnimator.EllipsoidTranslation -= offsetStand;

                    //jumpAnimator.TargetNode.Position += offsetCrouch;


                    jumpAnimator.EllipsoidTranslation += offsetCrouch;
                    Camera.Position += offsetCrouch;
                    Camera.Target += offsetCrouch;
                    
                }
                else
                {
                    //jumpAnimator.TargetNode.Position -= offsetCrouch;
                    secondMethodCrouch = true;
                    ShotNode.Visible = false;

                    //jumpAnimator.EllipsoidTranslation -= offsetCrouch;
                    Camera.Position = Camera.Position -offsetCrouch;
                    Camera.Target = Camera.Target - offsetCrouch;
                    

                    //jumpAnimator.TargetNode.Position += offsetStand;

                    //jumpAnimator.EllipsoidTranslation += offsetStand;
                    Camera.Position += offsetStand;
                    Camera.Target += offsetStand;
                }
            }

            Crouch = crouch;



            AnimationTypeMD2 animation = CurrAnimation;
            float animSpeed = 0;
            
            if (!AIEntity.bIsLive)
            {
                if (TimeSinceFallToDeath <= TimeFallToDeath)
                {
                    animation = AnimationTypeMD2.Death_Fall_Forward;
                    animSpeed = AnimationSpeedDeath;
                }
                else
                {
                    animation = AnimationTypeMD2.Boom;
                    animSpeed = AnimationSpeedBoom;
                }
            }
            else
                if (shot)
                {
                    if (crouch)
                    {
                        animation = AnimationTypeMD2.Crouch_Attack;
                        animSpeed = AnimationSpeedCrouchAttack;

                    }
                    else
                    {
                        animation = AnimationTypeMD2.Attack;
                        animSpeed = AnimationSpeedAttack;
                    }
                    
                }
                else
                {
                    if (jumpAnimator.Falling)
                    {
                        animation = AnimationTypeMD2.Jump;
                        animSpeed = AnimationSpeedJump;
                    }
                    else
                    {
                        
                            if (TurnForward || TurnBack || TurnLeft || TurnRight)
                                if (crouch)
                                {
                                    animation = AnimationTypeMD2.Crouch_Walk;
                                    animSpeed = AnimationSpeedCrouchWalk;
                                }
                                else
                                {
                                    animation = AnimationTypeMD2.Run;
                                    animSpeed = AnimationSpeedRun;
                                }
                            else
                                if (crouch)
                                {
                                    animation = AnimationTypeMD2.Crouch_Stand;
                                    animSpeed = AnimationSpeedCrouchStand;
                                }
                                else
                                {
                                    animation = AnimationTypeMD2.Stand;
                                    animSpeed = AnimationSpeedStand;
                                }
                    }
                }

            

            if (CurrAnimation != animation || (shot && TimeSinceLastShot >= ShotDelayTime && AIEntity.bIsLive))
            {
                ShotNode.SetMD2Animation(animation);
                ShotNode.AnimationSpeed = animSpeed;
                CurrAnimation = animation;
            }

            
        }
        public override bool update(uint elapsedTime)
        {



            if (Crouch)
            {
                AIEntity.setOffset(offsetCrouch);

               
            }
            else
            {
                AIEntity.setOffset(offsetStand);


            }
            


            TimeSinceLastShot += elapsedTime;
            

            if (base.update(elapsedTime))
                return true;

            if (!AIEntity.bIsLive)
            {
                if(TimeSinceFallToDeath<=TimeFallToDeath)
                    TimeSinceFallToDeath += elapsedTime;


                AnalyzeProjectiles();

                return true;
            }

            if (Health <= 0)
            {
               // die();

                //bIsLive = false;

                //sceneManager.AddToDeletionQueue(CharacterNode);
                //if(CharacterNode != null)
                //  CharacterNode.Remove();

                AIEntity.bIsLive = false;

                
            }




            if (AIEntity != null)
            {
                Vector3Df pos = new Vector3Df();
                if (FPS)
                    pos = Camera.AbsolutePosition;
                else
                    pos = ShotNode.AbsolutePosition;

                AIEntity.setPosition(pos);
            }

//             if (!FPS)
//             {
//                 Camera.Target = ShotNode.AbsolutePosition + cameraTargetRelativeParent;
//             }
            

            AnalyzeProjectiles();
  

     
	        return false;
  
        }

        public void fire()
        {
            if (AIEntity.bIsLive)
            {
                //if (CharacterType == E_CHARACTER_TYPE.ECT_CHASING && TimeSinceLastShot >= SHOT_DELAY_TIME && Ammo > 0) 
                if (TimeSinceLastShot >= ShotDelayTime && Ammo > 0)
                {
                    Vector3Df offset = new Vector3Df();
                    CProjectile proj = null;
                    Matrix mat = Camera.AbsoluteTransformation;

//                      if (FPS)
//                      {
// //                          if(Crouch)
// //                              offset = new Vector3Df(CProjectile.StartPositionPlayerCrouch.X,
// //                                  CProjectile.StartPositionPlayerCrouch.Y, CProjectile.StartPositionPlayerCrouch.Z);
// //                          else
// //                              offset = new Vector3Df(CProjectile.StartPositionPlayerStand.X,
// //                                  CProjectile.StartPositionPlayerStand.Y, CProjectile.StartPositionPlayerStand.Z);
// 
//                          //Matrix mat = Camera.AbsoluteTransformation;
//                          mat.TransformVector(ref offset);
//                          proj = new CProjectile(offset, Crouch, false,
//                             (Camera.Target - Camera.AbsolutePosition).Normalize(), sceneManager, mat);
//                      }
//                      else
//                      {
                        //offset = ShotNode.AbsolutePosition + new Vector3Df(0, 15, 0);
                    
                    if(FPS)
                        proj = new CProjectile(ShotNode.AbsolutePosition+move, Crouch, false,
                            Camera.Target, sceneManager, mat, FPS);
                    else
                        proj = new CProjectile(ShotNode.AbsolutePosition + move, Crouch, false,
                            ShotNode.AbsolutePosition + move + (Camera.Target-Camera.Position), sceneManager, mat, FPS);

                    // }
                    // new Vector3Df(1, 1, 1);//3.5f,-6.5f,20);
                    //Vector3Df offset = new Vector3Df(1, -2, 1);



                    //,projectileSettings);

                    if (proj != null)
                        Projectiles.Add(proj);

                    TimeSinceLastShot = 0;
                    Ammo--;
                    
                    
                    //TimeSinceLastRefill = 0;


                }
            }
        }
    
		public AnimatedMeshSceneNode getNode()
        {
            return ShotNode; 
        }
    }
}
