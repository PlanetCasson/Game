using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

/// <summary>
/// <para>The script that changes or resets the current model; 
/// specifically, this will set every traverser at the start of its phase in whaterver graph is selected.</para>
/// </summary>
public class ChangeModelButton : MonoBehaviour
{

    public Button changeModel_btn;
    public GameObject master;
    public Dropdown model_select;
    public Text victoryText;
    private ModelSelector ms;
    private SphereKernel kernel;

    void Start()
    {
        Button btn = changeModel_btn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        kernel = master.GetComponent<SphereKernel>();
        ms = model_select.GetComponent<ModelSelector>();
    }

    void TaskOnClick()
    {
        victoryText.text = "";
        foreach (Transform child in master.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string url = ms.models[model_select.value];
        kernel.changeModel(url);
    }
}
