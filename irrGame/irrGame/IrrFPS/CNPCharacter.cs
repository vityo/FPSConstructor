using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;
using IrrlichtLime.Video;
using IrrlichtLime.Core;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrFPS
{
    public class CNPCharacter : CCharacter 
    {
//   
// 		public enum E_NPCHARACTER_TYPE
//         {
// 			CHASING_CHARACTER,
// 			FLEEING_CHARACTER     
// 		}
        
        
		//protected TextSceneNode EnemyInteractText;
		protected AnimationTypeMD2 CurrAnimation;

        //public bool enemyVisible;

       // public CollisionResponseSceneNodeAnimator jumpAnimator { get; set; }

        //public CNPCharacter(SCharacterDesc desc, E_CHARACTER_TYPE type, string meshPath, string texturePath, MetaTriangleSelector selector) :
        public CNPCharacter(SCharacterDesc desc, string meshPath, string texturePath, MetaTriangleSelector selector, float Range,
            Vector3Df FieldOfViewDimensions, float MoveSpeed, float AtDestinationThreshold) :
            base(desc)
            //, aProjectileSettings, aCharacterSettings)
            //, type) 
        {
	        AnimatedMesh mesh = sceneManager.GetMesh(meshPath);

	        if (mesh == null)
            {
		        Console.WriteLine("Character Mesh load failed");

		        return;
	        }

	        CharacterNode = sceneManager.AddAnimatedMeshSceneNode(mesh);

	        if (CharacterNode == null)
            {
		        Console.WriteLine("Character Node creation failed");

		        return;
	        }

	        CharacterNode.SetMaterialFlag(MaterialFlag.Lighting, false);
	        CharacterNode.SetMD2Animation(AnimationTypeMD2.Stand);
	        CurrAnimation = AnimationTypeMD2.Stand;
	        CharacterNode.SetMaterialTexture(0, sceneManager.VideoDriver.GetTexture(texturePath));
  
	        CharacterNode.Animate(0);
	        AABBox box = CharacterNode.BoundingBoxTransformed;
  
	        //EnemyInteractText = sceneManager.AddTextSceneNode(sceneManager.GUIEnvironment.Skin.GetFont(IrrlichtLime.GUI.GUIDefaultFont.Default),  "!!! !!! !!!", new Color(255,255,0,0), CharacterNode);
	        //EnemyInteractText.Position = new Vector3Df(0, (box.MaxEdge.Y - box.MinEdge.Y)/2.0f, 0);
	        //EnemyInteractText.Visible = false;
  
	        SCombatNPCDesc npcDesc = new SCombatNPCDesc();
	        npcDesc.StartWaypointID = desc.StartWaypointID;
	        npcDesc.WaypointGroupName = desc.WaypointGroupName;
	        npcDesc.Range = Range;
            npcDesc.FovDimensions = FieldOfViewDimensions;
            //new Vector3Df(600, 600, 0);
	        npcDesc.UserData = this;
	        npcDesc.Scale = box.MaxEdge - box.MinEdge;
            npcDesc.MoveSpeed = MoveSpeed;
	        npcDesc.Offset = new Vector3Df(0, (box.MaxEdge.Y - box.MinEdge.Y)/2.0f, 0);
            npcDesc.AtDestinationThreshold = AtDestinationThreshold;
            AIEntity = AIManager.createCombatNPC(npcDesc);//, CharacterNode);

	        if (AIEntity == null)
                Console.WriteLine("Failed NPC creation");


           // jumpAnimator = desc.sceneManager.CreateCollisionResponseAnimator(selector,
           //     CharacterNode, CharacterNode.BoundingBox.MaxEdge - CharacterNode.BoundingBox.Center, new Vector3Df(0, -100, 0));
          //  CharacterNode.AddAnimator(jumpAnimator);
        }

        ~CNPCharacter()
        {
//             if (CharacterNode != null)
//             {
//                 CharacterNode.Remove();
//                 CharacterNode = null;
//             }
               
        }


        public override bool update(uint elapsedTime)
        {
            if (base.update(elapsedTime) || ((INPC)AIEntity) == null) 
                return true;

//             if (!bIsLive)
//                 return true;

	        CharacterNode.Position = AIEntity.getAbsolutePosition();
	        CharacterNode.Rotation = AIEntity.getNode().Rotation;
  
	        Pos = CharacterNode.AbsolutePosition;
  
	        if (AIManager.isDebugVisible()) 
            {
		        if (((INPC)AIEntity).isVisibleToOtherEntity())
                    CharacterNode.SetMaterialFlag(MaterialFlag.Wireframe, true);
		        else
                    CharacterNode.SetMaterialFlag(MaterialFlag.Wireframe, false);

	        }
            else
                CharacterNode.SetMaterialFlag(MaterialFlag.Wireframe, false);

	        return false;
       
        }

        
		public AnimatedMeshSceneNode getNode() 
        {
            return CharacterNode; 
        }


    
		            

    }
}
