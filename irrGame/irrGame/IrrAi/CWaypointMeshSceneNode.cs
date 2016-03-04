using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.Video;
using IrrlichtLime.Core;

using IrrGame.IrrAi.Interface;

namespace IrrGame.IrrAi
{
    
    public class CWaypointMeshSceneNode : SceneNode 
    {
      	private List<MeshBuffer> CubeBuffers;
		private List<MeshBuffer> LinkBuffers;
		private List<TextSceneNode> TextNodes;
		private Color Colour;
		private AABBox BoundingBox;
		private int WaypointSize;
		private int CurrentBuffer;

        private void addNewBuffers()
        {
            MeshBuffer buffer1 = MeshBuffer.Create(VertexType.Standard, IndexType._16Bit);

            buffer1.Material.Lighting = false;

            CubeBuffers.Add(buffer1);

            MeshBuffer buffer2 = MeshBuffer.Create(VertexType.Standard, IndexType._16Bit);
            buffer2.Material.Lighting = false;
            buffer2.Material.BackfaceCulling = false;

            LinkBuffers.Add(buffer2);
        }

        private void appendCube(Vector3Df pos)
        {
            MeshBuffer buffer = MeshBuffer.Create(VertexType.Standard, IndexType._16Bit);
    
            // Vertices
            Vertex3D[] vertices = new Vertex3D[12];
            vertices[0] = new Vertex3D(0,0,0, -1,-1,-1, Colour, 0, 1);
            vertices[1] = new Vertex3D(1,0,0,  1,-1,-1, Colour, 1, 1);
            vertices[2]  = new Vertex3D(1,1,0,  1, 1,-1, Colour, 1, 0); 
	        vertices[3]  = new Vertex3D(0,1,0, -1, 1,-1, Colour, 0, 0); 
	        vertices[4]  = new Vertex3D(1,0,1,  1,-1, 1, Colour, 0, 1); 
	        vertices[5]  = new Vertex3D(1,1,1,  1, 1, 1, Colour, 0, 0); 
	        vertices[6]  = new Vertex3D(0,1,1, -1, 1, 1, Colour, 1, 0); 
	        vertices[7]  = new Vertex3D(0,0,1, -1,-1, 1, Colour, 1, 1); 
	        vertices[8]  = new Vertex3D(0,1,1, -1, 1, 1, Colour, 0, 1); 
	        vertices[9]  = new Vertex3D(0,1,0, -1, 1,-1, Colour, 1, 1); 
	        vertices[10] = new Vertex3D(1,0,1,  1,-1, 1, Colour, 1, 0); 
	        vertices[11] = new Vertex3D(1,0,0,  1,-1,-1, Colour, 0, 0); 
            
            for (int i = 0 ; i < 12 ; ++i)
            {
		        vertices[i].Position -= new Vector3Df(0.5f, 0.0f, 0.5f); // don't alter Y position
		        vertices[i].Position *= (float)WaypointSize;
	 	        vertices[i].Position += pos;
	        }     

            buffer.Append(vertices,new ushort[]{ 0,2,1, 0,3,2, 1,5,4, 1,2,5,   4,6,7, 4,5,6, 
                            7,3,0, 7,6,3, 9,5,2, 9,8,5, 0,11,10, 0,10,7 });
            
	        CubeBuffers[CurrentBuffer].Append(buffer);
	        CubeBuffers[CurrentBuffer].BoundingBox.AddInternalPoint(vertices[0].Position);
            CubeBuffers[CurrentBuffer].BoundingBox.AddInternalPoint(vertices[5].Position);
        }

        private void appendLink(Vector3Df from, Vector3Df to, Vector3Df offset)
        {
            Vector3Df NORM = new Vector3Df(0,1,0);
            Vector2Df TC = new Vector2Df(0,0);
            int NUM_INDICES = 9;

            MeshBuffer buffer = MeshBuffer.Create(VertexType.Standard, IndexType._16Bit); ;
  
	        // Vertices  
	        Vector3Df vec = new Line3Df(from, to).Vector.Normalize()*(float)WaypointSize;
	        Vector3Df cross = vec.CrossProduct(NORM).Normalize(); 

	        // Arrow head
            Vertex3D[] vertices = new Vertex3D[7];
            vertices[0] = new Vertex3D((to-vec/2.0f) + offset,											NORM, Colour, TC);                            
	        vertices[1] = new Vertex3D((to-vec/2.0f) + ((cross*(float)(-WaypointSize))-vec*1.5f) + offset,	NORM, Colour, TC);                            
	        vertices[2] = new Vertex3D((to-vec/2.0f) + ((cross*(float)WaypointSize)-vec*1.5f) + offset,	NORM, Colour, TC);              
	        // Arrow body              
	        vertices[3] = new Vertex3D((from+vec*2.0f) + (cross*(float)(-WaypointSize)*0.25f) + offset,		NORM, Colour, TC);                            
	        vertices[4] = new Vertex3D((from+vec*2.0f) + (cross*(float)WaypointSize*0.25f) + offset,		NORM, Colour, TC); 
	        vertices[5] = new Vertex3D((to-vec*2.0f) + (cross*(float)(-WaypointSize)*0.25f) + offset,		NORM, Colour, TC);                            
	        vertices[6] = new Vertex3D((to-vec*2.0f) + (cross*(float)WaypointSize*0.25f) + offset,			NORM, Colour, TC);
    
            buffer.Append(vertices,new ushort[]{ 0,1,2, 3,4,6, 6,5,3  });

	
  
	        LinkBuffers[CurrentBuffer].Append(buffer);

        }

        private void addTextNode(Vector3Df pos, int id)
        {
	        TextSceneNode textNode = this.SceneManager.AddTextSceneNode(
                this.SceneManager.GUIEnvironment.Skin.GetFont(IrrlichtLime.GUI.GUIDefaultFont.Default), 
                id.ToString(), new Color(255, 255-Colour.Red, 255-Colour.Green, 255-Colour.Blue), this.Parent, pos);
	        TextNodes.Add(textNode);
        }
	
        public static int WAYPOINTMESH_SCENE_NODE_ID = 1001;

        public CWaypointMeshSceneNode(SceneNode parent, SceneManager smgr, int id, Color colour, int size):base(parent, smgr, -1)
        {
            CubeBuffers = new List<MeshBuffer>();
		    LinkBuffers = new List<MeshBuffer>();
            TextNodes = new List<TextSceneNode>();
            BoundingBox = new AABBox();

            colour = new Color(255, 255, 255, 255);
            size = 5;
            id = -1;

            WaypointSize = size;

            CurrentBuffer = 0;

            addNewBuffers();

            setColour(colour);


            base.OnRegisterSceneNode += new RegisterSceneNodeEventHandler(OnRegisterSceneNode);
	
        }

		~CWaypointMeshSceneNode() {}

        public void remove()
        {
            clear();
  
	        base.Remove(); 
        }

        public virtual void OnRegisterSceneNode()
        {
            if (Visible)
		        this.SceneManager.RegisterNodeForRendering(this,SceneNodeRenderPass.Solid);
        }

        public virtual void render()
        {
           // VideoDriver driver = this.SceneManager.VideoDriver;

    for (int i = 0 ; i < CubeBuffers.Count ; ++i) 
		if (CubeBuffers[i].VertexCount>0 && CubeBuffers[i].IndexCount>0)
        {
			// Draw waypoints (cubes)
			VideoDriver driver = this.SceneManager.VideoDriver;
			driver.SetMaterial(CubeBuffers[i].Material);
			driver.SetTransform(TransformationState.World, AbsoluteTransformation);
			driver.DrawMeshBuffer(CubeBuffers[0]); 
		}
	  
	for (int i = 0 ; i < LinkBuffers.Count() ; ++i) 
		if (LinkBuffers[i].VertexCount>0 && LinkBuffers[i].IndexCount>0)
        {
			// Draw links (arrows)
			VideoDriver driver = this.SceneManager.VideoDriver;
			driver.SetMaterial(LinkBuffers[i].Material);
            driver.SetTransform(TransformationState.World, AbsoluteTransformation);
			driver.DrawMeshBuffer(LinkBuffers[0]); 
		}
        }

        public virtual Material getMaterial(int i)
        {
            switch (i)
            {
                case 0: return CubeBuffers[0].Material;
                case 1: return LinkBuffers[0].Material;
                default: return CubeBuffers[0].Material;
            }
        }

        public virtual SceneNode clone(SceneNode newParent, SceneManager newManager)
        {
            if (newParent==null) 
                newParent = Parent;
            if (newManager==null)
                newManager = this.SceneManager;

            CWaypointMeshSceneNode nb = new CWaypointMeshSceneNode(newParent, newManager, ID, Colour, WaypointSize);

            nb.MemberwiseClone();

            nb.Drop();

            return nb;
        }

        public void addWaypoint(IWaypoint waypoint)
        {
            if (waypoint==null) 
                return;
 
	        if (CubeBuffers.Count == 0)
            {
		        addNewBuffers();
		        CurrentBuffer = 0;
	        }

	        if (CubeBuffers[CurrentBuffer].VertexCount + 12 >= 65536 || LinkBuffers[CurrentBuffer].VertexCount +
                (7 * waypoint.getNeighbours().Count) >= 65536)
            {
		        addNewBuffers();
		        ++CurrentBuffer;
	        }

	        appendCube(waypoint.getPosition());
 
	        Vector3Df offset = new Vector3Df(0,WaypointSize/2.0f,0);
  
	        addTextNode(waypoint.getPosition() + offset, waypoint.getID());
  
            foreach (SNeighbour nbrIter in waypoint.getNeighbours())
            {
                appendLink(waypoint.getPosition(), nbrIter.Waypoint.getPosition(), offset);
            }
	
        }

        public void setColour(Color colour)
        {
            Colour = colour;

            for (int b = 0; b < CubeBuffers.Count; ++b)
            {
                for (int v = 0; v < CubeBuffers[b].VertexCount; ++v)
                    ((Vertex3D)(CubeBuffers[b].GetVertex(v))).Color = Colour;
                CubeBuffers[b].Material.DiffuseColor = Colour;
            }

            for (int b = 0; b < LinkBuffers.Count; ++b)
            {
                for (int v = 0; v < LinkBuffers[b].VertexCount; ++v)
                    ((Vertex3D)(LinkBuffers[b].GetVertex(v))).Color = Colour;
                LinkBuffers[b].Material.DiffuseColor = Colour;
            }

            for (int b = 0; b < TextNodes.Count; ++b)
            {
                TextNodes[b].SetTextColor(new Color(255, 255 - Colour.Red, 255 - Colour.Green, 255 - Colour.Blue));       
	        }     
        
        }

        public void clear()
        {
            for (int i = 0; i < CubeBuffers.Count; ++i)
                CubeBuffers[i].Drop();
            CubeBuffers.Clear();

            for (int i = 0; i < LinkBuffers.Count; ++i)
                LinkBuffers[i].Drop();
            LinkBuffers.Clear();

            for (int i = 0; i < TextNodes.Count; ++i)
                TextNodes[i].Remove();
            TextNodes.Clear();

            CurrentBuffer = 0;
        }
				
		public virtual int getMaterialCount() { return 2; }
		public virtual AABBox getBoundingBox() { return BoundingBox; }    
        public SceneNodeType getType() { return (SceneNodeType)WAYPOINTMESH_SCENE_NODE_ID; }    
        public void recalculateBoundingBox() 
        {              
			for (int i = 0 ; i < CubeBuffers.Count ; ++i) 
				BoundingBox.AddInternalBox(CubeBuffers[i].BoundingBox);
        }
        public void setWaypointSize(int size) { WaypointSize = size; }
        

		
    }
}

