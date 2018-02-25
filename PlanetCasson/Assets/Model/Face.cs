using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Face representation in the modified Quad-Edge data structure.
	/// </summary>
	public class Face : FaceVertex
	{
		/// <summary>
		/// <para>The first Edge facing the CCW direction and borders the face.</para>
		/// </summary>
		public Edge EdgeListHead
		{
			get { return EdgeListHead.Rot; }
			set { EdgeListHead = value.InvRot; }
		}

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
}
