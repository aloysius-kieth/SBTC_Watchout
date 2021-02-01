using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using DG.Tweening;
using Coffee.UIExtensions;

public class ScoreManager : MonoBehaviour
{
    #region SINGLETON
    public static ScoreManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion
    public int Score { get; set; }
    public TextMeshProUGUI scoreText;

    public Animator scoreTextAnim;

    private void Start()
    {
        Score = 0;
        scoreText.text = "0";
    }

    public void AddScore(int amt)
    {
        if (AppManager.gameManager.IsGameover) return;
        Score += amt;
        scoreText.text = Score.ToString();
    }

    public void OnBonusGet(int amt)
    {
        AddScore(amt);
        scoreTextAnim.SetTrigger("ScaleScoreText");
    }

    public void ChangeTextColor(bool change)
    {
        if (change)
        {
            scoreText.GetComponent<UIGradient>().enabled = true;
        }
        else scoreText.GetComponent<UIGradient>().enabled = false;
    }

    public void CheckEnterTop10()
    {
        if (Score <= 0 || Score <= AppManager.gameManager.lastPlaceScore) AppManager.gameManager.IsTop10 = false;
        else AppManager.gameManager.IsTop10 = true;
    }

    public void Reset()
    {
        Score = 0;
        scoreText.text = "0";
        ChangeTextColor(false);
    }

}
