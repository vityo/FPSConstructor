using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Video;
using IrrlichtLime.Scene;
using IrrlichtLime.GUI;
using IrrlichtLime.IO;

namespace IrrConstructor
{
    

    public partial class Form1 : Form
    {
        
        public static List<string> driverTypeList = new List<string>() { "DirectX", "OpenGL" };
        public static List<string> skyTypeListRus = new List<string>() { "Коробка", "Купол" };
        public static List<string> skyTypeListEng = new List<string>() { "Box", "Dome" };
        public static bool collapse = false;

        public enum CameraControlState { None, Zoom, Pan, RotateFirst, RotateAround, UpDown, CreateWaypoint, MoveWaypoint,
            DeleteWaypoint, CreateEdge, DeleteEdge };
        public static CameraControlState cameraControlState = CameraControlState.None;

        public static bool mouseDown = false;
        public static bool cameraReset = false;
        public static Vector2Di cursorLast = new Vector2Di(-1,-1);
        public static Vector2Di panelRenderingWindowSize = new Vector2Di(-1, -1);
        public static Vector3Df cameraPositionStart = new Vector3Df(250, 250, 250);
        public static Vector3Df cameraTargetRelativeStart = new Vector3Df(-250, -250, -250);
        public static Vector3Df cameraPosition = new Vector3Df(250, 250, 250);
        public static Vector3Df cameraTargetRelative = new Vector3Df(-250,-250,-250);
        public static Vector3Df cameraUpVector = new Vector3Df(0, 1, 0);

        public static Color sphereTargetColor = new Color(128, 128, 255, 30);
        public static float sphereTargetSize = 3;
        public static int sphereTargetFace = 15;
        public static float cubeTargetSize = 15;
        public static Color cubeTargetSelectColor = new Color(128, 200, 255, 30);
        public static Color cubeTargetColor = new Color(200, 200, 200, 30);
        public static Color playerTargetSelectColor = new Color(255, 200, 128, 30);
        public static Color playerTargetColor = new Color(200, 200, 100, 30);
        
        public static float lineThickness = 5;
        public static Color lineSelectColor = new Color(128, 255, 255, 255);
        public static Color lineColor = new Color(255, 255, 255, 255);
        //public static float sphereWaypointTargetSize = 10;
        //public static int sphereTargetFace = 15;
        
        
        
        
        
        public static int selectIdWaypoint = -1;
        public static int selectIdWaypointLast = -1;
        public static int selectIdLine = -1;

        public static float mouseRotateFirstSpeed = 1;
        public static float mouseRotateAroundSpeed = 1;
        public static float mouseZoomSpeed = 1;
        public static float mousePanSpeed = 1;

//         public static int MinWidthForm = 752;
//         public static int MinHeightForm = 475;
        public static int spacePropertyGrid = 475 - 390;
        public static int panelRenderingWidth = 520;
        public static int panelRenderingHeight = 390;
        private bool userWantExit = false; // if "true", we shut down rendering thread and then exit app


        public static bool createWaypointFirst = true;
        public static bool moveWaypointFirst = true;
        public static bool createLineFirst = true;
        public static bool deleteAllWaypoints = false;
        public static bool deleteAllLines = false;
        public static bool deleteEmptyWaypoints = false;
        public static bool moveWaypointFirstPlayer = false;
        public static bool aiGraphicsInitialize = false;

        //засунуть в вэйпойнтс с айдишником -1!!!
        public static Vector3Df playerPosition = new Vector3Df(0, 0, 0);

        public class Waypoint
        {
            public class Edge
            {
                public Vector3Df start;
                public Vector3Df end;
                public int startId;
                public int endId;
                public int id;

                public Edge(Vector3Df aStart, Vector3Df aEnd, int aStartId, int aEndId, int aId)
                {
                    start = aStart;
                    end = aEnd;
                    startId = aStartId;
                    endId = aEndId;
                    id = aId;
                }
            }

            public static List<Waypoint> waypoints = new List<Waypoint>();
            public static List<Edge> edges = new List<Edge>();
            public static int waypointMinID = 0;
            public static int edgeMinID = 0;

            public int id { get; set; }
            public List<int> neighbours { get; set; }
            public Vector3Df position { get; set; }

            public string GetPositionString()
            {
                return (position.X + " " + position.Y + " " + position.Z).Replace(',', '.').Replace(' ', ',');
            }

            public string GetNeighboursString()
            {
                string neighboursString = "";

                for (int i = 0; i < neighbours.Count - 1; i++)
                {
                    neighboursString += neighbours[i].ToString()+",";
                }

                neighboursString += neighbours[neighbours.Count - 1].ToString();

                return neighboursString;
            }
            public static bool IsEmptyWaypoints()
            {
                for (int i = 0; i < waypoints.Count; i++)
                {
                    if (waypoints[i].neighbours.Count == 0)
                    {
                        return true;
                    }
                }

                return false;
            }


            public static List<int> DeleteEmptyWaypoints()
            {
                List<int> resultIDs = new List<int>();

                for (int i = 0; i < waypoints.Count; i++)
                {
                    if (waypoints[i].neighbours.Count == 0)
                    {
                        resultIDs.Add(waypoints[i].id);

                        waypoints.RemoveAt(i);

                        i--;
                    }
                }

                return resultIDs;
            }

            public static Edge GetEdge(int idEdge)
            {
                return edges.First(x => x.id == idEdge);
            }

            public void SetPosition(Vector3Df aPosition)
            {
                position = aPosition;

                foreach (int edgeID in GetEdgesIDAroundWaypoint())
                {
                    Edge edge = edges.First(x => x.id == edgeID);

                    if (edge.startId == id)
                        edge.start = position;

                    if (edge.endId == id)
                        edge.end = position;
                }
                
            }
            
//             public bool SetNeighbours(int idWaypoint, List<int> neighbours)
//             {
//                 foreach (Waypoint waypoint in waypoints)
//                 {
//                     if (waypoint.id == idWaypoint)
//                     {
//                         waypoint.neighbours = neighbours;
// 
//                         return true;
//                     }
//                 }
// 
//                 return false;
//             }

            public Waypoint()
            {
                

                
            }

            public static void Reset()
            {
                waypoints = new List<Waypoint>();
                edges = new List<Edge>();
            }

            public static void ResetEdges()
            {
                foreach (Waypoint waypoint in waypoints)
                {
                    waypoint.neighbours = new List<int>();
                }

                edges = new List<Edge>();
            }

            public static Waypoint CreateWaypoint(Vector3Df position)
            {
                Waypoint waypoint = new Waypoint();

                waypoint.id = GetMinIDWaypoint();
                waypoint.position = position;
                waypoint.neighbours = new List<int>();
                
                waypoints.Add(waypoint);

                return waypoint;
            }

            public static Waypoint CreateWaypoint(int id, Vector3Df position, List<int> neighbours)
            {
                Waypoint waypoint = new Waypoint();

                waypoint.id = id;
                waypoint.position = position;
                waypoint.neighbours = neighbours;

                waypoints.Add(waypoint);

                return waypoint;
            }

            public static Edge CreateEdge(int idWaypoint1, int idWaypoint2)
            {
                if (idWaypoint1 == idWaypoint2)
                    return null;

                Waypoint waypoint1 = GetWaypoint(idWaypoint1);
                Waypoint waypoint2 = GetWaypoint(idWaypoint2);

                if(waypoint1.neighbours.Contains(idWaypoint2) || 
                    waypoint2.neighbours.Contains(idWaypoint1))
                    return null;
                
                waypoint1.neighbours.Add(idWaypoint2);
                waypoint2.neighbours.Add(idWaypoint1);

                Edge edge = new Edge(waypoint1.position, waypoint2.position, idWaypoint1, idWaypoint2, GetMinIDEdge());
                
                edges.Add(edge);

                return edge;
            }

            public static int GetMinIDWaypoint()
            {
                int minID = waypointMinID;

                foreach (Waypoint waypoint in waypoints)
                {
                    if (waypoint.id == minID)
                        minID++;
                }

                return minID;
            }

            public static int GetMinIDEdge()
            {
                int minID = edgeMinID;

                foreach (Edge edge in edges)
                {
                    if (edge.id == minID)
                        minID++;
                }

                return minID;
            }


            public static Waypoint GetWaypoint(int id)
            {
                foreach (Waypoint waypoint in waypoints)
                {
                    if (waypoint.id == id)
                        return waypoint;
                }

                return null;
            }
            
            public static List<int> GetEdgesIDAroundWaypoint(int idWaypoint)
            {
                List<int> listResult = new List<int>();

                foreach (Edge edge in edges)
                {
                    if (edge.startId == idWaypoint || edge.endId == idWaypoint)
                    {
                        listResult.Add(edge.id);
                    }
                }

                return listResult;
            }

            public List<int> GetEdgesIDAroundWaypoint()
            {
                List<int> listResult = new List<int>();

                foreach (Edge edge in edges)
                {
                    if (edge.startId == id || edge.endId == id)
                    {
                        listResult.Add(edge.id);
                    }
                }

                return listResult;
            }
            public static bool DeleteEdgeByID(int idEdge)
            {
                int index = -1;

                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].id == idEdge)
                    {
                        index = i;
                        

                        break;
                    }
                }

                Waypoint waypointStart = waypoints.First(x => x.id ==edges[index].startId);
                Waypoint waypointEnd = waypoints.First(x => x.id ==edges[index].endId);
                
                for(int i = 0; i< waypointStart.neighbours.Count;i++)
                    if(waypointStart.neighbours[i]==waypointEnd.id)
                    {
                        waypointStart.neighbours.RemoveAt(i);

                        break;
                    }

                for (int i = 0; i < waypointEnd.neighbours.Count; i++)
                    if (waypointEnd.neighbours[i] == waypointStart.id)
                    {
                        waypointEnd.neighbours.RemoveAt(i);

                        break;
                    }

                edges.RemoveAt(index);

                return false;
            }
            public static bool DeleteWaypointByID(int id)
            {
                bool find = false;

                for (int i = 0; i < waypoints.Count; i++ )
                {
                    if (waypoints[i].id == id)
                    {
                        waypoints.RemoveAt(i);
                        find = true;
                        
                    }
                }

                if (find)
                {
                    foreach (Waypoint waypoint in waypoints)
                    {
                        for (int i = 0; i < waypoint.neighbours.Count; i++ )
                        {
                            if (waypoint.neighbours[i] == id)
                            {
                                waypoint.neighbours.RemoveAt(i);

                                break;
                            }
                        }
                    }

                    for (int i = 0; i < edges.Count;i++ )
                    {
                        if (edges[i].startId == id || edges[i].endId == id)
                        {
                            edges.RemoveAt(i);

                            i--;
                        }
                    }
                }

                return find;
            }

            public static void CalculateEdges()
            {
                foreach (Waypoint waypoint in waypoints)
                {
                    foreach (int idTo in waypoint.neighbours)
                    {
                        bool edgeExist = false;

                        foreach (Edge edge in edges)
                        {
                            if ((edge.startId == waypoint.id && edge.endId == idTo) ||
                                (edge.startId == idTo && edge.endId == waypoint.id))
                            {
                                edgeExist = true;
                                break;
                            }
                        }

                        if (!edgeExist)
                            edges.Add(new Edge(waypoint.position, GetWaypoint(idTo).position, waypoint.id, idTo,GetMinIDEdge()));
                    }
                }
            }
        }
            
        public enum SkyType
        {
            Dome,
            Box
        }

        public static class UtilityProperty
        {
            public static bool getColourFrom(string readBuffer, out ColorProperty col)
            {
                col = new ColorProperty();

                try
                {
                    

                    string[] aStr = readBuffer.Split(new char[] { ',' });

                    if (aStr.Length == 4)
                    {
                        col.Red = byte.Parse(aStr[0]);
                        col.Green = byte.Parse(aStr[1]);
                        col.Blue = byte.Parse(aStr[2]);
                        col.Alpha = byte.Parse(aStr[3]);
                    }
                    else
                        return false;

                    return true;
                }
                catch (System.Exception ex)
                {

                    return false;
                }

            }



            public static bool getVector3dfFrom(string sBuffer, out Vector3DfProperty vec)
            {
                vec = new Vector3DfProperty();

                try
                {
                    

                    string[] aStr = sBuffer.Split(new char[] { ',' });
                    if (aStr.Length == 3)
                    {
                        aStr[0] = aStr[0].Replace('.', ',');
                        aStr[1] = aStr[1].Replace('.', ',');
                        aStr[2] = aStr[2].Replace('.', ',');

                        vec.X = float.Parse(aStr[0]);
                        vec.Y = float.Parse(aStr[1]);
                        vec.Z = float.Parse(aStr[2]);
                    }
                    else
                        return false;

                    return true;
                }
                catch (System.Exception ex)
                {
                    
                    return false;
                }

            }

            public static bool getDimension2DfFrom(string sBuffer, out Dimension2DfProperty dim)
            {
                dim = new Dimension2DfProperty(); 
                
                try
                {


                    string[] aStr = sBuffer.Split(new char[] { ',' });

                    if (aStr.Length == 2)
                    {
                        aStr[0] = aStr[0].Replace('.', ',');
                        aStr[1] = aStr[1].Replace('.', ',');


                        dim.Width = float.Parse(aStr[0]);
                        dim.Height = float.Parse(aStr[1]);
                    }
                    else
                        return false;

                    return true;
                }
                catch (System.Exception ex)
                {
                    return false;
                }

            }



        }

        public class BooleanTypeConverter : BooleanConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context,
              CultureInfo culture,
              object value,
              Type destType)
            {
                return (bool)value ?
                  "Да" : "Нет";
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
              CultureInfo culture,
              object value)
            {
                return (string)value == "Да";
            }
        }

        public class PropertySorter : ExpandableObjectConverter
        {
            #region Методы

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            /// <summary>
            /// Возвращает упорядоченный список свойств
            /// </summary>
            public override PropertyDescriptorCollection GetProperties(
              ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                PropertyDescriptorCollection pdc =
                  TypeDescriptor.GetProperties(value, attributes);

                ArrayList orderedProperties = new ArrayList();

                foreach (PropertyDescriptor pd in pdc)
                {
                    Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];

                    if (attribute != null)
                    {
                        // атрибут есть - используем номер п/п из него
                        PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                        orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                    }
                    else
                    {
                        // атрибута нет – считаем, что 0
                        orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                    }
                }

                // сортируем по Order-у
                orderedProperties.Sort();

                // формируем список имен свойств
                ArrayList propertyNames = new ArrayList();

                foreach (PropertyOrderPair pop in orderedProperties)
                    propertyNames.Add(pop.Name);

                // возвращаем
                return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
            }

            #endregion
        }

        #region PropertyOrder Attribute

        /// <summary>
        /// Атрибут для задания сортировки
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class PropertyOrderAttribute : Attribute
        {
            private int _order;
            public PropertyOrderAttribute(int order)
            {
                _order = order;
            }

            public int Order
            {
                get { return _order; }
            }
        }

        #endregion

        #region PropertyOrderPair

        /// <summary>
        /// Пара имя/номер п/п с сортировкой по номеру
        /// </summary>
        public class PropertyOrderPair : IComparable
        {
            private int _order;
            private string _name;

            public string Name
            {
                get { return _name; }
            }

            public PropertyOrderPair(string name, int order)
            {
                _order = order;
                _name = name;
            }

            /// <summary>
            /// Собственно метод сравнения
            /// </summary>
            public int CompareTo(object obj)
            {
                int otherOrder = ((PropertyOrderPair)obj)._order;

                if (otherOrder == _order)
                {
                    // если Order одинаковый - сортируем по именам
                    string otherName = ((PropertyOrderPair)obj)._name;
                    return string.Compare(_name, otherName);
                }
                else if (otherOrder > _order)
                    return -1;

                return 1;
            }
        }

        #endregion

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ColorProperty
        {
            [DisplayName("Красный")]
            [Description("Количество красного цвета")]
            [PropertyOrder(10)]
            public byte Red { get; set; }

            [DisplayName("Зелёный")]
            [Description("Количество зелёного цвета")]
            [PropertyOrder(20)]
            public byte Green { get; set; }
            
            [DisplayName("Синий")]
            [Description("Количество синого цвета")]
            [PropertyOrder(30)]
            public byte Blue { get; set; }

            [DisplayName("Прозрачность")]
            [Description("Количество прозрачности цвета")]
            [PropertyOrder(40)]
            public byte Alpha { get; set; }

            public override string ToString()
            {
                return (Red + " " + Green + " " + Blue+" "+Alpha).Replace(',','.').Replace(' ',',');
            }

            public ColorProperty Copy()
            {
                return (ColorProperty)this.MemberwiseClone();
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Vector3DfProperty
        {
            [DisplayName("X")]
            [Description("Координата Х.")]
            [PropertyOrder(10)]
            public float X { get; set; }

            [DisplayName("Y")]
            [Description("Координата Y.")]
            [PropertyOrder(20)]
            public float Y { get; set; }

            [DisplayName("Z")]
            [Description("Координата Z.")]
            [PropertyOrder(30)]
            public float Z { get; set; }

            public override string ToString()
            {
                return (X + " " + Y + " " + Z).Replace(',','.').Replace(' ',',');
            }

            public Vector3Df ToVector3Df()
            {
                return new Vector3Df(X,Y,Z);
            }

            public Vector3DfProperty Copy()
            {
                return (Vector3DfProperty)this.MemberwiseClone();
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class AnimationSpeedProperty
        {
            [DisplayName("Падение")]
            [Description("Скорость анимации падения в смерти.")]
            [PropertyOrder(10)]
            public float Death { get; set; }

            [DisplayName("Смерть")]
            [Description("Скорость анимации смерти.")]
            [PropertyOrder(20)]
            public float Boom { get; set; }

            [DisplayName("Атака в приседе")]
            [Description("Скорость анимации атаки в приседе.")]
            [PropertyOrder(30)]
            public float CrouchAttack { get; set; }
                
            [DisplayName("Атака")]
            [Description("Скорость анимации атаки стоя.")]
            [PropertyOrder(40)]
            public float Attack { get; set; }

            [DisplayName("Прыжок")]
            [Description("Скорость анимации прыжка.")]
            [PropertyOrder(50)]
            public float Jump { get; set; }

            [DisplayName("Присед")]
            [Description("Скорость анимации в приседе.")]
            [PropertyOrder(60)]
            public float CrouchWalk { get; set; }

            [DisplayName("Бег")]
            [Description("Скорость анимации бега.")]
            [PropertyOrder(70)]
            public float Run { get; set; }

            [DisplayName("Присед")]
            [Description("Скорость анимации стоя в приседе.")]
            [PropertyOrder(80)]
            public float CrouchStand { get; set; }

            [DisplayName("Стоя")]
            [Description("Скорость анимации стоя.")]
            [PropertyOrder(90)]
            public float Stand { get; set; }

            
            public override string ToString()
            {
                return Death + ", " + Boom + ", " + CrouchAttack + ", " + Attack +
                    ", " + Jump + ", " + CrouchWalk + "," + Run + ", " + CrouchStand + ", " + Stand;
            }

            public AnimationSpeedProperty Copy()
            {
                return (AnimationSpeedProperty)this.MemberwiseClone();
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ShotNodeProperty
        {
            [DisplayName("Масштаб")]
            [Description("Масштаб по координатам.")]
            [PropertyOrder(10)]
            public Vector3DfProperty Scale { get; set; }

            [DisplayName("Позиция")]
            [Description("Положение в пространстве.")]
            [PropertyOrder(20)]
            public Vector3DfProperty Position { get; set; }

            [DisplayName("Поворот")]
            [Description("Поворот в пространстве.")]
            [PropertyOrder(30)]
            public Vector3DfProperty Rotation { get; set; }

            [DisplayName("Модель")]
            [Description("Путь к модели.")]
            [PropertyOrder(40)]
            [Editor(typeof(AllFileNameEditor), typeof(UITypeEditor))]
            public string ModelPath { get; set; }

            [DisplayName("Текстура")]
            [Description("Путь к текстуре.")]
            [PropertyOrder(50)]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string TexturePath { get; set; }

            public ShotNodeProperty()
            {
                Scale = new Vector3DfProperty();
                Position = new Vector3DfProperty();
                Rotation = new Vector3DfProperty();
            }

            public override string ToString()
            {
                if (ModelPath != null && TexturePath != null)
                    return ModelPath + " " + TexturePath;
                else
                    return "";
            }

            public ShotNodeProperty Copy()
            {
                ShotNodeProperty returnShotNodeProperty = (ShotNodeProperty)this.MemberwiseClone();

                returnShotNodeProperty.Scale = this.Scale.Copy();
                returnShotNodeProperty.Position = this.Position.Copy();
                returnShotNodeProperty.Rotation = this.Rotation.Copy();

                return returnShotNodeProperty;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Dimension2DfProperty
        {
            [DisplayName("Ширина")]
            [Description("Ширина объекта.")]
            [PropertyOrder(10)]
            public float Width { get; set; }

            [DisplayName("Высота")]
            [Description("Высота объекта.")]
            [PropertyOrder(20)]
            public float Height { get; set; }

            public override string ToString()
            {
                return (Width + " " + Height).Replace(',', '.').Replace(' ', ','); ;
            }

            public Dimension2DfProperty Copy()
            {
                return (Dimension2DfProperty)this.MemberwiseClone();
            }
        }


        public class JpgPngFileNamesEditor : UITypeEditor
        {
            private OpenFileDialog ofd;

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                        provider.GetService(typeof(IWindowsFormsEditorService));

                    if (editorService != null)
                    {
                        ofd = new OpenFileDialog();
                        ofd.Multiselect = true;
                        ofd.Filter = "Png files (*.png)|*.png|Jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                        ofd.FileName = "";
                        ofd.Title = "Выберите изображения";

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            return ofd.FileNames;
                        }
                    }
                }
                return base.EditValue(context, provider, value);
            }
        }

        public class JpgPngFileNameEditor : UITypeEditor
        {
            private OpenFileDialog ofd;

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                        provider.GetService(typeof(IWindowsFormsEditorService));

                    if (editorService != null)
                    {
                        ofd = new OpenFileDialog();
                        //ofd.Multiselect = true;
                        ofd.Filter = "Png files (*.png)|*.png|Jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                        ofd.FileName = "";
                        ofd.Title = "Выберите изображение";

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            return ofd.FileName;
                        }
                    }
                }
                return base.EditValue(context, provider, value);
            }
        }

        public class AllFileNameEditor : UITypeEditor
        {
            private OpenFileDialog ofd;

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                        provider.GetService(typeof(IWindowsFormsEditorService));

                    if (editorService != null)
                    {
                        ofd = new OpenFileDialog();
                        //ofd.Multiselect = true;
                        ofd.Filter = "All files (*.*)|*.*";
                        ofd.FileName = "";
                        ofd.Title = "Выберите файл";

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            return ofd.FileName;
                        }
                    }
                }
                return base.EditValue(context, provider, value);
            }
        }

        public class AllFileNamesEditor : UITypeEditor
        {
            private OpenFileDialog ofd;

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                        provider.GetService(typeof(IWindowsFormsEditorService));

                    if (editorService != null)
                    {
                        ofd = new OpenFileDialog();
                        ofd.Multiselect = true;
                        ofd.Filter = "All files (*.*)|*.*";
                        ofd.FileName = "";
                        ofd.Title = "Выберите файлы";

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            return ofd.FileNames;
                        }
                    }
                }
                return base.EditValue(context, provider, value);
            }
        }

        class CollectionTypeConverter : TypeConverter
        {
            /// <summary>
            /// Только в строку
            /// </summary>
            public override bool CanConvertTo(
              ITypeDescriptorContext context, Type destType)
            {
                return destType == typeof(string);
            }

            /// <summary>
            /// И только так
            /// </summary>
            public override object ConvertTo(
              ITypeDescriptorContext context, CultureInfo culture,
              object value, Type destType)
            {


                if (value != null)
                {
                    string[] strArr = ((IEnumerable)value).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

                    if (strArr.Length > 0)
                        return string.Join(" ", strArr);
                    else
                        return "<Список>";
                }
                else
                    return "<Список>";
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class BillBoardProperty
        {
            [DisplayName("Период")]
            [Description("Период смены текстуры.")]
            [PropertyOrder(10)]
            public float TextureTimePerFrame { get; set; }

            [DisplayName("Размеры")]
            [Description("Физические размеры текстуры.")]
            [PropertyOrder(20)]
            public Dimension2DfProperty Dimension { get; set; }


            [DisplayName("Текстуры")]
            [Description("Пути к текстурам.")]
            [PropertyOrder(30)]
            [Editor(typeof(JpgPngFileNamesEditor), typeof(UITypeEditor))]
            [TypeConverter(typeof(CollectionTypeConverter))]
            public string[] TexturePaths { get; set;}

            public BillBoardProperty()
            {
                Dimension = new Dimension2DfProperty();
            }

            public override string ToString()
            {
                return "";
            }

            public BillBoardProperty Copy()
            {
                BillBoardProperty returnBillBoardProperty = (BillBoardProperty)this.MemberwiseClone();

                returnBillBoardProperty.Dimension = this.Dimension.Copy();
                string[] texturePaths = new string[this.TexturePaths.Length];
                Array.Copy(this.TexturePaths,texturePaths,texturePaths.Length);
                returnBillBoardProperty.TexturePaths = texturePaths;

                return returnBillBoardProperty;
            }
        }


        class DriverTypeConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(
              ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(
              ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(
              ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(driverTypeList);
            }
        }

        class SkyTypeConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(
              ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(
              ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(
              ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(skyTypeListRus);
            }
        }




        /// <summary>
        /// Атрибут для поддержки динамически показываемых свойств
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = true)]
        class DynamicPropertyFilterAttribute : Attribute
        {
            string _propertyName;

            /// <summary>
            /// Название свойства, от которого будет зависить видимость
            /// </summary>
            public string PropertyName
            {
                get { return _propertyName; }
            }

            string _showOn;

            /// <summary>
            /// Значения свойства, от которого зависит видимость 
            /// (через запятую, если несколько), при котором свойство, к
            /// которому применен атрибут, будет видимо. 
            /// </summary>
            public string ShowOn
            {
                get { return _showOn; }
            }

            /// <summary>
            /// Конструктор  
            /// </summary>
            /// <param name="propName">Название свойства, от которого будет 
            /// зависеть видимость</param>
            /// <param name="value">Значения свойства (через запятую, если несколько), 
            /// при котором свойство, к которому применен атрибут, будет видимо.</param>
            public DynamicPropertyFilterAttribute(string propertyName, string value)
            {
                _propertyName = propertyName;
                _showOn = value;
            }
        }

        /// <summary>
        /// Базовый класс для объектов, поддерживающих динамическое 
        /// отображение свойств в PropertyGrid
        /// </summary>
        public class FilterablePropertyBase : ICustomTypeDescriptor
        {

            protected PropertyDescriptorCollection
              GetFilteredProperties(Attribute[] attributes)
            {
                PropertyDescriptorCollection pdc
                  = TypeDescriptor.GetProperties(this, attributes, true);

                PropertyDescriptorCollection finalProps =
                  new PropertyDescriptorCollection(new PropertyDescriptor[0]);

                foreach (PropertyDescriptor pd in pdc)
                {
                    bool include = false;
                    bool dynamic = false;

                    foreach (Attribute a in pd.Attributes)
                    {
                        if (a is DynamicPropertyFilterAttribute)
                        {
                            dynamic = true;

                            DynamicPropertyFilterAttribute dpf =
                             (DynamicPropertyFilterAttribute)a;

                            PropertyDescriptor temp = pdc[dpf.PropertyName];
                            
                            object obj = temp.GetValue(this);
                            
                            if(obj!=null)
                                if (dpf.ShowOn.IndexOf(obj.ToString()) > -1)
                                    include = true;
                        }
                    }

                    if (!dynamic || include)
                        finalProps.Add(pd);
                }

                return finalProps;
            }

            #region ICustomTypeDescriptor Members

            public TypeConverter GetConverter()
            {
                return TypeDescriptor.GetConverter(this, true);
            }

            public EventDescriptorCollection GetEvents(Attribute[] attributes)
            {
                return TypeDescriptor.GetEvents(this, attributes, true);
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return TypeDescriptor.GetEvents(this, true);
            }

            public string GetComponentName()
            {
                return TypeDescriptor.GetComponentName(this, true);
            }

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                return this;
            }

            public AttributeCollection GetAttributes()
            {
                return TypeDescriptor.GetAttributes(this, true);
            }

            public PropertyDescriptorCollection GetProperties(
              Attribute[] attributes)
            {
                return GetFilteredProperties(attributes);
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return GetFilteredProperties(new Attribute[0]);
            }

            public object GetEditor(Type editorBaseType)
            {
                return TypeDescriptor.GetEditor(this, editorBaseType, true);
            }

            public PropertyDescriptor GetDefaultProperty()
            {
                return TypeDescriptor.GetDefaultProperty(this, true);
            }

            public EventDescriptor GetDefaultEvent()
            {
                return TypeDescriptor.GetDefaultEvent(this, true);
            }

            public string GetClassName()
            {
                return TypeDescriptor.GetClassName(this, true);
            }

            #endregion
        }

        [TypeConverter(typeof(PropertySorter))]
        public class Parameters : FilterablePropertyBase
        {
            public IntPtr WindowID;

//             [DisplayName("Путь установки")]
//             [Description("Путь, по которому будет установлена игра.")]
//             [Category("01. Игра")]
//             [PropertyOrder(10)]
//             public string InstallPath { get; set; }

            [DisplayName("Название")]
            [Description("Название игры.")]
            [Category("01. Игра")]
            [PropertyOrder(20)]
            public string Name { get; set; }


            [DisplayName("От первого лица")]
            [Description("Да - игра от первого лица, нет - от третьего.")]
            [TypeConverter(typeof(BooleanTypeConverter))]
            [Category("01. Игра")]
            [PropertyOrder(30)]
            public bool FPS { get; set; }

            [DisplayName("Ширина")]
            [Description("Ширина окна.")]
            [Category("02. Окно")]
            [PropertyOrder(10)]
            public int Width { get; set; }

            [DisplayName("Высота")]
            [Description("Высота окна.")]
            [Category("02. Окно")]
            [PropertyOrder(20)]
            public int Height { get; set; }

            [DisplayName("Бит на пиксель")]
            [Description("Количество бит на пиксель (количество цветов).")]
            [Category("02. Окно")]
            [PropertyOrder(30)]
            public byte BitsPerPixel { get; set; }

            [DisplayName("Изображение победы")]
            [Description("Путь к изображению победы. При победе изображение выведется на экран.")]
            [Category("03. Интерфейс")]
            [PropertyOrder(10)]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string WinImagePath { get; set; }

            [DisplayName("Изображение проигрыша")]
            [Description("Путь к изображению проигрыша. При поражении изображение выведется на экран.")]
            [Category("03. Интерфейс")]
            [PropertyOrder(20)]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string LoseImagePath { get; set; }

            [DisplayName("Шрифт цифр")]
            [Description("Путь к шрифту цифр. Показывают количество здоровья и патронов.")]
            [Category("03. Интерфейс")]
            [PropertyOrder(30)]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string FontDigitPath { get; set; }

            [DisplayName("Цвет здоровья")]
            [Description("Цвет цифр, которые показывают количество здоровья.")]
            [Category("03. Интерфейс")]
            [PropertyOrder(40)]
            public ColorProperty ColorHealth { get; set; }

            [DisplayName("Цвет патронов")]
            [Description("Цвет цифр, которые показывают количество патронов.")]
            [Category("03. Интерфейс")]
            [PropertyOrder(50)]
            public ColorProperty ColorProjectiles { get; set; }

            [DisplayName("Драйвер")]
            [Description("Тип драйвера.")]
            [Category("04. Графика")]
            [TypeConverter(typeof(DriverTypeConverter))]
            [PropertyOrder(10)]
            public string _DriverType { get; set; }

            public DriverType DriverTypeGet()
            {
                return _DriverType == "DirectX" ?
                    DriverType.Direct3D9 : DriverType.OpenGL;
            }

            [DisplayName("Полный экран")]
            [Description("Да - на полный экран, нет - в окне.")]
            [Category("04. Графика")]
            [PropertyOrder(20)]
            [TypeConverter(typeof(BooleanTypeConverter))]
            public bool FullScreen { get; set; }

            [DisplayName("Сглаживание")]
            [Description("Уровень сглаживания.")]
            [Category("04. Графика")]
            [PropertyOrder(30)]
            public byte Antialiasing { get; set; }

            [DisplayName("Вертикальная синхронизация")]
            [Description("Синхронизация кадровой частоты с частотой вертикальной развёртки монитора.")]
            [Category("04. Графика")]
            [PropertyOrder(40)]
            [TypeConverter(typeof(BooleanTypeConverter))]
            public bool VSync { get; set; }

            [DisplayName("Небо")]
            [Description("Тип неба.")]
            [Category("05. Небо")]
            [TypeConverter(typeof(SkyTypeConverter))]
            [PropertyOrder(10)]
            public string SkyType { get; set; }

            

            [DisplayName("Верх")]
            [Description("Путь к изображению верхней части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(20)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string TopTexturePath { get; set; }

            [DisplayName("Низ")]
            [Description("Путь к изображению нижней части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(30)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string BottomTexturePath { get; set; }

            [DisplayName("Лево")]
            [Description("Путь к изображению левой части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(40)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string LeftTexturePath { get; set; }

            [DisplayName("Право")]
            [Description("Путь к изображению правой части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(50)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string RightTexturePath { get; set; }

            [DisplayName("Фронт")]
            [Description("Путь к изображению фронтальной части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(60)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string FrontTexturePath { get; set; }

            [DisplayName("Тыл")]
            [Description("Путь к изображению тыльной части коробки неба.")]
            [Category("05. Небо")]
            [PropertyOrder(70)]
            [DynamicPropertyFilter("SkyType", "Коробка")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string BackTexturePath { get; set; }

            [DisplayName("Изображение")]
            [Description("Путь к изображению купола неба.")]
            [Category("05. Небо")]
            [PropertyOrder(20)]
            [DynamicPropertyFilter("SkyType", "Купол")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string TexturePathSky { get; set; }

            [DisplayName("Горизонтальные грани")]
            [Description("Количество горизонтальных граней купола неба.")]
            [Category("05. Небо")]
            [PropertyOrder(30)]
            [DynamicPropertyFilter("SkyType", "Купол")]
            public int HoriRes { get; set; }
            
            [DisplayName("Вертикальные грани")]
            [Description("Количество вертикальных граней купола неба.")]
            [Category("05. Небо")]
            [PropertyOrder(40)]
            [DynamicPropertyFilter("SkyType", "Купол")]
            public int VertRes { get; set; }

            [DisplayName("Процент текстуры")]
            [Description("Процент текстуры купола (0..1).")]
            [Category("05. Небо")]
            [PropertyOrder(50)]
            [DynamicPropertyFilter("SkyType", "Купол")]
            public float TexturePercentage { get; set; }

            [DisplayName("Процент купола")]
            [Description("Процент размера купола (0..2).")]
            [Category("05. Небо")]
            [PropertyOrder(60)]
            [DynamicPropertyFilter("SkyType", "Купол")]
            public float SpherePercentage { get; set; }

            [DisplayName("Карта")]
            [Description("Путь к карте игры.")]
            [Category("06. Карта")]
            [PropertyOrder(10)]
            [Editor(typeof(AllFileNameEditor), typeof(UITypeEditor))]
            public string MapPath { get; set; }

            [DisplayName("Объект")]
            [Description("Минимальное количество полигонов на каждый объект карты.")]
            [Category("06. Карта")]
            [PropertyOrder(20)]
            public int MinimalPolysPerNode { get; set; }

            [DisplayName("ИИ файл")]
            [Description("ИИ пути из файла или из программы конструктора.")]
            [Category("06. Карта")]
            [TypeConverter(typeof(BooleanTypeConverter))]
            [PropertyOrder(25)]
            public bool MapAI { get; set; }

            [DisplayName("Карта ИИ")]
            [Description("Путь к карте искуственного интеллекта игры.")]
            [Category("06. Карта")]
            [PropertyOrder(30)]
            [DynamicPropertyFilter("MapAI", "True")]
            [Editor(typeof(AllFileNameEditor), typeof(UITypeEditor))]
            public string MapAIPath { get; set; }

            [DisplayName("Дополнительные медиа-файлы")]
            [Description("Есть ли у карты дополнительные медиа-файлы.")]
            [Category("06. Карта")]
            [TypeConverter(typeof(BooleanTypeConverter))]
            [PropertyOrder(35)]
            public bool MapExtraFile { get; set; }

            [DisplayName("Доп. файлы")]
            [Description("Дополнительные медиа-файлы, относящиеся к карте.")]
            [Category("06. Карта")]
            [PropertyOrder(40)]
            [Editor(typeof(AllFileNamesEditor), typeof(UITypeEditor))]
            [DynamicPropertyFilter("MapExtraFile", "True")]
            [TypeConverter(typeof(CollectionTypeConverter))]
            public string[] MapExtraFiles { get; set; }

            [DisplayName("Здоровье")]
            [Description("Максимальное количество здоровья.")]
            [Category("07. Персонаж")]
            [PropertyOrder(10)]
            public int MaxHealth { get; set; }

            [DisplayName("Патроны")]
            [Description("Максимальное количество патронов.")]
            [Category("07. Персонаж")]
            [PropertyOrder(20)]
            public int MaxAmmo { get; set; }

            [DisplayName("Стрельба")]
            [Description("Задержка скорости стрельбы.")]
            [Category("07. Персонаж")]
            [PropertyOrder(30)]
            public uint TimeShotDelay { get; set; }

            [DisplayName("Падение")]
            [Description("Время падения персонажа при смерти.")]
            [Category("07. Персонаж")]
            [PropertyOrder(40)]
            public uint TimeFallToDeath { get; set; }

            [DisplayName("Реген здоровья")]
            [Description("Включение регенерации здоровья.")]
            [Category("07. Персонаж")]
            [PropertyOrder(50)]
            [TypeConverter(typeof(BooleanTypeConverter))]
            public bool RegenerateHealth { get; set; }

            [DisplayName("Период здоровья")]
            [Description("Период регенерации здоровья.")]
            [Category("07. Персонаж")]
            [PropertyOrder(60)]
            public int RefillPeriodHealth { get; set; }

            [DisplayName("Реген патронов")]
            [Description("Включение регенарации патронов.")]
            [Category("07. Персонаж")]
            [PropertyOrder(70)]
            [TypeConverter(typeof(BooleanTypeConverter))]
            public bool RegenerateAmmo { get; set; }

            [DisplayName("Период патронов")]
            [Description("Период регенарации патронов.")]
            [Category("07. Персонаж")]
            [PropertyOrder(80)]
            public int RefillPeriodAmmo { get; set; }

            [DisplayName("Урон")]
            [Description("Урон от патронов.")]
            [Category("07. Персонаж")]
            [PropertyOrder(90)]
            public int DrawnHealth { get; set; }

            [DisplayName("Сдвиг стоя")]
            [Description("Сдвиг персонажа стоя.")]
            [Category("07. Персонаж")]
            [PropertyOrder(100)]
            public Vector3DfProperty OffsetStand { get; set; }

            [DisplayName("Сдвиг присед")]
            [Description("Сдвиг персонажа в приседе.")]
            [Category("07. Персонаж")]
            [PropertyOrder(110)]
            public Vector3DfProperty OffsetCrouch { get; set; }

            [DisplayName("Анимация")]
            [Description("Скорость анимации.")]
            [Category("07. Персонаж")]
            [PropertyOrder(120)]
            public AnimationSpeedProperty animationSpeed { get; set; }


            [DisplayName("Боты")]
            [Description("Количество ботов.")]
            [PropertyOrder(10)]
            [Category("08. Бот")]
            public int NumBots { get; set; }

            [DisplayName("Прыжок")]
            [Description("Время прыжка.")]
            [PropertyOrder(20)]
            [Category("08. Бот")]
            public uint TimeJump { get; set; }

            [DisplayName("Присед")]
            [Description("Время приседа.")]
            [PropertyOrder(30)]
            [Category("08. Бот")]
            public uint TimeCrouching { get; set; }

            [DisplayName("Радиус")]
            [Description("Радиус обзора.")]
            [PropertyOrder(40)]
            [Category("08. Бот")]
            public float Range { get; set; }

            [DisplayName("Обзор")]
            [Description("Размер обзора.")]
            [PropertyOrder(50)]
            [Category("08. Бот")]
            public Vector3DfProperty FieldOfViewDimensions { get; set; }

            [DisplayName("Скорость")]
            [Description("Скорость передвижения.")]
            [PropertyOrder(60)]
            [Category("08. Бот")]
            public float MoveSpeedBot { get; set; }

            [DisplayName("Порог")]
            [Description("Порог приближения к точке передвижения.")]
            [PropertyOrder(70)]
            [Category("08. Бот")]
            public float AtDestinationThreshold { get; set; }

            [DisplayName("Сила прыжка")]
            [Description("Сила вектора ускорения прыжка.")]
            [PropertyOrder(80)]
            [Category("08. Бот")]
            public float JumpUpFactor { get; set; }

            [DisplayName("Сила прыжка в сторону")]
            [Description("Сила вектора ускорения прыжка в сторону.")]
            [PropertyOrder(90)]
            [Category("08. Бот")]
            public float JumpAroundFactor { get; set; }

            [DisplayName("Модель")]
            [Description("Путь к модели.")]
            [PropertyOrder(100)]
            [Category("08. Бот")]
            [Editor(typeof(AllFileNameEditor), typeof(UITypeEditor))]
            public string ModelPathBot { get; set; }

            [DisplayName("Текстура")]
            [Description("Путь к текстуре.")]
            [PropertyOrder(110)]
            [Category("08. Бот")]
            [Editor(typeof(JpgPngFileNameEditor), typeof(UITypeEditor))]
            public string TexturePathBot { get; set; }

            [DisplayName("Скорость")]
            [Description("Скорость игрока.")]
            [PropertyOrder(10)]
            [Category("09. Игрок")]
            public float MoveSpeedPlayer { get; set; }

            [DisplayName("Поворот")]
            [Description("Скорость поворота.")]
            [PropertyOrder(20)]
            [Category("09. Игрок")]
            public float RotateSpeed { get; set; }

            [DisplayName("Прыжок")]
            [Description("Сила прыжка.")]
            [PropertyOrder(30)]
            [Category("09. Игрок")]
            public float JumpSpeed { get; set; }

            [DisplayName("Положение")]
            [Description("Положение в пространстве.")]
            [PropertyOrder(40)]
            [Category("09. Игрок")]
            public Vector3DfProperty Position { get; set; }

            [DisplayName("Прицел")]
            [Description("Прицел камеры.")]
            [PropertyOrder(50)]
            [Category("09. Игрок")]
            public Vector3DfProperty Target { get; set; }

            [DisplayName("Видимость")]
            [Description("Дальность видимости.")]
            [PropertyOrder(60)]
            [Category("09. Игрок")]
            public float FarValue { get; set; }

            [DisplayName("Масштаб")]
            [Description("Масштаб игрока.")]
            [PropertyOrder(70)]
            [Category("09. Игрок")]
            public Vector3DfProperty Scale { get; set; }

            [DisplayName("Радиус")]
            [Description("Радиус эллипсоида коллизии.")]
            [PropertyOrder(80)]
            [Category("09. Игрок")]
            public Vector3DfProperty EllipsoidRadius { get; set; }

            [DisplayName("Сдвиг")]
            [Description("Сдвиг эллипсоида коллизии.")]
            [PropertyOrder(90)]
            [Category("09. Игрок")]
            public Vector3DfProperty EllipsoidTranslation { get; set; }

            [DisplayName("Гравитация")]
            [Description("Сила гравитации.")]
            [PropertyOrder(100)]
            [Category("09. Игрок")]
            public Vector3DfProperty GravityPerSecond { get; set; }

            [DisplayName("Модель стрельбы")]
            [Description("Модель, закреплённая за камерой.")]
            [PropertyOrder(110)]
            [Category("09. Игрок")]
            public ShotNodeProperty shotNode { get; set; }

            [DisplayName("Бот стоя")]
            [Description("Начальное положение выстрела у бота стоя.")]
            [PropertyOrder(10)]
            [Category("10. Выстрел")]
            public Vector3DfProperty StartPositionBotStand { get; set; }

            [DisplayName("Бот присед")]
            [Description("Начальное положение выстрела у бота в приседе.")]
            [PropertyOrder(20)]
            [Category("10. Выстрел")]
            public Vector3DfProperty StartPositionBotCrouch { get; set; }

            [DisplayName("Игрок стоя")]
            [Description("Начальное положение выстрела у игрока стоя.")]
            [PropertyOrder(30)]
            [Category("10. Выстрел")]
            public Vector3DfProperty StartPositionPlayerStand { get; set; }

            [DisplayName("Игрок присед")]
            [Description("Начальное положение выстрела у мгрока в приседе.")]
            [PropertyOrder(40)]
            [Category("10. Выстрел")]
            public Vector3DfProperty StartPositionPlayerCrouch { get; set; }

            [DisplayName("Скорость")]
            [Description("Скорость пули.")]
            [PropertyOrder(50)]
            [Category("10. Выстрел")]
            public float Speed { get; set; }

            [DisplayName("Дистанция")]
            [Description("Максимальная дистанция полёта пули.")]
            [PropertyOrder(60)]
            [Category("10. Выстрел")]
            public float MaxDistanceTravelled { get; set; }

            [DisplayName("Модель стрельбы полёта выстрела")]
            [Description("Модель выстрела, который выпущен и находится в полёте.")]
            [PropertyOrder(70)]
            [Category("10. Выстрел")]
            public BillBoardProperty LiveBillBoard { get; set; }

            [DisplayName("Модель стрельбы попадания выстрела")]
            [Description("Модель выстрела, который попал в цель.")]
            [PropertyOrder(80)]
            [Category("10. Выстрел")]
            public BillBoardProperty DieBillBoard { get; set; }
            
            public Parameters()
            {
                ColorHealth = new ColorProperty();
                ColorProjectiles = new ColorProperty();

                OffsetStand = new Vector3DfProperty();
                OffsetCrouch = new Vector3DfProperty();

                animationSpeed = new AnimationSpeedProperty();

                FieldOfViewDimensions = new Vector3DfProperty();

                Position = new Vector3DfProperty();
                Target = new Vector3DfProperty();
                Scale = new Vector3DfProperty();
                EllipsoidRadius = new Vector3DfProperty();
                EllipsoidTranslation = new Vector3DfProperty();
                GravityPerSecond = new Vector3DfProperty();
                shotNode = new ShotNodeProperty();

                StartPositionBotStand = new Vector3DfProperty();
                StartPositionBotCrouch = new Vector3DfProperty();
                StartPositionPlayerStand = new Vector3DfProperty();
                StartPositionPlayerCrouch = new Vector3DfProperty();

                LiveBillBoard = new BillBoardProperty();
                DieBillBoard = new BillBoardProperty();
            }

            public Parameters Copy()
            {
                Parameters returnParameters = (Parameters)this.MemberwiseClone();

                returnParameters.ColorHealth = this.ColorHealth.Copy();
                returnParameters.ColorProjectiles = this.ColorProjectiles.Copy();

                returnParameters.OffsetStand = this.OffsetStand.Copy();
                returnParameters.OffsetCrouch = this.OffsetCrouch.Copy();

                returnParameters.animationSpeed = this.animationSpeed.Copy();

                returnParameters.FieldOfViewDimensions = this.FieldOfViewDimensions.Copy();

                returnParameters.Position = this.Position.Copy();
                returnParameters.Target = this.Target.Copy();
                returnParameters.Scale = this.Scale.Copy();
                returnParameters.EllipsoidRadius = this.EllipsoidRadius.Copy();
                returnParameters.EllipsoidTranslation = this.EllipsoidTranslation.Copy();
                returnParameters.GravityPerSecond = this.GravityPerSecond.Copy();
                returnParameters.shotNode = this.shotNode.Copy();

                returnParameters.StartPositionBotStand = this.StartPositionBotStand.Copy();
                returnParameters.StartPositionBotCrouch = this.StartPositionBotCrouch.Copy();
                returnParameters.StartPositionPlayerStand = this.StartPositionPlayerStand.Copy();
                returnParameters.StartPositionPlayerCrouch = this.StartPositionPlayerCrouch.Copy();

                returnParameters.LiveBillBoard = this.LiveBillBoard.Copy();
                returnParameters.DieBillBoard = this.DieBillBoard.Copy();

                return returnParameters;
            }

        }


        private void AdjustPropertyGridColumnWidth()
        {
            PropertyInfo controlsProp = propertyGrid.GetType().GetProperty("Controls");
            Control.ControlCollection cc = controlsProp.GetValue(propertyGrid, null) as Control.ControlCollection;

            foreach (Control c in cc)
            {
                if (c.GetType().Name == "PropertyGridView")
                {
                    MethodInfo mst = c.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    PropertyInfo widthProp = c.GetType().GetProperty("Width");
                    int width = (int)widthProp.GetValue(c, null);
                    mst.Invoke(c, new object[] { width *55/100 });
                    break;
                }
            }
        }



        public Form1()
        {
            InitializeComponent();

            //toolStrip.ite

        }
        


                

        private void backgroundRendering_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Parameters parameters = (Parameters)e.Argument;




            IrrlichtCreationParameters irrCrPar = new IrrlichtCreationParameters();
            irrCrPar.AntiAliasing = parameters.Antialiasing;
            irrCrPar.DriverType = parameters.DriverTypeGet();
            irrCrPar.WindowSize = new Dimension2Di(panelRenderingWindow.Width, panelRenderingWindow.Height);
            irrCrPar.BitsPerPixel = parameters.BitsPerPixel;
            //irrCrPar.Fullscreen = parameters.FullScreen;
            irrCrPar.VSync = parameters.VSync;

            irrCrPar.WindowID = parameters.WindowID;// panelRenderingWindow.Handle;



            IrrlichtDevice device = IrrlichtDevice.CreateDevice(irrCrPar);




            if (device == null)
                return;

            VideoDriver driver = device.VideoDriver;
            SceneManager smgr = device.SceneManager;

            driver.SetTextureCreationFlag(TextureCreationFlag.Always32Bit, true);



            smgr.LoadScene(parameters.MapPath);


            string[] mapPath = parameters.MapPath.Split(new char[] { '\\', '/', '.' });

            if (mapPath[mapPath.Length - 1] == "irr")
                smgr.LoadScene(parameters.MapPath);
            else
            {
                device.FileSystem.AddFileArchive(parameters.MapPath);



                AnimatedMesh mesh = smgr.GetMesh(mapPath[mapPath.Length - 2] + ".bsp");

                smgr.AddOctreeSceneNode(mesh.GetMesh(0), null, -1, parameters.MinimalPolysPerNode);
            }

            //selector
            MetaTriangleSelector metaTriangleSelector = smgr.CreateMetaTriangleSelector();

            List<SceneNode> listSceneNode = smgr.GetSceneNodesFromType(SceneNodeType.Any);
            
            foreach (SceneNode sceneNode in listSceneNode)
            {
                TriangleSelector triangleSelector = null;

                switch (sceneNode.Type)
                {
                    case SceneNodeType.Cube:
                    case SceneNodeType.AnimatedMesh:
                        triangleSelector = smgr.CreateTriangleSelectorFromBoundingBox(sceneNode);
                        break;

                    case SceneNodeType.Mesh:
                    case SceneNodeType.Sphere:
                        triangleSelector = smgr.CreateTriangleSelector(((MeshSceneNode)sceneNode).Mesh, sceneNode);
                        break;

                    case SceneNodeType.Terrain:
                        triangleSelector = smgr.CreateTerrainTriangleSelector((TerrainSceneNode)sceneNode);
                        break;

                    case SceneNodeType.Octree:
                        triangleSelector = smgr.CreateOctreeTriangleSelector(((MeshSceneNode)sceneNode).Mesh, sceneNode);
                        break;
                }

                if (triangleSelector != null)
                {
                    metaTriangleSelector.AddTriangleSelector(triangleSelector);
                    triangleSelector.Drop();
                }


            }




            CameraSceneNode camera = smgr.AddCameraSceneNode(null, cameraPosition,
              cameraPosition+cameraTargetRelative, -1, true);


            MeshSceneNode sphereTarget = smgr.AddSphereSceneNode(sphereTargetSize, sphereTargetFace, null, -1, camera.Target);
            Material sphereMaterial = sphereTarget.GetMaterial(0);
            sphereMaterial.Lighting = true;

            sphereMaterial.AmbientColor = sphereTargetColor;
            sphereMaterial.DiffuseColor = sphereTargetColor;
            sphereMaterial.EmissiveColor = sphereTargetColor;
            sphereMaterial.SpecularColor = sphereTargetColor;
            

            //sphereTarget.GetMaterial(0).AmbientColor.Set(Color.OpaqueWhite);
//             node->getMaterial(0).DiffuseColor.set(255, 255, 0, 0);
//             Texture sphereTexture = null;
//             smgr.VideoDriver.MakeColorKeyTexture(sphereTexture, new Color(255, 255, 255, 255));
//             sphereTarget.SetMaterialTexture(0, sphereTexture);

                // create skybox and skydome
            driver.SetTextureCreationFlag(TextureCreationFlag.CreateMipMaps, false);

            SceneNode skySceneNode = null;
            if (parameters.SkyType == skyTypeListRus[1])
            {
                skySceneNode = smgr.AddSkyDomeSceneNode(driver.GetTexture(parameters.TexturePathSky),
                    parameters.HoriRes, parameters.VertRes, parameters.TexturePercentage, parameters.SpherePercentage);
            }
            else
            {
                skySceneNode = smgr.AddSkyBoxSceneNode(
                        parameters.TopTexturePath, parameters.BottomTexturePath,
                        parameters.LeftTexturePath, parameters.RightTexturePath,
                        parameters.FrontTexturePath, parameters.BackTexturePath);
            }


                
            driver.SetTextureCreationFlag(TextureCreationFlag.CreateMipMaps, true);

            if(parameters.MapExtraFile)
                foreach (string filePath in parameters.MapExtraFiles)
                {
                    driver.GetTexture(filePath);
                }


            //end start game

            List<MeshSceneNode> waypointTargets = new List<MeshSceneNode>();

            List<MeshSceneNode> waypointLines = new List<MeshSceneNode>();

            List<AnimatedMeshSceneNode> CharacterNodes = new List<AnimatedMeshSceneNode>();

            AnimatedMeshSceneNode PlayerNode = null;
            MeshSceneNode playerTarget = null;

            AIGraphicsInitialize(ref smgr, ref parameters, ref waypointTargets, ref waypointLines,
               ref CharacterNodes, ref PlayerNode, ref playerTarget);

            

            

            //CharacterNodes.Add(CharacterNode);
             
             

            int lastFPS = -1;

            /*
            float size = 300;
            Waypoint[] waypointArr = new Waypoint[8];
            Vector3Df point1 = new Vector3Df(size, 2*size, size);
            waypointArr[0] = Waypoint.CreateWaypoint(point1);
            MeshSceneNode cubeTarget1 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[0].id, point1);
            waypointTargets.Add(cubeTarget1);
            Vector3Df point2 = new Vector3Df(-size, 2 * size, size);
            waypointArr[1] = Waypoint.CreateWaypoint(point2);
            MeshSceneNode cubeTarget2 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[1].id, point2);
            waypointTargets.Add(cubeTarget2);
            Vector3Df point3 = new Vector3Df(-size, 2 * size, -size);
            waypointArr[2] = Waypoint.CreateWaypoint(point3);
            MeshSceneNode cubeTarget3 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[2].id, point3);
            waypointTargets.Add(cubeTarget3);
            Vector3Df point4 = new Vector3Df(size, 2 * size, -size);
            waypointArr[3] = Waypoint.CreateWaypoint(point4);
            MeshSceneNode cubeTarget4 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[3].id, point4);
            waypointTargets.Add(cubeTarget4);
            Vector3Df point5 = new Vector3Df(size, -size, size);
            waypointArr[4] = Waypoint.CreateWaypoint(point5);
            MeshSceneNode cubeTarget5 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[4].id, point5);
            waypointTargets.Add(cubeTarget5);
            Vector3Df point6 = new Vector3Df(-size, -size, size);
            waypointArr[5] = Waypoint.CreateWaypoint(point6);
            MeshSceneNode cubeTarget6 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[5].id, point6);
            waypointTargets.Add(cubeTarget1);
            Vector3Df point7 = new Vector3Df(-size, -size, -size);
            waypointArr[6] = Waypoint.CreateWaypoint(point7);
            MeshSceneNode cubeTarget7 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[6].id, point7);
            waypointTargets.Add(cubeTarget7);
            Vector3Df point8 = new Vector3Df(size, -size, -size);
            waypointArr[7] = Waypoint.CreateWaypoint(point8);
            MeshSceneNode cubeTarget8 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr[7].id, point8);
            waypointTargets.Add(cubeTarget8);

            for (int i = 0; i < 8 - 1; i++)
                for (int j = i+1; j < 8; j++)
                {
                    Line3Df lineBetween = new Line3Df(waypointArr[i].position, waypointArr[j].position);

                    Vector3Df centerPoint = new Vector3Df((lineBetween.Start.X + lineBetween.End.X) / 2,
                        (lineBetween.Start.Y + lineBetween.End.Y) / 2, (lineBetween.Start.Z + lineBetween.End.Z) / 2);


                    Vector3Df rotateAngle = getAngleBetween3Vectors(lineBetween.Vector);

                    MeshSceneNode cube = smgr.AddCubeSceneNode(1, null, 0, centerPoint, rotateAngle,
                            new Vector3Df(lineThickness, lineBetween.Length, lineThickness));

                    waypointLines.Add(cube);
                }
            */
            /*
            Vector3Df point9 = new Vector3Df(size, size, size);
            Waypoint waypointArr1 = Waypoint.CreateWaypoint(point9);
            MeshSceneNode cubeTarget9 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr1.id, point9);
            waypointTargets.Add(cubeTarget9);
            Vector3Df point10 = new Vector3Df(size, size, -size);
            Waypoint waypointArr2 = Waypoint.CreateWaypoint(point10);
            MeshSceneNode cubeTarget10 = smgr.AddCubeSceneNode(cubeTargetSize,
                null, waypointArr2.id, point10);
            waypointTargets.Add(cubeTarget10);

                    Line3Df lineBetween1 = new Line3Df(waypointArr1.position, waypointArr2.position);

                    Vector3Df centerPoint1 = new Vector3Df((lineBetween1.Start.X + lineBetween1.End.X) / 2,
                        (lineBetween1.Start.Y + lineBetween1.End.Y) / 2, (lineBetween1.Start.Z + lineBetween1.End.Z) / 2);


                    Vector3Df rotateAngle1 = getAngleBetween3Vectors(lineBetween1.Vector);

                    MeshSceneNode cube1 = smgr.AddCubeSceneNode(1, null, 0, centerPoint1, rotateAngle1,
                            new Vector3Df(lineThickness, lineBetween1.Length, lineThickness));

                    waypointLines.Add(cube1);
                
            */
            

                    while (device.Run())
                    {

                        //                 if (device.WindowActive)
                        //                 {

                        if (aiGraphicsInitialize)
                        {
                            AIGraphicsInitialize(ref smgr, ref parameters, ref waypointTargets, ref waypointLines,
               ref CharacterNodes, ref PlayerNode, ref playerTarget);

                            aiGraphicsInitialize = false;
                        }

                        if (cameraReset)
                        {
                            camera.Position = cameraPositionStart;
                            camera.Target = cameraPositionStart + cameraTargetRelativeStart;

                            cameraPosition = cameraPositionStart;
                            cameraTargetRelative = cameraTargetRelativeStart;

                            cameraReset = false;
                        }
                        else
                        {
                            camera.Position = cameraPosition;
                            camera.Target = cameraPosition + cameraTargetRelative;

                        }

                        sphereTarget.Position = camera.Target;

                        if (deleteEmptyWaypoints)
                        {
                            DeleteEmptyWaypoints(waypointTargets);

                            ResetBots(smgr, CharacterNodes, parameters);
                        }

                        if (deleteAllWaypoints)
                        {
                            DeleteAllWaypoints(waypointTargets, waypointLines);

                            ResetBots(smgr, CharacterNodes, parameters);
                        }

                        if (deleteAllLines)
                            DeleteAllLines(waypointLines);

                        if (mouseDown && cameraControlState == CameraControlState.CreateWaypoint)
                        {
                            CreateWaypoint(waypointTargets, smgr, camera, metaTriangleSelector);

                            ResetBots(smgr, CharacterNodes, parameters);

                           
                                

                            
                        }

                        if (!mouseDown && cameraControlState == CameraControlState.CreateWaypoint)
                            createWaypointFirst = true;

                        if (mouseDown && cameraControlState == CameraControlState.MoveWaypoint)
                        {
                            MoveWaypoint(waypointTargets, waypointLines, smgr, camera, metaTriangleSelector,
                                playerTarget, PlayerNode);
                        
                            ResetBots(smgr, CharacterNodes, parameters);
                        }

                        if (!mouseDown && cameraControlState == CameraControlState.MoveWaypoint)
                        {
                            moveWaypointFirst = true;
                            moveWaypointFirstPlayer = false;
                        }

                        if (mouseDown && cameraControlState == CameraControlState.DeleteWaypoint)
                        {
                            DeleteWaypoint(waypointTargets, waypointLines, smgr, camera);

                            ResetBots(smgr, CharacterNodes, parameters);
                        }





                        if (mouseDown && cameraControlState == CameraControlState.CreateEdge)
                            CreateLine(waypointTargets, waypointLines, smgr, camera);

                        if (!mouseDown && cameraControlState == CameraControlState.CreateEdge)
                            createLineFirst = true;

                        if (mouseDown && cameraControlState == CameraControlState.DeleteEdge)
                            DeleteLine(waypointLines, smgr, camera);



                        ChangeMaterialWaypoints(waypointTargets,playerTarget);

                        ChangeMaterialLines(waypointLines);






                        skySceneNode.Visible = toolStripButtonSkyVisible.Checked;

                        foreach (SceneNode sceneNode in listSceneNode)
                        {
                            switch (sceneNode.Type)
                            {
                                case SceneNodeType.Cube:
                                case SceneNodeType.AnimatedMesh:
                                case SceneNodeType.Mesh:
                                case SceneNodeType.Sphere:
                                case SceneNodeType.Terrain:
                                case SceneNodeType.Octree:
                                    sceneNode.Visible = toolStripButtonMapVisible.Checked;
                                    break;
                            }

                        }

                        sphereTarget.Visible = cameraControlState == CameraControlState.Pan ||
                            cameraControlState == CameraControlState.RotateAround ||
                            cameraControlState == CameraControlState.RotateFirst ||
                            cameraControlState == CameraControlState.UpDown ||
                            cameraControlState == CameraControlState.Zoom;


                        foreach (MeshSceneNode waypoint in waypointTargets)
                        {
                            waypoint.Visible = toolStripButtonAIVisible.Checked;

                        }

                        foreach (MeshSceneNode waypointLine in waypointLines)
                        {
                            waypointLine.Visible = toolStripButtonAIVisible.Checked;

                        }

                        foreach (AnimatedMeshSceneNode CharacterNode in CharacterNodes)
                        {
                            CharacterNode.Visible = toolStripButtonBotsVisible.Checked;

                        }

                        driver.BeginScene(true, true, new Color(0));





                        smgr.DrawAll();





                        driver.EndScene();

                        int fps = driver.FPS;

                        if (lastFPS != fps)
                        {
                            worker.ReportProgress(fps, driver.Name);
                            lastFPS = fps;
                        }


                        if (worker.CancellationPending)
                            device.Close();

                    }

            device.Drop();

            

        }

        public void AIGraphicsInitialize(ref SceneManager smgr, ref Parameters parameters, 
            ref List<MeshSceneNode> waypointTargets,
            ref List<MeshSceneNode> waypointLines,
            ref List<AnimatedMeshSceneNode> CharacterNodes, ref AnimatedMeshSceneNode PlayerNode,
            ref MeshSceneNode playerTarget)
        {
            //bool reset = false;

            foreach (MeshSceneNode waypoint in waypointTargets)
            {
                waypoint.Remove();

              //  reset = true;
            }

            foreach (MeshSceneNode line in waypointLines)
            {
                line.Remove();

                //reset = true;
            }

            foreach (AnimatedMeshSceneNode CharacterNode in CharacterNodes)
            {
                CharacterNode.Remove();

                //reset = true;
            }

            if (PlayerNode != null)
            {
                PlayerNode.Remove();

                //reset = true;
            }


            if (playerTarget != null)
            {
                playerTarget.Remove();

             //   reset = true;
            }

           // Waypoint.Reset();


            waypointLines = new List<MeshSceneNode>();
            waypointTargets = new List<MeshSceneNode>();
            CharacterNodes = new List<AnimatedMeshSceneNode>();

            foreach (Waypoint waypoint in Waypoint.waypoints)
            {

                MeshSceneNode cubeTarget = smgr.AddCubeSceneNode(cubeTargetSize,
                    null, waypoint.id, waypoint.position);



                waypointTargets.Add(cubeTarget);
            }





            foreach (Waypoint.Edge edge in Waypoint.edges)
            {



                Line3Df lineBetween = new Line3Df(edge.start, edge.end);

                Vector3Df centerPoint = new Vector3Df((lineBetween.Start.X + lineBetween.End.X) / 2,
                    (lineBetween.Start.Y + lineBetween.End.Y) / 2, (lineBetween.Start.Z + lineBetween.End.Z) / 2);

                Vector3Df rotateAngle = getAngleBetween3Vectors(lineBetween.Vector);

                MeshSceneNode cube = smgr.AddCubeSceneNode(1, null, edge.id, centerPoint, rotateAngle,
                        new Vector3Df(lineThickness, lineBetween.Length, lineThickness));

                waypointLines.Add(cube);
            }


            //players


            ResetBots(smgr, CharacterNodes, parameters);

            AnimatedMesh meshPlayer = smgr.GetMesh(parameters.shotNode.ModelPath);

            PlayerNode = smgr.AddAnimatedMeshSceneNode(meshPlayer);


            PlayerNode.SetMaterialFlag(MaterialFlag.Lighting, false);
            PlayerNode.SetMD2Animation(AnimationTypeMD2.Stand);

            PlayerNode.SetMaterialTexture(0, smgr.VideoDriver.GetTexture(parameters.shotNode.TexturePath));

            PlayerNode.Scale = parameters.shotNode.Scale.ToVector3Df();

            PlayerNode.Position = playerPosition +
                new Vector3Df(0, (PlayerNode.BoundingBox.MaxEdge.Y - PlayerNode.BoundingBox.MinEdge.Y) / 2.0f, 0);

            PlayerNode.ID = 0;

            playerTarget = smgr.AddCubeSceneNode(cubeTargetSize,
             null, 0, PlayerNode.Position);


        }

        public void ResetBots(SceneManager smgr, List<AnimatedMeshSceneNode> CharacterNodes, Parameters parameters)
        {

            int numberNeed = parameters.NumBots > Waypoint.waypoints.Count ? Waypoint.waypoints.Count : parameters.NumBots;


            if (CharacterNodes.Count != numberNeed)
            {
                //пересоздаем ботов.


                for (int i = 0; i < CharacterNodes.Count; i++)
                {
                    CharacterNodes[i].Remove();
                }

                CharacterNodes.Clear();

                List<int> indexWaypointsBot = new List<int>();


                if (numberNeed > 0)
                {

                    do
                    {
                        int index = (new System.Random()).Next(Waypoint.waypoints.Count);

                        bool exist = false;

                        foreach (int indexIn in indexWaypointsBot)
                        {
                            if (index == indexIn)
                            {
                                exist = true;
                                break;
                            }
                        }

                        if (!exist)
                        {
                            indexWaypointsBot.Add(index);
                        }
                    } while (indexWaypointsBot.Count < numberNeed);


                    for (int i = 0; i < numberNeed; i++)
                    {
                        AnimatedMesh mesh = smgr.GetMesh(parameters.ModelPathBot);

                        AnimatedMeshSceneNode CharacterNode = smgr.AddAnimatedMeshSceneNode(mesh);


                        CharacterNode.SetMaterialFlag(MaterialFlag.Lighting, false);
                        CharacterNode.SetMD2Animation(AnimationTypeMD2.Stand);

                        CharacterNode.SetMaterialTexture(0, smgr.VideoDriver.GetTexture(parameters.TexturePathBot));



                        CharacterNode.Position = Waypoint.waypoints[indexWaypointsBot[i]].position +
                            new Vector3Df(0, (CharacterNode.BoundingBox.MaxEdge.Y - CharacterNode.BoundingBox.MinEdge.Y) / 2.0f, 0);

                        CharacterNode.ID = Waypoint.waypoints[indexWaypointsBot[i]].id;

                        CharacterNodes.Add(CharacterNode);
                    }
                }
            }
            else
            {
                //меняем их позиции
                for(int i = 0; i<CharacterNodes.Count;i++)
                {
                    Waypoint waypoint = Waypoint.GetWaypoint(CharacterNodes[i].ID);

                    if(waypoint!=null)
                        CharacterNodes[i].Position = waypoint.position +
                            new Vector3Df(0, (CharacterNodes[i].BoundingBox.MaxEdge.Y - CharacterNodes[i].BoundingBox.MinEdge.Y) / 2.0f, 0);
                    else
                    {
                        int id = -1;

                        bool busy = false;

                        do 
                        {
                            busy = false;

                            id = Waypoint.waypoints[(new System.Random()).Next(Waypoint.waypoints.Count)].id;

                            for (int j = 0; j < CharacterNodes.Count;j++)
                            {
                                if (i != j && CharacterNodes[j].ID == id)
                                    busy = true;
                            }
                        } while (busy);

                        CharacterNodes[i].ID = id;
                        
                        CharacterNodes[i].Position = Waypoint.GetWaypoint(CharacterNodes[i].ID).position +
                            new Vector3Df(0, (CharacterNodes[i].BoundingBox.MaxEdge.Y - CharacterNodes[i].BoundingBox.MinEdge.Y) / 2.0f, 0);
                    }
                }
            }
        }

        public void DeleteEmptyWaypoints(List<MeshSceneNode> waypointTargets)
        {
            List<int> waypointEmptyIDs = Waypoint.DeleteEmptyWaypoints();

            foreach (int waypointID in waypointEmptyIDs)
            {
                for (int i = 0; i < waypointTargets.Count; i++)
                {
                    if (waypointTargets[i].ID == waypointID)
                    {
                        waypointTargets[i].Remove();

                        waypointTargets.RemoveAt(i);

                        break;
                    }
                }


            }

            deleteEmptyWaypoints = false;
        }

        public void DeleteAllWaypoints(List<MeshSceneNode> waypointTargets, List<MeshSceneNode> waypointLines)
        {
            Waypoint.Reset();

            foreach (MeshSceneNode meshSceneNode in waypointTargets)
            {
                meshSceneNode.Remove();
            }

            waypointTargets.Clear();

            foreach (MeshSceneNode meshSceneNode in waypointLines)
            {
                meshSceneNode.Remove();
            }

            waypointLines.Clear();

            deleteAllWaypoints = false;
        }

        public void DeleteAllLines(List<MeshSceneNode> waypointLines)
        {
            Waypoint.ResetEdges();

            foreach (MeshSceneNode meshSceneNode in waypointLines)
            {
                meshSceneNode.Remove();
            }

            waypointLines.Clear();

            deleteAllLines = false;
        }

        public void ChangeMaterialWaypoints(List<MeshSceneNode> waypointTargets, MeshSceneNode playerTarget)
        {
            Material materialPlayer = playerTarget.GetMaterial(0);
                materialPlayer.Lighting = true;

            if (selectIdWaypoint == playerTarget.ID)
            {

                materialPlayer.AmbientColor = playerTargetSelectColor;
                materialPlayer.DiffuseColor = playerTargetSelectColor;
                materialPlayer.EmissiveColor = playerTargetSelectColor;
                materialPlayer.SpecularColor = playerTargetSelectColor;
            }
            else
            {
                materialPlayer.AmbientColor = playerTargetColor;
                materialPlayer.DiffuseColor = playerTargetColor;
                materialPlayer.EmissiveColor = playerTargetColor;
                materialPlayer.SpecularColor = playerTargetColor;
            }

            foreach (MeshSceneNode meshSceneNode in waypointTargets)
            {
                Material material = meshSceneNode.GetMaterial(0);
                material.Lighting = true;

                if (selectIdWaypoint == meshSceneNode.ID || selectIdWaypointLast == meshSceneNode.ID)
                {

                    material.AmbientColor = cubeTargetSelectColor;
                    material.DiffuseColor = cubeTargetSelectColor;
                    material.EmissiveColor = cubeTargetSelectColor;
                    material.SpecularColor = cubeTargetSelectColor;
                }
                else
                {
                    material.AmbientColor = cubeTargetColor;
                    material.DiffuseColor = cubeTargetColor;
                    material.EmissiveColor = cubeTargetColor;
                    material.SpecularColor = cubeTargetColor;
                }

            }
        }

        public void ChangeMaterialLines(List<MeshSceneNode> waypointLines)
        {
            foreach (MeshSceneNode meshSceneNode in waypointLines)
            {
                Material material = meshSceneNode.GetMaterial(0);
                material.Lighting = true;

                if (selectIdLine == meshSceneNode.ID)
                {

                    material.AmbientColor = lineSelectColor;
                    material.DiffuseColor = lineSelectColor;
                    material.EmissiveColor = lineSelectColor;
                    material.SpecularColor = lineSelectColor;
                }
                else
                {
                    material.AmbientColor = lineColor;
                    material.DiffuseColor = lineColor;
                    material.EmissiveColor = lineColor;
                    material.SpecularColor = lineColor;
                }

            }
        }

        public void CreateLine(List<MeshSceneNode> waypointTargets, List<MeshSceneNode> waypointLines, SceneManager smgr,
            CameraSceneNode camera)
        {
            if (createLineFirst)
            {

                Vector3Df collisionPoint;
                Triangle3Df collisionTri;
                SceneNode collisionNode;

                Line3Df ray = smgr.SceneCollisionManager.GetRayFromScreenCoordinates(cursorLast, camera);


                for (int i = 0; i < waypointTargets.Count; i++)
                {
                    TriangleSelector triangleSelector = smgr.CreateTriangleSelector(waypointTargets[i].Mesh, waypointTargets[i]);

                    bool res = smgr.SceneCollisionManager.GetCollisionPoint(
                        ray, triangleSelector,
                        out collisionPoint, out collisionTri, out collisionNode);

                    if (res)
                    {
                        selectIdWaypointLast = selectIdWaypoint;

                        selectIdWaypoint = waypointTargets[i].ID;

                        if (selectIdWaypointLast != -1)
                        {

                            if (selectIdWaypointLast != selectIdWaypoint)
                            {
                                Waypoint.Edge edge = Waypoint.CreateEdge(selectIdWaypointLast, selectIdWaypoint);


                                if (edge != null)
                                {
                                    Line3Df lineBetween = new Line3Df(edge.start, edge.end);

                                    Vector3Df centerPoint = new Vector3Df((lineBetween.Start.X + lineBetween.End.X) / 2,
                                        (lineBetween.Start.Y + lineBetween.End.Y) / 2, (lineBetween.Start.Z + lineBetween.End.Z) / 2);


                                    Vector3Df rotateAngle = getAngleBetween3Vectors(lineBetween.Vector);
                                    //Vector3Df rotateAngle = getAngleBetween3Vectors(lineBetween.Vector);
                                    //Vector3Df rotateAngle = new Vector3Df(0,0,0);
                                    //0 90 139  180 90 49   (1-2)   (2-1) ни та ни та не правильно <-x y z-> 0 0 49 правильно
                                    MeshSceneNode cube = smgr.AddCubeSceneNode(1, null, edge.id, centerPoint, rotateAngle,
                                            new Vector3Df(lineThickness, lineBetween.Length, lineThickness));



                                    waypointLines.Add(cube);

                                    selectIdLine = edge.id;
                                }
                                else
                                    selectIdLine = -1;
                            }
                        }

                        break;
                    }

                }






                createLineFirst = false;
            }
        }

        public void CreateWaypoint(List<MeshSceneNode> waypointTargets, SceneManager smgr,
            CameraSceneNode camera, TriangleSelector triangleSelector)
        {
            Vector3Df collisionPoint;
            Triangle3Df collisionTri;
            SceneNode collisionNode;
            //Vector3Df norm = (new Line3Df(camera.Position, camera.Target)).Vector.Normalize();
            Line3Df ray = smgr.SceneCollisionManager.GetRayFromScreenCoordinates(cursorLast, camera);

            bool res = smgr.SceneCollisionManager.GetCollisionPoint(
               ray, triangleSelector,
                out collisionPoint, out collisionTri, out collisionNode);

            if (res)
            {

                if (createWaypointFirst)
                {



                    Waypoint waypoint = Waypoint.CreateWaypoint(collisionPoint);

                    selectIdWaypoint = waypoint.id;

                    MeshSceneNode cubeTarget = smgr.AddCubeSceneNode(cubeTargetSize,
                        null, waypoint.id, collisionPoint);

                    waypointTargets.Add(cubeTarget);


                    createWaypointFirst = false;
                }
                else
                {
                    //move waypoint in logic
                    Waypoint waypoint = Waypoint.GetWaypoint(selectIdWaypoint);

                    waypoint.SetPosition(collisionPoint);

                    //move waypoint in graphics

                    waypointTargets.First(x => x.ID == selectIdWaypoint).Position = collisionPoint;
                }
            }
        }

        public void MoveWaypoint(List<MeshSceneNode> waypointTargets, List<MeshSceneNode> waypointLines, SceneManager smgr,
            CameraSceneNode camera, TriangleSelector metaTriangleSelector, 
            MeshSceneNode playerTarget, AnimatedMeshSceneNode PlayerNode)
        {
            Vector3Df collisionPoint;
            Triangle3Df collisionTri;
            SceneNode collisionNode;

            Line3Df ray = smgr.SceneCollisionManager.GetRayFromScreenCoordinates(cursorLast, camera);

            bool res = false;


            TriangleSelector triangleSelectorPlayerTarget = smgr.CreateTriangleSelector(playerTarget.Mesh, playerTarget);

            bool resPlayerTarget = smgr.SceneCollisionManager.GetCollisionPoint(
                ray, triangleSelectorPlayerTarget,
                out collisionPoint, out collisionTri, out collisionNode);

            if (resPlayerTarget || moveWaypointFirstPlayer)
            {

                bool resPlayerTarget2 = smgr.SceneCollisionManager.GetCollisionPoint(
                        ray, metaTriangleSelector,
                        out collisionPoint, out collisionTri, out collisionNode);



                if (resPlayerTarget2)
                {
                    //move waypoint in logic
                    playerPosition = collisionPoint;

                    //move waypoint in graphics

                    playerTarget.Position = collisionPoint;

                    PlayerNode.Position = collisionPoint;

                    moveWaypointFirstPlayer = true;
                }
            }
            else
            {
                if (moveWaypointFirst)
                {
                    for (int i = 0; i < waypointTargets.Count; i++)
                    {
                        TriangleSelector triangleSelector = smgr.CreateTriangleSelector(waypointTargets[i].Mesh, waypointTargets[i]);

                        res = smgr.SceneCollisionManager.GetCollisionPoint(
                            ray, triangleSelector,
                            out collisionPoint, out collisionTri, out collisionNode);

                        if (res)
                        {
                            selectIdWaypoint = waypointTargets[i].ID;

                            moveWaypointFirst = false;

                            break;
                        }
                    }
                }

                if (res || !moveWaypointFirst)
                {
                    bool res2 = smgr.SceneCollisionManager.GetCollisionPoint(
                        ray, metaTriangleSelector,
                        out collisionPoint, out collisionTri, out collisionNode);



                    if (res2)
                    {






                        //move waypoint in logic
                        Waypoint waypoint = Waypoint.GetWaypoint(selectIdWaypoint);

                        waypoint.SetPosition(collisionPoint);

                        //move waypoint in graphics

                        waypointTargets.First(x => x.ID == selectIdWaypoint).Position = collisionPoint;

                        //move lines in graphics

                        List<int> edgesIDsAround = waypoint.GetEdgesIDAroundWaypoint();

                        foreach (int edgeID in edgesIDsAround)
                        {
                            Waypoint.Edge edge = Waypoint.GetEdge(edgeID);

                            Line3Df lineBetween = new Line3Df(edge.start, edge.end);

                            Vector3Df centerPoint = new Vector3Df((lineBetween.Start.X + lineBetween.End.X) / 2,
                                (lineBetween.Start.Y + lineBetween.End.Y) / 2, (lineBetween.Start.Z + lineBetween.End.Z) / 2);


                            Vector3Df rotateAngle = getAngleBetween3Vectors(lineBetween.Vector);


                            MeshSceneNode line = waypointLines.First(x => x.ID == edgeID);

                            line.Scale = new Vector3Df(lineThickness, lineBetween.Length, lineThickness);
                            line.Position = centerPoint;
                            line.Rotation = rotateAngle;



                        }

                        //                     waypoint
                        //                     waypointLines
                        //                     waypointTargets


                    }

                }
            }

        }

        public void DeleteWaypoint(List<MeshSceneNode> waypointTargets, List<MeshSceneNode> waypointLines, SceneManager smgr,
            CameraSceneNode camera)
        {

            Vector3Df collisionPoint;
            Triangle3Df collisionTri;
            SceneNode collisionNode;

            Line3Df ray = smgr.SceneCollisionManager.GetRayFromScreenCoordinates(cursorLast, camera);
                

            for (int i = 0; i < waypointTargets.Count; i++)
            {
                TriangleSelector triangleSelector = smgr.CreateTriangleSelector(waypointTargets[i].Mesh, waypointTargets[i]);

                bool res = smgr.SceneCollisionManager.GetCollisionPoint(
                    ray, triangleSelector,
                    out collisionPoint, out collisionTri, out collisionNode);

                if (res)
                {







                    int delIdWaypoint = waypointTargets[i].ID;

                    

                    //del edges in graphics
                    List<int> edgesIDsToDel = Waypoint.GetEdgesIDAroundWaypoint(delIdWaypoint);

                    
                      
                        foreach (int edgeIDToDel in edgesIDsToDel)
                    {
                        for (int j = 0; j < waypointLines.Count; j++)
                        {
                            if (waypointLines[j].ID == edgeIDToDel)
                            {
                                waypointLines[j].Remove();

                                waypointLines.RemoveAt(j);

                                break;
                            }
                        }
                    }
                    



                    //del waypoint in graphics
                    waypointTargets[i].Remove();

                    waypointTargets.RemoveAt(i);

                    //del inside objects
                    Waypoint.DeleteWaypointByID(delIdWaypoint);

                    selectIdWaypoint = -1;


                    

                    break;
                }

            }




        }

        public void DeleteLine(List<MeshSceneNode> waypointLines, SceneManager smgr,
            CameraSceneNode camera)
        {

            Vector3Df collisionPoint;
            Triangle3Df collisionTri;
            SceneNode collisionNode;

            Line3Df ray = smgr.SceneCollisionManager.GetRayFromScreenCoordinates(cursorLast, camera);


            for (int i = 0; i < waypointLines.Count; i++)
            {
                TriangleSelector triangleSelector = smgr.CreateTriangleSelector(waypointLines[i].Mesh, waypointLines[i]);

                bool res = smgr.SceneCollisionManager.GetCollisionPoint(
                    ray, triangleSelector,
                    out collisionPoint, out collisionTri, out collisionNode);

                if (res)
                {

                    Waypoint.DeleteEdgeByID(waypointLines[i].ID);
                    
                    waypointLines[i].Remove();

                    waypointLines.RemoveAt(i);

                    

                    selectIdLine = -1;




                    break;
                }

            }




        }

        public Vector3Df getAngleBetween3Vectors(Vector3Df vector)
        {
            /*Vector3Df vectorXY = new Vector3Df(vector.X, vector.Y, 0);
            Vector3Df vectorYZ = new Vector3Df(0, vector.Y, vector.Z);
            Vector3Df vectorXZ = new Vector3Df(vector.X, 0, vector.Z);

            Vector3Df vectorX = new Vector3Df(1,0,0);
            Vector3Df vectorY = new Vector3Df(0,1,0);
            Vector3Df vectorZ = new Vector3Df(0,0,1);

//             vectorXY.Normalize();
//             vectorYZ.Normalize();
//             vectorXZ.Normalize();

            float a = vectorYZ.Length > 0 ? getAngleBetween2Vectors(vectorYZ, vectorY) : (float)Math.PI / 2; //x 
            float b = vectorXZ.Length > 0 ? getAngleBetween2Vectors(vectorXZ, vectorZ) : (float)Math.PI / 2; //y правильно
            float c = vectorXY.Length > 0 ? getAngleBetween2Vectors(vectorXY, vectorY) : (float)Math.PI / 2; //z

            //float directAngle = 90;
             */
            //vector.Normalize();
            //vector = new Vector3Df(vector.X<0?-vector.X:vector.X,vector.Y<0?-vector.Y:vector.Y,
              //  vector.Z<0?-vector.Z:vector.Z);
            
            //float a = (float)Math.Acos(vector.Y);
            //float b = (float)Math.Acos(vector.Z);
            //float c = (float)Math.Acos(vector.X);

            //double toAngle = 180d / Math.PI;

            //a = (float)(a * toAngle);
            //b = (float)(b * toAngle);
            //c = (float)(c * toAngle);
            //два последовательных угла поворота только нужно


            Vector3Df firstRotate = new Vector3Df(0, (float)Math.Sqrt(vector.Length*vector.Length-vector.Z*vector.Z), vector.Z);
            Vector3Df firstRotateXY = new Vector3Df(0, firstRotate.Y, 0);
            Vector3Df vectorXY = new Vector3Df(vector.X, vector.Y, 0);
            Vector3Df vectorY = new Vector3Df(0, 1, 0);
            float a = vector.Length>0?getAngleBetween2Vectors(vectorY,firstRotate):0;
            float b = 0;
            float c = firstRotateXY.Length>0&&vectorXY.Length>0?getAngleBetween2Vectors(firstRotateXY,vectorXY):0;

            if (vector.X >= 0)
                c *= -1;

            if (vector.Z <= 0)
                a *= -1;

           /* while (a > directAngle)
                a -= directAngle;

            while (a < -directAngle)
                a += directAngle;
            
            while (b > directAngle)
                b -= directAngle;

            while (b < -directAngle)
                b += directAngle;
            
            while (c > directAngle)
                c -= directAngle;

            while (c < -directAngle)
                c += directAngle;
            */


            return new Vector3Df(a,b,c);
        }

        public float getAngleBetween2Vectors(Vector3Df vector1, Vector3Df vector2)
        {
            double toAngle = 180d / Math.PI;

            return (float)(Math.Acos(vector1.DotProduct(vector2) / (vector1.Length * vector2.Length))*toAngle);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundRendering.IsBusy)
            {
                backgroundRendering.CancelAsync();
                e.Cancel = true;

                userWantExit = true;


            }

        }

        private void backgroundRendering_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int f = e.ProgressPercentage;
            string d = e.UserState as string;

            toolStripStatusLabel1.Text = string.Format("Rendering {1} fps using {0} driver", d, f);
        }

        private void backgroundRendering_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // if exception occured in rendering thread -- we display the message
            if (e.Error != null)
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // if user want exit - we close main form
            // note: it is the only way to close form correctly -- only when device dropped,
            // so background worker not running
            if (userWantExit)
                Close();

            //toolStripStatusLabel1.Text = "Ready";
            
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Parameters parameters = new Parameters();

            PropertyGridRefresh(parameters);

            PropertyGridCollapseExpand();

            toolStripStatusLabel1.Text = "Ready";
        }

        public void PropertyGridRefresh(Parameters parameters)
        {
            
            propertyGrid.SelectedObject = parameters;

            propertyGrid.CollapseAllGridItems();

            AdjustPropertyGridColumnWidth();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();

            toolStripButtonPlay.Image = IrrConstructor.Properties.Resources.stop;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public bool CheckParameters(Parameters parametersChecking, bool setup)
        {

            try
            {


                if (parametersChecking.Width <= 0)
                    throw new Exception("Ошибка в Окно.Ширина");

                if (parametersChecking.Height <= 0)
                    throw new Exception("Ошибка в Окно.Высота");

                if (parametersChecking.BitsPerPixel <= 0)
                    throw new Exception("Ошибка в Окно.Пиксели");

                if (!File.Exists(parametersChecking.WinImagePath))
                    throw new Exception("Ошибка в Интерфейс.КартинкаПобеды");

                if (!File.Exists(parametersChecking.LoseImagePath))
                    throw new Exception("Ошибка в Интерфейс.КартинкаПоражения");

                if (!File.Exists(parametersChecking.FontDigitPath))
                    throw new Exception("Ошибка в Интерфейс.ШрифтЦифр");

                if (!(parametersChecking._DriverType == driverTypeList[0] ||
                    parametersChecking._DriverType == driverTypeList[1]))
                    throw new Exception("Ошибка в Графика.ТипДрайвера");


                if (!(parametersChecking.SkyType == skyTypeListRus[0] ||
                    parametersChecking.SkyType == skyTypeListRus[1]))
                    throw new Exception("Ошибка в Небо.ТипНеба");

                SkyType skyType = parametersChecking.SkyType == "Dome" ? SkyType.Dome : SkyType.Box;

                if (skyType == SkyType.Dome)
                {
                    if (parametersChecking.HoriRes < 3)
                        throw new Exception("Ошибка в Небо.Горизонтально");

                    if (parametersChecking.VertRes < 2)
                        throw new Exception("Ошибка в Небо.Вертикально");

                    if (parametersChecking.TexturePercentage < 0 || parametersChecking.TexturePercentage > 1)
                        throw new Exception("Ошибка в Небо.ПроцентТекстуры");

                    if (parametersChecking.SpherePercentage < 0 || parametersChecking.SpherePercentage > 2)
                        throw new Exception("Ошибка в Небо.ПроцентКупола");

                    if (!File.Exists(parametersChecking.TexturePathSky))
                        throw new Exception("Ошибка в Небо.ПутьНеба");
                }
                else
                {
                    if (!File.Exists(parametersChecking.TopTexturePath))
                        throw new Exception("Ошибка в Небо.Верх");

                    if (!File.Exists(parametersChecking.BottomTexturePath))
                        throw new Exception("Ошибка в Небо.Низ");

                    if (!File.Exists(parametersChecking.LeftTexturePath))
                        throw new Exception("Ошибка в Небо.Лево");

                    if (!File.Exists(parametersChecking.RightTexturePath))
                        throw new Exception("Ошибка в Небо.Право");

                    if (!File.Exists(parametersChecking.FrontTexturePath))
                        throw new Exception("Ошибка в Небо.Фронт");

                    if (!File.Exists(parametersChecking.BackTexturePath))
                        throw new Exception("Ошибка в Небо.Тыл");
                }

                if (!File.Exists(parametersChecking.MapPath))
                    throw new Exception("Ошибка в Карта.ПутьКарты");

                if (parametersChecking.MinimalPolysPerNode < 0)
                    throw new Exception("Ошибка в Карта.Объект");




                if (parametersChecking.MapAI)
                {
                    if (!File.Exists(parametersChecking.MapAIPath))
                        throw new Exception("Ошибка в Карта.ПутьАИКарты");
                }

                if(Waypoint.waypoints.Count<parametersChecking.NumBots && setup)
                    throw new Exception("Ошибка в Карта.АИКарта, Бот.Боты недостаточно точек или слишком много ботов");

                if(parametersChecking.MapExtraFile)
                    foreach (string mapExtraFilePath in parametersChecking.MapExtraFiles)
                        if (!File.Exists(mapExtraFilePath))
                            throw new Exception("Ошибка в Карта.ДопФайл.Путь");

                if (parametersChecking.MaxHealth <= 0)
                    throw new Exception("Ошибка в Персонаж.МаксХП");

                if (parametersChecking.MaxAmmo <= 0)
                    throw new Exception("Ошибка в Персонаж.МаксПатронов");

                if (parametersChecking.TimeShotDelay <= 0)
                    throw new Exception("Ошибка в Персонаж.ЗадержкаВыстрела");

                if (parametersChecking.TimeFallToDeath <= 0)
                    throw new Exception("Ошибка в Персонаж.ВремяПаденияСмерти");

                if (parametersChecking.RefillPeriodHealth < 0)
                    throw new Exception("Ошибка в Персонаж.ПериодХП");

                if (parametersChecking.RefillPeriodAmmo < 0)
                    throw new Exception("Ошибка в Персонаж.ПериодПатроны");

                if (parametersChecking.DrawnHealth <= 0)
                    throw new Exception("Ошибка в Персонаж.Урон");

                if (parametersChecking.animationSpeed.Death < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.ПадениеСмерть");

                if (parametersChecking.animationSpeed.Boom < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.Смерть");

                if (parametersChecking.animationSpeed.CrouchAttack < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.ПриседАтака");

                if (parametersChecking.animationSpeed.Attack < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.АтакаСтоя");

                if (parametersChecking.animationSpeed.Jump < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.Прыжок");

                if (parametersChecking.animationSpeed.CrouchWalk < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.ПриседБег");

                if (parametersChecking.animationSpeed.Run < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.СтояБег");

                if (parametersChecking.animationSpeed.CrouchStand < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.ПриседСтоя");

                if (parametersChecking.animationSpeed.Stand < 0)
                    throw new Exception("Ошибка в Персонаж.Анимация.Стоя");

                if (parametersChecking.NumBots < 1)
                    throw new Exception("Ошибка в Бот.Боты");

                if (parametersChecking.TimeJump <= 0)
                    throw new Exception("Ошибка в Бот.ВремяПрыжок");

                if (parametersChecking.TimeCrouching <= 0)
                    throw new Exception("Ошибка в Бот.Присед");

                if (parametersChecking.Range < 0)
                    throw new Exception("Ошибка в Бот.Range");

                if (parametersChecking.MoveSpeedBot < 0)
                    throw new Exception("Ошибка в Бот.Скорость");

                if (parametersChecking.AtDestinationThreshold < 0)
                    throw new Exception("Ошибка в Бот.Предел");

                if (parametersChecking.JumpUpFactor < 0)
                    throw new Exception("Ошибка в Бот.УскорениеПрыжокВверх");

                if (parametersChecking.JumpAroundFactor < 0)
                    throw new Exception("Ошибка в Бот.УскорениеПрыжокВСторону");

                if (!File.Exists(parametersChecking.ModelPathBot))
                    throw new Exception("Ошибка в Бот.Модель");

                if (!File.Exists(parametersChecking.TexturePathBot))
                    throw new Exception("Ошибка в Бот.Текстура");

                if (parametersChecking.MoveSpeedPlayer < 0)
                    throw new Exception("Ошибка в Игрок.Скорость");

                if (parametersChecking.RotateSpeed <= 0)
                    throw new Exception("Ошибка в Игрок.Поворот");

                if (parametersChecking.JumpSpeed < 0)
                    throw new Exception("Ошибка в Игрок.ПрыжокСкорость");

                if (parametersChecking.FarValue <= 0)
                    throw new Exception("Ошибка в Игрок.Видимость");

                if (!File.Exists(parametersChecking.shotNode.ModelPath))
                    throw new Exception("Ошибка в Игрок.МодельВыстрела.Модель");

                if (!File.Exists(parametersChecking.shotNode.TexturePath))
                    throw new Exception("Ошибка в Игрок.МодельВыстрела.Текстура");

                if (parametersChecking.Speed <= 0)
                    throw new Exception("Ошибка в Выстрел.Скорость");

                if (parametersChecking.MaxDistanceTravelled <= 0)
                    throw new Exception("Ошибка в Выстрел.МаксДистанция");

                if (parametersChecking.LiveBillBoard.TextureTimePerFrame <= 0)
                    throw new Exception("Ошибка в Выстрел.МодельЖизни.ПериодТекстуры");

                if (parametersChecking.LiveBillBoard.Dimension.Width <= 0 ||
                    parametersChecking.LiveBillBoard.Dimension.Height <= 0)
                    throw new Exception("Ошибка в Выстрел.МодельЖизни.Размер");

                foreach (string texturePath in parametersChecking.LiveBillBoard.TexturePaths)
                    if (!File.Exists(texturePath))
                        throw new Exception("Ошибка в Выстрел.МодельЖизни.Текстура.Путь");

                if (parametersChecking.DieBillBoard.TextureTimePerFrame <= 0)
                    throw new Exception("Ошибка в Выстрел.МодельСмерти.ПериодТекстуры");

                if (parametersChecking.DieBillBoard.Dimension.Width <= 0 ||
                    parametersChecking.DieBillBoard.Dimension.Height <= 0)
                    throw new Exception("Ошибка в Выстрел.МодельСмерти.Размер");


                foreach (string texturePath in parametersChecking.DieBillBoard.TexturePaths)
                    if (!File.Exists(texturePath))
                        throw new Exception("Ошибка в Выстрел.МодельСмерти.Текстура.Путь");

                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }

        }

        static bool LoadWaypointsFromXML(string fileName)
        {
            try
            {
                
                XDocument xDocument = XDocument.Load(fileName);

                foreach (XElement xElement in xDocument.Root.Element("WaypointGroup").Elements())
                {
                    

                    int id;
                    if (!int.TryParse(xElement.Attribute("id").Value, out id))
                        throw new Exception("Ошибка в " + fileName + ": Waypoint.id");

                    List<int> neighbours = xElement.Attribute("neighbours").Value.Split(',').Select(n => Convert.ToInt32(n)).ToList();

                    Vector3DfProperty vectorTemp;
                    if (!UtilityProperty.getVector3dfFrom(xElement.Attribute("position").Value, out vectorTemp))
                        throw new Exception("Ошибка в " + fileName + ": Waypoint.position");
                    

                    Waypoint.CreateWaypoint(id,vectorTemp.ToVector3Df(),neighbours);
                }


                Waypoint.CalculateEdges();

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        static bool LoadParametersFromXML(string fileName, out Parameters parametersReturn)//, ref Parameters parameters)
        {
            parametersReturn = new Parameters();


            try
            {


                XDocument xDocument = XDocument.Load(fileName);

                //parametersReturn.InstallPath = xDocument.Root.Element("Game").Attribute("InstallPath").Value;
                parametersReturn.Name = xDocument.Root.Element("Game").Attribute("Name").Value;


                bool FPS;
                if (!bool.TryParse(xDocument.Root.Element("Game").Attribute("FPS").Value,
                    out FPS))
                    throw new Exception("Ошибка в "+fileName+": Game.FPS");
                parametersReturn.FPS = FPS;

                int Width;
                if (!int.TryParse(xDocument.Root.Element("Window").Attribute("Width").Value, out Width))
                    throw new Exception("Ошибка в "+fileName+": Window.Width");
                parametersReturn.Width = Width;

                int Height;
                if (!int.TryParse(xDocument.Root.Element("Window").Attribute("Height").Value, out Height))
                    throw new Exception("Ошибка в " + fileName + ": Window.Height");
                parametersReturn.Height = Height;

                byte BitsPerPixel;
                if (!byte.TryParse(xDocument.Root.Element("Window").Attribute("BitsPerPixel").Value, out BitsPerPixel))
                    throw new Exception("Ошибка в " + fileName + ": Window.BitsPerPixel");
                parametersReturn.BitsPerPixel = BitsPerPixel;

                parametersReturn.WinImagePath = xDocument.Root.Element("Interface").Attribute("WinImagePath").Value;

                parametersReturn.LoseImagePath = xDocument.Root.Element("Interface").Attribute("LoseImagePath").Value;

                parametersReturn.FontDigitPath = xDocument.Root.Element("Interface").Attribute("FontDigitPath").Value;

                ColorProperty ColorHealth;
                if (!UtilityProperty.getColourFrom(xDocument.Root.Element("Interface").Attribute("ColorHealth").Value,
                    out ColorHealth))
                    throw new Exception("Ошибка в " + fileName + ": Interface.ColorHealth");
                parametersReturn.ColorHealth = ColorHealth;

                ColorProperty ColorProjectiles;
                if (!UtilityProperty.getColourFrom(xDocument.Root.Element("Interface").Attribute("ColorProjectiles").Value,
                    out ColorProjectiles))
                    throw new Exception("Ошибка в " + fileName + ": Interface.ColorProjectiles");
                parametersReturn.ColorProjectiles = ColorProjectiles;

                byte Antialiasing;
                if (!byte.TryParse(xDocument.Root.Element("Graphics").Attribute("Antialiasing").Value,
                    out Antialiasing))
                    throw new Exception("Ошибка в " + fileName + ": Graphics.Antialiasing");
                parametersReturn.Antialiasing = Antialiasing;

                parametersReturn._DriverType = xDocument.Root.Element("Graphics").Attribute("DriverType").Value;

                bool FullScreen;
                if (!bool.TryParse(xDocument.Root.Element("Graphics").Attribute("FullScreen").Value,
                    out FullScreen))
                    throw new Exception("Ошибка в " + fileName + ": Graphics.FullScreen");
                parametersReturn.FullScreen = FullScreen;

                bool VSync;
                if (!bool.TryParse(xDocument.Root.Element("Graphics").Attribute("VSync").Value,
                    out VSync))
                    throw new Exception("Ошибка в " + fileName + ": Graphics.VSync");
                parametersReturn.VSync = VSync;

                string skyTypeEng = xDocument.Root.Element("Sky").Attribute("Type").Value;
                string skyTypeRus = skyTypeEng == skyTypeListEng[0] ? skyTypeListRus[0] : skyTypeListRus[1];

                parametersReturn.SkyType = skyTypeRus;

                SkyType skyType = skyTypeEng == "Dome" ? SkyType.Dome : SkyType.Box;

               
                if (skyType == SkyType.Dome)
                {

                    int HoriRes;
                    if (!int.TryParse(xDocument.Root.Element("Sky").Attribute("HoriRes").Value,
                        out HoriRes))
                        throw new Exception("Ошибка в "+fileName+": Sky.HoriRes");
                    parametersReturn.HoriRes = HoriRes;

                    int VertRes;
                    if (!int.TryParse(xDocument.Root.Element("Sky").Attribute("VertRes").Value,
                        out VertRes))
                        throw new Exception("Ошибка в "+fileName+": Sky.VertRes");
                    parametersReturn.VertRes = VertRes;

                    float TexturePercentage;
                    if (!float.TryParse(xDocument.Root.Element("Sky").Attribute("TexturePercentage").Value.Replace('.', ','),
                        out TexturePercentage))
                        throw new Exception("Ошибка в "+fileName+": Sky.TexturePercentage");
                    parametersReturn.TexturePercentage = TexturePercentage;

                    float SpherePercentage;
                    if (!float.TryParse(xDocument.Root.Element("Sky").Attribute("SpherePercentage").Value.Replace('.', ','),
                        out SpherePercentage))
                        throw new Exception("Ошибка в "+fileName+": Sky.SpherePercentage");
                    parametersReturn.SpherePercentage = SpherePercentage;

                    parametersReturn.TexturePathSky = xDocument.Root.Element("Sky").Attribute("TexturePath").Value;

                }
                else
                {
                    parametersReturn.TopTexturePath = xDocument.Root.Element("Sky").Attribute("TopTexturePath").Value;

                    parametersReturn.BottomTexturePath = xDocument.Root.Element("Sky").Attribute("BottomTexturePath").Value;

                    parametersReturn.LeftTexturePath = xDocument.Root.Element("Sky").Attribute("LeftTexturePath").Value;

                    parametersReturn.RightTexturePath = xDocument.Root.Element("Sky").Attribute("RightTexturePath").Value;

                    parametersReturn.FrontTexturePath = xDocument.Root.Element("Sky").Attribute("FrontTexturePath").Value;

                    parametersReturn.BackTexturePath = xDocument.Root.Element("Sky").Attribute("BackTexturePath").Value;

                }

                parametersReturn.MapPath = xDocument.Root.Element("Map").Attribute("MapPath").Value;

                int MinimalPolysPerNode;
                if (!int.TryParse(xDocument.Root.Element("Map").Attribute("MinimalPolysPerNode").Value,
                    out MinimalPolysPerNode))
                    throw new Exception("Ошибка в " + fileName + ": Map.MinimalPolysPerNode");
                parametersReturn.MinimalPolysPerNode = MinimalPolysPerNode;

                parametersReturn.MapAIPath = xDocument.Root.Element("Map").Attribute("MapAIPath").Value;

                
                parametersReturn.MapAI = parametersReturn.MapAIPath != "";


                

                List<string> listMapExtraFilePaths = new List<string>();
                foreach (XElement elem in xDocument.Root.Element("Map").Elements())
                {
                    if (elem.Name == "MapExtraFile")
                    {
                        listMapExtraFilePaths.Add(elem.Attribute("Path").Value);
                    }
                }

                if (listMapExtraFilePaths.Count > 0)
                {
                    parametersReturn.MapExtraFiles = listMapExtraFilePaths.ToArray();
                    parametersReturn.MapExtraFile = true;
                }
                else
                {
                    parametersReturn.MapExtraFiles = null;
                    parametersReturn.MapExtraFile = false;
                }

                int MaxHealth;
                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("MaxHealth").Value,
                        out MaxHealth))
                    throw new Exception("Ошибка в "+fileName+": Character.MaxHealth");
                parametersReturn.MaxHealth = MaxHealth;

                int MaxAmmo;
                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("MaxAmmo").Value,
                        out MaxAmmo))
                    throw new Exception("Ошибка в "+fileName+": Character.MaxAmmo");
                parametersReturn.MaxAmmo = MaxAmmo;

                uint TimeShotDelay;
                if (!uint.TryParse(xDocument.Root.Element("Character").Attribute("TimeShotDelay").Value,
                        out TimeShotDelay))
                    throw new Exception("Ошибка в "+fileName+": Character.TimeShotDelay");
                parametersReturn.TimeShotDelay = TimeShotDelay;

                uint TimeFallToDeath;
                if (!uint.TryParse(xDocument.Root.Element("Character").Attribute("TimeFallToDeath").Value,
                    out TimeFallToDeath))
                    throw new Exception("Ошибка в "+fileName+": Character.TimeFallToDeath");
                parametersReturn.TimeFallToDeath = TimeFallToDeath;

                bool RegenerateHealth;
                if (!bool.TryParse(xDocument.Root.Element("Character").Attribute("RegenerateHealth").Value,
                    out RegenerateHealth))
                    throw new Exception("Ошибка в "+fileName+": Character.RegenerateHealth");
                parametersReturn.RegenerateHealth = RegenerateHealth;
                
                int RefillPeriodHealth;
                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("RefillPeriodHealth").Value,
                    out RefillPeriodHealth))
                    throw new Exception("Ошибка в "+fileName+": Character.RefillPeriodHealth");
                parametersReturn.RefillPeriodHealth = RefillPeriodHealth;

                bool RegenerateAmmo;
                if (!bool.TryParse(xDocument.Root.Element("Character").Attribute("RegenerateAmmo").Value,
                    out RegenerateAmmo))
                    throw new Exception("Ошибка в "+fileName+": Character.RegenerateAmmo");
                parametersReturn.RegenerateAmmo = RegenerateAmmo;

                int RefillPeriodAmmo;
                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("RefillPeriodAmmo").Value,
                    out RefillPeriodAmmo))
                    throw new Exception("Ошибка в "+fileName+": Character.RefillPeriodAmmo");
                parametersReturn.RefillPeriodAmmo = RefillPeriodAmmo;

                int DrawnHealth;
                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("DrawnHealth").Value,
                    out DrawnHealth))
                    throw new Exception("Ошибка в "+fileName+": Character.DrawnHealth");
                parametersReturn.DrawnHealth = DrawnHealth;

                Vector3DfProperty OffsetStand;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Character").Attribute("OffsetStand").Value,
                    out OffsetStand))
                    throw new Exception("Ошибка в "+fileName+": Character.OffsetStand");
                parametersReturn.OffsetStand = OffsetStand;

                Vector3DfProperty OffsetCrouch;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Character").Attribute("OffsetCrouch").Value,
                    out OffsetCrouch))
                    throw new Exception("Ошибка в "+fileName+": Character.OffsetCrouch");
                parametersReturn.OffsetCrouch = OffsetCrouch;

                float Death;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Death").Value.Replace('.', ','), out Death))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Death");
                parametersReturn.animationSpeed.Death = Death;

                float Boom;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Boom").Value.Replace('.', ','), out Boom))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Boom");
                parametersReturn.animationSpeed.Boom = Boom;

                float CrouchAttack;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchAttack").Value.Replace('.', ','), out CrouchAttack))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.CrouchAttack");
                parametersReturn.animationSpeed.CrouchAttack = CrouchAttack;

                float Attack;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Attack").Value.Replace('.', ','), out Attack))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Attack");
                parametersReturn.animationSpeed.Attack = Attack;

                float Jump;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Jump").Value.Replace('.', ','), out Jump))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Jump");
                parametersReturn.animationSpeed.Jump = Jump;

                float CrouchWalk;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchWalk").Value.Replace('.', ','), out CrouchWalk))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.CrouchWalk");
                parametersReturn.animationSpeed.CrouchWalk = CrouchWalk;

                float Run;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Run").Value.Replace('.', ','), out Run))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Run");
                parametersReturn.animationSpeed.Run = Run;

                float CrouchStand;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchStand").Value.Replace('.', ','), out CrouchStand))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.CrouchStand");
                parametersReturn.animationSpeed.CrouchStand = CrouchStand;

                float Stand;
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Stand").Value.Replace('.', ','), out Stand))
                    throw new Exception("Ошибка в "+fileName+": Character.AnimationSpeed.Stand");
                parametersReturn.animationSpeed.Stand = Stand;

                int NumBots;
                if (!int.TryParse(xDocument.Root.Element("Bots").Attribute("NumBots").Value, out NumBots))
                    throw new Exception("Ошибка в "+fileName+": Bots.NumBots");
                parametersReturn.NumBots = NumBots;

                uint TimeJump;
                if (!uint.TryParse(xDocument.Root.Element("Bots").Attribute("TimeJump").Value,
                        out TimeJump))
                    throw new Exception("Ошибка в "+fileName+": Bots.TimeJump");
                parametersReturn.TimeJump = TimeJump;

                uint TimeCrouching;
                if (!uint.TryParse(xDocument.Root.Element("Bots").Attribute("TimeCrouching").Value,
                    out TimeCrouching))
                    throw new Exception("Ошибка в "+fileName+": Bots.TimeCrouching");
                parametersReturn.TimeCrouching = TimeCrouching;

                float Range;
                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("Range").Value.Replace('.', ','),
                    out Range))
                    throw new Exception("Ошибка в "+fileName+": Bots.Range");
                parametersReturn.Range = Range;

                Vector3DfProperty FieldOfViewDimensions;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Bots").Attribute("FieldOfViewDimensions").Value,
                    out FieldOfViewDimensions))
                    throw new Exception("Ошибка в "+fileName+": Bots.FieldOfViewDimensions");
                parametersReturn.FieldOfViewDimensions = FieldOfViewDimensions;

                float MoveSpeed;
                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("MoveSpeed").Value.Replace('.', ','),
                    out MoveSpeed))
                    throw new Exception("Ошибка в "+fileName+": Bots.MoveSpeed");
                parametersReturn.MoveSpeedBot = MoveSpeed;

                float AtDestinationThreshold;
                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("AtDestinationThreshold").Value.Replace('.', ','),
                    out AtDestinationThreshold))
                    throw new Exception("Ошибка в "+fileName+": Bots.AtDestinationThreshold");
                parametersReturn.AtDestinationThreshold = AtDestinationThreshold;

                float JumpUpFactor;
                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("JumpUpFactor").Value.Replace('.', ','),
                    out JumpUpFactor))
                    throw new Exception("Ошибка в "+fileName+": Bots.JumpUpFactor");
                parametersReturn.JumpUpFactor = JumpUpFactor;

                float JumpAroundFactor;
                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("JumpAroundFactor").Value.Replace('.', ','),
                    out JumpAroundFactor))
                    throw new Exception("Ошибка в "+fileName+": Bots.JumpAroundFactor");
                parametersReturn.JumpAroundFactor = JumpAroundFactor;
 
                parametersReturn.ModelPathBot = xDocument.Root.Element("Bots").Attribute("ModelPath").Value;

                parametersReturn.TexturePathBot = xDocument.Root.Element("Bots").Attribute("TexturePath").Value;

                float MoveSpeedPlayer;
                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("MoveSpeed").Value.Replace('.', ','),
                    out MoveSpeedPlayer))
                    throw new Exception("Ошибка в "+fileName+": Player.MoveSpeed");
                parametersReturn.MoveSpeedPlayer = MoveSpeedPlayer;

                float RotateSpeed;
                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("RotateSpeed").Value.Replace('.', ','),
                    out RotateSpeed))
                    throw new Exception("Ошибка в "+fileName+": Player.RotateSpeed");
                parametersReturn.RotateSpeed = RotateSpeed;

                float JumpSpeed;
                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("JumpSpeed").Value.Replace('.', ','),
                    out JumpSpeed))
                    throw new Exception("Ошибка в "+fileName+": Player.JumpSpeed");
                parametersReturn.JumpSpeed = JumpSpeed;

                Vector3DfProperty Position;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Position").Value,
                    out Position))
                    throw new Exception("Ошибка в "+fileName+": Player.Position");
                parametersReturn.Position = Position;

                Vector3DfProperty Target;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Target").Value,
                    out Target))
                    throw new Exception("Ошибка в "+fileName+": Player.Target");
                parametersReturn.Target = Target;

                float FarValue;
                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("FarValue").Value.Replace('.', ','),
                        out FarValue))
                    throw new Exception("Ошибка в "+fileName+": Player.FarValue");
                parametersReturn.FarValue = FarValue;

                Vector3DfProperty Scale;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Scale").Value,
                    out Scale))
                    throw new Exception("Ошибка в "+fileName+": Player.Scale");
                parametersReturn.Scale = Scale;

                Vector3DfProperty EllipsoidRadius;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("EllipsoidRadius").Value,
                    out EllipsoidRadius))
                    throw new Exception("Ошибка в "+fileName+": Player.EllipsoidRadius");
                parametersReturn.EllipsoidRadius = EllipsoidRadius; 

                Vector3DfProperty EllipsoidTranslation;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("EllipsoidTranslation").Value,
                    out EllipsoidTranslation))
                    throw new Exception("Ошибка в "+fileName+": Player.EllipsoidTranslation");
                parametersReturn.EllipsoidTranslation = EllipsoidTranslation;

                Vector3DfProperty GravityPerSecond;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("GravityPerSecond").Value,
                    out GravityPerSecond))
                    throw new Exception("Ошибка в "+fileName+": Player.GravityPerSecond");
                parametersReturn.GravityPerSecond = GravityPerSecond;

                Vector3DfProperty ScaleNode;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Scale").Value,
                out ScaleNode))
                    throw new Exception("Ошибка в "+fileName+": Player.ShotNode.Scale");
                parametersReturn.shotNode.Scale = ScaleNode;
                
                Vector3DfProperty PositionNode;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Position").Value,
                    out PositionNode))
                    throw new Exception("Ошибка в "+fileName+": Player.ShotNode.Position");
                parametersReturn.shotNode.Position = PositionNode;

                Vector3DfProperty RotationNode;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Rotation").Value,
                    out RotationNode))
                    throw new Exception("Ошибка в "+fileName+": Player.ShotNode.Rotation");
                parametersReturn.shotNode.Rotation = RotationNode;

                parametersReturn.shotNode.ModelPath = xDocument.Root.Element("Player").Element("ShotNode").Attribute("ModelPath").Value;

                parametersReturn.shotNode.TexturePath = xDocument.Root.Element("Player").Element("ShotNode").Attribute("TexturePath").Value;

                Vector3DfProperty StartPositionBotStand;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionBotStand").Value,
                    out StartPositionBotStand))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.StartPositionBotStand");
                parametersReturn.StartPositionBotStand = StartPositionBotStand;

                Vector3DfProperty StartPositionBotCrouch;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionBotCrouch").Value,
                    out StartPositionBotCrouch))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.StartPositionBotCrouch");
                parametersReturn.StartPositionBotCrouch = StartPositionBotCrouch;

                Vector3DfProperty StartPositionPlayerStand;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionPlayerStand").Value,
                    out StartPositionPlayerStand))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.StartPositionPlayerStand");
                parametersReturn.StartPositionPlayerStand = StartPositionPlayerStand;

                Vector3DfProperty StartPositionPlayerCrouch;
                if (!UtilityProperty.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionPlayerCrouch").Value,
                    out StartPositionPlayerCrouch))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.StartPositionPlayerCrouch");
                parametersReturn.StartPositionPlayerCrouch = StartPositionPlayerCrouch;

                float SpeedProj;
                if (!float.TryParse(xDocument.Root.Element("Projectile").Attribute("Speed").Value.Replace('.', ','),
                    out SpeedProj))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.Speed");
                parametersReturn.Speed = SpeedProj;

                float MaxDistanceTravelled;
                if (!float.TryParse(xDocument.Root.Element("Projectile").Attribute("MaxDistanceTravelled").Value.Replace('.', ','),
                        out MaxDistanceTravelled))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.MaxDistanceTravelled");
                parametersReturn.MaxDistanceTravelled = MaxDistanceTravelled;

                if (parametersReturn.MaxDistanceTravelled <= 0)
                    throw new Exception("Ошибка в " + fileName + ": Projectile.MaxDistanceTravelled");

                float TextureTimePerFrameLive;
                if (!float.TryParse(xDocument.Root.Element("Projectile").Element("LiveBillBoard").
                    Attribute("TextureTimePerFrame").Value.Replace('.', ','),
                    out TextureTimePerFrameLive))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.LiveBillBoard.TextureTimePerFrame");
                parametersReturn.LiveBillBoard.TextureTimePerFrame = TextureTimePerFrameLive;

                Dimension2DfProperty DimensionLive;
                if (!UtilityProperty.getDimension2DfFrom(xDocument.Root.Element("Projectile").Element("LiveBillBoard").
                    Attribute("Dimension").Value, out DimensionLive))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.LiveBillBoard.Dimension");
                parametersReturn.LiveBillBoard.Dimension = DimensionLive;

                List<string> TexturePathsLive = new List<string>();

                foreach (XElement elem in xDocument.Root.Element("Projectile").Element("LiveBillBoard").Elements())
                    TexturePathsLive.Add(elem.Attribute("Path").Value);

                parametersReturn.LiveBillBoard.TexturePaths = TexturePathsLive.ToArray();

                float TextureTimePerFrameDie;
                if (!float.TryParse(xDocument.Root.Element("Projectile").Element("DieBillBoard").
                    Attribute("TextureTimePerFrame").Value.Replace('.', ','),
                    out TextureTimePerFrameDie))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.DieBillBoard.TextureTimePerFrame");
                parametersReturn.DieBillBoard.TextureTimePerFrame = TextureTimePerFrameDie;

                Dimension2DfProperty DimensionDie;
                if (!UtilityProperty.getDimension2DfFrom(xDocument.Root.Element("Projectile").Element("DieBillBoard").
                    Attribute("Dimension").Value, out DimensionDie))
                    throw new Exception("Ошибка в " + fileName + ": Projectile.DieBillBoard.Dimension");
                parametersReturn.DieBillBoard.Dimension = DimensionDie;

                List<string> TexturePathsDie = new List<string>();

                foreach (XElement elem in xDocument.Root.Element("Projectile").Element("DieBillBoard").Elements())
                    TexturePathsDie.Add(elem.Attribute("Path").Value);

                parametersReturn.DieBillBoard.TexturePaths = TexturePathsDie.ToArray();

                
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*" ;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Parameters parameters;

                if (LoadParametersFromXML(openFileDialog.FileName, out parameters))
                {
                    PropertyGridRefresh(parameters);

                    collapse = false;
                    PropertyGridCollapseExpand();

                    toolStripButtonPlay.Image = IrrConstructor.Properties.Resources.stop;
                }
                
            }

        }

        public void PropertyGridCollapseExpand()
        {
            if (collapse)
            {
                propertyGrid.ExpandAllGridItems();

                toolStripButtonCollapseExpand.Image = IrrConstructor.Properties.Resources.bullet_toggle_minus_5779.ToBitmap();

                collapse = false;
            }
            else
            {
                propertyGrid.CollapseAllGridItems();

                toolStripButtonCollapseExpand.Image = IrrConstructor.Properties.Resources.bullet_toggle_plus_3833.ToBitmap();
                
                collapse = true;
            }
        }



        private void toolStripButtonCollapseExpand_Click(object sender, EventArgs e)
        {
            PropertyGridCollapseExpand();
        }

        public void SaveWaypointsToXML(string fileName)
        {
//             try
//             {
            string firstTag = "FileInfo";

            XmlTextWriter textWritter = new XmlTextWriter(fileName, Encoding.UTF8);
            textWritter.WriteStartDocument();

            textWritter.WriteStartElement(firstTag);
            textWritter.WriteAttributeString("fileVersion", "0.50");
            textWritter.WriteAttributeString("numWaypointGroups", "1");
            textWritter.WriteAttributeString("numEntities", "0");
            textWritter.WriteEndElement();

            textWritter.Close();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

//                 XmlAttribute fileVersion = document.CreateAttribute("fileVersion");
//                 fileVersion.InnerText = "0.50";
//                 document.LastChild.Attributes.Append(fileVersion);
// 
//                 XmlAttribute numWaypointGroups = document.CreateAttribute("numWaypointGroups");
//                 numWaypointGroups.InnerText = "1";
//                 document.LastChild.Attributes.Append(numWaypointGroups);
// 
//                 XmlAttribute numEntities = document.CreateAttribute("numEntities");
//                 numEntities.InnerText = "0";
//                 document.LastChild.Attributes.Append(numEntities);

                
            XmlNode WaypointGroup = document.CreateElement("WaypointGroup");
            document.DocumentElement.AppendChild(WaypointGroup);

            XmlAttribute name = document.CreateAttribute("name");
            name.InnerText = "Group1";
            WaypointGroup.Attributes.Append(name);

            XmlAttribute waypointSize = document.CreateAttribute("waypointSize");
            waypointSize.InnerText = "5";
            WaypointGroup.Attributes.Append(waypointSize);

            XmlAttribute colour = document.CreateAttribute("colour");
            colour.InnerText = "255,255,255,255";
            WaypointGroup.Attributes.Append(colour);

            XmlAttribute numWaypoints = document.CreateAttribute("numWaypoints");
            numWaypoints.InnerText = Waypoint.waypoints.Count.ToString();
            WaypointGroup.Attributes.Append(numWaypoints);

            
            foreach (Waypoint waypoint in Waypoint.waypoints)
            {
                XmlNode waypointNode = document.CreateElement("Waypoint");
                WaypointGroup.AppendChild(waypointNode);

                XmlAttribute id = document.CreateAttribute("id");
                id.InnerText = waypoint.id.ToString();
                waypointNode.Attributes.Append(id);

                XmlAttribute neighbours = document.CreateAttribute("neighbours");
                neighbours.InnerText = waypoint.GetNeighboursString();
                waypointNode.Attributes.Append(neighbours);

                XmlAttribute position = document.CreateAttribute("position");
                position.InnerText = waypoint.GetPositionString();
                waypointNode.Attributes.Append(position);

            }

            
            document.Save(fileName);
            
            //    return true;
//             }
//             catch(Exception ex)
//             {
//                 MessageBox.Show("Error to save file " + fileName);
// 
//                 return false;
//             }
        }

        public void SaveParametersToXML(string fileName, Parameters parameters)
        {
//             try
//             {

                XmlTextWriter textWritter = new XmlTextWriter(fileName, Encoding.UTF8);
                textWritter.WriteStartDocument();

                textWritter.WriteStartElement("Parameters");
                textWritter.WriteEndElement();

                textWritter.Close();

                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                XmlNode Game = document.CreateElement("Game");
                document.DocumentElement.AppendChild(Game);

                //XmlAttribute InstallPath = document.CreateAttribute("InstallPath");
                //InstallPath.InnerText = parameters.InstallPath;
                //Game.Attributes.Append(InstallPath);
                XmlAttribute Name = document.CreateAttribute("Name");
                Name.InnerText = parameters.Name;
                Game.Attributes.Append(Name);
                XmlAttribute FPS = document.CreateAttribute("FPS");
                FPS.InnerText = parameters.FPS.ToString();
                Game.Attributes.Append(FPS);

                XmlNode window = document.CreateElement("Window");
                document.DocumentElement.AppendChild(window);

                XmlAttribute Width = document.CreateAttribute("Width");
                Width.InnerText = parameters.Width.ToString();
                window.Attributes.Append(Width);
                XmlAttribute Height = document.CreateAttribute("Height");
                Height.InnerText = parameters.Height.ToString();
                window.Attributes.Append(Height);
                XmlAttribute BitsPerPixel = document.CreateAttribute("BitsPerPixel");
                BitsPerPixel.InnerText = parameters.BitsPerPixel.ToString();
                window.Attributes.Append(BitsPerPixel);

                XmlNode Interface = document.CreateElement("Interface");
                document.DocumentElement.AppendChild(Interface);

                XmlAttribute WinImagePath = document.CreateAttribute("WinImagePath");
                WinImagePath.InnerText = parameters.WinImagePath;
                Interface.Attributes.Append(WinImagePath);
                XmlAttribute LoseImagePath = document.CreateAttribute("LoseImagePath");
                LoseImagePath.InnerText = parameters.LoseImagePath;
                Interface.Attributes.Append(LoseImagePath);
                XmlAttribute FontDigitPath = document.CreateAttribute("FontDigitPath");
                FontDigitPath.InnerText = parameters.FontDigitPath;
                Interface.Attributes.Append(FontDigitPath);
                XmlAttribute ColorHealth = document.CreateAttribute("ColorHealth");
                ColorHealth.InnerText = parameters.ColorHealth.ToString();
                Interface.Attributes.Append(ColorHealth);
                XmlAttribute ColorProjectiles = document.CreateAttribute("ColorProjectiles");
                ColorProjectiles.InnerText = parameters.ColorProjectiles.ToString();
                Interface.Attributes.Append(ColorProjectiles);

                XmlNode graphics = document.CreateElement("Graphics");
                document.DocumentElement.AppendChild(graphics);

                XmlAttribute Antialiasing = document.CreateAttribute("Antialiasing");
                Antialiasing.InnerText = parameters.Antialiasing.ToString();
                graphics.Attributes.Append(Antialiasing);
                XmlAttribute DriverType = document.CreateAttribute("DriverType");
                DriverType.InnerText = parameters._DriverType;
                graphics.Attributes.Append(DriverType);
                XmlAttribute FullScreen = document.CreateAttribute("FullScreen");
                FullScreen.InnerText = parameters.FullScreen.ToString();
                graphics.Attributes.Append(FullScreen);
                XmlAttribute VSync = document.CreateAttribute("VSync");
                VSync.InnerText = parameters.VSync.ToString();
                graphics.Attributes.Append(VSync);

                XmlNode sky = document.CreateElement("Sky");
                document.DocumentElement.AppendChild(sky);

                if(parameters.SkyType == skyTypeListRus[0])
                {
                    XmlAttribute Type = document.CreateAttribute("Type");
                    Type.InnerText = skyTypeListEng[0];
                    sky.Attributes.Append(Type);
                    XmlAttribute TopTexturePath = document.CreateAttribute("TopTexturePath");
                    TopTexturePath.InnerText = parameters.TopTexturePath;
                    sky.Attributes.Append(TopTexturePath);
                    XmlAttribute BottomTexturePath = document.CreateAttribute("BottomTexturePath");
                    BottomTexturePath.InnerText = parameters.BottomTexturePath;
                    sky.Attributes.Append(BottomTexturePath);
                    XmlAttribute LeftTexturePath = document.CreateAttribute("LeftTexturePath");
                    LeftTexturePath.InnerText = parameters.LeftTexturePath;
                    sky.Attributes.Append(LeftTexturePath);
                    XmlAttribute RightTexturePath = document.CreateAttribute("RightTexturePath");
                    RightTexturePath.InnerText = parameters.RightTexturePath;
                    sky.Attributes.Append(RightTexturePath);
                    XmlAttribute FrontTexturePath = document.CreateAttribute("FrontTexturePath");
                    FrontTexturePath.InnerText = parameters.FrontTexturePath;
                    sky.Attributes.Append(FrontTexturePath);
                    XmlAttribute BackTexturePath = document.CreateAttribute("BackTexturePath");
                    BackTexturePath.InnerText = parameters.BackTexturePath;
                    sky.Attributes.Append(BackTexturePath);
                }
                else
                {
                    XmlAttribute Type = document.CreateAttribute("Type");
                    Type.InnerText = skyTypeListEng[1];
                    sky.Attributes.Append(Type);
                    XmlAttribute HoriRes = document.CreateAttribute("HoriRes");
                    HoriRes.InnerText = parameters.HoriRes.ToString();
                    sky.Attributes.Append(HoriRes);
                    XmlAttribute VertRes = document.CreateAttribute("VertRes");
                    VertRes.InnerText = parameters.VertRes.ToString();
                    sky.Attributes.Append(VertRes);
                    XmlAttribute TexturePercentage = document.CreateAttribute("TexturePercentage");
                    TexturePercentage.InnerText = parameters.TexturePercentage.ToString().Replace(',','.');
                    sky.Attributes.Append(TexturePercentage);
                    XmlAttribute SpherePercentage = document.CreateAttribute("SpherePercentage");
                    SpherePercentage.InnerText = parameters.SpherePercentage.ToString().Replace(',', '.');
                    sky.Attributes.Append(SpherePercentage);
                    XmlAttribute TexturePath = document.CreateAttribute("TexturePath");
                    TexturePath.InnerText = parameters.TexturePathSky;
                    sky.Attributes.Append(TexturePath);
                }

                XmlNode map = document.CreateElement("Map");
                document.DocumentElement.AppendChild(map);

                XmlAttribute MapPath = document.CreateAttribute("MapPath");
                MapPath.InnerText = parameters.MapPath;
                map.Attributes.Append(MapPath);
                XmlAttribute MinimalPolysPerNode = document.CreateAttribute("MinimalPolysPerNode");
                MinimalPolysPerNode.InnerText = parameters.MinimalPolysPerNode.ToString();
                map.Attributes.Append(MinimalPolysPerNode);
                XmlAttribute MapAIPath = document.CreateAttribute("MapAIPath");
                MapAIPath.InnerText = parameters.MapAIPath;
                map.Attributes.Append(MapAIPath);

                if(parameters.MapExtraFiles != null && parameters.MapExtraFile)
                    foreach (string mapExtraFilePath in parameters.MapExtraFiles)
                    {
                        XmlNode mapExtraFile = document.CreateElement("MapExtraFile");
                        map.AppendChild(mapExtraFile);

                        XmlAttribute path = document.CreateAttribute("Path");
                        path.InnerText = mapExtraFilePath;
                        mapExtraFile.Attributes.Append(path);
                    }



                XmlNode Character = document.CreateElement("Character");
                document.DocumentElement.AppendChild(Character);

                XmlAttribute MaxHealth = document.CreateAttribute("MaxHealth");
                MaxHealth.InnerText = parameters.MaxHealth.ToString();
                Character.Attributes.Append(MaxHealth);
                XmlAttribute MaxAmmo = document.CreateAttribute("MaxAmmo");
                MaxAmmo.InnerText = parameters.MaxAmmo.ToString();
                Character.Attributes.Append(MaxAmmo);
                XmlAttribute TimeShotDelay = document.CreateAttribute("TimeShotDelay");
                TimeShotDelay.InnerText = parameters.TimeShotDelay.ToString();
                Character.Attributes.Append(TimeShotDelay);
                XmlAttribute TimeFallToDeath = document.CreateAttribute("TimeFallToDeath");
                TimeFallToDeath.InnerText = parameters.TimeFallToDeath.ToString();
                Character.Attributes.Append(TimeFallToDeath);
                XmlAttribute RegenerateHealth = document.CreateAttribute("RegenerateHealth");
                RegenerateHealth.InnerText = parameters.RegenerateHealth.ToString();
                Character.Attributes.Append(RegenerateHealth);
                XmlAttribute RefillPeriodHealth = document.CreateAttribute("RefillPeriodHealth");
                RefillPeriodHealth.InnerText = parameters.RefillPeriodHealth.ToString();
                Character.Attributes.Append(RefillPeriodHealth);
                XmlAttribute RegenerateAmmo = document.CreateAttribute("RegenerateAmmo");
                RegenerateAmmo.InnerText = parameters.RegenerateAmmo.ToString();
                Character.Attributes.Append(RegenerateAmmo);
                XmlAttribute RefillPeriodAmmo = document.CreateAttribute("RefillPeriodAmmo");
                RefillPeriodAmmo.InnerText = parameters.RefillPeriodAmmo.ToString();
                Character.Attributes.Append(RefillPeriodAmmo);
                XmlAttribute DrawnHealth = document.CreateAttribute("DrawnHealth");
                DrawnHealth.InnerText = parameters.DrawnHealth.ToString();
                Character.Attributes.Append(DrawnHealth);
                XmlAttribute OffsetStand = document.CreateAttribute("OffsetStand");
                OffsetStand.InnerText = parameters.OffsetStand.ToString();
                Character.Attributes.Append(OffsetStand);
                XmlAttribute OffsetCrouch = document.CreateAttribute("OffsetCrouch");
                OffsetCrouch.InnerText = parameters.OffsetCrouch.ToString();
                Character.Attributes.Append(OffsetCrouch);

                XmlNode AnimationSpeed = document.CreateElement("AnimationSpeed");
                Character.AppendChild(AnimationSpeed);

                XmlAttribute Death = document.CreateAttribute("Death");
                Death.InnerText = parameters.animationSpeed.Death.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Death);
                XmlAttribute Boom = document.CreateAttribute("Boom");
                Boom.InnerText = parameters.animationSpeed.Boom.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Boom);
                XmlAttribute CrouchAttack = document.CreateAttribute("CrouchAttack");
                CrouchAttack.InnerText = parameters.animationSpeed.CrouchAttack.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(CrouchAttack);
                XmlAttribute Attack = document.CreateAttribute("Attack");
                Attack.InnerText = parameters.animationSpeed.Attack.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Attack);
                XmlAttribute Jump = document.CreateAttribute("Jump");
                Jump.InnerText = parameters.animationSpeed.Jump.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Jump);
                XmlAttribute CrouchWalk = document.CreateAttribute("CrouchWalk");
                CrouchWalk.InnerText = parameters.animationSpeed.CrouchWalk.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(CrouchWalk);
                XmlAttribute Run = document.CreateAttribute("Run");
                Run.InnerText = parameters.animationSpeed.Run.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Run);
                XmlAttribute CrouchStand = document.CreateAttribute("CrouchStand");
                CrouchStand.InnerText = parameters.animationSpeed.CrouchStand.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(CrouchStand);
                XmlAttribute Stand = document.CreateAttribute("Stand");
                Stand.InnerText = parameters.animationSpeed.Stand.ToString().Replace(',', '.');
                AnimationSpeed.Attributes.Append(Stand);

                XmlNode Bots = document.CreateElement("Bots");
                document.DocumentElement.AppendChild(Bots);

                XmlAttribute NumBots = document.CreateAttribute("NumBots");
                NumBots.InnerText = parameters.NumBots.ToString();
                Bots.Attributes.Append(NumBots);
                XmlAttribute TimeJump = document.CreateAttribute("TimeJump");
                TimeJump.InnerText = parameters.TimeJump.ToString();
                Bots.Attributes.Append(TimeJump);
                XmlAttribute TimeCrouching = document.CreateAttribute("TimeCrouching");
                TimeCrouching.InnerText = parameters.TimeCrouching.ToString();
                Bots.Attributes.Append(TimeCrouching);
                XmlAttribute Range = document.CreateAttribute("Range");
                Range.InnerText = parameters.Range.ToString().Replace(',', '.');
                Bots.Attributes.Append(Range);
                XmlAttribute FieldOfViewDimensions = document.CreateAttribute("FieldOfViewDimensions");
                FieldOfViewDimensions.InnerText = parameters.FieldOfViewDimensions.ToString();
                Bots.Attributes.Append(FieldOfViewDimensions);
                XmlAttribute MoveSpeedBot = document.CreateAttribute("MoveSpeed");
                MoveSpeedBot.InnerText = parameters.MoveSpeedBot.ToString().Replace(',', '.');
                Bots.Attributes.Append(MoveSpeedBot);
                XmlAttribute AtDestinationThreshold = document.CreateAttribute("AtDestinationThreshold");
                AtDestinationThreshold.InnerText = parameters.AtDestinationThreshold.ToString().Replace(',', '.');
                Bots.Attributes.Append(AtDestinationThreshold);
                XmlAttribute JumpUpFactor = document.CreateAttribute("JumpUpFactor");
                JumpUpFactor.InnerText = parameters.JumpUpFactor.ToString().Replace(',', '.');
                Bots.Attributes.Append(JumpUpFactor);
                XmlAttribute JumpAroundFactor = document.CreateAttribute("JumpAroundFactor");
                JumpAroundFactor.InnerText = parameters.JumpAroundFactor.ToString().Replace(',', '.');
                Bots.Attributes.Append(JumpAroundFactor);
                XmlAttribute ModelPathBot = document.CreateAttribute("ModelPath");
                ModelPathBot.InnerText = parameters.ModelPathBot;
                Bots.Attributes.Append(ModelPathBot);
                XmlAttribute TexturePathBot = document.CreateAttribute("TexturePath");
                TexturePathBot.InnerText = parameters.TexturePathBot;
                Bots.Attributes.Append(TexturePathBot);

                XmlNode Player = document.CreateElement("Player");
                document.DocumentElement.AppendChild(Player);

                XmlAttribute MoveSpeedPlayer = document.CreateAttribute("MoveSpeed");
                MoveSpeedPlayer.InnerText = parameters.MoveSpeedPlayer.ToString().Replace(',', '.');
                Player.Attributes.Append(MoveSpeedPlayer);
                XmlAttribute RotateSpeed = document.CreateAttribute("RotateSpeed");
                RotateSpeed.InnerText = parameters.RotateSpeed.ToString().Replace(',', '.');
                Player.Attributes.Append(RotateSpeed);
                XmlAttribute JumpSpeed = document.CreateAttribute("JumpSpeed");
                JumpSpeed.InnerText = parameters.JumpSpeed.ToString().Replace(',', '.');
                Player.Attributes.Append(JumpSpeed);
                XmlAttribute Position = document.CreateAttribute("Position");
                Position.InnerText = parameters.Position.ToString();
                Player.Attributes.Append(Position);
                XmlAttribute Target = document.CreateAttribute("Target");
                Target.InnerText = parameters.Target.ToString();
                Player.Attributes.Append(Target);
                XmlAttribute FarValue = document.CreateAttribute("FarValue");
                FarValue.InnerText = parameters.FarValue.ToString().Replace(',', '.');
                Player.Attributes.Append(FarValue);
                XmlAttribute Scale = document.CreateAttribute("Scale");
                Scale.InnerText = parameters.Scale.ToString();
                Player.Attributes.Append(Scale);
                XmlAttribute EllipsoidRadius = document.CreateAttribute("EllipsoidRadius");
                EllipsoidRadius.InnerText = parameters.EllipsoidRadius.ToString();
                Player.Attributes.Append(EllipsoidRadius);
                XmlAttribute EllipsoidTranslation = document.CreateAttribute("EllipsoidTranslation");
                EllipsoidTranslation.InnerText = parameters.EllipsoidTranslation.ToString();
                Player.Attributes.Append(EllipsoidTranslation);
                XmlAttribute GravityPerSecond = document.CreateAttribute("GravityPerSecond");
                GravityPerSecond.InnerText = parameters.GravityPerSecond.ToString();
                Player.Attributes.Append(GravityPerSecond);

                XmlNode ShotNode = document.CreateElement("ShotNode");
                Player.AppendChild(ShotNode);

                XmlAttribute shotNodeScale = document.CreateAttribute("Scale");
                shotNodeScale.InnerText = parameters.shotNode.Scale.ToString();
                ShotNode.Attributes.Append(shotNodeScale);
                XmlAttribute shotNodePosition = document.CreateAttribute("Position");
                shotNodePosition.InnerText = parameters.shotNode.Position.ToString();
                ShotNode.Attributes.Append(shotNodePosition);
                XmlAttribute shotNodeRotation = document.CreateAttribute("Rotation");
                shotNodeRotation.InnerText = parameters.shotNode.Rotation.ToString();
                ShotNode.Attributes.Append(shotNodeRotation);
                XmlAttribute shotNodeModelPath = document.CreateAttribute("ModelPath");
                shotNodeModelPath.InnerText = parameters.shotNode.ModelPath;
                ShotNode.Attributes.Append(shotNodeModelPath);
                XmlAttribute shotNodeTexturePath = document.CreateAttribute("TexturePath");
                shotNodeTexturePath.InnerText = parameters.shotNode.TexturePath;
                ShotNode.Attributes.Append(shotNodeTexturePath);

                XmlNode Projectile = document.CreateElement("Projectile");
                document.DocumentElement.AppendChild(Projectile);

                XmlAttribute StartPositionBotStand = document.CreateAttribute("StartPositionBotStand");
                StartPositionBotStand.InnerText = parameters.StartPositionBotStand.ToString();
                Projectile.Attributes.Append(StartPositionBotStand);
                XmlAttribute StartPositionBotCrouch = document.CreateAttribute("StartPositionBotCrouch");
                StartPositionBotCrouch.InnerText = parameters.StartPositionBotCrouch.ToString();
                Projectile.Attributes.Append(StartPositionBotCrouch);
                XmlAttribute StartPositionPlayerStand = document.CreateAttribute("StartPositionPlayerStand");
                StartPositionPlayerStand.InnerText = parameters.StartPositionPlayerStand.ToString();
                Projectile.Attributes.Append(StartPositionPlayerStand);
                XmlAttribute StartPositionPlayerCrouch = document.CreateAttribute("StartPositionPlayerCrouch");
                StartPositionPlayerCrouch.InnerText = parameters.StartPositionPlayerCrouch.ToString();
                Projectile.Attributes.Append(StartPositionPlayerCrouch);
                XmlAttribute SpeedProjectile = document.CreateAttribute("Speed");
                SpeedProjectile.InnerText = parameters.Speed.ToString().Replace(',', '.');
                Projectile.Attributes.Append(SpeedProjectile);
                XmlAttribute MaxDistanceTravelled = document.CreateAttribute("MaxDistanceTravelled");
                MaxDistanceTravelled.InnerText = parameters.MaxDistanceTravelled.ToString().Replace(',', '.');
                Projectile.Attributes.Append(MaxDistanceTravelled);

                XmlNode LiveBillBoard = document.CreateElement("LiveBillBoard");
                Projectile.AppendChild(LiveBillBoard);

                XmlAttribute TextureTimePerFrame = document.CreateAttribute("TextureTimePerFrame");
                TextureTimePerFrame.InnerText = parameters.LiveBillBoard.TextureTimePerFrame.ToString().Replace(',', '.');
                LiveBillBoard.Attributes.Append(TextureTimePerFrame);
                XmlAttribute Dimension = document.CreateAttribute("Dimension");
                Dimension.InnerText = parameters.LiveBillBoard.Dimension.ToString();
                LiveBillBoard.Attributes.Append(Dimension);

                foreach (string texturePath in parameters.LiveBillBoard.TexturePaths)
                {
                    XmlNode texture = document.CreateElement("Texture");
                    LiveBillBoard.AppendChild(texture);

                    XmlAttribute path = document.CreateAttribute("Path");
                    path.InnerText = texturePath;
                    texture.Attributes.Append(path);

                }

                XmlNode DieBillBoard = document.CreateElement("DieBillBoard");
                Projectile.AppendChild(DieBillBoard);

                XmlAttribute TextureTimePerFrameDie = document.CreateAttribute("TextureTimePerFrame");
                TextureTimePerFrameDie.InnerText = parameters.DieBillBoard.TextureTimePerFrame.ToString().Replace(',', '.');
                DieBillBoard.Attributes.Append(TextureTimePerFrameDie);
                XmlAttribute DimensionDie = document.CreateAttribute("Dimension");
                DimensionDie.InnerText = parameters.DieBillBoard.Dimension.ToString();
                DieBillBoard.Attributes.Append(DimensionDie);

                foreach (string texturePath in parameters.DieBillBoard.TexturePaths)
                {
                    XmlNode texture = document.CreateElement("Texture");
                    DieBillBoard.AppendChild(texture);

                    XmlAttribute path = document.CreateAttribute("Path");
                    path.InnerText = texturePath;
                    texture.Attributes.Append(path);

                }


                document.Save(fileName);
// 
//                 return true;
//             }
//             catch (System.Exception ex)
//             {
//                 MessageBox.Show("Error to save file " + fileName);
// 
//                 return false;
//             }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveParametersToXML(saveFileDialog.FileName, (Parameters)propertyGrid.SelectedObject);

            }
        }

        private void toolStripButtonSetupGame_Click(object sender, EventArgs e)
        {
            bool setupStart = true;

            if(Waypoint.IsEmptyWaypoints())
            {
                setupStart = MessageBox.Show("Все точки без связей будут удалены. Продолжить?", "", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes;

                if (setupStart)
                {
                    deleteEmptyWaypoints = true;

                }
            }


            if (setupStart)
            {
                while (deleteEmptyWaypoints)
                {

                }

                if (CheckParameters((Parameters)propertyGrid.SelectedObject, true))
                {
                    using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                    {
                        folderBrowserDialog.Description = "Укажите путь для установки:";

                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (Directory.GetDirectories(folderBrowserDialog.SelectedPath).Length > 0 ||
                                Directory.GetFiles(folderBrowserDialog.SelectedPath).Length > 0)
                            {
                                if (MessageBox.Show("Папка не пуста, очистить содержимое? (Рекомендуется для нормальной установки)",
                                    "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Directory.Delete(folderBrowserDialog.SelectedPath, true);

                                    Directory.CreateDirectory(folderBrowserDialog.SelectedPath);

                                }
                            }

                            if (SetupGame(folderBrowserDialog.SelectedPath, ((Parameters)propertyGrid.SelectedObject).Copy()))
                                MessageBox.Show("Игра успешно установлена.");
                            else
                            {
                                Directory.Delete(folderBrowserDialog.SelectedPath, true);

                                Directory.CreateDirectory(folderBrowserDialog.SelectedPath);
                            }


                        }
                    }
                }


            }
        }

        public bool SetupGame(string folderPath, Parameters parameters)
        {
            try
            {
            
                folderPath+='\\';
                string mediaFolderName = "Media";
                string irrlichtDllName = "Irrlicht.dll";
                string irrlichtLimeDllName = "IrrlichtLime.dll";
                string irrGameName = "IrrGame.irrexe";

                if (!File.Exists(folderPath + irrlichtDllName))
                    File.Copy(irrlichtDllName, folderPath + irrlichtDllName);

                if (!File.Exists(folderPath + irrlichtLimeDllName))
                    File.Copy(irrlichtLimeDllName, folderPath + irrlichtLimeDllName);

                if (!File.Exists(folderPath + parameters.Name + ".exe"))
                    File.Copy(irrGameName, folderPath + parameters.Name + ".exe");

                if (!Directory.Exists(folderPath + mediaFolderName))
                    Directory.CreateDirectory(folderPath + mediaFolderName);

                string WinImageName = parameters.WinImagePath.Split('\\').Last();
                string WinImagePath = mediaFolderName+"\\"+ WinImageName;
                if (!File.Exists(folderPath + WinImagePath))
                    File.Copy(parameters.WinImagePath, folderPath + WinImagePath);
                parameters.WinImagePath = WinImagePath;

                string LoseImageName = parameters.LoseImagePath.Split('\\').Last();
                string LoseImagePath = mediaFolderName + "\\" + LoseImageName;
                if (!File.Exists(folderPath + LoseImagePath))
                    File.Copy(parameters.LoseImagePath, folderPath + LoseImagePath);
                parameters.LoseImagePath = LoseImagePath;

                string FontDigitName = parameters.FontDigitPath.Split('\\').Last();
                string FontDigitPath = mediaFolderName + "\\" + FontDigitName;
                if (!File.Exists(folderPath + FontDigitPath))
                    File.Copy(parameters.FontDigitPath, folderPath + FontDigitPath);
                parameters.FontDigitPath = FontDigitPath;

                if (parameters.SkyType == skyTypeListRus[0])
                {
                    string skyBoxFolderName = "Sky";

                    if (!Directory.Exists(folderPath + mediaFolderName + "\\" + skyBoxFolderName))
                        Directory.CreateDirectory(folderPath + mediaFolderName + "\\" + skyBoxFolderName);

                    string TopTextureName = parameters.TopTexturePath.Split('\\').Last();
                    string TopTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + TopTextureName;
                    if (!File.Exists(folderPath + TopTexturePath))
                        File.Copy(parameters.TopTexturePath, folderPath + TopTexturePath);
                    parameters.TopTexturePath = TopTexturePath;

                    string BottomTextureName = parameters.BottomTexturePath.Split('\\').Last();
                    string BottomTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + BottomTextureName;
                    if (!File.Exists(folderPath + BottomTexturePath))
                        File.Copy(parameters.BottomTexturePath, folderPath + BottomTexturePath);
                    parameters.BottomTexturePath = BottomTexturePath;

                    string LeftTextureName = parameters.LeftTexturePath.Split('\\').Last();
                    string LeftTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + LeftTextureName;
                    if (!File.Exists(folderPath + LeftTexturePath))
                        File.Copy(parameters.LeftTexturePath, folderPath + LeftTexturePath);
                    parameters.LeftTexturePath = LeftTexturePath;

                    string RightTextureName = parameters.RightTexturePath.Split('\\').Last();
                    string RightTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + RightTextureName;
                    if (!File.Exists(folderPath + RightTexturePath))
                        File.Copy(parameters.RightTexturePath, folderPath + RightTexturePath);
                    parameters.RightTexturePath = RightTexturePath;

                    string FrontTextureName = parameters.FrontTexturePath.Split('\\').Last();
                    string FrontTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + FrontTextureName;
                    if (!File.Exists(folderPath + FrontTexturePath))
                        File.Copy(parameters.FrontTexturePath, folderPath + FrontTexturePath);
                    parameters.FrontTexturePath = FrontTexturePath;

                    string BackTextureName = parameters.BackTexturePath.Split('\\').Last();
                    string BackTexturePath = mediaFolderName + "\\" + skyBoxFolderName + "\\" + BackTextureName;
                    if (!File.Exists(folderPath + BackTexturePath))
                        File.Copy(parameters.BackTexturePath, folderPath + BackTexturePath);
                    parameters.BackTexturePath = BackTexturePath;

                }
                else
                {
                    string TextureSkyName = parameters.TexturePathSky.Split('\\').Last();
                    string TexturePathSky = mediaFolderName + "\\" + TextureSkyName;
                    if (!File.Exists(folderPath + TexturePathSky))
                        File.Copy(parameters.TexturePathSky, folderPath + TexturePathSky);
                    parameters.TexturePathSky = TexturePathSky;

                }

                string MapName = parameters.MapPath.Split('\\').Last();
                string MapPath = mediaFolderName + "\\" + MapName;
                if (!File.Exists(folderPath + MapPath))
                    File.Copy(parameters.MapPath, folderPath + MapPath);
                parameters.MapPath = MapPath;

                string MapAIName = "MapAI.irrai";
                string MapAIPath = mediaFolderName + "\\" + MapAIName;
                parameters.MapAIPath = folderPath + MapAIPath;
                if (!File.Exists(parameters.MapAIPath))
                    SaveWaypointsToXML(parameters.MapAIPath);
                
               /*

                if(parameters.MapAI)
                {
                    string MapAIName = parameters.MapAIPath.Split('\\').Last();
                    string MapAIPath = mediaFolderName + "\\" + MapAIName;
                    File.Copy(parameters.MapAIPath, folderPath + MapAIPath);
                    parameters.MapAIPath = MapAIPath;
                }
                else
                {
                    string MapAIName = "MapAI.irrai";
                    string MapAIPath = mediaFolderName + "\\" + MapAIName;
                    parameters.MapAIPath = MapAIPath;

                    SaveWaypointsToXML(parameters.MapAIPath);
                }
                */


                if (parameters.MapExtraFiles != null && parameters.MapExtraFile)
                {
                    foreach (string extraFilePath in parameters.MapExtraFiles)
                    {
                        string extraFileName = extraFilePath.Split('\\').Last();

                        if (!File.Exists(folderPath + extraFileName))
                            File.Copy(extraFilePath, folderPath + extraFileName);
                    }
                }

                string ModelNameBot = parameters.ModelPathBot.Split('\\').Last();
                string ModelPathBot = mediaFolderName + "\\" + ModelNameBot;
                if (!File.Exists(folderPath + ModelPathBot))
                    File.Copy(parameters.ModelPathBot, folderPath + ModelPathBot);
                parameters.ModelPathBot = ModelPathBot;

                string TextureNameBot = parameters.TexturePathBot.Split('\\').Last();
                string TexturePathBot = mediaFolderName + "\\" + TextureNameBot;
                if (!File.Exists(folderPath + TexturePathBot))
                    File.Copy(parameters.TexturePathBot, folderPath + TexturePathBot);
                parameters.TexturePathBot = TexturePathBot;

                string ModelNameShotNode = parameters.shotNode.ModelPath.Split('\\').Last();
                string ModelPathShotNode = mediaFolderName + "\\" + ModelNameShotNode;
                if (!File.Exists(folderPath + ModelPathShotNode))
                    File.Copy(parameters.shotNode.ModelPath, folderPath + ModelPathShotNode);
                parameters.shotNode.ModelPath = ModelPathShotNode;

                string TextureNameShotNode = parameters.shotNode.TexturePath.Split('\\').Last();
                string TexturePathShotNode = mediaFolderName + "\\" + TextureNameShotNode;
                if (!File.Exists(folderPath + TexturePathShotNode))
                    File.Copy(parameters.shotNode.TexturePath, folderPath + TexturePathShotNode);
                parameters.shotNode.TexturePath = TexturePathShotNode;

                string projectileFolderName = "Projectile";
                if (!Directory.Exists(folderPath + projectileFolderName))
                    Directory.CreateDirectory(folderPath + mediaFolderName + "\\" + projectileFolderName);
                
                string liveBillBoardFolderName = "LiveBillBoard";
                if (!Directory.Exists(folderPath + mediaFolderName + "\\" + projectileFolderName + "\\" + liveBillBoardFolderName))
                    Directory.CreateDirectory(folderPath + mediaFolderName + "\\" + projectileFolderName + "\\" + liveBillBoardFolderName);

                for (int i = 0; i < parameters.LiveBillBoard.TexturePaths.Length;i++ )
                {
                    string TextureName = parameters.LiveBillBoard.TexturePaths[i].Split('\\').Last();
                    string TexturePath = mediaFolderName + "\\" + projectileFolderName + "\\" +
                        liveBillBoardFolderName + "\\" + TextureName;
                    if (!File.Exists(folderPath + TexturePath))
                        File.Copy(parameters.LiveBillBoard.TexturePaths[i], folderPath + TexturePath);
                    parameters.LiveBillBoard.TexturePaths[i] = TexturePath;

                }

                string dieBillBoardFolderName = "DieBillBoard";
                if (!Directory.Exists(folderPath + mediaFolderName + "\\" + projectileFolderName + "\\" + dieBillBoardFolderName))
                    Directory.CreateDirectory(folderPath + mediaFolderName + "\\" + projectileFolderName + "\\" + dieBillBoardFolderName);

                for (int i = 0; i < parameters.DieBillBoard.TexturePaths.Length; i++)
                {
                    string TextureName = parameters.DieBillBoard.TexturePaths[i].Split('\\').Last();
                    string TexturePath = mediaFolderName + "\\" + projectileFolderName + "\\" +
                        dieBillBoardFolderName + "\\" + TextureName;
                    if (!File.Exists(folderPath + TexturePath))
                        File.Copy(parameters.DieBillBoard.TexturePaths[i], folderPath + TexturePath);
                    parameters.DieBillBoard.TexturePaths[i] = TexturePath;


                }




                if (!File.Exists(folderPath + "Parameters.xml"))
                    SaveParametersToXML(folderPath + "Parameters.xml", parameters);


                

                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        private void toolStripButtonPlay_Click(object sender, EventArgs e)
        {
            Parameters parameters = (Parameters)propertyGrid.SelectedObject;

            playerPosition = parameters.Position.ToVector3Df();

            Waypoint.Reset();


            if(parameters.MapAI)
            {
                LoadWaypointsFromXML(parameters.MapAIPath);
            }

            if (CheckParameters(parameters,false))
            {

                if (parameters.MapExtraFiles != null && parameters.MapExtraFile)
                {
                    foreach (string extraFilePath in parameters.MapExtraFiles)
                    {
                        string extraFileName = extraFilePath.Split('\\').Last();
                        if (!File.Exists(extraFileName))
                            File.Copy(extraFilePath, extraFileName);
                    }
                }

                toolStripButtonAIVisible.Checked = true;
                toolStripButtonBotsVisible.Checked = true;
                toolStripButtonMapVisible.Checked = true;
                toolStripButtonSkyVisible.Checked = true;

                panelRenderingWindowSize.X = panelRenderingWindow.Width;
                panelRenderingWindowSize.Y = panelRenderingWindow.Height;

                selectIdWaypoint = -1;

                InitializeIrrlichtDevice(parameters);

                
                toolStripButtonPlay.Image = IrrConstructor.Properties.Resources.play;

            }
        }

        private void InitializeIrrlichtDevice(Parameters parameters)
        {

            if (backgroundRendering.IsBusy)
            {
                backgroundRendering.CancelAsync();

                while (backgroundRendering.IsBusy)
                    Application.DoEvents(); 


                panelRenderingWindow.Invalidate();
            }


            parameters.WindowID = panelRenderingWindow.Handle;

            backgroundRendering.RunWorkerAsync(parameters);

            toolStripStatusLabel1.Text = "Starting rendering...";
        }

        private void panelRenderingWindow_MouseMove(object sender, MouseEventArgs e)
        {
//             lock (backgroundRendering)
//             {

            if (cursorLast.X == -1)
                cursorLast.X = e.X;

            if (cursorLast.Y == -1)
                cursorLast.Y = e.Y;


            if (mouseDown)
            {
                float changeX = e.X * panelRenderingWindowSize.X / panelRenderingWindow.Width - cursorLast.X;
                float changeY = e.Y * panelRenderingWindowSize.Y / panelRenderingWindow.Height - cursorLast.Y;

                switch (cameraControlState)
                {
                    case CameraControlState.Zoom:
                        if(changeY!=0)
                            CameraZoom(-changeY);

                        break;

                    case CameraControlState.Pan:
                        CameraPan(changeX, changeY);
                        break;

                    case CameraControlState.RotateFirst:
                        CameraRotateFirst(changeX, changeY);
                        break;

                    case CameraControlState.RotateAround:
                        CameraRotateAround(changeX, -changeY);
                        break;

                    case CameraControlState.UpDown:
                        if (changeY != 0)
                            CameraUpDown(-changeY);

                        break;


                }
                
                
            }

            cursorLast.X = e.X *panelRenderingWindowSize.X / panelRenderingWindow.Width;
            cursorLast.Y = e.Y * panelRenderingWindowSize.Y / panelRenderingWindow.Height;

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

            if (degTNow + degrees < 0.1)
                degrees = 0.1 - degTNow;

            if (degTNow + degrees > 179.9)
                degrees = 179.9 - degTNow;

            double degVecRad = degrees * Math.PI / 180d;
            double cos = Math.Cos(degVecRad);

            MPovorot(vector.CrossProduct(upVector).Normalize(), cos, Math.Sin(degVecRad), ref vector);

        }

        public void CameraRotateFirst(float changeX, float changeY)
        {
            cameraTargetRelative.RotateXZby(-changeX * mouseRotateFirstSpeed);
            RotateVectorForwardToUpVector(ref cameraTargetRelative, cameraUpVector, changeY * mouseRotateFirstSpeed);
        }

        public void CameraRotateAround(float changeX, float changeY)
        {
            Vector3Df cameraFromTargetToPosition = new Vector3Df(-cameraTargetRelative.X,
                -cameraTargetRelative.Y,-cameraTargetRelative.Z);


            cameraFromTargetToPosition.RotateXZby(-changeX * mouseRotateAroundSpeed);
            RotateVectorForwardToUpVector(ref cameraFromTargetToPosition, cameraUpVector, changeY * mouseRotateAroundSpeed);

            cameraPosition = cameraPosition + cameraTargetRelative + cameraFromTargetToPosition;
            cameraTargetRelative = new Vector3Df(-cameraFromTargetToPosition.X,
                -cameraFromTargetToPosition.Y, -cameraFromTargetToPosition.Z);

        }

        public void CameraZoom(float changeY)
        {
            Vector3Df directionNormalize = new Vector3Df(cameraTargetRelative.X,cameraTargetRelative.Y,
                cameraTargetRelative.Z);
            directionNormalize.Normalize();

            Vector3Df directionMove = new Vector3Df(directionNormalize.X * mouseZoomSpeed * changeY,
                directionNormalize.Y * mouseZoomSpeed * changeY, directionNormalize.Z * mouseZoomSpeed * changeY);

            if ((cameraTargetRelative - directionMove).Length < mouseZoomSpeed * changeY)
                return;

            cameraPosition += directionMove;
            cameraTargetRelative -= directionMove;
            
        }
        public void CameraUpDown(float changeY)
        {
            Vector3Df directionNormalize = new Vector3Df(cameraTargetRelative.X, cameraTargetRelative.Y,
                cameraTargetRelative.Z);
            directionNormalize.Normalize();

            Vector3Df directionMove = new Vector3Df(directionNormalize.X * mouseZoomSpeed * changeY,
                directionNormalize.Y * mouseZoomSpeed * changeY, directionNormalize.Z * mouseZoomSpeed * changeY);

            cameraPosition += directionMove;
            

        }
        public void CameraPan(float changeX, float changeY)
        {
            Vector3Df rightVectorNormalize = cameraUpVector.CrossProduct(cameraTargetRelative);

            rightVectorNormalize.Normalize();

            Vector3Df moveRight = new Vector3Df(rightVectorNormalize.X * mousePanSpeed * changeX,
                rightVectorNormalize.Y * mousePanSpeed * changeX, rightVectorNormalize.Z * mousePanSpeed * changeX);

            Vector3Df upVectorNormalize = rightVectorNormalize.CrossProduct(cameraTargetRelative);
            
            upVectorNormalize.Normalize();

            Vector3Df moveUp = new Vector3Df(upVectorNormalize.X * mousePanSpeed * changeY,
                upVectorNormalize.Y * mousePanSpeed * changeY, upVectorNormalize.Z * mousePanSpeed * changeY);



            cameraPosition -= moveRight;
            cameraPosition -= moveUp;

            

        }


        private void panelRenderingWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;

            
        }

        private void panelRenderingWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void toolStripButtonZoom_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonZoom.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.Zoom;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonMove_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonMove.Checked)
            {
                toolStripButtonZoom.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.Pan;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonRotateFirst_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonRotateFirst.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.RotateFirst;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonRotateAround_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonRotateAround.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.RotateAround;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonUpDown_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonUpDown.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.UpDown;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            propertyGrid.Height = this.Height - spacePropertyGrid;

            panelRenderingWindow.Width = this.Width - (this.MinimumSize.Width - panelRenderingWidth);
            panelRenderingWindow.Height = this.Height - (this.MinimumSize.Height - panelRenderingHeight);
        }

        private void toolStripButtonCreateWaypoint_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonCreateWaypoint.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.CreateWaypoint;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonSelectWaypoint_Click(object sender, EventArgs e)
        {
            
        }

        private void panelRenderingWindow_Move(object sender, EventArgs e)
        {

        }

        private void toolStripButtonDeleteWaypoint_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonDeleteWaypoint.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.DeleteWaypoint;
            }
            else
                cameraControlState = CameraControlState.None;

        }

        private void toolStripButtonCreateEdge_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonCreateEdge.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.CreateEdge;
            }
            else
                cameraControlState = CameraControlState.None;

        }

        private void toolStripButtonDeleteWaypoints_Click(object sender, EventArgs e)
        {
            
            deleteAllWaypoints = true;
        }

        private void toolStripButtonResetCamera_Click(object sender, EventArgs e)
        {
            cameraReset = true;
        }

        private void toolStripButtonMoveWaypoint_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonMoveWaypoint.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonDeleteEdge.Checked = false;

                cameraControlState = CameraControlState.MoveWaypoint;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonDeleteEdge_Click(object sender, EventArgs e)
        {
            selectIdWaypoint = -1;
            selectIdWaypointLast = -1;
            selectIdLine = -1;

            if (toolStripButtonDeleteEdge.Checked)
            {
                toolStripButtonMove.Checked = false;
                toolStripButtonRotateFirst.Checked = false;
                toolStripButtonRotateAround.Checked = false;
                toolStripButtonZoom.Checked = false;
                toolStripButtonUpDown.Checked = false;
                toolStripButtonCreateWaypoint.Checked = false;
                toolStripButtonDeleteWaypoint.Checked = false;
                toolStripButtonCreateEdge.Checked = false;
                toolStripButtonMoveWaypoint.Checked = false;

                cameraControlState = CameraControlState.DeleteEdge;
            }
            else
                cameraControlState = CameraControlState.None;
        }

        private void toolStripButtonDeleteEdges_Click(object sender, EventArgs e)
        {
            deleteAllLines = true;
        }

        private void propertyGrid_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonLoadAI_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "IRRAI files (*.irrai)|*.irrai|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Parameters parameters;

                Waypoint.Reset();
                
                LoadWaypointsFromXML(openFileDialog.FileName);

                aiGraphicsInitialize = true;
            }
        }

        private void toolStripButtonSaveAI_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "IRRAI files (*.irrai)|*.irrai|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveWaypointsToXML(saveFileDialog.FileName);

            }
        }
    }
}
