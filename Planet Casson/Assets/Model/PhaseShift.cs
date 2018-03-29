﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Objects;


/// <summary>
/// Script to allow player to shift the phase of the traversal object
/// along each face of the graph. 
/// </summary>

public class PhaseShift : MonoBehaviour {

	// Hit Detection
	private bool isDragging; 
	private Vector3 mouseOrig;
	private Ray ray;
	private RaycastHit hit;
	private GameObject traversal;

	// Use this for initialization
	void Start () {
		Debug.Log("In");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Here");
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {

				traversal = hit.transform.gameObject;
				isDragging = true;
			}
		}

		if (Input.GetMouseButtonUp(0)) isDragging = false;

		if (isDragging) dragTraversals(traversal);
	}

	void dragTraversals(GameObject traversal) {
		traversal.GetComponent<TraversalObject>().Position = ((Mathf.Clamp(Input.mousePosition.x,0,999) / 1000));
	}
}

