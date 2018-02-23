using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class SphereKernel : MonoBehaviour
{
    public GameObject vertexObj;
    public GameObject edgeObj;

	//public GameObject asdf;

    Cell SphereKernelCell;

    // Use this for initialization
    void Start()
    {
        SphereKernelCell = Cell.MakePrimitiveCell();
        SphereKernelCell.calculatePositions();
        SphereKernelCell.instantiateGraph(this, vertexObj, edgeObj);
		/* Example for adding components
		asdf = new GameObject();
		asdf.name = "asdf";
		asdf.AddComponent<LineRenderer>();
		*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}