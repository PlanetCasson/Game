using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Model.Objects;

/// <summary>
/// Package defining the implementation of the modified Quad-Edge data structure
/// </summary>
namespace Model
{
	/// <summary>
	/// Quad-Edge data structure representing the graph iteself.
	/// <para>The Cell is a Quad-Edge structure that contains a list of edges, verticies, and faces.
	/// It is capable of representing all surfaces that are locally homeomorphic to the plane.
	/// It is encoded in such a way that it contains both the graph itself and its dual graph.
	/// For more details on how this encoding is carried out, see Edge.</para>
	/// <para>Benefits of this data structure is the easy and fast access of locality data and the O(1) CW and CCW traversal around points/faces.
	/// This second point will be extremely important in our game about CCW traversal on graphs embedded in surfaces.</para>
	/// \image html cellmodification.jpeg
	/// </summary>
	public class Cell
	{
		List<Vertex> verticies;
		List<Edge> edges;
		List<Face> faces;

		//make a tetrahedron quad edge graph
		public static Cell MakePrimitiveCell()
		{
			Cell c = new Cell();
			c.verticies = new List<Vertex>();
			c.verticies.Add(Vertex.NewVertex());
            c.verticies[0].pos = new Vector3(0, 0, 40);
			c.verticies.Add(Vertex.NewVertex());
            c.verticies[1].pos = new Vector3(30, 0, 0);
            c.verticies.Add(Vertex.NewVertex());
            c.verticies[2].pos = new Vector3(-20, -20, 0);
            c.verticies.Add(Vertex.NewVertex());
            c.verticies[3].pos = new Vector3(-20, 20, 0);

            c.faces = new List<Face>();
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());

			c.edges = Edge.ConnectTetraCell(c.verticies, c.faces);
			c.makeVertexEdge(c.verticies[0], c.faces[1], c.faces[2]);
			c.makeFaceEdge(c.faces[2], c.verticies.Last(), c.verticies[2]);
			c.killFaceEdge(c.faces.Last(), c.verticies.Last(), c.verticies[2]);
			c.killVertexEdge(c.verticies.Last(), c.faces[1], c.faces[2]);
			return c;
		}

		public void calculatePositions(GameObject[] vObjs, GameObject[] eObjs, GameObject[] fObjs)
        {
            for (int i = 0; i < verticies.Count; i++)
            {
                vObjs[i].transform.position = verticies[i].pos;
            }
            for (int i = 0; i < edges.Count; i++)
            {
                LineRenderer lr = eObjs[i].GetComponent<LineRenderer>();
                lr.positionCount = 2;
                Vertex orig = (edges[i].Orig) as Vertex;
                Vertex dest = (edges[i].Dest) as Vertex;
                lr.SetPosition(0, orig.pos);
                lr.SetPosition(1, dest.pos);
            }
            for (int i = 0; i < faces.Count; i++)
            {
                Vector3 sum = new Vector3(0, 0, 0);
                int count = 0;
                Edge start = faces[i].EdgeListHead.Onext();
                Edge current = start;
                do
                {
                    sum += (current.Right as Vertex).pos; //right should be the vertex that's origin of the edge's dual
                                                          //the edge's dual edge is a CCW pointing edge bordering faces[i]
                    current = current.Onext(); //Onext traversal finds the next edge in CCW dir that points out of face
                    count++;
                } while (current != start);
                Vector3 avg = sum / count;
                fObjs[i].transform.position = avg;
            }
        }

		public GameObject[][] instantiateGraph(SphereKernel kernel, GameObject vertexObj, GameObject edgeObj, GameObject faceObj)
		{
			GameObject[] vObjs = new GameObject[verticies.Count];
			GameObject[] eObjs = new GameObject[edges.Count];
			GameObject[] fObjs = new GameObject[faces.Count];

			for (int i = 0; i < verticies.Count; i++)
			{
				vObjs[i] = Object.Instantiate(vertexObj, verticies[i].pos, Quaternion.identity, kernel.gameObject.transform);
                vObjs[i].GetComponent<VertexObject>().graph = this;
                vObjs[i].GetComponent<VertexObject>().vertex = verticies[i];
                vObjs[i].GetComponent<VertexObject>().sphereKernel = kernel;

            }
			for (int i = 0; i < edges.Count; i++)
			{
				eObjs[i] = Object.Instantiate(edgeObj, Vector3.zero, Quaternion.identity, kernel.gameObject.transform);
				LineRenderer lr = eObjs[i].GetComponent<LineRenderer>();
				lr.positionCount = 2;
				Vertex orig = (edges[i].Orig) as Vertex;
				Vertex dest = (edges[i].Dest) as Vertex;
				lr.SetPosition(0, orig.pos);
				lr.SetPosition(1, dest.pos);
			}
			for (int i = 0; i < faces.Count; i++)
			{
				Vector3 sum = new Vector3(0, 0, 0);
                int count = 0;
				Edge start = faces[i].EdgeListHead.Onext();
				Edge current = start;
				do
				{
					sum += (current.Right as Vertex).pos; //right should be the vertex that's origin of the edge's dual
														  //the edge's dual edge is a CCW pointing edge bordering faces[i]
					current = current.Onext(); //Onext traversal finds the next edge in CCW dir that points out of face
                    count++;
				} while (current != start);
                Vector3 avg = sum / count;
                fObjs[i] = Object.Instantiate(faceObj, avg, Quaternion.identity, kernel.gameObject.transform);
            }
            return new GameObject[3][] { vObjs, eObjs, fObjs };
		}

		//splits a vertex and create a new edge in between
		//preserves euler characteristics
		//use this to change graph
		public Edge makeVertexEdge(Vertex v, Face left, Face right)
		{
			//finds all edges that needs to be moved
			List<Edge> moveE = findMoveEdges(v.EdgeListHead, left, right);

			if (moveE == null) return null;

			Vertex newv = Vertex.NewVertex();
			Edge newe = Edge.SplitFaceVertex(v, newv, left, right, moveE);
			verticies.Add(newv);
			edges.Add(newe);
			return newe;
		}

		//"unsplits" a vertex
		//preserves euler characteristics
		//use this to change graph
		//can create digons(use carefully)
		public void killVertexEdge(Vertex v, Face left, Face right)
		{
			List<Edge> moveE = findMoveEdges(v.EdgeListHead, left, right);

			Edge.RejoinFaceVertex(moveE.Last().Onext().Dest, v, left, right, moveE);

			if (!edges.Remove(moveE.Last().Onext().Sym))
				edges.Remove(moveE.Last().Onext());
			verticies.Remove(v);
		}

		//subdivide a face
		//preserves euler characteristics
		//use this to change graph
		public Edge makeFaceEdge(Face f, Vertex orig, Vertex dest)
		{
			//finds all edges that needs to be moved
			List<Edge> moveE = findMoveEdges(f.EdgeListHead, dest, orig);

			Face newf = Face.NewFace();
			Edge newe = Edge.SplitFaceVertex(f, newf, dest, orig, moveE);
			faces.Add(newf);
			edges.Add(newe.Rot);
			return newe;
		}

		//undos makeFaceEdge
		public void killFaceEdge(Face f, Vertex orig, Vertex dest)
		{

			List<Edge> moveE = findMoveEdges(f.EdgeListHead, dest, orig);
			Edge delE = moveE.Last().Onext().Sym;
			Edge.RejoinFaceVertex(delE.Orig, f, dest, orig, moveE);

			if (!edges.Remove(delE.Rot))
				edges.Remove(delE.InvRot);
			faces.Remove(f);
		}

		//find all edges pointing away from orig of start
		//that is also between left and right
		//that is also on "top"
		private List<Edge> findMoveEdges(Edge start, FaceVertex left, FaceVertex right)
		{
			Edge temp = start;
			while (temp.Right != right)
			{
				temp = temp.Onext();
				if (temp == start) //made a loop
				{
					return null;
				}
			}
			List<Edge> moveE = new List<Edge>();
			moveE.Add(temp);
			while (temp.Left != left)
			{
				temp = temp.Onext();
				moveE.Add(temp);
				if (temp == moveE[0]) //made a loop
				{
					return null;
				}
			}
			if (moveE.Last().Left != (FaceVertex)left)
				moveE.RemoveAt(moveE.Count - 1);

			return moveE;
		}
	}
}
