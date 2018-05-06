using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;
using UnityEngine.SceneManagement;

/// <summary>
/// Starts the game with the first model in the StreamingAssets folder
/// </summary>
public class StartGameButton : MonoBehaviour
{

    public Button start_btn;

    void Start()
    {
        Button btn = start_btn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
    }
}
