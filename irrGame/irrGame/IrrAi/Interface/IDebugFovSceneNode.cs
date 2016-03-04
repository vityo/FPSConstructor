using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Scene;

namespace IrrGame.IrrAi.Interface
{
    public class IDebugFOVSceneNode : SceneNode
    {
        
        public static int CONEFOV_SCENE_NODE_ID = 1000;

        protected AABBox Box;
        protected Vector3Df Dimensions;
        
        public IDebugFOVSceneNode(SceneNode parent, SceneManager mgr, int id) : base(parent, mgr, id) 
        {
            Box = new AABBox();
        }
        
        ~IDebugFOVSceneNode() { }
        
        public virtual AABBox getBoundingBox() { return Box; }
        
        public virtual void setDimensions(Vector3Df dim) { Dimensions = dim; }

    }
}
