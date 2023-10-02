using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Called when a player has a game over
    void Start()
    {
        int score = GameManager.instance.GetScore();
        scoreText.text = "Score: " + score;
        GameManager.instance.ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayAgainPressed()
    {
        SceneManager.UnloadSceneAsync("GameOver");
        GameManager.instance.StartGame();
        GameManager.instance.PlayAgain();
    }


    public void OnExitPressed()
    {
        Debug.Log("Close Game");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}
