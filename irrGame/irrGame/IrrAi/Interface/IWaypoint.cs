using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;



namespace IrrGame.IrrAi.Interface
{

    public struct SNeighbour 
    {
        public IWaypoint Waypoint;
        public float Distance;  

    }

    public abstract class IWaypoint
    {
      	    
            protected int ID;
            protected Vector3Df Position;
        
		    protected List<SNeighbour> Neighbours;
		    protected string NeighbourString;

		    public IWaypoint(int id, Vector3Df position)
            {
                Neighbours = new List<SNeighbour>();

                Position = new Vector3Df();
                //position.X = position.Y = position.Z = 0;
			    ID = id;
			    Position.X = position.X;
                Position.Y = position.Y;
                Position.Z = position.Z;
		    }

		    ~IWaypoint() 
            {
			//    remove();
		    }

		   // public virtual void remove()
           // {
//                 foreach (SNeighbour iter in Neighbours)
//                 {
//                     if (iter.Waypoint!=null)
//                         iter.Waypoint.removeNeighbour(this);
//                 }
// 
//                 Neighbours.Clear();
		//    }

            public virtual List<SNeighbour> getNeighbours()
            {
 			    return Neighbours;
 		    }

            public virtual bool equals(IWaypoint waypoint)
            {
			    return ID == waypoint.getID();
		    }

            public virtual Vector3Df getPosition()
            {
			    return Position;
		    }

            public virtual void setPosition(Vector3Df pos)
            {
			    Position = pos;
		    }

            public virtual int getID()
            {
			    return ID;
		    }

            public static bool operator >(IWaypoint wypt1, IWaypoint wypt2)
            {
			    return wypt1.ID > wypt2.getID();
		    } 
				
            public static bool operator <(IWaypoint wypt1, IWaypoint wypt2)
            {
			    return wypt1.ID < wypt2.getID();
		    } 

		    public abstract void addNeighbour(IWaypoint w);

           // public abstract void removeNeighbour(IWaypoint w);

            public abstract bool hasNeighbour(IWaypoint w);

		    public static bool contains(List<IWaypoint> arr, IWaypoint waypoint)
            {
			    if (waypoint==null) 
                    return false;
			    
                for (int i = 0 ; i < arr.Count ; ++i)
				    if (arr[i] == waypoint)
                        return true;

			    return false; 
		    }

            public static bool contains(List<SNeighbour> list, IWaypoint waypoint)
            {
                foreach (SNeighbour iter in list)
                {
                    if (waypoint == iter.Waypoint)
                        return true;
                }

			    return false; 
		    }

		    public static string printWaypointIDs(IWaypoint[] arr)
            {
			    if (arr.Length == 0)
                    return "";

                string retStr = "";

			    for (int i = 0 ; i < arr.Length ; ++i) 
				    retStr+=arr[i].ID.ToString()+" ";

                return retStr;
		    }

              public static bool operator==(IWaypoint a, IWaypoint b)
              {
                  object oa = (object)a;
                  object ob = (object)b;

                  if (!(oa == null && ob == null)&&(oa==null || ob == null))
                      return false;

                  if (oa == null && ob == null)
                      return true;

                  return a.ID == b.ID;
              }
  
              public static bool operator!=(IWaypoint a, IWaypoint b)
              {
                  object oa = (object)a;
                  object ob = (object)b;

                  if (!(oa == null && ob == null) && (oa == null || ob == null))
                      return true;

                  if (oa == null && ob == null)
                      return false;

                  return a.ID != b.ID;
              }
    }
}


