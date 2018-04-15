using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasterManager : MonoBehaviour {

	private TraversalDrag tdRaycast;
	private TwoWayEdgeToggle twedRaycast;

	// Use this for initialization
	void Start () {
		tdRaycast = new TraversalDrag();
		twedRaycast = new TwoWayEdgeToggle();
	}
	
	// Update is called once per frame
	void Update () {
		if (tdRaycast.Update())
			return;
		twedRaycast.Update();
	}
}
