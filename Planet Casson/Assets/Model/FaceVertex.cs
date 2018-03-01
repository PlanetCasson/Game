using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// abstract class for Face and Vertex class. 
	/// <para>It is used to express the morphability between Faces and Verticies in a graph and its dual graph.
	/// A vertex in the graph is simultaneously a face in the dual graph. The same is true for the face where it is a vertex in the dual graph.</para>
	/// </summary>
	public abstract class FaceVertex
	{
		private Edge _edgeListHead;
		/// <summary>
		/// <para>When the runtime class is Face:</para>
		/// <para>Stores the first dual Edge pointing from the center of the face towards an edge surrounding the face.
		/// The dual of this edge is an edge facing CCW direction bordering this face.</para>
		/// <para>When the runtime class is Vertex:</para>
		/// <para>Stores the first Edge pointing from the center of this vertex.</para>
		/// <para>will throw an ArgumentException when the value set for EdgeListHead is not a valid value</para>
		/// <para>To find all edges pointing out of the FaceVertex, you can take the edge and traversing in the <see cref="Onext"/> direction until
		/// you encounter the starting edge.</para>
		/// <para>Note, this will throw an ArgumentException when the value set for EdgeListHead is not a valid value</para>
		/// </summary>
		public Edge EdgeListHead
		{
			get { return _edgeListHead; }
			set
			{
				if (value.Orig == this)
					_edgeListHead = value;
				else
					throw new System.ArgumentException("Edge attempting to make EdgeListHead does not satisfy the correct properties.");
			}
		}
	}

	/// <summary>
	/// Face representation in the Quad-Edge data structure.
	/// <para>To see how the Edge linkage of the Face is managed, see <see cref="FaceVertex"/>.</para>
	/// </summary>
	public class Face : FaceVertex
	{
		/// <summary>
		/// <para>Factory for creating new Faces. 
		/// The newly Generated Face has no surrounding Edges.</para>
		/// <para>Example:</para>
		/// <pre><code>Face f = Face.NewFace();</code></pre>
		/// </summary>
		/// <returns>The newly created Face</returns>
		public static Face NewFace()
		{
			return new Face();
		}
	}

	/// <summary>
	/// Vertex representation in the modified Quad-Edge data structure.
	/// <para>To see how the Edge linkage of the Vertex is managed, see <see cref="FaceVertex"/>.</para>
	/// </summary>
	public class Vertex : FaceVertex
	{
		/// <summary>
		/// <para>The vertex's position in space.</para>
		/// </summary>
		public Vector3 pos;

		/// <summary>
		/// <para>Factory for creating new verticies. 
		/// The newly Generated Vertex's position is null and it is not connected to any edges.</para>
		/// <para>Example:</para>
		/// <pre><code>Vertex v = Vertex.NewVertex();</code></pre>
		/// </summary>
		/// <returns>The newly created vertex</returns>
		public static Vertex NewVertex()
		{
			Vertex v = new Vertex();
			v.pos = Vector3.zero;
			return v;
		}
	}
}
