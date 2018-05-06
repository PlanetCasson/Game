﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
		public List<Edge> edges;
		List<Face> faces;
		//speed of traversers in terms of phases per frame
		public float velocity = 0.001F;

		/// <summary>
		/// <para>Loads a Cell from a simple .obj file in the Assets/StreamingAssets folder. It does not import normal offsets, or textures, only the connection and vertex position data.</para>
		/// <para>This function only parses lines beginning with v and f in the obj file, therefore multiple objects are non supported as well.</para>
		/// <para>This simply parses through the .obj file, and for each line beginning with v, it creates a new vertex, and for each line beginning with f,
		/// it creates a face, and loads its associated face index into the corresponding connections list. These lists are then used to call <see cref="Edge.ConnectCell"/> which
		/// returns a list of the edges used to connect the verticies and faces into a graph. These lists are then stored in a cell, and that cell which contains a graph is returned.</para>
		/// </summary>
		/// <param name="fileName">name of the file containing the graph to load.</param>
		/// <returns>a graph(cell) representing the object stored as the .obj file</returns>
		public static Cell LoadCell(string fileName)
		{
			string path = Application.dataPath + "/StreamingAssets/" + fileName;
			Cell c = new Cell();
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					List<Vertex> verticies = new List<Vertex>();
					List<Face> faces = new List<Face>();
					List<List<int>> connections = new List<List<int>>();
					Dictionary<Vertex, Edge> partialEdges = new Dictionary<Vertex, Edge>();
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						string[] tokens = line.Split(' ');
						if (tokens.Length > 1 && tokens.Last() == string.Empty) System.Array.Resize(ref tokens, tokens.Length - 1);
						switch (tokens[0])
						{
							case "v":
								Vertex v = Vertex.NewVertex(new Vector3(System.Convert.ToSingle(tokens[1]), System.Convert.ToSingle(tokens[2]), System.Convert.ToSingle(tokens[3])));
								verticies.Add(v);
								break;
							case "f":
								Face f = Face.NewFace();
								faces.Add(f);
								connections.Add(new List<int>());
								for (int i = 1; i < tokens.Length; i++)
								{
									string[] toklets = tokens[i].Split('/');
									connections.Last().Add(System.Convert.ToInt32(toklets[0]) - 1);
								}
								break;
							default:
								break;
						}
					}
					List<Edge> edges = Edge.ConnectCell(verticies, faces, connections);
					c.verticies = verticies;
					c.edges = edges;
					c.faces = faces;
				}
			}
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
				EdgeInterface ei = eObjs[i].GetComponent<EdgeInterface>();
				ei.SetEdgeView(edges[i]);
			}
            for (int i = 0; i < faces.Count; i++)
            {
                float h = (float)i / (float)faces.Count;
                fObjs[i].GetComponent<FaceInterface>().SetFaceView(faces[i], Color.HSVToRGB(h, 0.95f, 0.8f));
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
				EdgeInterface ei = eObjs[i].GetComponent<EdgeInterface>();
				ei.SetEdgeView(edges[i]);
				ei.ModelEdge.CollisionPhase = -1;
				ei.ModelEdge.CollisionVel = 0;
			}
			for (int i = 0; i < faces.Count; i++)
			{
				fObjs[i] = Object.Instantiate(faceObj, Vector3.zero, Quaternion.identity, kernel.gameObject.transform);
                float h = (float)i / (float)faces.Count;
                fObjs[i].GetComponent<FaceInterface>().SetFaceView(faces[i], Color.HSVToRGB(h, 0.95f, 0.8f));
            }
            return new GameObject[3][] { vObjs, eObjs, fObjs };
		}
		/// <summary>
		/// <para>Iterate through faces of the Cell and create a traversal object for each face.</para>
		/// </summary>
		public void instantiateTraversals(MonoBehaviour obj, GameObject traverserObj)
		{
			GameObject[] tObjs = new GameObject[faces.Count];

			for (int i = 0; i < faces.Count; i++)
			{
				//Instantiate new game object
				tObjs[i] = Object.Instantiate(traverserObj, obj.gameObject.transform);
                float h = (float)i / (float)faces.Count;
				tObjs[i].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(h, 0.95f, 0.8f);
				//Obtain the first available edge
				Edge oneOfTheEdges = faces[i].EdgeListHead.Rot;
				//create a new traversal object on first availiable edge
				tObjs[i].AddComponent<TraversalObject>();
				tObjs[i].GetComponent<TraversalObject>().AssignTraversalValues(oneOfTheEdges, 0.5F, velocity);
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
		/// <para>May contain Errors.</para>
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
		/// <returns>New edge that was created by the face division.</returns>
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
		/// <para>May contain errors.</para>
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
