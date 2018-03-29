using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Objects;

public class SphereKernel : MonoBehaviour
{
	public GameObject vertexObj;
	public GameObject edgeObj;
	public GameObject faceObj;
	public GameObject traverserObj;

	//public GameObject asdf;

	private Cell SphereKernelCell;
    private GameObject[] VertexObjects;
    private GameObject[] EdgeObjects;
    private GameObject[] FaceObjects;

	// Use this for initialization
	void Start()
	{
		SphereKernelCell = Cell.LoadCell("../test.obj");
    GameObject[][] tmp = SphereKernelCell.instantiateGraph(this, vertexObj, edgeObj, faceObj);
    VertexObjects = tmp[0]; EdgeObjects = tmp[1]; FaceObjects = tmp[2];
    foreach(GameObject vert in VertexObjects) { vert.GetComponent<VertexObject>().live = true; }
    SphereKernelCell.calculatePositions(VertexObjects, EdgeObjects, FaceObjects);
    SphereKernelCell.instantiateTraversals(this, traverserObj);
    /* Example for adding components
		asdf = new GameObject();
		asdf.name = "asdf";
		asdf.AddComponent<LineRenderer>();
		*/
    }

    public void recalc()
    {
        SphereKernelCell.calculatePositions(VertexObjects, EdgeObjects, FaceObjects);
    }

	// Update is called once per frame
	void Update()
	{

	}
}