using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Objects;

/// <summary>
/// <para>Script to allow player to shift the phase of the traversal object
/// along each face of the graph.</para>
/// </summary>
public class TraversalDrag
{

	// Hit Detection
	private bool isDragging;
	private TraversalObject traversal;
	private Vector3 center;
	private float initPhase;
	private float initAngle;
	private int dir;

	/// <summary>
	/// <para>Not called by Unity game loop. Selects traversal objects via 
	/// ray casting. Moves the traversal objects along their corresponding faces 
	/// by circular dragging about the center of the face.</para>
	/// </summary>
	/// <returns>Boolean indicating whether the object has been selected or not</returns>
	public bool Update()
	{
		if (Input.GetMouseButtonDown(0))
		{ 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			//note please put all traversers in "Traverser" layer or layer 8
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
			{
				traversal = hit.transform.gameObject.GetComponent<TraversalObject>();
				center = Camera.main.WorldToViewportPoint((traversal.CurrentEdge.Left as Face).getFaceCenter());
				initPhase = traversal.Phase;
				initAngle = angleToHorizontal(Camera.main.ScreenToViewportPoint(Input.mousePosition) - center);
				dir = findDir(traversal.CurrentEdge);
				isDragging = true;
				GameObject.Find("GameObject").GetComponent<SphereKernel>().frameCount = 0;
				return true;
			}
		}
		if (isDragging)
		{
			if (Input.GetMouseButtonUp(0))
				isDragging = false;
			float newAngle = angleToHorizontal(Camera.main.ScreenToViewportPoint(Input.mousePosition) - center);
			float p = (dir * (newAngle - initAngle) / (2 * Mathf.PI) + initPhase);
			traversal.Phase = p - Mathf.Floor(p);
			GameObject.Find("GameObject").GetComponent<SphereKernel>().frameCount = 0;
			return true;
		}
		return false;
	}
	/// <summary>
	/// <para>Finds the angle of the vector with respect to the horizontal</para>
	/// </summary>
	/// <param name="dir">Directional Vector</param>
	/// <returns>The angle of the vector with respect to the horizontal</returns>
	private float angleToHorizontal(Vector3 dir)
	{
		float angle;
		dir.z = 0;
		if (dir.y > 0)
			angle = Mathf.Acos(Vector3.Dot(dir.normalized, Vector3.right));
		else
			angle = Mathf.Acos(Vector3.Dot(dir.normalized, Vector3.left)) + Mathf.PI;
		return angle;
	}
	/// <summary>
	/// <para>Finds the direction of CCW rotation from the perspective of the camera.</para>
	/// <para>Walks along 4 boundary edges. For each boundary edge it takes the vector from the 
	/// center of the face to the origin of the edge and projects the vector into screen 
	/// coordinates. Finds the difference between the angle of the consecutive edges. If 
	/// a majority of them are increasing, then the CCW traversal direction is positive, 
	/// otherwise it is negative.</para>
	/// </summary>
	/// <param name="edge">Boundary edge along a face</param>
	/// <returns>The CCW traversal direction.</returns>
	private int findDir(Edge edge)
	{
		int dir = 0;
		int rev = 0;
		float a1 = angleToHorizontal(Camera.main.WorldToViewportPoint((edge.Orig as Vertex).pos) - center);
		float a2 = angleToHorizontal(Camera.main.WorldToViewportPoint((edge.Lnext().Orig as Vertex).pos) - center);
		float a3 = angleToHorizontal(Camera.main.WorldToViewportPoint((edge.Lnext().Lnext().Orig as Vertex).pos) - center);
		float a4 = angleToHorizontal(Camera.main.WorldToViewportPoint((edge.Lnext().Lnext().Lnext().Orig as Vertex).pos) - center);
		if (a2 > a1) dir++;
		else rev++;
		if (a3 > a2) dir++;
		else rev++;
		if (a4 > a3) dir++;
		else rev++;
		return (dir > rev) ? 1 : -1;
	}
}
