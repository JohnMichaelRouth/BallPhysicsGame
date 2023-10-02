using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance;

    public Image timerBar;
    public TextMeshProUGUI comboCountText;
    public float maxTime = 5f;
    public int maxComboCount = 9;
    int comboCount;
    float timeLeft;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

        // Start is called before the first frame update
    void Start()
    {
        timeLeft = 0;
        comboCount = 0;
        timerBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime * 1/Time.timeScale;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        if(timeLeft <= 0)
        {
            comboCount = 0;
            comboCountText.text = "";
        }
    }

    public void RefreshCombo()
    {
        timeLeft = maxTime;
        if(comboCount < maxComboCount)
        {
            comboCount++;
        }
        comboCountText.text = "x" + comboCount;
    }

    public int GetComboCount()
    {
        return comboCount;
    }
}
