using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

public class PauseButton : MonoBehaviour
{

    public Button pause_btn;

    void Start()
    {
        Button btn = pause_btn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        TraversalObject.playing = !TraversalObject.playing;
    }
}
