using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public class Vertex
	{
		public Vector3 pos;
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
}
