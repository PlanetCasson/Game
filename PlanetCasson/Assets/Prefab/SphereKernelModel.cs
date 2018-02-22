using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class SphereKernel : MonoBehaviour
{
    Model.Cell SphereKernelCell;
    Model.GraphModel SphereKernelModel;

	// Use this for initialization
	void Start ()
	{
        SphereKernelCell = Cell.MakePrimitiveCell();
        
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
