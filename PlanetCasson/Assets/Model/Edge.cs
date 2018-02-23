using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Model
{
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
}
