using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text ScoreText;
    public PlayerController PlayerController;
    public Canvas PauseOverlay;

	// Use this for initialization
	void Start () {
        PauseOverlay.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	    ScoreText.text = Mathf.Floor(PlayerController.GetScore()).ToString();
	}

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Application.LoadLevel("GameScene");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Application.LoadLevel("TitleScene");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        PauseOverlay.enabled = true;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        PauseOverlay.enabled = false;
    }
}
