using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// <para>Responsible for managing property changes between normal edge and double edge.</para>
/// </summary>
public class TwoWayEdgeToggle : MonoBehaviour {

	public int twoWayCount = 0;
	/// <summary>
	/// Called once per frame by Unity. Checks if the player is clicking on an edge.
	/// Toggles between a normal edge and a double edge and updates values accordingly.
	/// </summary>
	/// <returns>True if a mouse click was detected.</returns>
	public bool Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			//note please put all edges in "Edge" layer or layer 9
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
			{
				EdgeInterface ei = hit.transform.gameObject.GetComponent<EdgeInterface>();
				//float curWidth = ei.GetComponent<LineRenderer>().startWidth;
				//if (ei.ModelEdge.isTwoWay) ei.SetEdgeWidth(curWidth / 2);
				//else ei.SetEdgeWidth(curWidth * 2);
				ei.ModelEdge.isTwoWay = !ei.ModelEdge.isTwoWay;
				if (ei.ModelEdge.isTwoWay)
				{
					ei.ModelEdge.CollisionPhase = -1;
					ei.ModelEdge.CollisionVel = 0;
					ei.SetColor(new Color(0, 0.5F, 1));
					twoWayCount++;
				}
				else
				{
					ei.SetColor(new Color(0, 1, 0));
					twoWayCount--;

				}
			}
			return true;
		}
		return false;
	}
}
