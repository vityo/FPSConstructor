using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime;
using IrrlichtLime.IO;

using System.Xml;
using System.Xml.Linq;

namespace IrrGame.IrrAi
{
    public class CIrrAIFileWriter
    {
        private FileSystem fileSystem;

		public CIrrAIFileWriter(FileSystem aFileSystem)
        {
            fileSystem = aFileSystem; 
        }

        public bool writeToXML(CAIManager aimgr, string fileName)
        {
            try
            {
            
                XmlTextWriter textWritter = new XmlTextWriter(fileName, Encoding.UTF8);
                textWritter.WriteStartDocument();

                textWritter.WriteStartElement("FileInfo");
                textWritter.WriteEndElement();

                textWritter.Close();

                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                XmlAttribute fileVersion = document.CreateAttribute("fileVersion");
                fileVersion.InnerText = "0.50";
                document.Attributes.Append(fileVersion);
                XmlAttribute numWaypointGroups = document.CreateAttribute("numWaypointGroups");
                numWaypointGroups.InnerText = aimgr.getWaypointGroups().Count.ToString();
                document.Attributes.Append(numWaypointGroups);
                XmlAttribute numEntities = document.CreateAttribute("numEntities");
                numEntities.InnerText = aimgr.getEntities().Entities.Count.ToString(); ;
                document.Attributes.Append(numEntities);

                foreach (SWaypointGroup waypointGroup in aimgr.getWaypointGroups())
                {
                    XmlNode WaypointGroupNode = document.CreateElement("WaypointGroup");
                    document.DocumentElement.AppendChild(WaypointGroupNode);

                    XmlAttribute name = document.CreateAttribute("name");
                    name.InnerText = waypointGroup.Name;
                    WaypointGroupNode.Attributes.Append(name);
                    XmlAttribute waypointSize = document.CreateAttribute("waypointSize");
                    waypointSize.InnerText = waypointGroup.WaypointSize.ToString();
                    WaypointGroupNode.Attributes.Append(waypointSize);
                    XmlAttribute colour = document.CreateAttribute("colour");
                    colour.InnerText = waypointGroup.Colour.Alpha.ToString() + ',' + waypointGroup.Colour.Red.ToString() + ',' +
                         waypointGroup.Colour.Green.ToString() + ',' + waypointGroup.Colour.Blue.ToString();
                    WaypointGroupNode.Attributes.Append(colour);
                    XmlAttribute numWaypoints = document.CreateAttribute("numWaypoints");
                    numWaypoints.InnerText = waypointGroup.Waypoints.Count.ToString();
                    WaypointGroupNode.Attributes.Append(numWaypoints);

                    foreach (IWaypoint waypoint in waypointGroup.Waypoints)
                    {
                        XmlNode WaypointNode = document.CreateElement("Waypoint");
                        WaypointGroupNode.AppendChild(WaypointNode);

                        XmlAttribute id = document.CreateAttribute("id");
                        id.InnerText = waypoint.getID().ToString();
                        WaypointNode.Attributes.Append(id);
                        XmlAttribute neighbours = document.CreateAttribute("neighbours");
                        neighbours.InnerText = ((CWaypoint)waypoint).getNeighbourString();
                        WaypointNode.Attributes.Append(neighbours);
                        XmlAttribute position = document.CreateAttribute("position");
                        position.InnerText = waypoint.getPosition().X.ToString() + ',' + waypoint.getPosition().Y.ToString() + waypoint.getPosition().Z.ToString();
                        WaypointNode.Attributes.Append(position);
                    }
                }

                foreach (IAIEntity entity in aimgr.getEntities().Entities)
                {
                    XmlNode entityNode = document.CreateElement("Entity");
                    document.AppendChild(entityNode);

                    XmlAttribute type = document.CreateAttribute("type");
                    type.InnerText = ((int)entity.getType()).ToString();
                    entityNode.Attributes.Append(type);

                    List<string> names = new List<string>();
                    List<string> values = new List<string>();

                    entity.writeOutXMLDescription(ref names, ref values);

                    for(int i = 0; i< names.Count; i++)
                    {
                        XmlAttribute atr = document.CreateAttribute(names[i]);
                        atr.InnerText = values[i];
                        entityNode.Attributes.Append(atr);
                    }
                }

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
