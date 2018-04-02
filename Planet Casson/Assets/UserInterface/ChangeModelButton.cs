using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

public class ChangeModelButton : MonoBehaviour
{

    public Button changeModel_btn;
    public GameObject master;
    public Dropdown model_select;
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
        foreach (Transform child in master.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string url = ms.models[model_select.value];
        kernel.changeModel(url);
    }
}
