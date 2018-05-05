﻿using System;
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
		//Locations of collisions
		private Queue<float> _collisionLocations = new Queue<float>();


		public Edge CurrentEdge
		{
			get { return _current; }
		}
		public float Phase
		{
			get { return _phase; }
			set
			{
				_phase = value - Mathf.Floor(value);
				int edgei;
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
		public void OnTriggerEnter(Collider collider)
		{
			if (collider.isTrigger && collider.gameObject.GetComponent<TraversalObject>() && TraversalObject.playing)
			{
				TraversalObject otherTraverser = collider.gameObject.GetComponent<TraversalObject>();

				if (this._current != null && checkValidCollision(otherTraverser))
				{
					try
					{
						//Fetch audio source from the GameObject
						if (_collideSound == null) _collideSound = GetComponent<AudioSource>();
						_collideSound.Play();

						//tell edge there is a collision
						this._current.CollisionCall(_vel);
						//reset spherekernel's frameCount
						GameObject.Find("GameObject").GetComponent<SphereKernel>().frameCount = 0;
					}
					catch (NullReferenceException e)
					{
						print("potential audio reference error");
					}
				}
			}
		}
		/// <summary>
		/// Check that the collision that just occured warrants a collision update
		/// </summary>
		/// <param name="otherTrav"></param>
		/// <returns></returns>
		private bool checkValidCollision(TraversalObject otherTrav)
		{
			bool result = false;
			//make sure there hasn't been a collision on that edge yet
			if (this._current.isTwoWay == false)
			{
				//check if on same edge
				if (this._current.Sym == otherTrav._current) result = true;
				//check if on same vertex
				if (this._current.Dest == otherTrav._current.Dest || this._current.Orig == otherTrav._current.Orig
					|| this._current.Dest == otherTrav._current.Orig || this._current.Orig == otherTrav._current.Dest)
				{
					result = true;
				}
			}
			return result;
		}
		/// <summary>
		/// Called by the Unity Engine at the start of the game. Instantiates the collision sound
		/// </summary>
		public void Start()
		{
			
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
				Phase += _vel;
			}
			else
			{
				Phase = Phase;
			}
		}
	}
}