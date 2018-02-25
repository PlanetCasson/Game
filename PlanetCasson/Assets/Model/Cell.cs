using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

/// <summary>
/// Package defining the implementation of the modified Quad-Edge data structure
/// </summary>
namespace Model
{
	/// <summary>
	/// Modified Quad-Edge data structure representing the graph iteself
	/// <para></para>
	/// \image html cellmodification.jpeg
	/// </summary>
	public class Cell
	{
		List<Vertex> verticies;
		List<Edge> edges;
		List<Face> faces;

		//private constructor to initilize a cell
		private Cell()
		{
			verticies = new List<Vertex>();
			edges = new List<Edge>();
			faces = new List<Face>();
		}

		//make a tetrahedron quad edge graph
		public static Cell MakePrimitiveCell()
		{
			Vertex vx = Vertex.NewVertex(); //Center Vertex
			Vertex vab = Vertex.NewVertex();
			Vertex vac = Vertex.NewVertex();
			Vertex vcb = Vertex.NewVertex();

			Edge ea = Edge.NewEdge();
			Edge eb = Edge.NewEdge();
			Edge ec = Edge.NewEdge();
			Edge eacx = Edge.NewEdge();
			Edge ecbx = Edge.NewEdge();
			Edge eabx = Edge.NewEdge();

			Face fa = Face.NewFace();
			Face fb = Face.NewFace();
			Face fc = Face.NewFace();
			Face fx = Face.NewFace();

			ea.ConnectEdge(vac, vab, fx, fa);
			eb.ConnectEdge(vab, vcb, fx, fb);
			ec.ConnectEdge(vcb, vac, fx, fc);
			eacx.ConnectEdge(vac, vx, fa, fc);
			ecbx.ConnectEdge(vcb, vx, fc, fb);
			eabx.ConnectEdge(vab, vx, fb, fa);

			Cell sphereCell = new Cell();
			sphereCell.verticies.AddRange(new List<Vertex>() { vx, vab, vac, vcb });
			sphereCell.faces.AddRange(new List<Face>() { fa, fb, fc, fx });
			sphereCell.edges.AddRange(new List<Edge>() { ea, eb, ec, eacx, ecbx, eabx });

			return sphereCell;
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
				lr.SetPosition(0, edges[i].Orig.pos);
				lr.SetPosition(1, edges[i].Dest.pos);
			}
		}

		//splits a vertex and create a new edge in between
		//preserves euler characteristics
		//use this to change graph
		public Edge makeVertexEdge(Vertex v, Face left, Face right)
		{
			//all edges are edges that points towards v

			//find edges that points towards v and boarders left and right
			Edge[] leftEdges = (Edge[])v.destEdges.Intersect(left.edges);
			Edge[] rightEdges = (Edge[])v.destEdges.Intersect(right.edges);

			//checks if there are 2 leftEdges and 2 rightEdges (as there should be)
			if (leftEdges.Length != 2 && rightEdges.Length != 2)
				return null;

			List<Edge> topEdges = new List<Edge>();

			//tests to see if leftEdges[0] is the bottom edge
			if (leftEdges[0].Dnext() == leftEdges[1])
				topEdges.Add(leftEdges[0]);
			else
				topEdges.Add(leftEdges[1]);

			//test to see if rightEdges[0] is the bottom edge
			if (rightEdges[0].Dprev() == rightEdges[1])
				topEdges.Add(rightEdges[0]);
			else
				topEdges.Add(rightEdges[1]);

			//fills top edges
			while (true)
			{
				int i = topEdges.Count - 2;
				Edge temp = topEdges[i].Dprev();
				if (temp == topEdges.Last())
				{
					break;
				}
				topEdges.Insert(i + 1, temp);
			}

			Vertex newV = Vertex.NewVertex();
			for (int i = 0; i < topEdges.Count; i++)
			{
				topEdges[i].Dest = newV;
			}
			Edge newE = Edge.NewEdge();
			newE.ConnectEdge(v, newV, left, right);
			verticies.Add(newV);
			edges.Add(newE);
			return newE;
		}

		//"unsplits" a vertex
		//preserves euler characteristics
		//use this to change graph
		//can create digons(use carefully)
		public void killVertexEdge(Vertex v, Face left, Face right)
		{
			Edge disposedEdge = null;
			Vertex rebindV;

			foreach (Edge e in left.edges.Intersect(right.edges))
			{
				if (e.Dest == v)
				{
					disposedEdge = e;
					break;
				}
			}

			//check to see if valid deletion
			if (disposedEdge == null) return;

			rebindV = disposedEdge.Orig;

			//disconnect the edge
			disposedEdge.DisconnectEdge();

			//rebind edges on v to the vertex at disposedEdge.Orig
			foreach (Edge e in v.destEdges)
			{
				e.Dest = rebindV;
			}

			edges.Remove(disposedEdge);
			verticies.Remove(v);
		}

		//subdivide a face
		//preserves euler characteristics
		//use this to change graph
		public Edge makeFaceEdge(Face f, Vertex orig, Vertex dest)
		{
			//find an edge pointing away from the starting point and on f
			List<Edge> fEdges = (List<Edge>)orig.origEdges.Intersect(f.edges);
			//check that there should be 2
			if (fEdges.Count != 2)
				return null;
			//remove edge pointing left
			if (fEdges[0].Oprev() == fEdges[1])
				fEdges.Remove(fEdges[0]);
			else
				fEdges.Remove(fEdges[1]);

			//check to see if already hit dest
			if (fEdges.Last().Dest != dest)
			{
				//accumulate traversed edges until hit dest
				do
				{
					fEdges.Add(fEdges.Last().Lnext());
				} while (fEdges.Last().Dest != dest);
			}
			//fEdges should now contains all edges(pointing in ccw dir) for new face except new edge

			Face newF = Face.NewFace();
			for (int i = 0; i < fEdges.Count; i++)
			{
				fEdges[i].Left = newF;
			}
			Edge newE = Edge.NewEdge();
			newE.ConnectEdge(orig, dest, f, newF);
			faces.Add(newF);
			edges.Add(newE);
			return newE;
		}

		//undos makeFaceEdge
		public void killFaceEdge(Edge e)
		{

			Face rf = e.Right;
			Face lf = e.Left;
			e.DisconnectEdge();
			foreach (Edge e1 in rf.edges)
			{
				e1.Left = lf;
			}
			edges.Remove(e);
			faces.Remove(rf);
		}
	}
}
