using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SrartTutorialButton : MonoBehaviour {

	public Button tut_btn;

    void Start()
    {
        Button btn = tut_btn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }
}
