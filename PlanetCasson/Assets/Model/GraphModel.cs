using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
	public class GraphModel : MonoBehaviour
	{

		public Vector3[] verticies;
		public Vector2Int[] edges;
		public HashSet<int>[] faces;
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

	public class Vertex
	{
		uint id;
		Vector3 pos;
		public List<Edge> origEdges; //edges with orig at this vertex
		public List<Edge> destEdges; //edges with dest at this vertex
	}

	public class Edge
	{
		uint id;
		Vertex orig;
		Vertex dest;
		Face left;
		Face right;
		Edge Sym;

		//find edges whose dest is this's orig
		//from those edges select the ones in right face
		public Edge Rnext()
		{
			right.edges.Remove(Sym);
			Edge temp = orig.destEdges.Intersect(right.edges).FirstOrDefault();
			right.edges.Add(Sym);
			return temp;
		}

		//finds edges whose orig is this's dest
		//from those edges select the ones in left face
		public Edge Lnext()
		{
			left.edges.Remove(Sym);
			Edge temp = dest.origEdges.Intersect(left.edges).FirstOrDefault();
			left.edges.Add(Sym);
			return temp;
		}

		//finds edges with same orig as this
		//from those edges select the ones in left face
		public Edge Onext()
		{
			return orig.origEdges.Intersect(left.edges).FirstOrDefault();
		}

		//finds edges with same dest as this
		//from those edges select the ones in right face
		public Edge Dnext()
		{
			return dest.destEdges.Intersect(right.edges).FirstOrDefault();
		}

		//finds edges whose orig is this's dest
		//from those edges select the ones in right face
		public Edge Rprev()
		{
			return dest.origEdges.Intersect(right.edges).FirstOrDefault();
		}
		
		//finds edge whose dest is this's orig
		//from those edges select the ones in left face
		public Edge Lprev()
		{
			return orig.destEdges.Intersect(left.edges).FirstOrDefault();
		}

		//finds edge with same orig as this
		//from those edges select the ones in right face
		public Edge Oprev()
		{
			return orig.origEdges.Intersect(right.edges).FirstOrDefault();
		}

		//finds edge with same dest as this
		public Edge Dprev()
		{
			return dest.destEdges.Intersect(left.edges).FirstOrDefault();
		}
	}

	public class Face
	{
		uint id;
		public HashSet<Edge> edges;
	}

	public class Cell
	{
		Vertex[] verticies;
		uint[] vid;
		Face[] faces;
		uint[] fid;
	}
}