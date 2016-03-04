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
    

/**
\brief Struct describing a combat style non-player character
*/
    public class SCombatNPCDesc : SNPCDesc 
    {
        public Vector3Df FovDimensions;
        public float Range;  
        public bool FovOcclusionCheck;
	    public bool CheckFovForEnemies;
	    public bool CheckFovForAllies;
    
    	public SCombatNPCDesc() : base() 
        {
            FovDimensions = new Vector3Df(100,100,0);
            Range = 100.0f;    
            FovOcclusionCheck = true;
		    CheckFovForEnemies = true;
		    CheckFovForAllies = false;
        }

        public SCombatNPCDesc(string scale, string name, string offset, string position, string rotation,
            string waypointGroupName, string startWaypointID, string moveSpeed, string atDestinationThreshold,
            string fovDimensions, string range, string fovOcclusionCheck, string checkFovForEnemies, string checkFovForAllies) :
            base(scale, name, offset, position, rotation, waypointGroupName, startWaypointID, moveSpeed, atDestinationThreshold)
        {
            string strc;
            strc = fovDimensions;
            Utility.getVector3dfFrom(strc, ref FovDimensions);
            Range = float.Parse(range);
            FovOcclusionCheck = int.Parse(fovOcclusionCheck) != 0;
            CheckFovForEnemies = int.Parse(checkFovForEnemies) != 0;
            CheckFovForAllies = int.Parse(checkFovForAllies) != 0;
        }
	}




    

    public abstract class ICombatNPC : INPC
    {
        protected float Range;
		protected bool FovOcclusionCheck;
		protected bool CheckFovForEnemies;
		protected bool CheckFovForAllies;
		protected IFieldOfView FieldOfView;
		protected IDebugFOVSceneNode DebugFOV;

		public ICombatNPC(SCombatNPCDesc desc, IAIManager aimgr, SceneManager smgr, int id) : 
            base(desc, aimgr, smgr, E_AIENTITY_TYPE.EAIET_COMBATNPC, id) 
        {
			FieldOfView = null;
			DebugFOV = null;
			Range = desc.Range;
			FovOcclusionCheck = desc.FovOcclusionCheck;
			CheckFovForEnemies = desc.CheckFovForEnemies;
			CheckFovForAllies = desc.CheckFovForAllies;
		}

		~ICombatNPC() {}

		public void reset() 
        {
            base.reset(); 
        }

		public void setRange(float range) 
        {
            Range = range; 
        }

		public float getRange() 
        {
            return Range; 
        }

		public void setFOVDimensions(Vector3Df dim) 
        {
			if (FieldOfView!=null)
                FieldOfView.setDimensions(dim);
			if (DebugFOV!=null)
                DebugFOV.setDimensions(dim);
		}

		public void getFOVDimensions(Vector3Df dim)
        {
            if (FieldOfView!=null)
                dim = FieldOfView.getDimensions();
        }

		public bool isUsingFOVOcclusion() 
        {
            return FovOcclusionCheck; 
        }

		public void setUsesFOVOcclusion(bool val) 
        {
            FovOcclusionCheck = val; 
        }

		public bool checksFOVForEnemies()
        {
            return CheckFovForEnemies; 
        }

		public void setChecksFOVForEnemies(bool val) 
        {
            CheckFovForEnemies = val; 
        }

		public bool checksFOVForAllies()
        {
            return CheckFovForAllies; 
        }

		public void setChecksFOVForAllies(bool val) 
        {
            CheckFovForAllies = val; 
        }

		public abstract bool isVisibleToNPC(IAIEntity entity);
    }
}


