using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayEdgeToggle : MonoBehaviour {

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
				float curWidth = ei.GetComponent<LineRenderer>().startWidth;
				if (ei.ModelEdge.isTwoWay) ei.SetEdgeWidth(curWidth / 2);
				else ei.SetEdgeWidth(curWidth * 2);
				ei.ModelEdge.isTwoWay = !ei.ModelEdge.isTwoWay;
				Debug.Log("asdf");
			}
			return true;
		}
		return false;
	}
}
