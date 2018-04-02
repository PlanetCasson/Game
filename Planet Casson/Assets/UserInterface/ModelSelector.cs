using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ModelSelector : MonoBehaviour
{
    public Dropdown dropdown_select;
    public List<string> models;

	// Use this for initialization
	void Start ()
    {
        models = new List<string>();
        DirectoryInfo d = new DirectoryInfo(Application.dataPath + "/StreamingAssets/");//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.obj"); //Getting Text files;
        foreach (FileInfo file in Files)
        {
            models.Add(file.Name);
            Debug.Log(file.Name);
        }
        dropdown_select.AddOptions(models);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
