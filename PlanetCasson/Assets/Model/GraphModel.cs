using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
	public class GraphModel : MonoBehaviour
	{

		public Vector3[] verticies;
		public Vector2Int[] edges;
		public HashSet<int>[] faces;
		public GameObject vertexObj;
		public GameObject edgeObj;

		private GameObject[] vObjs;
		private GameObject[] eObjs;

		
		public void Construct()
		{
			vObjs = new GameObject[verticies.Length];
			eObjs = new GameObject[edges.Length];
			for (int i = 0; i < verticies.Length; i++)
			{
				vObjs[i] = Object.Instantiate(vertexObj, verticies[i], Quaternion.identity, gameObject.transform);
			}
			for (int i = 0; i < edges.Length; i++)
			{
				eObjs[i] = Object.Instantiate(edgeObj, Vector3.zero, Quaternion.identity, gameObject.transform);
				LineRenderer lr = eObjs[i].GetComponent<LineRenderer>();
				lr.positionCount = 2;
				lr.SetPosition(0, verticies[edges[i].x]);
				lr.SetPosition(1, verticies[edges[i].y]);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}

	public class Vertex
	{
		public Vector3 pos;
		public List<Edge> origEdges; //edges with orig at this vertex
		public List<Edge> destEdges; //edges with dest at this vertex

		//private constructor, use NewVertex factory
		private Vertex()
		{
			origEdges = new List<Edge>();
			destEdges = new List<Edge>();
		}

		//factory for new verticies
		public static Vertex NewVertex()
		{
			return new Vertex();
		}
	}

	public class Edge
	{
        public Vector2Int connected;
		private Vertex _orig;
		private Vertex _dest;
		private Face _left;
		private Face _right;
		private Edge _sym;

		public Vertex Orig
		{
			get { return _orig; }
			set
			{
				if (_orig != null) _orig.origEdges.Remove(this);
				_orig = value;
				if (_orig != null) _orig.origEdges.Add(this);
				if (_sym._dest != null) _sym._dest.destEdges.Remove(_sym);
				_sym._dest = value;
				if (_sym._dest != null) _sym._dest.destEdges.Add(_sym);
			}
		}
		public Vertex Dest
		{
			get { return _dest; }
			set
			{
				if (_dest != null) _dest.destEdges.Remove(this);
				_dest = value;
				if (_dest != null) _dest.destEdges.Add(this);
				if (_sym._orig != null) _sym._orig.origEdges.Remove(_sym);
				_sym._orig = value;
				if (_sym._orig != null) _sym._orig.origEdges.Add(_sym);
			}
		}
		public Face Left
		{
			get { return _left; }
			set
			{
				if (_left != null) _left.edges.Remove(this);
				_left = value;
				if (_left != null) _left.edges.Add(this);
				if (_sym._right != null) _sym._right.edges.Remove(this);
				_sym._right = value;
				if (_sym._right != null) _sym._right.edges.Add(this);
			}
		}
		public Face Right
		{
			get { return _right; }
			set
			{
				if (_right != null) _right.edges.Remove(this);
				_right = value;
				if (_right != null) _right.edges.Add(this);
				if (_sym._left != null) _sym._left.edges.Remove(this);
				_sym._left = value;
				if (_sym._left != null) _sym._left.edges.Add(this);
			}
		}
		public Edge Sym
		{
			get { return _sym; }
		}

		//private constructor use NewEdge factory
		private Edge() { }

		//factory for new Edges
		public static Edge NewEdge()
		{
			Edge e1 = new Edge();
			Edge e2 = new Edge();
			e1._sym = e2;
			e2._sym = e1;
			return e1;
		}

		public void ConnectEdge(Vertex orig, Vertex dest, Face left, Face right)
		{
			Orig = orig;
			Dest = dest;
			Left = left;
			Right = right;
		}

		public void DisconnectEdge()
		{
			Orig = null;
			Dest = null;
			Left = null;
			Right = null;
		}

		//find edges whose dest is this's orig
		//from those edges select the ones in right face
		public Edge Rnext()
		{
			Right.edges.Remove(Sym);
			Edge temp = Orig.destEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(Sym);
			return temp;
		}

		//finds edges whose orig is this's dest
		//from those edges select the ones in left face
		public Edge Lnext()
		{
			Left.edges.Remove(Sym);
			Edge temp = Dest.origEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(Sym);
			return temp;
		}

		//finds edges with same orig as this
		//from those edges select the ones in left face
		public Edge Onext()
		{
			Left.edges.Remove(this);
			Edge temp = Orig.origEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(this);
			return temp;
		}

		//finds edges with same dest as this
		//from those edges select the ones in right face
		public Edge Dnext()
		{
			Right.edges.Remove(this);
			Edge temp = Dest.destEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(this);
			return temp;
		}

		//finds edges whose orig is this's dest
		//from those edges select the ones in right face
		public Edge Rprev()
		{
			Right.edges.Remove(Sym);
			Edge temp = Dest.origEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(Sym);
			return temp;
		}
		
		//finds edge whose dest is this's orig
		//from those edges select the ones in left face
		public Edge Lprev()
		{
			Left.edges.Remove(Sym);
			Edge temp = Orig.destEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(Sym);
			return temp;
		}

		//finds edge with same orig as this
		//from those edges select the ones in right face
		public Edge Oprev()
		{
			Right.edges.Remove(this);
			Edge temp = Orig.origEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(this);
			return temp;
		}

		//finds edge with same dest as this
		public Edge Dprev()
		{
			Left.edges.Remove(this);
			Edge temp = Dest.destEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(this);
			return temp;
		}
	}

	public class Face
	{
		public HashSet<Edge> edges;

		private Face()
		{
			edges = new HashSet<Edge>();
		}

		public static Face NewFace()
		{
			return new Face();
		}
	}

	//the graph representation
	public class Cell
	{
		List<Vertex> verticies;
		List<Edge> edges;
		List<Face> faces;

		private Cell()
		{
			verticies = new List<Vertex>();
			edges = new List<Edge>();
			faces = new List<Face>();
		}

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
            for(int i = 0; i < verticies.Count; i++)
            {
                verticies[i].pos = new Vector3(i%2, i%3, i%5); //This is a really dumb way to visualize graphs and we need to figure out something better
            }

            for(int i = 0; i < edges.Count; i ++)
            {
                int dIndex = verticies.IndexOf(edges[i].Dest);
                int oIndex = verticies.IndexOf(edges[i].Orig);
                edges[i].connected = new Vector2Int(oIndex, dIndex);
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
                lr.SetPosition(0, verticies[edges[i].connected.x].pos);
                lr.SetPosition(1, verticies[edges[i].connected.y].pos);
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