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
	/// </summary>
	public class Cell
	{
		//lists to store verticies, edges, and faces in this cell
		//if an edge is in the list, then it's companion edges are guaranteed not to be in here
		List<Vertex> verticies;
		List<Edge> edges;
		List<Face> faces;

		/// <summary>
		/// <para>Factory for making the most basic Quad-Edge graph embedded in the sphere, the tetrahedron.
		/// This creates the verticies and faces of the tetrahedron and links them together with the <see cref="Edge.ConnectTetraCell"/> function in Edge.</para>
		/// <para>Note that the positions of all the verticies are initialized to zero. after MakePrimitiveCell.</para>
		/// </summary>
		/// <returns>A tetrahedron graph embedded on a sphere. The positions of the verticies are initialized to 0.</returns>
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
			//testing stuff
			c.makeVertexEdge(c.verticies[0], c.faces[1], c.faces[2]);
			c.makeFaceEdge(c.faces[2], c.verticies.Last(), c.verticies[2]);
			c.verticies.Last().pos = new Vector3(10, 2, 20);
			//c.killFaceEdge(c.faces.Last(), c.verticies.Last(), c.verticies[2]);
			//c.killVertexEdge(c.verticies.Last(), c.faces[1], c.faces[2]);
			return c;
		}

		/// <summary>
		/// <para>ben's function, please comment it.</para>
		/// </summary>
		/// <param name="vObjs"></param>
		/// <param name="eObjs"></param>
		/// <param name="fObjs"></param>
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
				//computing center of face
				LinkedList<Vector3> Vpos = new LinkedList<Vector3>();
				Vector3 sum = new Vector3(0, 0, 0);
				Edge start = faces[i].EdgeListHead.Onext();
				Edge current = start;
//				Vertex prev = faces[i].EdgeListHead.Right as Vertex;
				do
				{
					Vpos.AddLast((current.Right as Vertex).pos); //right should be the vertex that's origin of the edge's dual
																 //the edge's dual edge is a CCW pointing edge bordering faces[i]
					sum += Vpos.Last();
					current = current.Onext(); //Onext traversal finds the next edge in CCW dir that points out of face
				} while (current != start);
				Vector3 avg = sum / Vpos.Count;
				Vpos.AddFirst(avg);

				//construct mesh
				int j;
				Vector2[] UVs = new Vector2[Vpos.Count];
				int[] trigs = new int[3 * (Vpos.Count - 1)]; //-1 to get number of verticies surrounding face = #of trigs
				float UVstep = 2 * Mathf.PI / Vpos.Count;
				UVs[0] = new Vector2(0.5f, 0.5f);
				UVs[1] = new Vector2((Mathf.Cos(UVstep) + 1) / 2, (Mathf.Sin(UVstep) + 1) / 2);
				trigs[0] = 0;
				trigs[1] = 1;
				trigs[2] = 2;
				for (j = 2; j < Vpos.Count; j++)
				{
					UVs[j] = new Vector2((Mathf.Cos(j * UVstep) + 1) / 2, (Mathf.Sin(j * UVstep) + 1) / 2);
					trigs[3 * (j - 2)] = 0;
					trigs[3 * (j - 2) + 1] = j - 1;
					trigs[3 * (j - 2) + 2] = j;
				}
				trigs[3 * (j - 2)] = 0;
				trigs[3 * (j - 2) + 1] = j - 1;
				trigs[3 * (j - 2) + 2] = 1;

				Mesh mesh = fObjs[i].GetComponent<MeshFilter>().mesh;
				mesh.Clear();
				mesh.vertices = Vpos.ToArray<Vector3>();
				mesh.uv = UVs;
				mesh.triangles = trigs;
			}
        }

		/// <summary>
		/// <para>ben's function, please comment it</para>
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="vertexObj"></param>
		/// <param name="edgeObj"></param>
		/// <param name="faceObj"></param>
		/// <returns></returns>
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
				//computing center of face
				LinkedList<Vector3> Vpos = new LinkedList<Vector3>();
				Vector3 sum = new Vector3(0, 0, 0);
				Edge start = faces[i].EdgeListHead.Onext();
				Edge current = start;
				Vertex prev = faces[i].EdgeListHead.Right as Vertex;
				do
				{
					Vpos.AddLast((current.Right as Vertex).pos); //right should be the vertex that's origin of the edge's dual
															 //the edge's dual edge is a CCW pointing edge bordering faces[i]
					sum += Vpos.Last();
					current = current.Onext(); //Onext traversal finds the next edge in CCW dir that points out of face
				} while (current != start);
                Vector3 avg = sum / Vpos.Count;
				Vpos.AddFirst(avg);

				//construct mesh
				int j;
				Vector2[] UVs = new Vector2[Vpos.Count];
				int[] trigs = new int[3*(Vpos.Count - 1)]; //-1 to get number of verticies surrounding face = #of trigs
				float UVstep = 2 * Mathf.PI / Vpos.Count;
				UVs[0] = new Vector2(0.5f, 0.5f);
				UVs[1] = new Vector2((Mathf.Cos(UVstep) + 1) / 2, (Mathf.Sin(UVstep) + 1) / 2);
				trigs[0] = 0;
				trigs[1] = 1;
				trigs[2] = 2;
				for(j = 2; j < Vpos.Count; j++)
				{
					UVs[j] = new Vector2((Mathf.Cos(j*UVstep)+1)/2, (Mathf.Sin(j*UVstep)+1)/2);
					trigs[3 * (j - 2)] = 0;
					trigs[3 * (j - 2) + 1] = j - 1;
					trigs[3 * (j - 2) + 2] = j;
				}
				trigs[3 * (j - 2)] = 0;
				trigs[3 * (j - 2) + 1] = j - 1;
				trigs[3 * (j - 2) + 2] = 1;

				fObjs[i] = Object.Instantiate(faceObj, Vector3.zero, Quaternion.identity, kernel.gameObject.transform);
				Mesh mesh = fObjs[i].GetComponent<MeshFilter>().mesh;
				mesh.Clear();
				mesh.vertices = Vpos.ToArray<Vector3>();
				mesh.uv = UVs;
				mesh.triangles = trigs;
            }
            return new GameObject[3][] { vObjs, eObjs, fObjs };
		}
		/// <summary>
		/// Iterate through faces of Cell and create a traversal object for each face
		/// </summary>
		/// <returns>list of traversal objects</returns>
		public void instantiateTraversals(MonoBehaviour obj, GameObject traverserObj)
		{
			GameObject[] tObjs = new GameObject[faces.Count];

			for (int i = 0; i < faces.Count; i++)
			{
				//Instantiate new game object
				tObjs[i] = Object.Instantiate(traverserObj, obj.gameObject.transform);
				tObjs[i].GetComponent<MeshRenderer>().material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 150);
				//Obtain the first available edge
				Edge oneOfTheEdges = faces[i].EdgeListHead.Rot;
				//create a new traversal object on first availiable edge
				tObjs[i].AddComponent<TraversalObject>();
				tObjs[i].GetComponent<TraversalObject>().AssignTraversalValues(oneOfTheEdges, 0.5F, 0.005F);
				Vector3 position = tObjs[i].GetComponent<TraversalObject>().getVectorPosition(0.5F);
				//Translate object with to new position
				tObjs[i].transform.position = position;
			}
		}

		/// <summary>
		/// <para>The method of expanding from the primitive quad-edge structure. makeVertexEdge splits the vertex "v" into two
		/// and connect it with an edge whose left face is "left" and right face is "right".</para>
		/// <para>This first finds a list of edges that needs to be moved to the new vertex, creates a new, unlinked vertex, and calls
		/// <see cref="Edge.SplitFaceVertex"/> with the appropriate parameters.</para>
		/// <para>If the specified left and right face does not share a vertex, an ArgumentException error will be thrown.(uncatched from SplitFaceVertex).</para>
		/// </summary>
		/// <param name="v">The vertex to be split.</param>
		/// <param name="left">The face to the left of the vertex.</param>
		/// <param name="right">The face to the right of the vertex.</param>
		/// <returns>An Edge that runs from the old vertex to the new vertex.</returns>
		public Edge makeVertexEdge(Vertex v, Face left, Face right)
		{
			//finds all edges that needs to be moved
			List<Edge> moveE = findMoveEdges(v, left, right);

			if (moveE == null) return null;

			Vertex newv = Vertex.NewVertex();
			Edge newe = Edge.SplitFaceVertex(v, newv, left, right, moveE);
			verticies.Add(newv);
			edges.Add(newe);
			return newe;
		}

		/// <summary>
		/// <para>The method of reducing the quad-edge structure. killVertexEdge does the opposite of makeVertexEdge and "unsplits" a vertex
		/// by deleting the vertex v and transfers its edges to the other vertex shared between the left and right faces.</para>
		/// <para>This first finds a list of edges that needs to be moved to the other vertex, and calls <see cref="Edge.RejoinFaceVertex"/>
		/// with the appropriate parameters.</para>
		/// <para>If the specified left and right faces does not share 2 verticies where one of them is the vertex v, then an ArgumentException error
		/// will be thrown.</para>
		/// <para>Note, this might create digons.</para>
		/// </summary>
		/// <param name="v">The vertex to be deleted.</param>
		/// <param name="left">The face to the left of the vertex.</param>
		/// <param name="right">The face to the right of the vertex.</param>
		public void killVertexEdge(Vertex v, Face left, Face right)
		{
			List<Edge> moveE = findMoveEdges(v, left, right);
			Edge delE = moveE.Last().Onext().Sym;
			Edge.RejoinFaceVertex(delE.Orig, v, left, right, moveE);

			if (!edges.Remove(delE))
				edges.Remove(delE.Sym);
			verticies.Remove(v);
		}

		/// <summary>
		/// <para>The method of expanding from the primitive quad-edge structure. makeFaceEdge subdivides the face "f" into two
		/// along an edge that points from orig to dest.</para>
		/// <para>This first finds a list of edges that needs to be moved to surround the new face, creates a new, unlinked face, and calls
		/// <see cref="Edge.SplitFaceVertex"/> with the appropriate parameters.</para>
		/// <para>To utilize SplitFaceVertex, we pass in arguments to indicate splitting a vertex corresponding to the face f in the dual graph.</para>
		/// <para>If the specified face does not contain the verticies orig and dest, an ArgumentException error will be thrown.(uncatched from SplitFaceVertex).</para>
		/// </summary>
		/// <param name="f">The face to be subdivided.</param>
		/// <param name="orig">The origin of the edge to subdivide f.</param>
		/// <param name="dest">The dest of the edge to subdivide f.</param>
		/// <returns></returns>
		public Edge makeFaceEdge(Face f, Vertex orig, Vertex dest)
		{
			//finds all edges that needs to be moved
			List<Edge> moveE = findMoveEdges(f, dest, orig);

			Face newf = Face.NewFace();
			Edge newe = Edge.SplitFaceVertex(f, newf, dest, orig, moveE);
			faces.Add(newf);
			edges.Add(newe.Rot);
			return newe;
		}

		/// <summary>
		/// <para>The method of reducing the quad-edge structure. killFaceEdge does the opposite of makeFaceEdge and "rejoins" a subdivided face
		/// by deleting the face f and transfers its edges to the face to the left of the edge running from orig to dest.</para>
		/// <para>This first finds a list of edges that needs to be moved to the other face, and calls <see cref="Edge.RejoinFaceVertex"/>
		/// with the appropriate parameters.</para>
		/// <para>TO utilize RejoinFaceVertex, we pass in arguments to indicate rejoining a vertex corresponding to the face f in the dual graph.</para>
		/// <para>If the specified face does not contain an edge running from the orig to dest that also has another face to its right, then an ArgumentException error
		/// will be thrown.</para>
		/// <para>Note, this might create digons.</para>
		/// </summary>
		/// <param name="f">The face to be deleted.</param>
		/// <param name="orig">The orig of the edge that currently divides the face.</param>
		/// <param name="dest">The dest of the edge that currently divides the face.</param>
		public void killFaceEdge(Face f, Vertex orig, Vertex dest)
		{

			List<Edge> moveE = findMoveEdges(f, dest, orig);
			Edge delE = moveE.Last().Onext().Sym;
			Edge.RejoinFaceVertex(delE.Orig, f, dest, orig, moveE);

			if (!edges.Remove(delE.Rot))
				edges.Remove(delE.InvRot);
			faces.Remove(f);
		}

		//find all edges pointing away from fv
		//that satisfies:
		//start at an edge bordering right, find an edge pointing out of fv in a CCW rotation step
		//repeat until accumulated all edges until the edge bordering left
		private List<Edge> findMoveEdges(FaceVertex fv, FaceVertex left, FaceVertex right)
		{
			Edge temp = fv.EdgeListHead;
			while (temp.Right != right)
			{
				temp = temp.Onext();
				if (temp == fv.EdgeListHead) //made a loop
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
