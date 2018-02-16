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

		// Use this for initialization
		void Start()
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
		Vector3 pos;
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
	}

	public class Cell
	{
		Vertex[] verticies;
		uint[] vid;
		Face[] faces;
		uint[] fid;

		//splits a vertex and create a new edge in between
		//preserves euler characteristics
		//use this to change graph
		public Edge makeVertexEdge(Vertex v, Face left, Face right)
		{
			//all edges are edges that points towards v

			//find edges that points towards v and boarders left and right
			Edge[] leftEdges = (Edge[])v.destEdges.Intersect(left.edges);
			Edge[] rightEdges = (Edge[])v.destEdges.Intersect(right.edges);

			List<Edge> topEdges = new List<Edge>();

			//checks if there are 2 leftEdges and 2 rightEdges (as there should be)
			if (leftEdges.Length != 2 && rightEdges.Length != 2)
				return null;

			//tests to see if leftEdges[0] is the bottom edge
			if (leftEdges[0].Dnext() == leftEdges[1])
				topEdges.Add(leftEdges[1]);
			else
				topEdges.Add(leftEdges[0]);

			//test to see if rightEdges[0] is the bottom edge
			if (rightEdges[0].Dprev() == rightEdges[1])
				topEdges.Add(rightEdges[1]);
			else
				topEdges.Add(rightEdges[0]);

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
			return newE;
		}

		//"unsplits" a vertex
		//preserves euler characteristics
		//use this to change graph
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

			//disconnect the edge (get ready for garbage collection)
			disposedEdge.DisconnectEdge();

			//rebind edges on v to the vertex at disposedEdge.Orig
			foreach (Edge e in v.destEdges)
			{
				e.Dest = rebindV;
			}
		}

		public Edge makeFaceEdge(Face f, Vertex orig, Vertex dest)
		{
			
		}
	}
}