using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Vertex representation in the modified Quad-Edge data structure.
	/// </summary>
	public class Vertex : FaceVertex
	{
		/// <summary>
		/// <para>The first Edge pointing out of this vertex.</para>
		/// </summary>
		public Edge EdgeListHead
		{
			get { return EdgeListHead; }
			set { EdgeListHead = value; }
		}
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
