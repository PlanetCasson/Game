using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Objects;

/// <summary>
/// <para>Main class responsible for managing the game's cell object and using it to build the graph.</para>
/// </summary>
public class SphereKernel : MonoBehaviour
{
	//public values of base GameObjects
	public GameObject vertexObj;
	public GameObject edgeObj;
	public GameObject faceObj;
	public GameObject traverserObj;
	/// <summary>
	/// <para>Counter used to indicate a full cycle (without collisions) has been reached. It is reset to
	/// 0 if a collision occurs. Incremented in SphereKernel's Update method.</para>
	/// </summary>
	public int frameCount;

	/// <summary>
	/// <para>Main cell used to build the graph and its traversals. Major key.</para>
	/// </summary>
	private Cell SphereKernelCell;
	/// <summary>
	/// <para>List of gameObjects that encompass Vertexes, Edges, and Faces respectively.</para>
	/// </summary>
    private GameObject[] VertexObjects;
    private GameObject[] EdgeObjects;
    private GameObject[] FaceObjects;

	/// <summary>
	/// <para>Called by Unity at the start of the scene. Here the SphereKernel creates a graph
	/// based off one of the availiable obj files using SphereKernelCell. It then instantiates
	/// all the game objects tied to parts of the graph.</para>
	/// </summary>
	void Start()
	{
		SphereKernelCell = Cell.LoadCell("cube.obj");
        GameObject[][] tmp = SphereKernelCell.instantiateGraph(this, vertexObj, edgeObj, faceObj);
        VertexObjects = tmp[0]; EdgeObjects = tmp[1]; FaceObjects = tmp[2];
        foreach(GameObject vert in VertexObjects) { vert.GetComponent<VertexObject>().live = true; }
        SphereKernelCell.calculatePositions(VertexObjects, EdgeObjects, FaceObjects);
        SphereKernelCell.instantiateTraversals(this, traverserObj);
    }
	/// <summary>
	/// <para>Called when the player chooses to change the graph/model. A new obj file is imported and
	/// its graph is created along with all necessary parts and game objects.</para>
	/// </summary>
	/// <param name="url">Location of new model</param>
    public void changeModel(string url)
    {
        SphereKernelCell = Cell.LoadCell(url);
        GameObject[][] tmp = SphereKernelCell.instantiateGraph(this, vertexObj, edgeObj, faceObj);
        VertexObjects = tmp[0]; EdgeObjects = tmp[1]; FaceObjects = tmp[2];
        foreach (GameObject vert in VertexObjects) { vert.GetComponent<VertexObject>().live = true; }
        SphereKernelCell.calculatePositions(VertexObjects, EdgeObjects, FaceObjects);
        SphereKernelCell.instantiateTraversals(this, traverserObj);
    }
	/// <summary>
	/// <para>Helper function to recalculate positions of the vertices, edges, and faces in the graph</para>
	/// </summary>
    public void recalc()
    {
        SphereKernelCell.calculatePositions(VertexObjects, EdgeObjects, FaceObjects);
    }

	/// <summary>
	/// <para>Called by Unity once per frame. Responsible for incrementing the frameCount variable and checking when a full game loop has occured.
	/// If the frameCount hits a full loop, this means no collisions occured and the player beat the level.</para>
	/// </summary>
	void Update()
	{
		if (++frameCount > (1 / SphereKernelCell.velocity))
		{
			//Trigger level complete stuff
			Debug.Log("You did it!");
		}
	}
}