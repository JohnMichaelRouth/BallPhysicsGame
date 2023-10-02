using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI scoreText; 

    private int currEnemies;
    private bool onMenu = true;
    private int score;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (!SceneManager.GetSceneByName("TitleScreen").isLoaded)
            onMenu = true;
            SceneManager.LoadScene("TitleScreen", LoadSceneMode.Additive);

        currEnemies = 0;
    }

    private void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    public void EnemyKilled(int scoreValue)
    {
        currEnemies--;
        ComboManager.instance.RefreshCombo();
        int comboCount = ComboManager.instance.GetComboCount();
        score += scoreValue*comboCount;
        UpdateScoreText();
    }

    public void EnemySpawned()
    {
        currEnemies++;
    }

    public void EnemyDespawned() 
    { 
        currEnemies--;
    }

    public int GetCurrEnemies()
    {
        return currEnemies;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public void StartGame()
    {
        onMenu = false;
    }

    public bool GetOnMenu()
    {
        return onMenu;
    }

    public void PlayerDied()
    {
        if (!SceneManager.GetSceneByName("TitleScreen").isLoaded)
            onMenu = true;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetGame()
    {
        score = 0;
        UpdateScoreText ();
    }
}
