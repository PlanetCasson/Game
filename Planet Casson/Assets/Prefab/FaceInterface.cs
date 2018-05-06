using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Model;

/// <summary>
/// <para>Responsible for updating properties within the face game objects.</para>
/// </summary>
public class FaceInterface : MonoBehaviour {

	private Face _ModelFace;
	/// <summary>
	/// <para>Face object that the game object emcompasses.</para>
	/// </summary>
	public Face ModelFace { get { return _ModelFace; } }
	/// <summary>
	/// <para>Initializes the edge visuals and start and end positions.</para>
	/// </summary>
	/// <param name="f">Face to be drawn.</param>
	/// <param name="color">Color of the face.</param>
	public void SetFaceView(Face f, Color color)
	{
		//computing center of face
		LinkedList<Vector3> Vpos = new LinkedList<Vector3>();
		Vector3 sum = new Vector3(0, 0, 0);
		Edge start = f.EdgeListHead.Onext();
		Edge current = start;
		Vertex prev = f.EdgeListHead.Right as Vertex;
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

		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = Vpos.ToArray<Vector3>();
		mesh.uv = UVs;
		mesh.triangles = trigs;
        color.a = 0.6f;
        GetComponent<MeshRenderer>().material.color = color;

        _ModelFace = f;
	}
}
