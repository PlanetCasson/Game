﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	/// <summary>
	/// Abstract class for objects that traverse the graph
	/// </summary>
	public class TraversalObject : MonoBehaviour
	{
        public static Boolean playing = true;
		//the current edge the traversal object is on
		private Edge _current;
		//position is a measurement of the percentage of the face the object has traveled
		private float _phase;
		//velocity is measured in percentOfEdge/frame
		private float _vel;
		//number of edges on associated face
		private List<Edge> _boundaryE;
		//audio source for collision sounds
		AudioSource _collideSound;

		/// <summary>
		/// <para>The current edge the traversal object is on</para>
		/// </summary>
		public Edge CurrentEdge
		{
			get { return _current; }
		}
		/// <summary>
		/// <para>A measurement of the percentage of the face the object has traveled. Value from 0 to 1</para>
		/// </summary>
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
		/// <summary>
		/// <para>Rate at which the traversal object's phase changes.</para>
		/// </summary>
		public float Velocity
		{
			get { return _vel; }
			set
			{
				_vel = value;
			}
		}

		/// <summary>
		/// <para>Assigns value to a new traversal object.</para>
		/// </summary>
		/// <param name="currentEdge">The edge the traversal object will be on.</param>
		/// <param name="phase">The phase of the traversal object.</param>
		/// <param name="velocity">The velocity of the traversal object.</param>
		/// <returns>New traversal object with the properties given in the parameters</returns>
		public void AssignTraversalValues(Edge currentEdge, float phase, float velocity)
		{
			_boundaryE = (currentEdge.Left as Face).getBoundEdges();
			_phase = phase;
			_vel = velocity;
		}

		/// <summary>
		/// <para>Checks if the traversal object collides with another traversal object.
		/// Check that the collision is valid, then play the collision sound, then 
		/// notify the current edge a collision has taken place and reset the SphereKernel's frameCount.</para>
		/// </summary>
		/// <param name="collider">The collider that this traversal object collided with</param>
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
		/// <para>Helper function to check the collision between two traversal objects 
		/// is valid. Checks that the edge is not a double edge (allows collisions), 
		/// and ensures that the colliding objects are on the same edge or are on the same 
		/// vertex.</para>
		/// </summary>
		/// <param name="otherTrav">The other traversal object that this traversal object collided with</param>
		/// <returns>True if a valid collision, false otherwise.</returns>
		private bool checkValidCollision(TraversalObject otherTrav)
		{
			bool result = false;
			//check that edge is not a double edge
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
		/// <para>Called by the Unity Engine at the start of the game. Instantiates the collision sound</para>
		/// </summary>
		public void Start()
		{
			
		}
		/// <summary>
		/// <para>Called by the Unity Engine at every frame. Adjusts the position of the traversal object if the game
		/// is in the playing state.</para>
		/// </summary>
		public void Update()
		{
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