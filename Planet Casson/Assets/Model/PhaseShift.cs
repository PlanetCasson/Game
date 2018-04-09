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
	private Vector3 center;
	private float initPhase;
	private float initAngle;

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
				center = Camera.main.WorldToViewportPoint((traversal.GetComponent<TraversalObject>().CurrentEdge.Left as Face).getFaceCenter());
				initPhase = traversal.GetComponent<TraversalObject>().Phase;
				Vector3 initVector = Camera.main.ScreenToViewportPoint(Input.mousePosition) - center;
				initVector.z = 0;
				initVector = Vector3.Normalize(initVector);
				if (initVector.y > 0)
					initAngle = Mathf.Acos(Vector3.Dot(initVector, Vector3.right));
				else
					initAngle = Mathf.Acos(Vector3.Dot(initVector, Vector3.left)) + Mathf.PI;
				isDragging = true;
			}
		}

		if (Input.GetMouseButtonUp(0))
			isDragging = false;

		if (isDragging)
		{
			Vector3 newVector = Camera.main.ScreenToViewportPoint(Input.mousePosition) - center;
			newVector.z = 0;
			newVector = Vector3.Normalize(newVector);
			float newAngle;
			if (newVector.y > 0)
				newAngle = Mathf.Acos(Vector3.Dot(newVector, Vector3.right));
			else
				newAngle = Mathf.Acos(Vector3.Dot(newVector, Vector3.left)) + Mathf.PI;
			Debug.Log((newAngle) / (2 * Mathf.PI));
			traversal.GetComponent<TraversalObject>().Phase = (newAngle) /(2*Mathf.PI) + initPhase;
		}
	}
}

