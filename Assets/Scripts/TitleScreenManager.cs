using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNewGamePressed()
    {
        SceneManager.UnloadSceneAsync("TitleScreen");
        GameManager.instance.StartGame();
    }

    public void OnPlayAgainPressed()
    {
        SceneManager.UnloadSceneAsync("GameOver");
        GameManager.instance.StartGame();
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
