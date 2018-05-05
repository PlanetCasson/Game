﻿// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.
// Copied from https://gist.github.com/JISyed/5017805

using UnityEngine;
using System.Collections;

public class KeyboardCamera : MonoBehaviour
{
    //
    // VARIABLES
    //

    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float zoomSpeed = 800.0f;
    public float moveSpeed = 2.0f;
    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    private bool isMoving;    // Is the camera being moved?
    private bool isZooming;     // Is the camera zooming?


    //
    // UPDATE
    //

    void Update()
    {
        // WASD
        if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || 
                Input.GetKey (KeyCode.S)  || Input.GetKey (KeyCode.D)) {
            isMoving = true;
        }

        // Get the right mouse button
        if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // LShift
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            isZooming = true;
        }


        // Disable movements on button release
        if (!Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.A) 
            && !Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.D)) isMoving = false;
        if (!Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift)) isPanning = false;
        if (!Input.GetKey (KeyCode.LeftShift)) isZooming = false;


        // Move the camera on it's XY plane
        if (isMoving)
        {
            Vector3 vel = new Vector3();
            if (Input.GetKey (KeyCode.W)){
                vel += new Vector3(0, moveSpeed , 0);
            }
            if (Input.GetKey (KeyCode.S)){
                vel += new Vector3(0, moveSpeed*-1, 0);
            }
            if (Input.GetKey (KeyCode.A)){
                vel += new Vector3(moveSpeed*-1, 0, 0);
            }
            if (Input.GetKey (KeyCode.D)){
                vel += new Vector3(moveSpeed, 0, 0);
            }
            transform.Translate(vel, Space.Self);
        }
        
        // Rotate camera along X and Y axis
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(new Vector3(0,0,0), transform.right, -pos.y * turnSpeed);
            transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera linearly along Z axis
        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = pos.y * zoomSpeed * transform.forward;
            //Vector3 zin = new Vector3(0,0,zoomSpeed);
            transform.Translate(move, Space.World);
        }
    }
}