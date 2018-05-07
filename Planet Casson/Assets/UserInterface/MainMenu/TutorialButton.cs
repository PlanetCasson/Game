using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;
using UnityEngine.SceneManagement;

/// <summary>
/// Starts the game with the first model in the StreamingAssets folder
/// </summary>
public class TutorialButton : MonoBehaviour
{

	public Button tutorial_btn;

	void Start()
	{
		Button btn = tutorial_btn.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
	}
}
