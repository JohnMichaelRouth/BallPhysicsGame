using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalEnemies = 10;
    public TextMeshProUGUI enemyCountText; 

    private int enemyGoal;
    private int currEnemies;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        currEnemies = 0;
    }

    private void Start()
    {
        enemyGoal = totalEnemies;
        UpdateEnemyCountText();
    }

    public void EnemyKilled()
    {
        currEnemies--;
        enemyGoal--;
        UpdateEnemyCountText();

        if (enemyGoal == 0)
        {
            Debug.Log("You win!");
        }
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

    private void UpdateEnemyCountText()
    {
        if (enemyGoal > 0)
        {
            enemyCountText.text = "Enemies Remaining: " + enemyGoal;
        }
        else if (enemyGoal == 0) 
        {
            enemyCountText.text = "You Win!";
        }
        else
        {
            enemyCountText.text = "Extra Enemies Killed: " + -enemyGoal;
        }
        
    }
}
