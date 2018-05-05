﻿using System.Collections;
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
	}

	public void SetColor(Color c)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		lr.startColor = c;
		lr.endColor = c;
	}

	public void Update()
	{
		
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		if (_ModelEdge.CollisionPhase != -1)
		{
			//There has been a collision on the edge, adjust the color based on the edge's collision velocity
			//increment CollisionPhase by CollisionVel
			_ModelEdge.CollisionPhase += _ModelEdge.CollisionVel;
			if (_ModelEdge.CollisionPhase > 1)
			{
				//color fade over, reset information
				_ModelEdge.CollisionPhase = -1;
				_ModelEdge.CollisionVel = 0;
				lr.startColor = new Color(0, 1, 0);
				lr.endColor = new Color(0, 1, 0);
			}
			else
			{
				//linearly interpolate new color in fade
				Color newColor = Color.Lerp(new Color(1, 0, 0), new Color(0, 1, 0), _ModelEdge.CollisionPhase);
				lr.startColor = newColor;
				lr.endColor = newColor;
			}
		}
	}
}
