using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime;
using IrrlichtLime.Video;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;

using  System.Xml.Linq;

namespace IrrGame.IrrAi
{
    public class CIrrAIFileParser
    {
        
        public CIrrAIFileParser() { }

        public bool parseXML(CAIManager aimgr, string fileName)
        {
            try
            {

                SWaypointGroup currentGroup = null;

                XDocument xDocument = XDocument.Load(fileName);

                int numWaypointGroups = int.Parse(xDocument.Root.Attribute("numWaypointGroups").Value);
                int numEntities = int.Parse(xDocument.Root.Attribute("numEntities").Value);

                IEnumerable<XElement> xElemWaypointsEntities = xDocument.Root.Elements();

                foreach (XElement xElemWaypointEntitie in xElemWaypointsEntities)
                {
                    if (xElemWaypointEntitie.Name == "WaypointGroup")
                    {
                        string name = xElemWaypointEntitie.Attribute("name").Value;
                        int waypointSize = int.Parse(xElemWaypointEntitie.Attribute("waypointSize").Value);
                        string colour = xElemWaypointEntitie.Attribute("colour").Value;
                        int numWaypoints = int.Parse(xElemWaypointEntitie.Attribute("numWaypoints").Value);

                        currentGroup = aimgr.createWaypointGroup();
                        if (currentGroup != null)
                        {
                            currentGroup.setName(name);
                            currentGroup.WaypointSize = waypointSize;
                            Color col = new Color();

                            Utility.getColourFrom(colour, ref col);

                            currentGroup.setColour(col);
                        }

                        IEnumerable<XElement> xElemWaypoints = xElemWaypointEntitie.Elements();

                        foreach (XElement xElemWaypoint in xElemWaypoints)
                        {
                            if (xElemWaypoint.Name == "Waypoint")
                            {
                                int id = int.Parse(xElemWaypoint.Attribute("id").Value);
                                string neighbours = xElemWaypoint.Attribute("neighbours").Value;
                                string position = xElemWaypoint.Attribute("position").Value;
                                Vector3Df pos=new Vector3Df(0,0,0);
                                Utility.getVector3dfFrom(position, ref pos);

                                IWaypoint waypoint = aimgr.createWaypoint(currentGroup, id, pos);

                                if (waypoint != null)
                                {
                                    ((CWaypoint)waypoint).setNeighbourString(neighbours);
                                }
                            }
                        }
                    }
                  /*  else
                        if (xElemWaypointEntitie.Name == "Entity")
                        {
                            E_AIENTITY_TYPE type = (E_AIENTITY_TYPE)(int.Parse(xElemWaypointEntitie.Attribute("type").Value));
                            switch (type)
                            {
                                case E_AIENTITY_TYPE.EAIET_COMBATNPC:
                                    SCombatNPCDesc desc = new SCombatNPCDesc(xElemWaypointEntitie.Attribute("scale").Value, xElemWaypointEntitie.Attribute("name").Value,
                                        xElemWaypointEntitie.Attribute("offset").Value, xElemWaypointEntitie.Attribute("position").Value, xElemWaypointEntitie.Attribute("rotation").Value,
                                        xElemWaypointEntitie.Attribute("waypointGroupName").Value, xElemWaypointEntitie.Attribute("startWaypointID").Value, xElemWaypointEntitie.Attribute("moveSpeed").Value,
                                        xElemWaypointEntitie.Attribute("atDestinationThreshold").Value, xElemWaypointEntitie.Attribute("fovDimensions").Value, xElemWaypointEntitie.Attribute("range").Value,
                                        xElemWaypointEntitie.Attribute("fovOcclusionCheck").Value, xElemWaypointEntitie.Attribute("checkFovForEnemies").Value, xElemWaypointEntitie.Attribute("checkFovForAllies").Value);

                                    aimgr.createCombatNPC(desc);
                                    break;

                                case E_AIENTITY_TYPE.EAIET_PATHFINDINGNPC:
                                    SNPCDesc desc1 = new SNPCDesc(xElemWaypointEntitie.Attribute("scale").Value, xElemWaypointEntitie.Attribute("name").Value,
                                            xElemWaypointEntitie.Attribute("offset").Value, xElemWaypointEntitie.Attribute("position").Value, xElemWaypointEntitie.Attribute("rotation").Value,
                                            xElemWaypointEntitie.Attribute("waypointGroupName").Value, xElemWaypointEntitie.Attribute("startWaypointID").Value, xElemWaypointEntitie.Attribute("moveSpeed").Value,
                                            xElemWaypointEntitie.Attribute("atDestinationThreshold").Value);

                                    aimgr.createPathFindingNPC(desc1);
                                    break;

                                case E_AIENTITY_TYPE.EAIET_PLAYER:
                                    SAIEntityDesc desc2 = new SAIEntityDesc(xElemWaypointEntitie.Attribute("scale").Value, xElemWaypointEntitie.Attribute("name").Value,
                                        xElemWaypointEntitie.Attribute("offset").Value, xElemWaypointEntitie.Attribute("position").Value, xElemWaypointEntitie.Attribute("rotation").Value);

                                    aimgr.createPlayerAIEntity(desc2);
                                    break;

                                case E_AIENTITY_TYPE.EAIET_ENTRYEXIT_SENSOR:
                                    SAIEntityDesc desc3 = new SAIEntityDesc(xElemWaypointEntitie.Attribute("scale").Value, xElemWaypointEntitie.Attribute("name").Value,
                                         xElemWaypointEntitie.Attribute("offset").Value, xElemWaypointEntitie.Attribute("position").Value, xElemWaypointEntitie.Attribute("rotation").Value);

                                    aimgr.createEntryExitSensor(desc3);
                                    break;

                                case E_AIENTITY_TYPE.EAIET_UNKNOWN:
                                    throw new Exception("Unknown entity type in .irrai file, not loaded\n");

                                    break;

                            }


                        }
                   */
                }

                aimgr.linkWaypoints(currentGroup);

                Console.WriteLine("ParseComplete");

                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }    
    }
}
