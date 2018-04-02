using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
	/// <summary>
	/// Edge representation in the Quad-Edge data structure.
	/// <para>Each Edge is a directed edge in the Quad-Edge, and has a correspinding symmetric edge, dual edge, and symmetric dual edge. We shall call them companion edges.</para>
	/// The following diagram shows the relation between an edge and its corresponding edges:</para>
	/// \image html SymEdges.JPG
	/// <para>The edge and its symmetric edge come together to form an undirected edge.
	/// And the dual edge and the symmetric dual edge come together to form an undirected dual edge.
	/// The public Methods in the Edge class modify both the undirected edge and the undirected dual edge, and therefore graph and the dual graph simultaneously.
	/// In essense, any modification to an instance of the Edge class is modifying all four linked edges.</para>
	/// <para>Each directed edge stores only its origin, and a link to its dual edge. When it and its 3 other corresponding edges are linked,
	/// the information in the image below can be obtained from each directed edge. See <see cref="Orig"/>, <see cref="Dest"/>, <see cref="Left"/>, <see cref="Right"/>.</para>
	/// \image html vertface.jpeg
	/// <para>In addition to storing the origin and dual edge of each edge, the next edge in the CCW direction whose origin is shared with this directed edge
	/// is also stored (see <see cref="Onext"/>). This information is used to facilitate the fast edge traversal of the graph through the following functions defined in the image below:</para>
	/// \image html edgetraversal.jpeg
	/// <para>Normal Edges have <see cref="Orig"/> and <see cref="Dest"/> as verticies and <see cref="Left"/> and <see cref="Right"/> as edges. 
	/// When an edge has its Orig and Dest being faces, or its Left and Right being verticies, that means it is an edge on the dual graph.
	/// The relationship between dual edges, dual graphs and their normal graph counterparts is illustrated in the image below.
	/// The solid lines depict the normal graph, and the dotted lines depict the corresponding dual graph.</para>
	/// \image html dual.jpeg
	/// <para>Due to the large amount of bookeeping to keep the edge relations valid, only a few functions are available(public) for addition and deletion of edges.
	/// <see cref="SplitFaceVertex"/>, <see cref="RejoinFaceVertex"/>, <see cref="ConnectTetraCell"/>.</para>
	/// <para>images comes from <a href="https://www.cs.cmu.edu/afs/andrew/scs/cs/15-463/2001/pub/src/a2/quadedge.html">here</a>. All the credit is theirs</para>
	/// </summary>
	public class Edge
	{
		private FaceVertex _orig;
		private Edge _rot;
		private Edge _onext;

		/// <summary>
		/// <para>The origin of this directed edge.</para>
		/// </summary>
		public FaceVertex Orig { get { return _orig; } }
		/// <summary>
		/// <para>Destination of the edge. </para>
		/// <para>Gets the origin of the symmetric edge(see <see cref="Sym"/>).</para>
		/// </summary>
		public FaceVertex Dest { get { return Sym._orig; } }
		/// <summary>
		/// <para>Element to the left of the edge.</para>
		/// <para>Gets the origin of the symmetric dual edge(see <see cref="InvRot"/>).</para>
		/// </summary>
		public FaceVertex Left { get { return InvRot._orig; } }
		/// <summary>
		/// <para>Element to the right of the edge.</para>
		/// <para>Gets the origin of the dual edge(see <see cref="Rot"/>).</para>
		/// </summary>
		public FaceVertex Right { get { return Rot._orig; } }
		/// <summary>
		/// <para>Gets the dual edge.</para>
		/// </summary>
		public Edge Rot { get { return _rot; } }
		/// <summary>
		/// <para>Gets the symmetric edge.</para>
		/// <para>The symmetric edge gotten from taking the dual edge's dual edge.</para>
		/// </summary>
		public Edge Sym { get { return _rot._rot; } }
		/// <summary>
		/// <para>Gets the symmetric dual edge.</para>
		/// <para>The symmetric dual edge is gotten from taking the dual edge's dual edge's dual edge.</para>
		/// </summary>
		public Edge InvRot { get { return _rot._rot._rot; } }

		//Helper function for generating an edge that's already linked with its dual edge, symmetric edge, and symmetric dual edge.
		private static Edge NewEdge()
		{
			Edge e1 = new Edge();
			Edge e2 = new Edge();
			Edge e3 = new Edge();
			Edge e4 = new Edge();
			e1._rot = e3;
			e2._rot = e4;
			e3._rot = e2;
			e4._rot = e1;
			return e1;
		}

		/// <summary>
		/// <para>Method for inserting an edge while modifying the surrounding edges, verticies, and faces to preserve the topological properties of the graph.</para>
		/// <para>The only method for adding an edge that satisfies this criteria is with the subdivision of a face or splitting a vertex.
		/// A subdivision of a face in the graph is the same as the splitting of a vertex in the dual graph.
		/// The same also holds true in reverse. This means that although this method is used to split a vertex, it can also subdivide a face by splitting the corresponding vertex
		/// in the dual graph.</para>
		/// \image html vertexsplit.JPG
		/// <para>The above picture demonstrates the process of Vertex splitting with the edges in the dotted box representing those in the list movedEdges.</para>
		/// \image html subdivide.JPG
		/// <para>This shows that a subdivision problem(in black) can be solved by doing a vertex split on its dual graph(in gray).</para>
		/// <para>Using the inputs, this method moves the origin of all edges in movedEdges from oldFV to newFV. It then sets the <see cref="FaceVertex.EdgeListHead"/> 
		/// for the oldFV to a new edge and newFV to the new edge's symmetric edge.(see <see cref="Sym"/>). This step is to make 100% sure the EdgeListHead
		/// has a proper value after the vertex split. Following this, the algorithm then properly links that new edge, its dual, symmetric, and dual symmetric
		/// edges to the graph. Finally, it fixes the <see cref="Onext"/> relationship between the edges and returns this new edge.</para>
		/// <para>This method checks for the validity of the parameters and whether the vertex split/face subdivide is a valid operation. If not, it will throw
		/// and ArgumentException.</para>
		/// </summary>
		/// <param name="oldFV">Old vertex/face in the graph to be split/subdivided.</param>
		/// <param name="newFV">New unlinked FaceVertex object in the graph to be inserted. This FaceVertex must be between leftFV and rightFV.</param>
		/// <param name="leftFV">FaceVertex in the graph that lies to the left of a directed edge running from oldFV to newFV</param>
		/// <param name="rightFV">FaceVertex in the graph that lies to the right of said directed edge</param>
		/// <param name="movedEdges">List of edges whose origins are centered at oldFV that is to be moved to center at newFV. The list must contain only edges originally
		/// facing away from oldFV and are in CCW order. Furthermore, the last edge must be bordering leftFV and the first edge must be bordering rightFV.</param>
		/// <returns>An directed Edge, linked with its 3 other companion edges, that runs from oldFV to newFV</returns>
		public static Edge SplitFaceVertex(FaceVertex oldFV, FaceVertex newFV, FaceVertex leftFV, FaceVertex rightFV, List<Edge> movedEdges)
		{
			//calculating important edges;
			//movedEdges.Last() should be topLeft
			//movedEdges[0] should be topRight
			Edge botLeft = movedEdges.Last().Onext();
			Edge botRight = movedEdges[0].Oprev();
			Edge e = NewEdge();

			//error checking to make sure we don't fuck up the Quad-Edge in Edge insertion
			if (botLeft.Right != leftFV || botRight.Left != rightFV)
				throw new System.ArgumentException("inputs presents impossible situation for Vertex Split/Face Subdivide");
			for (int i = 0; i < movedEdges.Count; i++)
			{
				if (movedEdges[i]._orig != oldFV)
					throw new System.ArgumentException("movedEdges contain invalid edges!");
			}

			//moving moveEdges from oldFV to newFV
			for (int i = 0; i < movedEdges.Count; i++)
			{
				movedEdges[i]._orig = newFV;
			}

			//linking e with graph
			e._orig = oldFV;
			e.Sym._orig = newFV;
			e.InvRot._orig = leftFV;
			e.Rot._orig = rightFV;

			//set EdgeListHead on affected Vertex/Face to eliminate cases where the EdgeListHead is moved from oldFV to newFV
			oldFV.EdgeListHead = e;
			newFV.EdgeListHead = e.Sym;

			//adjusting onext relationship
			botRight._onext = e;
			e._onext = botLeft;
			movedEdges.Last()._onext = e.Sym;
			e.Sym._onext = movedEdges[0];

			movedEdges[0].Rot._onext = e.Rot;
			e.Rot._onext = botRight.InvRot;
			botLeft.Rot._onext = e.InvRot;
			e.InvRot._onext = movedEdges.Last().InvRot;

			return e;
		}

		/// <summary>
		/// <para>The reverse of the process described in <see cref="SplitFaceVertex"/>. This rejoins two verticies of a graph.
		/// It is also capable fo rejoining two faces of a graph by rejoining the corresponding verticies of the dual graph.</para>
		/// <para>This method moves the origin of all edges in movedEdges from delFV to oldFV. It then sets the <see cref="FaceVertex.EdgeListHead"/> 
		/// of oldFV to the first element in movedEdges. Then, it restores proper <see cref="Onext"/> relationship without the delFV and
		/// the edge running from oldFV to delFV. This automatically delinks that edge and delFV, and they will be deleted by the garbage collector.</para>
		/// <para>This method checks for the validity of the parameters and whether the vertex split/face subdivide is a valid operation. If not, it will throw
		/// and ArgumentException.</para>
		/// </summary>
		/// <param name="oldFV">Old vertex in the graph for movedEdges to all move to after delFV is deleted</param>
		/// <param name="delFV">Vertex to be deleted.</param>
		/// <param name="leftFV">Face to the left of the directed edge running from oldFV to delFV</param>
		/// <param name="rightFV">Face to the right of the above defined edge.</param>
		/// <param name="movedEdges">List of all edges around delFV except for the one mentioned in leftFV. They required to be in the same order as 
		/// they are defined in <see cref="SplitFaceVertex"/>.</param>
		public static void RejoinFaceVertex(FaceVertex oldFV, FaceVertex delFV, FaceVertex leftFV, FaceVertex rightFV, List<Edge> movedEdges)
		{
			Edge delE = movedEdges[0].Oprev().Sym;
			Edge botLeft = delE.Onext();
			Edge botRight = delE.Oprev();

			//error checking to make sure we don't fuck up the Quad-Edge in Edge deletion
			if (botLeft.Right != leftFV || botRight.Left != rightFV)
				throw new System.ArgumentException("inputs presents impossible situation for Vertex/Face Rejoin");
			for (int i = 0; i < movedEdges.Count; i++)
			{
				if (movedEdges[i]._orig != delFV)
					throw new System.ArgumentException("movedEdges contain invalid edges!");
			}

			for (int i = 0; i < movedEdges.Count; i++)
			{
				movedEdges[i]._orig = oldFV;
			}
			oldFV.EdgeListHead = movedEdges[0];

			botRight._onext = movedEdges[0];
			movedEdges.Last()._onext = botLeft;
		}

		/// <summary>
		/// <para>Provided a list of verticies, faces, this method connects the verticies and faces together according to the connections list.</para>
		/// <para>This method loops through the faces list. For each face, it loops through the corresponding list in connections in order to construct the graph.</para>
		/// <para>Upon iterating through each 2 elements in each inner list of connections, a partially initalized edge is created that contains the directed edge from the first vertex to the second vertex and its dual edge.
		/// This partially initiallized edge is then put into a hashset. Upon continuing iteration of subsequent element pairs in each inner list of connections, if 2 verticies that form the same undirected edge is found
		/// in the hashset, then the originally initialized Edge's symmetric and dual symmetric edges are initialized.</para>
		/// </summary>
		/// <param name="verticies">list of verticies in the graph.</param>
		/// <param name="faces">list of faces in the graph.</param>
		/// <param name="connections"><para>A list of lists of ints that stores the connection information between the edges and verticies to form a graph.</para>
		/// <para>Each inner list corresponds to a face in the faces list with the same index and represents the connections of that face. 
		/// Integer elements of the inner list are indicies into the verticies list and points to verticies on the face. 
		/// They are ordered in the inner list such that the subsequent element points to the next counterclockwise vertex on the face.</para></param>
		/// <returns>list of edges used to connect the verticies and faces</returns>
		public static List<Edge> ConnectCell(List<Vertex> verticies, List<Face> faces, List<List<int>> connections)
		{
			Dictionary<EdgeCompareContainer, Edge> edgeCatalog = new Dictionary<EdgeCompareContainer, Edge>(); //catalog of edges by their orig verticies
			for (int i = 0; i < faces.Count; i++)
			{
				List<int> loop = connections[i];

				Vertex orig = verticies[loop.Last()];
				Vertex dest = verticies[loop[0]];
				Face left = faces[i];
				EdgeCompareContainer ekey = new EdgeCompareContainer(orig, dest);
				EdgeCompareContainer rekey = new EdgeCompareContainer(dest, orig);
				Edge e;
				if (edgeCatalog.TryGetValue(rekey, out e))
					e = e.Sym;
				else
				{
					e = Edge.NewEdge();
					edgeCatalog.Add(ekey, e);
				}

				for (int j = 1; j <= loop.Count; j++)
				{
					orig = verticies[loop[j-1]];
					dest = verticies[loop[j % loop.Count]];
					EdgeCompareContainer prevEkey = ekey;
					Edge preve = e;
					ekey = new EdgeCompareContainer(orig, dest);
					rekey = new EdgeCompareContainer(dest, orig);
					if (!edgeCatalog.TryGetValue(rekey, out e))
					{
						if (!edgeCatalog.TryGetValue(ekey, out e))
						{
							e = Edge.NewEdge();
							edgeCatalog.Add(ekey, e);
						}
						e._orig = orig;
						e.InvRot._orig = left;
						orig.EdgeListHead = e;
						left.EdgeListHead = e.InvRot;
					}
					else
					{
						e = e.Sym;
						e._orig = orig;
						e.InvRot._orig = left;
						orig.EdgeListHead = e;
						left.EdgeListHead = e.InvRot;
					}
					e._onext = preve.Sym;
					preve.InvRot._onext = e.InvRot;
				}
			}

			List<Edge> edges = new List<Edge>();
			foreach (KeyValuePair<EdgeCompareContainer, Edge> kvp in edgeCatalog)
			{
				edges.Add(kvp.Value);
			}
			return edges;
		}

		/// <summary>
		/// <para>Connects a list of verticies and faces to form the Quad-Edge graph of a tetrahedron.
		/// Sets up the initial connections and <see cref="Onext"/> relationship.
		/// This method takes in 4 verticies and 4 faces. If this is not satisfied, an ArgumentException will be thrown.</para>
		/// </summary>
		/// <param name="verticies">List of verticies to be connected.</param>
		/// <param name="faces">List of faces to be connected.</param>
		/// <returns>Returns a list of Edges used to connect the verticies and faces into a tetrahedron.
		/// If an edge appears in this list, it's companion edges will not.</returns>
		public static List<Edge> ConnectTetraCell(List<Vertex> verticies, List<Face> faces)
		{
			if (verticies.Count != 4 || faces.Count != 4)
				throw new System.ArgumentException("Incorrect number of verticies and faces to create a tetrahedron");

			List<Edge> edges = new List<Edge>();
			edges.Add(NewEdge());
			edges.Add(NewEdge());
			edges.Add(NewEdge());
			edges.Add(NewEdge());
			edges.Add(NewEdge());
			edges.Add(NewEdge());

			edges[0].ConnectEdge(verticies[0], verticies[1], faces[1], faces[2]);
			edges[1].ConnectEdge(verticies[0], verticies[2], faces[2], faces[3]);
			edges[2].ConnectEdge(verticies[0], verticies[3], faces[3], faces[1]);

			edges[3].ConnectEdge(verticies[1], verticies[2], faces[0], faces[2]);
			edges[4].ConnectEdge(verticies[2], verticies[3], faces[0], faces[3]);
			edges[5].ConnectEdge(verticies[3], verticies[1], faces[0], faces[1]);

			verticies[0].EdgeListHead = edges[2];
			verticies[1].EdgeListHead = edges[3];
			verticies[2].EdgeListHead = edges[4];
			verticies[3].EdgeListHead = edges[5];

			faces[0].EdgeListHead = edges[3].InvRot;
			faces[1].EdgeListHead = edges[0].InvRot;
			faces[2].EdgeListHead = edges[1].InvRot;
			faces[3].EdgeListHead = edges[2].InvRot;

			edges[0]._onext = edges[2];
			edges[2]._onext = edges[1];
			edges[1]._onext = edges[0];

			edges[5].Sym._onext = edges[0].Sym;
			edges[0].Sym._onext = edges[3].Sym;
			edges[3].Sym._onext = edges[5].Sym;

			edges[5]._onext = edges[4].Sym;
			edges[4].Sym._onext = edges[2].Sym;
			edges[2].Sym._onext = edges[5];

			edges[3].Sym._onext = edges[1].Sym;
			edges[1].Sym._onext = edges[4];
			edges[4]._onext = edges[3].Sym;

			edges[0].Rot._onext = edges[1].InvRot;
			edges[1].InvRot._onext = edges[3].Rot;
			edges[3].Rot._onext = edges[0].Rot;

			edges[0].InvRot._onext = edges[5].Rot;
			edges[5].Rot._onext = edges[2].Rot;
			edges[2].Rot._onext = edges[0].InvRot;

			edges[2].InvRot._onext = edges[4].Rot;
			edges[4].Rot._onext = edges[1].Rot;
			edges[1].Rot._onext = edges[2].InvRot;

			edges[4].InvRot._onext = edges[5].InvRot;
			edges[5].InvRot._onext = edges[3].InvRot;
			edges[3].InvRot._onext = edges[4].InvRot;

			return edges;
		}

		//helper function to link a set of linked edges to its surrounding verticies and faces.
		private void ConnectEdge(Vertex orig, Vertex dest, Face left, Face right)
		{
			_orig = orig;
			Sym._orig = dest;
			InvRot._orig = left;
			Rot._orig = right;
		}

		/// <summary>
		/// <para>finds the next Edge on the Face to the right of this edge in a CCW direction. This edge will point towards the origin of this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the dual edge(see <see cref="Rot"/>).</description></item>
		/// <item><description>Then finding <see cref="Onext"/> of it.</description></item>
		/// <item><description>Then taking its symmetric dual edge(see <see cref="InvRot"/>).</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the next Rnext</returns>
		public Edge Rnext()
		{
			return Rot.Onext().InvRot;
		}

		/// <summary>
		/// <para>finds the next Edge on the face to the left of this edge in a CCW direction. This edge will point away from the destination of this edge</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the symmetric dual edge (see <see cref="InvRot"/>)</description></item>
		/// <item><description>Then finding <see cref="Onext"/> of it.</description></item>
		/// <item><description>Then taking its dual edge(see <see cref="Rot"/>).</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Lnext traversal</returns>
		public Edge Lnext()
		{
			return InvRot.Onext().Rot;
		}

		/// <summary>
		/// <para>finds the next Edge pointing out from the origin in the CCW direction. This value is stored in the Edge class, and is initialized with <see cref="ConnectTetraCell"/>
		/// and properly maintained with <see cref="SplitFaceVertex"/> and <see cref="RejoinFaceVertex"/>.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// </summary>
		/// <returns>The next Edge in the Onext traversal</returns>
		public Edge Onext()
		{
			return _onext;
		}

		/// <summary>
		/// <para>finds the next Edge pointing towards the destination in the CCW direction.
		/// from this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the dual edge (see <see cref="Rot"/>)</description></item>
		/// <item><description>Then finding the <see cref="Oprev"/>)</description></item>
		/// <item><description>Then taking its dual edge</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Dnext traversal</returns>
		public Edge Dnext()
		{
			return Rot.Oprev().Rot;
		}

		/// <summary>
		/// <para>finds the next Edge on the Face to the right of this edge in a CW direction. This edge will point out of the destination of this edge.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the dual edge (see <see cref="Rot"/>).</description></item>
		/// <item><description>Then finding its <see cref="Oprev"/>.</description></item>
		/// <item><description>Then taking its symmetrical dual edge (see <see cref="InvRot"/>).<see cref="Sym"/></description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Rprev traversal</returns>
		public Edge Rprev()
		{
			return Rot.Oprev().InvRot;
		}

		/// <summary>
		/// <para>finds the next Edge on the Face to the left of this edge in a CW direction. This edge will point towards the origin of this edge.</para></para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the symmetric dual edge (see <see cref="InvRot"/>).</description></item>
		/// <item><description>Then finding its <see cref="Oprev"/>.</description></item>
		/// <item><description>Then taking its dual edge (see <see cref="Rot"/>).</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Lprev traversal</returns>
		public Edge Lprev()
		{
			return InvRot.Oprev().Rot;
		}

		/// <summary>
		/// <para>finds the next Edge pointing out of this edge's destination in the CW direction.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the dual edge (see <see cref="Rot">).</description></item>
		/// <item><description>Then finding its <see cref="Onext"/>.</description></item>
		/// <item><description>Then taking its dual edge.</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Oprev traversal</returns>
		public Edge Oprev()
		{
			return Rot.Onext().Rot;
		}

		/// <summary>
		/// <para>finds the next Edge pointing towards this edge's destination and in a CW direction.</para>
		/// <para>It does this by:</para>
		/// <list type="bullet">
		/// <item><description>Taking the symmetric dual edge (see <see cref="InvRot"/>).</description></item>
		/// <item><description>Then finding its <see cref="Onext"/>.</description></item>
		/// <item><description>Then taking its symmetrical dual edge.</description></item>
		/// </list>
		/// </summary>
		/// <returns>The next Edge in the Dprev traversal</returns>
		public Edge Dprev()
		{
			return InvRot.Onext().InvRot;
		}
	}

	struct EdgeCompareContainer
	{
		public Vertex orig;
		public Vertex dest;

		public EdgeCompareContainer(Vertex orig, Vertex dest)
		{
			this.orig = orig;
			this.dest = dest;
		}
	}

	class DirectedEdgeComparer : IEqualityComparer<EdgeCompareContainer>
	{
		public bool Equals(EdgeCompareContainer x, EdgeCompareContainer y)
		{
			if (x.orig == y.orig && x.dest == y.dest)
				return true;
			else
				return false;
		}

		public int GetHashCode(EdgeCompareContainer obj)
		{
			return ((obj.orig.GetHashCode() | 0x0000FFFF) + (obj.dest.GetHashCode() << 16)).GetHashCode();
		}
	}
}
