using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

/// <summary>
/// Pauses traversers for easier dragging
/// </summary>
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
        pause_btn.GetComponentInChildren<Text>().text = TraversalObject.playing ? "Pause" : "Play";
    }
}
