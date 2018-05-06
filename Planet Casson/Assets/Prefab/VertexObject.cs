using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using Model;

namespace Model.Objects
{
	/// <summary>
	/// A container for useful values in building verticies for the graph
	/// </summary>
    public class VertexObject : MonoBehaviour
    {
        public Cell graph;
        public Vertex vertex;
        public bool live = false;
        internal SphereKernel sphereKernel;
    }
}
