using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoresTextLeft;  // For the left column
    public TextMeshProUGUI highScoresTextRight; // For the right column
    private string scoresFilePath;

    void Start()
    {
        scoresFilePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        int score = GameManager.instance.GetScore();
        scoreText.text = "Score: " + score;
        SaveScore(score);
        DisplayHighScores();
        GameManager.instance.ResetGame();
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

    private void SaveScore(int newScore)
    {
        List<int> highScores = LoadScores();
        highScores.Add(newScore);
        highScores = highScores.OrderByDescending(s => s).Take(10).ToList();

        string json = JsonUtility.ToJson(new HighScoresList { highScores = highScores });
        File.WriteAllText(scoresFilePath, json);
    }

    private List<int> LoadScores()
    {
        if (File.Exists(scoresFilePath))
        {
            string json = File.ReadAllText(scoresFilePath);
            HighScoresList highScoresList = JsonUtility.FromJson<HighScoresList>(json);
            return highScoresList.highScores;
        }
        return new List<int>();
    }

    private void DisplayHighScores()
    {
        List<int> highScores = LoadScores();
        string highScoresStringLeft = "\n";
        string highScoresStringRight = "\n";

        for (int i = 0; i < highScores.Count; i++)
        {
            if (i < 5)
            {
                highScoresStringLeft += $"{i + 1}. {highScores[i]}\n";
            }
            else
            {
                highScoresStringRight += $"{i + 1}. {highScores[i]}\n";
            }
        }

        highScoresTextLeft.text = highScoresStringLeft;
        highScoresTextRight.text = highScoresStringRight;
    }

    [System.Serializable]
    private class HighScoresList
    {
        public List<int> highScores;
    }
}
