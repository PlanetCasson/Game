using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Edge representation in the modified Quad-Edge data structure.
	/// <para>Each Edge is a directed edge in the Quad-Edge, and also has a corresponding symmetric edge.</para>
	/// The following diagram shows the relation between an edge and its symmetric edge:</para>
	/// \image html SymEdges.jpg
	/// <para>The edge and its symmetric edge come together to form an undirected edge.
	/// The properties and methods in the Edge class modify both the edge and its symmetric edge.
	/// In essense, any modification to an instance of the Edge class is modifying the undirected
	/// edge formed from the edge itself and its linked symmetric edge</para>
	/// <para>Each Edge, in addition to its symmetric edge, also stores its nearby verticies and faces, which are defined below:</para>
	/// \image html vertface.jpeg
	/// <para>Due to the linked nature of an edge and its symmetric edge, only the origin vertex, left face, and symmetric edge needs to be stored.
	/// An edge's destination vertex can be gotten from its symmetric edge's origin, and an edge's right face can be gotten from its symmetric edge's right face.</para>
	/// <para>This information is used to facilitate the fast edge traversal of the graph through the following functions defined in the image below:</para>
	/// \image html edgetraversal.jpeg
	/// <para>the last 2 images comes from <a href="https://www.cs.cmu.edu/afs/andrew/scs/cs/15-463/2001/pub/src/a2/quadedge.html">here</a>. All the credit is theirs</para>
	/// </summary>
	public class Edge
	{
		private Vertex _orig;
		private Face _left;
		private Edge _sym;

		/// <summary>
		/// <para>The origin vertex of the edge, and the destination vertex of the corresponding symmetric edge, Sym.
		/// On modification of this property, the following actions are taken to maintain the correctness
		/// of the Quad-Edge structure.</para>
		/// <list type="bullet">
		/// <item><description>removes itself from the <see cref="Vertex.origEdges"/> collection
		/// and removes <see cref="Sym"/> from the <see cref="Vertex.destEdges"/> collection in Orig</description></item>
		/// <item><description>replaces Orig with a new Vertex</description></item>
		/// <item><description>adds itself to the origEdges collection 
		/// and adds Sym to the destEdges collection in the new Orig</description></item>
		/// </list>
		/// <para>Note that due to <see cref="Dest"/> in Sym being just a reference to Orig in this edge, the above steps also correctly updates the Dest in Sym</para>
		/// <para>Note that this does not check for the topological or spacial correctness of the operation</para>
		/// <para>The remove and addition from/to collections are null checked. This means that it is valid to assign Orig as null</para>
		/// </summary>
		public Vertex Orig
		{
			get { return _orig; }
			set
			{
				if (_orig != null)
				{
					_orig.origEdges.Remove(this);
					_orig.destEdges.Remove(_sym);
				}
				_orig = value;
				if (_orig != null)
				{
					_orig.origEdges.Add(this);
					_orig.destEdges.Add(_sym);
				}
			}
		}
		/// <summary>
		/// <para>The destination vertex of the edge, and the origin vertex of the corresponding symmetric edge, Sym.
		/// On modification of this property, the following actions are taken to maintain the correctness
		/// of the Quad-Edge structure.</para>
		/// <list type="bullet">
		/// <item><description>removes <see cref="Sym"/> from the <see cref="Vertex.origEdges"/> collection 
		/// and remove itself from the <see cref="Vertex.destEdges"/> collection in the <see cref="Orig"/> of Sym</description></item>
		/// <item><description>replaces the Orig of Sym with the new Dest</description></item>
		/// <item><description>adds Sym to the origEdges collection 
		/// and adds itself to the destEdges collection in the new Orig of the Sym edge</description></item>
		/// </list>
		/// <para>Note that due to Dest on this edge being just a reference to Orig in Sym, the above steps correctly updates Dest</para>
		/// <para>Note that this does not check for the topological or spacial correctness of the operation</para>
		/// <para>The remove and addition from/to collections are null checked. This means that it is valid to assign Dest as null</para>
		/// <see cref="Vertex.origEdges"/>
		/// <see cref="Vertex.destEdges"/>
		/// <see cref="Orig"/>
		/// <see cref="Sym"/>
		/// </summary>
		public Vertex Dest
		{
			get { return _sym._orig; }
			set
			{
				if (_sym._orig != null)
				{
					_sym._orig.origEdges.Remove(_sym);
					_sym._orig.destEdges.Remove(this);
				}
				_sym._orig = value;
				if (_sym._orig != null)
				{
					_sym._orig.origEdges.Add(_sym);
					_sym._orig.destEdges.Add(this);
				}
			}
		}
		/// <summary>
		/// <para>The face on the left side of the edge.
		/// On modification of this property, the following actions are taken to maintain the correctness
		/// of the Quad-Edge structure.</para>
		/// <list type="bullet">
		/// <item><description>removes itself and <see cref="Sym"/> from the <see cref="Face.edges"/> collection in Left</description></item>
		/// <item><description>replaces Left with a new Face</description></item>
		/// <item><description>adds itself and Sym to the edges collection in the new Left</description></item>
		/// </list>
		/// <para>Note that due to <see cref="Right"/> in Sym being just a reference to Left in this edge, the above steps also correctly updates Right in Sym</para>
		/// <para>Note that this does not check for the topological or spacial correctness of the operation</para>
		/// <para>The remove and addition from/to collections are null checked. This means that it is valid to assign Left as null</para>
		/// </summary>
		public Face Left
		{
			get { return _left; }
			set
			{
				if (_left != null)
				{
					_left.edges.Remove(this);
					_left.edges.Remove(_sym);
				}
				_left = value;
				if (_left != null)
				{
					_left.edges.Add(this);
					_left.edges.Add(_sym);
				}
			}
		}
		/// <summary>
		/// <para>The face on the right side of the edge.
		/// On modification of this property, the following actions are taken to maintain the correctness
		/// of the Quad-Edge structure.</para>
		/// <list type="bullet">
		/// <item><description>removes itself and <see cref="Sym"/> from the <see cref="Face.edges"/> collection in Right</description></item>
		/// <item><description>replaces <see cref="Left"/> in Sym with a new Face</description></item>
		/// <item><description>adds itself and Sym to the edges collection in the new Right</description></item>
		/// </list>
		/// <para>Node that due to Right on this edge being just a reference to Left in Sym, the above steps correctly updates Right</para>
		/// <para>Note that this does not check for the topological or spacial correctness of the operation</para>
		/// <para>The remove and addition from/to collections are null checked. This means that it is valid to assign Right as null</para>
		/// <see cref="Face.edges"/>
		/// <see cref="Sym"/>
		/// </summary>
		public Face Right
		{
			get { return _sym._left; }
			set
			{
				if (_sym._left != null)
				{
					_sym._left.edges.Remove(_sym);
					_sym._left.edges.Remove(this);
				}
				_sym._left = value;
				if (_sym._left != null)
				{
					_sym._left.edges.Add(_sym);
					_sym._left.edges.Add(this);
				}
			}
		}
		/// <summary>
		/// <para>The link to the Symmetric edge of this edge. This property is read-only</para>
		/// </summary>
		public Edge Sym
		{
			get { return _sym; }
		}

		//private constructor use NewEdge factory
		private Edge() { }

		/// <summary>
		/// <para>Factory for creating new undirected edges. 
		/// This method creates an edge, and its symmetric edge, and then links them together.
		/// The newly created pair of edges are not linked to anything except for each other.</para>
		/// <para>To create an undirected edge and then connect it to a graph, see <see cref="ConnectEdge"/>.</para>
		/// </summary>
		/// <returns>The newly created Edge</returns>
		public static Edge NewEdge()
		{
			Edge e1 = new Edge();
			Edge e2 = new Edge();
			e1._sym = e2;
			e2._sym = e1;
			return e1;
		}

		/// <summary>
		/// <para>This method links the edge to a graph through its adjacent faces and its endpoints.
		/// It links both the edge and its symmetric edge to said graph.</para>
		/// <para>Example:</para>
		/// <pre><code>
		/// Edge e = Edge.NewEdge();
		/// e.ConnectEdge(v1, v2, f1, f2);
		/// </code></pre>
		/// </summary>
		/// <param name="orig">Orig of the new edge</param>
		/// <param name="dest">Dest of the new edge</param>
		/// <param name="left">Left of the new edge</param>
		/// <param name="right">Right of the new edge</param>
		public void ConnectEdge(Vertex orig, Vertex dest, Face left, Face right)
		{
			Orig = orig;
			Dest = dest;
			Left = left;
			Right = right;
		}

		/// <summary>
		/// <para>This method unlinks the edge from the graph it's connected to.
		/// It unlinks both the edge and its symmetric edge from said graph.
		/// Note that this does not remove it from the edges collection in Cell</para>
		/// <see cref="Cell.edges"/>
		/// </summary>
		public void DisconnectEdge()
		{
			Orig = null;
			Dest = null;
			Left = null;
			Right = null;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Rnext traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing to this edge's <see cref="Orig"/>(see <see cref="Vertex.destEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Right"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Rnext traversal</returns>
		public Edge Rnext()
		{
			Right.edges.Remove(Sym);
			Edge temp = Orig.destEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(Sym);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Lnext traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing out of this edge's <see cref="Dest"/>(see <see cref="Vertex.origEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Left"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Lnext traversal</returns>
		public Edge Lnext()
		{
			Left.edges.Remove(Sym);
			Edge temp = Dest.origEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(Sym);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Onext traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing out of this edge's <see cref="Orig"/>(see <see cref="Vertex.origEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Left"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Onext traversal</returns>
		public Edge Onext()
		{
			Left.edges.Remove(this);
			Edge temp = Orig.origEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(this);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Dnext traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing to this edge's <see cref="Dest"/>(see <see cref="Vertex.destEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Right"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Dnext traversal</returns>
		public Edge Dnext()
		{
			Right.edges.Remove(this);
			Edge temp = Dest.destEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(this);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Rprev traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing out of this edge's <see cref="Dest"/>(see <see cref="Vertex.origEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Right"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Rprev traversal</returns>
		public Edge Rprev()
		{
			Right.edges.Remove(Sym);
			Edge temp = Dest.origEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(Sym);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Lprev traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing to this edge's <see cref="Orig"/>(see <see cref="Vertex.destEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Left"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Lprev traversal</returns>
		public Edge Lprev()
		{
			Left.edges.Remove(Sym);
			Edge temp = Orig.destEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(Sym);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Oprev traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing out of this edge's <see cref="Orig"/>(see <see cref="Vertex.origEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Right"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's <see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Oprev traversal</returns>
		public Edge Oprev()
		{
			Right.edges.Remove(this);
			Edge temp = Orig.origEdges.Intersect(Right.edges).FirstOrDefault();
			Right.edges.Add(this);
			return temp;
		}

		/// <summary>
		/// <para>finds the Edge gotten by one Dprev traversal step(defined in the image in this class's summary)
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking all edges pointing to this edge's <see cref="Dest"/>(see <see cref="Vertex.destEdges"/>)</description></item>
		/// <item><description>Taking all edges boardering this edge's <see cref="Left"/>(see <see cref="Face.edges"/>)</description></item>
		/// <item><description>Intersecting them and taking the edge that is not this edge's =<see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Dprev traversal</returns>
		public Edge Dprev()
		{
			Left.edges.Remove(this);
			Edge temp = Dest.destEdges.Intersect(Left.edges).FirstOrDefault();
			Left.edges.Add(this);
			return temp;
		}
	}
}
