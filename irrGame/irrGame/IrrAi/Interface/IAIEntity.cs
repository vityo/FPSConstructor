using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Video;
using IrrlichtLime.Scene;
using IrrlichtLime.Core;
using IrrlichtLime.IO;

namespace IrrGame.IrrAi.Interface
{
    public enum E_AIENTITY_TYPE
    {
        EAIET_COMBATNPC,

        EAIET_PATHFINDINGNPC,

        EAIET_PLAYER,

        EAIET_ENTRYEXIT_SENSOR,

        EAIET_UNKNOWN,

        EAIET_NUM_TYPES
    }



    public class SAIEntityDesc
    {
        public Vector3Df Scale;
        public string Name;
        public object UserData;
        public Vector3Df Offset;
        public Vector3Df Position;
        public Vector3Df Rotation;

        public SAIEntityDesc()
        {
            Scale = new Vector3Df(0, 0, 0);
            Offset = new Vector3Df(0, 0, 0);

            Position = new Vector3Df(0, 0, 0);
            Rotation = new Vector3Df(0, 0, 0);

            Name = "";
        }


        public SAIEntityDesc(string scale, string name, string offset, string position, string rotation)
        {
            string strc;
            strc = scale;
            Utility.getVector3dfFrom(strc, ref Scale);
            Name = name;
            strc = offset;
            Utility.getVector3dfFrom(strc, ref Offset);
            UserData = null;
            strc = position;
            Utility.getVector3dfFrom(strc, ref Position);
            strc = rotation;
            Utility.getVector3dfFrom(strc, ref Rotation);
        }
    }

    public class IAIEntity
    {

        protected IAIManager AIManager;
        protected SceneManager sceneManager;
        protected SceneNode Node;
        protected TextSceneNode TextNode;
        public Vector3Df NodeOffset;
        protected List<SEntityGroup> EnemyGroups, AllyGroups;
        protected object UserData;
        protected bool VisibleToOtherEntity;
        protected E_AIENTITY_TYPE Type;
        protected int ID;
        protected string Name;
        public bool bIsLive;

       // public AnimatedMeshSceneNode CharacterNodeAnimate { get; set; }
        

        public IAIEntity(SAIEntityDesc desc, IAIManager aimgr, SceneManager smgr, E_AIENTITY_TYPE type, int id)
        {
            bIsLive = true;
            AIManager = aimgr;
            sceneManager = smgr;
            Type = type;
            ID = id;
            UserData = desc.UserData;
            Name = desc.Name;
            NodeOffset = desc.Offset;
            VisibleToOtherEntity = false;
            EnemyGroups = new List<SEntityGroup>();
            AllyGroups = new List<SEntityGroup>();

           // Node = sceneManager.AddAnimatedMeshSceneNode(); AddAnimatedMeshSceneNode();//(AnimatedMesh)(sceneManager.AddCubeSceneNode()),AIManager.getDebugRootNode(), ID);
            Node = sceneManager.AddCubeSceneNode(1,AIManager.getDebugRootNode(), ID);

            if (Node == null)
                return;

            Node.Grab();
            Node.Scale = desc.Scale;
            Node.Rotation = desc.Rotation;
            Node.SetMaterialFlag(MaterialFlag.Lighting, false);
            Node.SetMaterialFlag(MaterialFlag.Wireframe, true);
            setPosition(desc.Position);

//             TextNode = smgr.AddTextSceneNode(sceneManager.GUIEnvironment.Skin.GetFont(IrrlichtLime.GUI.GUIDefaultFont.Default),
//                 Name, new Color(255, 255, 255, 255), Node);

        }



        ~IAIEntity()
        {

//             if (Node != null)
//             {
//                 Node.Drop();
//                // Node.Remove();
//                 Node = null;
//             }

//             if (EnemyGroup != null)
//             { // removing it from here will remove it from enemy's entity groups
//                 for (int i = 0; i < EnemyGroup.Entities.Count; ++i)
//                 {
//                     if (EnemyGroup.Entities[i] == this)
//                     {
//                         EnemyGroup.Entities.RemoveAt(i);
//                         i--;
//                     }
//                 }
//             }
// 
//             if (AllyGroup != null)
//             { // removing it from here will remove it from enemy's entity groups
//                 for (int i = 0; i < AllyGroup.Entities.Count; ++i)
//                 {
//                     if (AllyGroup.Entities[i] == this)
//                     {
//                         AllyGroup.Entities.RemoveAt(i);
//                         i--;
//                     }
//                 }
//             }

        }


        public void reset()
        {
            setVisibleToOtherEntity(false);
        }

        public virtual void update(uint elapsedTime)
        {
            Node.UpdateAbsolutePosition();
        }

        public void writeOutXMLDescription(ref List<string> names, ref List<string> values)
        {
            string strw = "";
            names.Add("scale");
            strw = Node.Scale.X.ToString();
            strw += ",";
            strw += Node.Scale.Y.ToString();
            strw += ",";
            strw += Node.Scale.Z.ToString();
            values.Add(strw);
            names.Add("name");
            values.Add(Name);
            names.Add("offset");
            strw = NodeOffset.X.ToString();
            strw += ",";
            strw += NodeOffset.Y.ToString();
            strw += ",";
            strw += NodeOffset.Z.ToString();
            values.Add(strw);
            names.Add("position");
            Vector3Df pos = Node.Position - NodeOffset;
            strw = pos.X.ToString();
            strw += ",";
            strw += pos.Y.ToString();
            strw += ",";
            strw += pos.Z.ToString();
            values.Add(strw);
            names.Add("rotation");
            Vector3Df rot = Node.Rotation;
            strw = rot.X.ToString();
            strw += ",";
            strw += rot.Y.ToString();
            strw += ",";
            strw += rot.Z;
            values.Add(strw);
        }

        public void rotateToFace(Vector3Df targetPosition)
        {

            Vector3Df r = targetPosition - Node.AbsolutePosition;
            Vector3Df angle = new Vector3Df();

            angle.Y = (float)Math.Atan2((double)r.X, (double)r.Z);
            angle.Y *= (float)(180 / Math.PI);

            if (angle.Y < 0)
                angle.Y += 360;

            if (angle.Y >= 360)
                angle.Y -= 360;

            angle -= new Vector3Df(0, 90, 0);

            Node.Rotation = angle;

        }

        public void setOffset(Vector3Df vec)
        {
            Vector3Df oldPos = Node.AbsolutePosition;
            Vector3Df currPos = oldPos - NodeOffset;
            NodeOffset = vec;
            Node.Position = currPos + NodeOffset;
        }

        public Vector3Df getOffset()
        {
            return NodeOffset;
        }

        public void setPosition(Vector3Df vec)
        {
            Node.Position = vec + NodeOffset;
            Node.UpdateAbsolutePosition();
        }

        public Vector3Df getAbsolutePosition()
        {
            return Node.AbsolutePosition;
        }

        public void addEnemyGroup(SEntityGroup grp)
        {
            EnemyGroups.Add(grp);
        }

        public void clearEnemyGroups()
        {
            EnemyGroups.Clear();
        }

        public void addAllyGroup(SEntityGroup grp)
        {
            AllyGroups.Add(grp);
        }

        public void clearAllyGroups()
        {
            AllyGroups.Clear();
        }

        public E_AIENTITY_TYPE getType()
        {
            return Type;
        }

        public bool isVisibleToOtherEntity()
        {
            return VisibleToOtherEntity;
        }

        public void setVisibleToOtherEntity(bool val)
        {
            VisibleToOtherEntity = val;
        }

        public void setUserData(object data)
        {
            UserData = data;
        }

        public object getUserData()
        {
            return UserData;
        }

        public SEntityGroup getEnemyGroup(int id)
        {
            return EnemyGroups[id];
        }

        public SEntityGroup getAllyGroup(int id)
        {
            return AllyGroups[id];
        }
        
        public List<SEntityGroup> getEnemyGroups()
        {
            return EnemyGroups;
        }

        public List<SEntityGroup> getAllyGroups()
        {
            return AllyGroups;
        }

        public int getEnemyGroupCount()
        {
            return EnemyGroups.Count;
        }

        public int getAllyGroupCount()
        {
            return AllyGroups.Count;
        }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return Name;
        }

        public void setName(string str)
        {
            Name = str;
            TextNode.SetText(Name);
        }

        public SceneNode getNode()
        {
            return Node;
        }

        public void setScale(Vector3Df vec)
        {
            Node.Scale = vec;
        }
    }

    
}

