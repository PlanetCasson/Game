using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// abstract class for Face and Vertex class. 
	/// <para>It is used to express the morphability between Faces and Verticies in a graph and its dual graph.</para>
	/// 
	/// <para>When the runtime class is Face:</para>
	/// <para>The first dual Edge pointing from the center of the face towards an edge surrounding the face.
	/// The dual of this edge is an edge facing CCW direction bordering this face.</para>
	/// <para>will throw an ArgumentException when the value set for EdgeListHead is not a valid value</para>
	/// 
	/// <para>When the runtime class is Vertex:</para>
	/// <para>The </para>
	/// </summary>
	public abstract class FaceVertex
	{
		/// <summary>
		/// <para>When the runtime class is Face:</para>
		/// <para>The first dual Edge pointing from the center of the face towards an edge surrounding the face.
		/// The dual of this edge is an edge facing CCW direction bordering this face. In other words, this stores
		/// the first edge pointing out from the vertex of the dual graph that corresponds to this face.</para>
		/// <para>When the runtime class is Vertex:</para>
		/// <para>The first Edge pointing out from this vertex.</para>
		/// <para>Note, this will throw an ArgumentException when the value set for EdgeListHead is not a valid value</para>
		/// </summary>
		public Edge EdgeListHead
		{
			get { return EdgeListHead; }
			set
			{
				if (value.Orig == this)
					EdgeListHead = value;
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
			return new Vertex();
		}
	}
}
