using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrAi
{
    public class CWaypoint: IWaypoint 
    {
        public CWaypoint(int id, Vector3Df position)
            : base(id, position)
        {
        }

        public override void addNeighbour(IWaypoint w)
        {
            if (w==null) 
                return;
  
            if (!contains(Neighbours, w))
            {
                string currStr = NeighbourString;
                string idStr = w.getID().ToString();

                if (!currStr.Contains(idStr))
                {
                    if (currStr.Length > 0)
                        currStr += ',';

                    currStr += idStr;
                    NeighbourString = currStr;
                }

                SNeighbour n;
                n.Waypoint = w;
                n.Distance = Position.GetDistanceFrom(w.getPosition());
                Neighbours.Add(n);   
            }
        }

       // public override void removeNeighbour(IWaypoint w)
       // {
//              if (w==null)
//                  return;
// 
//             foreach (SNeighbour iter in Neighbours)
//             {
//                 if(iter.Waypoint.getID() == w.getID())
//                 {
//                     string currStr = NeighbourString;
//                     string idStr = w.getID().ToString();
// 
//                     currStr = currStr.Replace(idStr, "");
//                     currStr = currStr.Replace(",,", ",");
// 
//                     if (currStr.Length > 0)
//                         if (currStr[currStr.Length - 1] == ',')
//                             if (currStr == ",")
//                                 currStr = "";
//                             else
//                                 currStr = currStr.Substring(0, currStr.Length - 2);
// 
//                     NeighbourString = currStr;
//                     Neighbours.Remove(iter);
// 
//                     return;
//                 }
//             }
// 
//             return;
       // }

        public override bool hasNeighbour(IWaypoint w)
        {
            return contains(Neighbours, w);
        }

        public void setID(int id) { ID = id; }
		public void setNeighbourString(string str) { NeighbourString = str; }
		public string getNeighbourString() { return NeighbourString; }
    }
}
