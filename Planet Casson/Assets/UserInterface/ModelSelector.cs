using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ModelSelector : MonoBehaviour
{
    public Dropdown dropdown_select;
    public GameObject master;
    public List<string> models;
    private SphereKernel kernel;

    // Use this for initialization
    void Start ()
    {
        kernel = master.GetComponent<SphereKernel>();
        models = new List<string>();
        DirectoryInfo d = new DirectoryInfo(Application.dataPath + "/StreamingAssets/");//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.obj"); //Getting Text files;
        foreach (FileInfo file in Files)
        {
            models.Add(file.Name);
            Debug.Log(file.Name);
        }
        dropdown_select.AddOptions(models);
        dropdown_select.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown_select);
        });
    }

    void DropdownValueChanged(Dropdown change)
    {
        foreach (Transform child in master.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string url = models[change.value];
        kernel.changeModel(url);
    }

    void Update() { }
}
