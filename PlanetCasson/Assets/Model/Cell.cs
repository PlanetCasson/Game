using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
			c.verticies.Add(Vertex.NewVertex());
			c.verticies.Add(Vertex.NewVertex());
			c.verticies.Add(Vertex.NewVertex());

			c.faces = new List<Face>();
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());
			c.faces.Add(Face.NewFace());

			c.edges = Edge.ConnectTetraCell(c.verticies, c.faces);
			if (c.edges == null) return null;
			return c;
		}

		public void calculatePositions()
		{
			for (int i = 0; i < verticies.Count; i++)
			{
				verticies[i].pos = new Vector3(i % 2, i % 3, i % 5); //This is a really dumb way to visualize graphs and we need to figure out something better
			}
		}

		public void instantiateGraph(MonoBehaviour obj, GameObject vertexObj, GameObject edgeObj)
		{
			GameObject[] vObjs = new GameObject[verticies.Count];
			GameObject[] eObjs = new GameObject[edges.Count];
			for (int i = 0; i < verticies.Count; i++)
			{
				vObjs[i] = Object.Instantiate(vertexObj, verticies[i].pos, Quaternion.identity, obj.gameObject.transform);
			}
			for (int i = 0; i < edges.Count; i++)
			{
				eObjs[i] = Object.Instantiate(edgeObj, Vector3.zero, Quaternion.identity, obj.gameObject.transform);
				LineRenderer lr = eObjs[i].GetComponent<LineRenderer>();
				lr.positionCount = 2;
				Vertex orig = (edges[i].Orig) as Vertex;
				Vertex dest = (edges[i].Dest) as Vertex;
				lr.SetPosition(0, orig.pos);
				lr.SetPosition(1, dest.pos);
			}
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
			edges.Add(newe);
			return newe;
		}

		//undos makeFaceEdge
		public void killFaceEdge(Face f, Vertex orig, Vertex dest)
		{

			List<Edge> moveE = findMoveEdges(f.EdgeListHead, dest, orig);

			Edge.RejoinFaceVertex(moveE.Last().Onext().Dest, f, dest, orig, moveE);

			if (!edges.Remove(moveE.Last().Onext().Sym))
				edges.Remove(moveE.Last().Onext());
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
