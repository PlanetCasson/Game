using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Model
{
	public class Face
	{
		public HashSet<Edge> edges;

		private Face()
		{
			edges = new HashSet<Edge>();
		}

		public static Face NewFace()
		{
			return new Face();
		}
	}
}
