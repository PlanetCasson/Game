using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
/// <summary>
/// <para>Responsible for updating properties within the edge game objects.</para>
/// </summary>
public class EdgeInterface : MonoBehaviour {

	private Edge _ModelEdge;
	/// <summary>
	/// <para>Edge object that the game object emcompasses.</para>
	/// </summary>
	public Edge ModelEdge { get { return _ModelEdge; } }
	/// <summary>
	/// <para>Initializes the edge visuals and start and end positions.</para>
	/// </summary>
	/// <param name="e">Edge being drawn.</param>
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
	/// <summary>
	/// <para>Helper function to set the color of the edge.</para>
	/// </summary>
	/// <param name="c">New edge color.</param>
	public void SetColor(Color c)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		lr.startColor = c;
		lr.endColor = c;
	}
	/// <summary>
	/// <para>Called by Unity every frame. Updates color of edge if the edge is currently fading back from red to green (after a collision has occured).</para>
	/// </summary>
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
				//color fade over, reset values and color
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
