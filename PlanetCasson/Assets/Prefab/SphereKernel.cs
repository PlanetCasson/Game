using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class SphereKernel : MonoBehaviour
{
    public GameObject vertexObj;
    public GameObject edgeObj;

    Model.Cell SphereKernelCell;

    // Use this for initialization
    void Start()
    {
        SphereKernelCell = Cell.MakePrimitiveCell();
        SphereKernelCell.calculatePositions();
        SphereKernelCell.instantiateGraph(this, vertexObj, edgeObj);
    }

    // Update is called once per frame
    void Update()
    {

    }
}