using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	/// <summary>
	/// Interface for Face and Vertex class. 
	/// <para>It is used to express the morphability between Faces and Verticies in a graph and its dual graph.</para>
	/// </summary>
	public interface FaceVertex
	{
		Edge EdgeListHead
		{
			get;
			set;
		}
	}
}
