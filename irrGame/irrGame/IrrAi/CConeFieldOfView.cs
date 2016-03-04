using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace IrrGame.IrrAi
{
    public class CConeFieldOfView: IFieldOfView 
    {
        public CConeFieldOfView(IAIManager aimgr, bool occlusionCheck, Vector3Df dim): 
            base(aimgr, occlusionCheck, dim) 
        {
	        setDimensions(dim);
        }

        ~CConeFieldOfView() { }

        //public bool isInFOV(Vector3Df pos) {}

        public override bool isInFOV(AABBox box, Vector3Df boxPos)
        {
	        Line3Df line = new Line3Df();
            
	        getLeftViewLine(line);
	        Vector3Df leftPoint = line.GetClosestPoint(boxPos);
	        getRightViewLine(line);
	        Vector3Df rightPoint = line.GetClosestPoint(boxPos);
	        Vector3Df extent = box.Extent;


	        if (Position.GetDistanceFrom(boxPos) < Dimensions.Y + (extent.X > extent.Z ? extent.X : extent.Z))
            {
		        leftPoint.Y = boxPos.Y;
		        rightPoint.Y = boxPos.Y;

		        if ( boxPos.IsBetweenPoints(leftPoint, rightPoint)
			        || box.IsInside(leftPoint)
			        || box.IsInside(rightPoint)) 
                {
			        Line3Df ray = new Line3Df(Position, boxPos);

			        if (OcclusionCheck && AIManager != null && AIManager.occlusionQuery(ray))
				        return false;
			        else
				        return true;
		        }
	        }
  
	        return false;
        }


        public void setDimensions(Vector3Df dim)
        {
            base.setDimensions(dim);

            Vertices[0] = new Vertex3D(0, 0, 0, 0, 1, 0, new Color(100, 255, 0, 0), 0, 1);
	        Vertices[1] = new Vertex3D(dim.Y,0,dim.X, 0,1,0, new Color(100,255,0,0), 1, 1);
	        Vertices[2] = new Vertex3D(dim.Y,0,-dim.X, 0,1,0, new Color(100,255,0,0), 0, 0);
        }
		
	    private Vertex3D[] Vertices = new Vertex3D[3];

        private void getLeftViewLine(Line3Df line)
        {
            Vector3Df endPoint = Vertices[2].Position;
            Matrix rotMat = new Matrix(), transMat = new Matrix(), combMat = new Matrix();
            rotMat.Rotation = Rotation;
            transMat.Translation = Position;
            combMat = transMat * rotMat;
            combMat.TransformVector(ref endPoint);
            line.Start = Position;
            line.End = endPoint;
        }

        private void getRightViewLine(Line3Df line)
        {
            Vector3Df endPoint = Vertices[1].Position;
            Matrix rotMat = new Matrix(), transMat = new Matrix(), combMat = new Matrix();
            rotMat.Rotation = Rotation;
            transMat.Translation = Position;
            combMat = transMat * rotMat;
            combMat.TransformVector(ref endPoint);
            line.Start = Position;
            line.End = endPoint;
        }

    }
}
