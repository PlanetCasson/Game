using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Vertex representation in the modified Quad-Edge data structure.
	/// </summary>
	public class Vertex
	{
		/// <summary>
		/// <para>The vertex's position in space.</para>
		/// </summary>
		public Vector3 pos;
		/// <summary>
		/// <para>A collection of all directed edges(see <see cref="Edge"/>) whose destination is at this vertex</para>
		/// </summary>
		public List<Edge> origEdges;
		/// <summary>
		/// <para>A collection of all directed edges(see <see cref="Edge"/>) whose origin is at this vertex</para>
		/// </summary>
		public List<Edge> destEdges;

		private Vertex()
		{
			origEdges = new List<Edge>();
			destEdges = new List<Edge>();
		}

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
