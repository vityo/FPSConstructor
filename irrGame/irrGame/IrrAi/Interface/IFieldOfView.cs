using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;

namespace IrrGame.IrrAi.Interface
{
    public abstract class IFieldOfView 
    { 
		protected IAIManager AIManager;
		protected Vector3Df Position;
		protected Vector3Df Rotation;		
		protected Vector3Df Dimensions;
		protected bool OcclusionCheck;

		public IFieldOfView(IAIManager aimgr, bool occlusionCheck, Vector3Df dimensions) 
        { 
			AIManager = aimgr;
			OcclusionCheck = occlusionCheck;
			Dimensions = dimensions;
		}
		
		~IFieldOfView() {}

  		public abstract bool isInFOV(AABBox box, Vector3Df boxPos);
  		
        public void setPosition(Vector3Df vec) 
        {
            Position = vec; 
        }
  		
        public Vector3Df getPosition() 
        {
            return Position; 
        }
  		
  		public void setRotation(Vector3Df vec)
        {
            Rotation = vec; 
        }
  		
  		public Vector3Df getRotation()
        {
            return Rotation; 
        }
  		
  		public void setOcclusionCheck(bool val) 
        {
            OcclusionCheck = val; 
        }
  		
  		public bool getOcclusionCheck()
        {
            return OcclusionCheck; 
        }
		
		public void setDimensions(Vector3Df dim)
        {
            Dimensions = dim; 
        }
		
		public Vector3Df getDimensions()
        {
            return Dimensions; 
        }
    }
}
