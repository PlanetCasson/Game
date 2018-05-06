using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>Counts the number of currently doubled edges and displays it to the user.</para>
/// </summary>
public class DoublesCounter : MonoBehaviour
{
    public Text text;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        text.text = "Doubles: " + TwoWayEdgeToggle.twoWayCount;
	}
}
