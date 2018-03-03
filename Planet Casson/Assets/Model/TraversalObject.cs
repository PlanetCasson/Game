﻿using UnityEngine;

namespace Model
{
	/// <summary>
	/// Abstract class for objects that will traverse the graph
	/// </summary>
	public class TraversalObject : MonoBehaviour
	{
		//Variables held by each traversal object
		private Edge _current;
		//position is a measurement of the percentage of the current edge that the object has travelled
		private float _pos;
		//velocity is measured in percentOfEdge/frame
		private float _vel;

		/// <summary>
		/// Method used to construct a new traversal object
		/// </summary>
		/// <param name="faceTravelled"></param>
		/// <param name="currentEdge"></param>
		/// <param name="position"></param>
		/// <param name="velocity"></param>
		/// <returns>New traversal object with the properties given in the parameters</returns>
		public void AssignTraversalValues(Edge currentEdge, float position, float velocity)
		{
			
			_current = currentEdge;
			_pos = position;
			_vel = velocity;
		}

		public Edge CurrentEdge
		{
			get { return _current; }
			set
			{
				_current = value;
			}
		}
		public float Position
		{
			get { return _pos; }
			set
			{
				_pos = value;
			}
		}
		public float Velocity
		{
			get { return _vel; }
			set
			{
				_vel = value;
			}
		}
		/// <summary>
		/// Gives position of object as a Vector3
		/// </summary>
		/// <returns>Vector3 position</returns>
		public Vector3 getVectorPosition (float percentPos)
		{
			if (_current.Orig is Vertex && _current.Dest is Vertex)
			{
				Vertex OrigVertex = (Vertex)_current.Orig;
				Vertex DestVertex = (Vertex)_current.Dest;
				//Use Lerp method to linerally interpolate the current position of the of the object
				return Vector3.Lerp(OrigVertex.pos, DestVertex.pos, percentPos);
			} else
			{
				throw new System.ArgumentException("Traversal Object's _current variable is an edge that doesn't point to vertexes");
			}
			
		}
		/// <summary>
		/// Called by the Unity Engine at every frame. Adjusts the position of the traversal object
		/// </summary>
		public void Update()
		{
			//calculate the new percentage of the edge the object will be at
			float newPercentPos = _pos + _vel;
			if (newPercentPos > 1.0)
			{
				//object has reached the end of the current edge, move to the next one
				_current = _current.Lnext();
				newPercentPos = newPercentPos - 1.0F;
			}
			transform.position = getVectorPosition(newPercentPos);
			_pos = newPercentPos;
		}
	}
}