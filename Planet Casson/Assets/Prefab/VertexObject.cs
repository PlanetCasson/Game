using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using Model;

namespace Model.Objects
{
    public class VertexObject : MonoBehaviour
    {
        public Cell graph;
        public Vertex vertex;
        public bool live = false;
        public bool selected = false;
        internal SphereKernel sphereKernel;

        // Use this for initialization
        void Start()
        {

        }

        void OnSelectionChange()
        {
            selected = !selected;
            print("Selected a vertex!");
        }

        // Update is called once per frame
        void Update()
        {
            if (live && transform.hasChanged)
            {
                vertex.pos = this.gameObject.transform.position;
                sphereKernel.recalc();
                transform.hasChanged = false;
            }
            //if (Selection.Contains(this.gameObject))
            //    Debug.Log("I'm selected!");
        }
    }
}
