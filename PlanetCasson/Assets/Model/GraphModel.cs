using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public class GraphModel : MonoBehaviour
	{

		public Vector3[] verticies;
		public Vector2Int[] edges;
		public GameObject vertexObj;
		public GameObject edgeObj;

		private GameObject[] vObjs;
		private GameObject[] eObjs;

		// Use this for initialization
		void Start()
		{
			vObjs = new GameObject[verticies.Length];
			eObjs = new GameObject[edges.Length];
			for (int i = 0; i < verticies.Length; i++)
			{
				vObjs[i] = Object.Instantiate(vertexObj, verticies[i], Quaternion.identity, gameObject.transform);
			}
			for (int i = 0; i < edges.Length; i++)
			{
				eObjs[i] = Object.Instantiate(edgeObj, Vector3.zero, Quaternion.identity, gameObject.transform);
				LineRenderer lr = eObjs[i].GetComponent<LineRenderer>();
				lr.positionCount = 2;
				lr.SetPosition(0, verticies[edges[i].x]);
				lr.SetPosition(1, verticies[edges[i].y]);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}