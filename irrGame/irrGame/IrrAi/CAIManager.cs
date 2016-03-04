using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrAi
{
    public class CAIManager : IAIManager
    {


        private List<SWaypointGroup> WaypointGroups;
        private List<SEntityGroup> EntityGroups;
        private SEntityGroup Entities;
        private int NextEntityID;
        private int NumWaypoints;
        private int NumEntities;
        private SceneNode DebugRootNode;

        public OcclusionQueryCallbackPtr oqcp;


        public CAIManager(IrrlichtDevice device)
            : base(device)
        {
            WaypointGroups = new List<SWaypointGroup>();
            EntityGroups = new List<SEntityGroup>();
            Entities = new SEntityGroup();

            NextEntityID = 0;
            NumWaypoints = 0;
          
            reinit();
        }
        ~CAIManager()
        {
            clear();
        }
        public override void reinit()
        {
            DebugRootNode = sceneManager.AddEmptySceneNode();
            DebugRootNode.Visible = DebugVisible;
            DebugRootNode.Grab();
        }
        public void resetAI()
        {
            Entities.Entities.Clear();
        }
        public override void update(uint elapsedTime)
        {
            foreach (IAIEntity entity in Entities.Entities)
            {
//                 if (entity.bIsLive)
//                 {
                    entity.reset();
                    entity.update(elapsedTime);
               // }
            }
            /*for (int i = 0; i < Entities.Entities.Count; ++i)
                Entities.Entities[i].reset();

            for (int i = 0; i < Entities.Entities.Count; ++i)
                Entities.Entities[i].update(elapsedTime);
             */
        }
        
        public override void clear()
        {
            if(Entities != null)
	            Entities.Entities.Clear();

            if(EntityGroups != null)
	            EntityGroups.Clear();
  
            if(WaypointGroups != null)
	            WaypointGroups.Clear();

	        NumWaypoints = 0;

	        if (DebugRootNode!=null)
            {
		        DebugRootNode.Remove();
		        DebugRootNode.Drop();
		        DebugRootNode = null;
	        }
        }
        
        public override bool loadAI(string fileName)
        {
            CIrrAIFileParser fileParser = new CIrrAIFileParser();

            bool ret = fileParser.parseXML(this, fileName);

            if(ret)
                Console.WriteLine("Loaded: {0} Waypoint Groups, {1} Waypoints, {2} Entities\n", WaypointGroups.Count, NumWaypoints, getNumEntities());

            return ret;
        }

        public void linkWaypoints(SWaypointGroup group)
        {
            if (group==null)
                return;

            foreach (CWaypoint mainWaypoint in group.Waypoints)
            {
                string name = mainWaypoint.getNeighbourString();
                string[] nameW = name.Split(new char[] { ',' });

                foreach(string nameS in nameW)
                {
                    int id = int.Parse(nameS);

                    IWaypoint neighbourWaypoint = getWaypointFromId(group, id);

                    if (neighbourWaypoint!=null)
                        mainWaypoint.addNeighbour(neighbourWaypoint);
                }
            }
        }
        
        
        public override bool saveAI(string fileName)
        {
            	if (Device==null) 
                    return false;

	        CIrrAIFileWriter fileWriter = new CIrrAIFileWriter(Device.FileSystem);
	        
            return fileWriter.writeToXML(this, fileName); 
        }
        
       
        public override IWaypoint getWaypointFromId(SWaypointGroup group, int id)
        {
            return group.Waypoints[id];
        }

        public override ICombatNPC createCombatNPC(SCombatNPCDesc desc)//,OcclusionQueryCallbackPtr oq)//, AnimatedMeshSceneNode characterNodeAnimate)
        {
            CCombatNPC npc = new CCombatNPC(desc, this, sceneManager, NextEntityID++);//, oq);//, characterNodeAnimate);
            Entities.addEntity(npc);

            return npc;
        }
        
        public override INPC createPathFindingNPC(SNPCDesc desc)
        {
            CPathFindingNPC npc = new CPathFindingNPC(desc, this, sceneManager, NextEntityID++);
            Entities.addEntity(npc);

            return npc;

        }

        public override IPlayerAIEntity createPlayerAIEntity(SAIEntityDesc desc)
        {
            CPlayerAIEntity paie = new CPlayerAIEntity(desc, this, sceneManager, NextEntityID++);
            Entities.addEntity(paie);
            return paie;

        }

        public override SEntityGroup createEntityGroup()
        {
            SEntityGroup group = new SEntityGroup();
            EntityGroups.Add(group);

            return group;
        }

        public override IWaypoint createWaypoint(SWaypointGroup group, Vector3Df pos)
        {
            return createWaypoint(group, group.Waypoints.Count, pos);
        }

        public IWaypoint createWaypoint(SWaypointGroup group, int id, Vector3Df pos)
        {
            if (group == null)
                return null;

            CWaypoint wypt = new CWaypoint(id, pos);

            if (wypt != null)
            {
                group.Waypoints.Add(wypt);
                ++NumWaypoints;
            }

            if (group.WaypointMeshNode != null)
                ((CWaypointMeshSceneNode)group.WaypointMeshNode).addWaypoint(wypt);

            return wypt;
        }

//         public override void removeWaypoint(SWaypointGroup group, IWaypoint waypoint)
//         {
//             if (group == null || waypoint == null) return;
// 
//             for (int i = 0; i < group.Waypoints.Count; ++i)
//                 if (group.Waypoints[i] == waypoint)
//                 {
//                     group.Waypoints.RemoveAt(i);
//                     waypoint.remove();
//                     NumWaypoints--;
//                     i--;
//                 }
//                 else 
//                     group.Waypoints[i].removeNeighbour(waypoint); 
// 
//             if (group.WaypointMeshNode != null)
//                 createDebugWaypointMesh(group);
//         }

//         public override void removeWaypointGroup(SWaypointGroup group)
//         {
//             if (group == null)
//                 return;
//     
// 	        for (int wg = 0 ; wg < WaypointGroups.Count ; ++wg) 
// 		        if (WaypointGroups[wg] == group)
//                 {
// 			
// 			        if (group.WaypointMeshNode != null)
//                     {
// 				        group.WaypointMeshNode.Remove();
// 				        group.WaypointMeshNode.Drop();
// 				        group.WaypointMeshNode = null;                      
// 			        }
// 			
// 			        for (int i = 0 ; i < group.Waypoints.Count ; ++i) 
//                     {
// 				        removeWaypoint(group, group.Waypoints[i]);
// 				        NumWaypoints--;
// 			        }
// 			
// 			        WaypointGroups.RemoveAt(wg);
// 			
// 			        break;          
// 		        } 
//         }

        public override SWaypointGroup createWaypointGroup()
        {
            SWaypointGroup newGroup = new SWaypointGroup();
            WaypointGroups.Add(newGroup);
            return newGroup;    
        }

        public override SWaypointGroup getWaypointGroupFromName(string name)
        {
            for (int i = 0; i < WaypointGroups.Count; ++i)
                if (WaypointGroups[i].getName().CompareTo(name) == 0)
                    return WaypointGroups[i];

            return null;
        }

        public override void removeAIEntity(IAIEntity entity)
        {
            for (int i = 0; i < Entities.Entities.Count; ++i)
                if (Entities.Entities[i] == entity)
                {
                    Entities.Entities.RemoveAt(i);
                    break;
                }
        }

        public override void setDebugVisible(bool val)
        {
            DebugVisible = val;
            DebugRootNode.Visible = DebugVisible;
        }

        public override void createDebugWaypointMesh(SWaypointGroup group)
        {
            if (group==null)
                return;

            if (group.WaypointMeshNode == null)
            {
                group.WaypointMeshNode = new CWaypointMeshSceneNode(DebugRootNode, sceneManager, -1, group.Colour, group.WaypointSize);
            }

            CWaypointMeshSceneNode meshNode = (CWaypointMeshSceneNode)group.WaypointMeshNode;
            
            meshNode.clear();

            for (int i = 0; i < group.Waypoints.Count; ++i)
                meshNode.addWaypoint(group.Waypoints[i]);

            meshNode.recalculateBoundingBox();    
        }

        public override void createDebugWaypointMeshes()
        {
            for (int i = 0; i < WaypointGroups.Count; ++i)
                createDebugWaypointMesh(WaypointGroups[i]);
        }

        public override void removeDebugWaypointMeshes()
        {
            for (int i = 0; i < WaypointGroups.Count; ++i)
                if (WaypointGroups[i].WaypointMeshNode != null)
                {
                    WaypointGroups[i].WaypointMeshNode.Remove();
                    WaypointGroups[i].WaypointMeshNode.Drop();
                    WaypointGroups[i].WaypointMeshNode = null;
                }     
        }

        public override IWaypoint getNearestWaypoint(SWaypointGroup group, Vector3Df pos)
        {
            if (group == null)
                return null;

            if (group.Waypoints.Count == 0)
                return null;

            IWaypoint nearestWaypoint = group.Waypoints[0];
            float nearestDistance = nearestWaypoint.getPosition().GetDistanceFrom(pos);

            for (int i = 1; i < group.Waypoints.Count; ++i)
            {
                float dist = group.Waypoints[i].getPosition().GetDistanceFrom(pos);

                if (dist < nearestDistance)
                {
                    nearestWaypoint = group.Waypoints[i];
                    nearestDistance = dist;
                }
            }

            return nearestWaypoint; 
        }

        public override IAIEntity getEntityFromName(string name)
        {
            for (int i = 0; i < Entities.Entities.Count; ++i)
                if (Entities.Entities[i].getName().CompareTo(name) == 0)
                    return Entities.Entities[i];

            return null;
        }

        public override IFieldOfView createConeFieldOfView(Vector3Df dim, bool occlusionCheck)
        {
            return new CConeFieldOfView(this, occlusionCheck, dim);
        }

        public override void removeFieldOfView(IFieldOfView fov)
        {
            fov = null;
        }

        public override IDebugFOVSceneNode createDebugConeFOVSceneNode(Vector3Df dim)
        {
            return new CDebugConeFOVSceneNode(DebugRootNode, sceneManager, -1, dim);
        }

        public override void removeDebugFOVScenNode(IDebugFOVSceneNode fovNode)
        {
            fovNode.Remove();
            fovNode.Drop();
        }

        public override IPathFinder createAStarPathFinder()
        {
            return new CAStarPathFinder();
        }

        public override IPathFinder createBreadthFirstPathFinder()
        {
            return new CBreadthFirstPathFinder();
        }
        
        public override void removePathFinder(IPathFinder pathFinder)
        {
            //garbage collector
        }

        public override IAISensor createEntryExitSensor(SAIEntityDesc desc)
        {
            IAISensor sensor = new CEntryExitSensor(desc, this, sceneManager, NextEntityID++);
            
            if (sensor!=null) 
                Entities.Entities.Add(sensor);

            return sensor;
        }

        public override void removeSensor(IAISensor sensor)
        {
            //garbage collector
        }

        public override int getNumWaypoints()
        {
            return NumWaypoints; 
        }

        public override int getNumEntities() 
        {
            return Entities.Entities.Count; 
        }

        public override SWaypointGroup getWaypointGroupFromIndex(int idx) 
        {
            return WaypointGroups[idx]; 
        }

        public override List<SWaypointGroup> getWaypointGroups() 
        {
            return WaypointGroups; 
        }

        public override SEntityGroup getEntities() 
        {
            return Entities; 
        }

        public override bool isDebugVisible()
        {
            return DebugVisible; 
        }

        public override void setOcclusionQueryCallback(OcclusionQueryCallbackPtr cb) 
        {
            oqcp = cb; 
        }

        public override bool occlusionQuery(Line3Df ray)
        {
            if (oqcp != null)
                return oqcp(ray);
            else
                return false;
        }

        public override void registerAIEntity(IAIEntity entity)
        {
            Entities.addEntity(entity);
        }

        public override SceneNode getDebugRootNode() 
        {
            return DebugRootNode; 
        }

    }
            
}

