using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using Model;

namespace Model.Objects
{
	/// <summary>
	/// Ben explain this class
	/// </summary>
    public class VertexObject : MonoBehaviour
    {
        public Cell graph;
        public Vertex vertex;
        public bool live = false;
        public bool selected = false;
        internal SphereKernel sphereKernel;
		/// <summary>
		/// Ben document this
		/// </summary>
        void OnSelectionChange()
        {
            selected = !selected;
            print("Selected a vertex!");
        }

        /// <summary>
		/// Ben Document this
		/// Called by Unity once per frame.
		/// </summary>
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
