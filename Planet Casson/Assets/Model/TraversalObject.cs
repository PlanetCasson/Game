using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Abstract class for objects that will traverse the graph
	/// </summary>
	public class TraversalObject : MonoBehaviour
	{
        public static Boolean playing = true;

		private Edge _current;
		//position is a measurement of the percentage of the face the object has traveled
		private float _phase;
		//velocity is measured in percentOfEdge/frame
		private float _vel;
		//number of edges on associated face
		private List<Edge> _boundaryE;
		//audio source for collision sounds
		AudioSource _collideSound;

		public Edge CurrentEdge
		{
			get { return _current; }
			set
			{
				_current = value;
			}
		}
		public float Phase
		{
			get { return _phase; }
			set
			{
				_phase = value - Mathf.Floor(value);
				int edgei;
				if (value < 0)
					//TODO implement stuff
				else
					edgei = (int)Mathf.Floor(_phase * _boundaryE.Count);
				_current = _boundaryE[edgei];
				if (_current.Orig is Vertex && _current.Dest is Vertex)
				{
					Vertex OrigVertex = (Vertex)_current.Orig;
					Vertex DestVertex = (Vertex)_current.Dest;
					//Use Lerp method to linerally interpolate the current position of the of the object
					gameObject.transform.position = Vector3.Lerp(OrigVertex.pos, DestVertex.pos, _phase * _boundaryE.Count - edgei);
				}
				else
					throw new System.ArgumentException("Traversal Object's _current variable is an edge that doesn't point to vertexes");
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
		/// Method used to construct a new traversal object
		/// </summary>
		/// <param name="faceTravelled"></param>
		/// <param name="currentEdge"></param>
		/// <param name="position"></param>
		/// <param name="velocity"></param>
		/// <returns>New traversal object with the properties given in the parameters</returns>
		public void AssignTraversalValues(Edge currentEdge, float phase, float velocity)
		{
			_boundaryE = (currentEdge.Left as Face).getBoundEdges();
			_phase = phase;
			_vel = velocity;
		}

		/// <summary>
		/// If Traversal Object collides with another, check that the collision is valid
		/// and play a sound
		/// </summary>
		/// <param name="collision"></param>
		public void OnCollisionEnter(Collision collision)
		{
			//check for valid collision
			try
			{
				_collideSound.Play();
			} catch (NullReferenceException e)
			{
				print("potential audio reference error");
			}
			//TODO: Check collision is valid i.e. same edge or vertex

		}
		/// <summary>
		/// Called by the Unity Engine at the start of the game. Instantiates the collision sound
		/// </summary>
		public void Start()
		{
			//Fetch audio source from the GameObject
			_collideSound = GetComponent<AudioSource>();
		}
		/// <summary>
		/// Called by the Unity Engine at every frame. Adjusts the position of the traversal object
		/// </summary>
		public void Update()
		{
			/*
			if (TraversalObject.playing)
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
            } else {
				transform.position = getVectorPosition(_pos);

			}
			*/
			if (TraversalObject.playing)
			{
				Phase += _vel / _boundaryE.Count;
			}
			else
			{
				Phase = Phase;
			}
		}
	}
}