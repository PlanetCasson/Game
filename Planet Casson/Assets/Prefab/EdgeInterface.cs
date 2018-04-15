using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class EdgeInterface : MonoBehaviour {

	private Edge _ModelEdge;
	public Edge ModelEdge { get { return _ModelEdge; } }

	public void SetEdgeView(Edge e)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		CapsuleCollider cc = gameObject.GetComponent<CapsuleCollider>();
		Vector3 orig = (e.Orig as Vertex).pos;
		Vector3 dest = (e.Dest as Vertex).pos;
		lr.positionCount = 2;
		lr.SetPosition(0, orig);
		lr.SetPosition(1, dest);
		cc.transform.position = orig + (dest - orig) / 2;
		cc.transform.LookAt(dest);
		cc.height = (dest - orig).magnitude;
		_ModelEdge = e;
	}

	public void SetEdgeWidth(float w)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		CapsuleCollider cc = gameObject.GetComponent<CapsuleCollider>();
		lr.startWidth = w;
		lr.endWidth = w;
		cc.radius = w / 2;
	}
}
