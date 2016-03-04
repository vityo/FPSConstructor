using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace IrrGame.IrrAi.Interface
{

    //public class IAIEntity{}
    //public struct SAIEntityDesc{}
    //public class IPlayerAIEntity{}
    //public class INPC{}
    //public struct SNPCDesc{}
   // public class ICombatNPC{}
    //public struct SCombatNPCDesc{}
    //public class IFieldOfView{}
    //public class IAISensor{}
    //public class IWaypoint{}

    public class SWaypointGroup
    {	
	    public List<IWaypoint> Waypoints;

	    public SceneNode WaypointMeshNode;

    
	    public string Name;

	    public Color Colour;  

	    public int WaypointSize;  

	    public SWaypointGroup()
        {
            Waypoints = new List<IWaypoint>();
		    WaypointMeshNode = null;
		    Name = "Group X";
		    Colour = new Color(255,255,255,255);  
		    WaypointSize = 5;             
	    } 

	    public void setName(string str) { Name = str; }

	    public string getName() { return Name; }

        public bool contains(IWaypoint waypoint)
        {
            return IWaypoint.contains(Waypoints, waypoint);  
        }

        public void setColour(Color colour)
        {
            Colour = colour;

	        if (WaypointMeshNode!=null) 
                ((CWaypointMeshSceneNode)WaypointMeshNode).setColour(Colour);  
        }

        public void setSize(int size)
        {
            WaypointSize = size;
            if (WaypointMeshNode!=null)
                ((CWaypointMeshSceneNode)WaypointMeshNode).setWaypointSize(WaypointSize);  
        }
    }


    public class SEntityGroup
    {
        public List<IAIEntity> Entities;

	    public SEntityGroup()
        {
            Entities = new List<IAIEntity>();
        }
        
	    public void addEntity(IAIEntity entity)
        {
		    Entities.Add(entity);
	    } 
    }

    public abstract class IAIManager
    {
        protected IrrlichtDevice Device;
        protected SceneManager sceneManager;
	    protected bool DebugVisible;

		public IAIManager(IrrlichtDevice device) 
        {
			Device = device;
			sceneManager = Device.SceneManager;
			DebugVisible = false;
		}

		~IAIManager() {}

        public abstract void reinit();

		public abstract void update(uint elapsedTime);

		public abstract void clear();

		public abstract bool loadAI(string fileName);

		public abstract bool saveAI(string fileName); 

		public abstract IWaypoint getWaypointFromId(SWaypointGroup group, int id); 

		public abstract INPC createPathFindingNPC(SNPCDesc desc);

        public abstract ICombatNPC createCombatNPC(SCombatNPCDesc desc);//, OcclusionQueryCallbackPtr oq);//, AnimatedMeshSceneNode characterNodeAnimate); 

		public abstract IPlayerAIEntity createPlayerAIEntity(SAIEntityDesc desc);

		public abstract SEntityGroup createEntityGroup(); 

		public abstract IWaypoint createWaypoint(SWaypointGroup group, Vector3Df pos); 

		//public abstract void removeWaypoint(SWaypointGroup group, IWaypoint waypoint); 

		//public abstract void removeWaypointGroup(SWaypointGroup group); 

		public abstract SWaypointGroup createWaypointGroup(); 

		public abstract SWaypointGroup getWaypointGroupFromName(string name); 

		public abstract void removeAIEntity(IAIEntity entity); 

		public abstract void setDebugVisible(bool val); 

		public abstract void createDebugWaypointMesh(SWaypointGroup group);

		public abstract void createDebugWaypointMeshes(); 

		public abstract void removeDebugWaypointMeshes();

		public abstract IWaypoint getNearestWaypoint(SWaypointGroup group, Vector3Df pos);

		public abstract IAIEntity getEntityFromName(string name);

		public abstract IFieldOfView createConeFieldOfView(Vector3Df dim, bool occlusionCheck);

		public abstract void removeFieldOfView(IFieldOfView fov);

		public abstract IDebugFOVSceneNode createDebugConeFOVSceneNode(Vector3Df dim);

		public abstract void removeDebugFOVScenNode(IDebugFOVSceneNode fovNode);

		public abstract IPathFinder createAStarPathFinder();

		public abstract IPathFinder createBreadthFirstPathFinder();

		public abstract void removePathFinder(IPathFinder pathFinder);

		public abstract IAISensor createEntryExitSensor(SAIEntityDesc desc);

		public abstract void removeSensor(IAISensor sensor);

		public abstract int getNumWaypoints();

		public abstract int getNumEntities();

		public abstract SWaypointGroup getWaypointGroupFromIndex(int idx);

		public abstract List<SWaypointGroup> getWaypointGroups();

		public abstract SEntityGroup getEntities();

		public abstract bool isDebugVisible();

        public abstract void setOcclusionQueryCallback(OcclusionQueryCallbackPtr cb);

		public abstract bool occlusionQuery(Line3Df ray);

		public abstract void registerAIEntity(IAIEntity entity);

		public abstract SceneNode getDebugRootNode();




        public delegate bool OcclusionQueryCallbackPtr(Line3Df ray);     
    }
}
