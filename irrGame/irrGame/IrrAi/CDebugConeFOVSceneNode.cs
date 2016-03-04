using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Scene;
using IrrlichtLime.Video;
using IrrlichtLime.IO;

namespace IrrGame.IrrAi
{
    public class CDebugConeFOVSceneNode : IDebugFOVSceneNode
    {
        private Vertex3D[] Vertices = new Vertex3D[3];
        private Material material;
		   
		public CDebugConeFOVSceneNode(SceneNode parent, SceneManager mgr, int id, Vector3Df dim) : base(parent, mgr, id)
        {
            material = new Material();

	        material.Wireframe = false;
	        material.Lighting = false;
	        material.BackfaceCulling = false;
	        material.Type = MaterialType.TransparentVertexAlpha;
	        material.ZWrite = false;
	        material.ZBuffer = ComparisonFunc.LessEqual;

	        setDimensions(dim);


            base.OnRegisterSceneNode += new RegisterSceneNodeEventHandler(OnRegisterSceneNode);
    
        }   

		~CDebugConeFOVSceneNode() {}

        public virtual void render()
        {
            ushort[] indices = new ushort[] { 0, 1, 2 };
            VideoDriver driver = this.SceneManager.VideoDriver;

            driver.SetMaterial(material);
            driver.SetTransform(TransformationState.World, AbsoluteTransformation);
            driver.DrawVertexPrimitiveList(Vertices, indices);
        }

        public virtual Material getMaterial(int i)
        {
            return material; 
        }

        public virtual uint getMaterialCount() 
        {
            return 1;
        }

        public virtual void serializeAttributes(Attributes outA, AttributeReadWriteOptions options)
        {
            base.SerializeAttributes(outA, options);
        }

        public virtual void deserializeAttributes(Attributes inA, AttributeReadWriteOptions options)
        {
            base.DeserializeAttributes(inA, options);
        }

        public virtual SceneNode clone(SceneNode newParent, SceneManager newManager)
        {
            if (newParent==null) newParent = Parent;
            if (newManager == null) newManager = this.SceneManager;

	        CDebugConeFOVSceneNode nb = new CDebugConeFOVSceneNode(newParent, newManager, ID, Dimensions);

            nb.MemberwiseClone();

	        nb.Drop();

	        return nb;
        }

        public virtual void OnRegisterSceneNode()
        {
            if (Visible)
                this.SceneManager.RegisterNodeForRendering(this,SceneNodeRenderPass.Transparent);

        }

        public SceneNodeType getType()
        {
            return (SceneNodeType)CONEFOV_SCENE_NODE_ID;
        }

        public virtual void setDimensions(Vector3Df dim)
        {
            base.setDimensions(dim);
            
	        Vertices[0] = new Vertex3D(0,0,0, 0,1,0, new Color(100,255,0,0), 0, 1);
	        Vertices[1] = new Vertex3D(dim.Y,0,dim.X/2.0f, 0,1,0, new Color(100,255,0,0), 1, 1);
            Vertices[2] = new Vertex3D(dim.Y, 0, -dim.X / 2.0f, 0, 1, 0, new Color(100, 255, 0, 0), 0, 0);

	        Box.Set(Vertices[0].Position);

	        for (int i = 1 ; i < 3 ; ++i)
		        Box.AddInternalPoint(Vertices[i].Position);
        }
    }
}

