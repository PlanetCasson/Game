using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Objects;

/// <summary>
/// Script to allow player to shift the phase of the traversal object
/// along each face of the graph. 
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

	//not called by Unity game loop
	public bool Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("Here");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			//note please put all traversers in "Traverser" layer or layer 8
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
			{
				Debug.Log("Hit");
				traversal = hit.transform.gameObject.GetComponent<TraversalObject>();
				center = Camera.main.WorldToViewportPoint((traversal.CurrentEdge.Left as Face).getFaceCenter());
				initPhase = traversal.Phase;
				initAngle = angleToHorizontal(Camera.main.ScreenToViewportPoint(Input.mousePosition) - center);
				dir = findDir(traversal.CurrentEdge);
				isDragging = true;
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
			return true;
		}
		return false;
	}

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
