using System;
using TMPro;
using UnityEngine;

public class ScorePrinter : MonoBehaviour
{
    float gameLength;
    int shotsFired;
    int enemiesKilled;
    float accuracy;
    float lifeLoss;
    float lifeRecover;

    float lifeTimeGameLength;
    int lifeTimeKills;
    int lifeTimeDeath;
    string comment;

    [SerializeField] TextMeshProUGUI gameLengthText;
    [SerializeField] TextMeshProUGUI shotsFiredText;
    [SerializeField] TextMeshProUGUI enemiesKilledText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI lifeLossText;
    [SerializeField] TextMeshProUGUI lifeRecoverText;
    [SerializeField] TextMeshProUGUI lifeTimeGameLengthText;
    [SerializeField] TextMeshProUGUI lifeTimeKillsText;
    [SerializeField] TextMeshProUGUI lifeTimeDeathText;
    [SerializeField] TextMeshProUGUI commentText;

    void OnEnable()
    {
        LoadStatistics();
        PrintScore();
    }

    void LoadStatistics()
    {
        gameLength = PlayerPrefs.GetFloat("GameLength", 0);
        shotsFired = PlayerPrefs.GetInt("ShotsFired", 0);
        enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
        accuracy = PlayerPrefs.GetFloat("Accuracy", 0);
        lifeLoss = PlayerPrefs.GetFloat("LifeLoss", 0);
        lifeRecover = PlayerPrefs.GetFloat("LifeRecover", 0);
        lifeTimeGameLength = PlayerPrefs.GetFloat("LifeTimeGameLength", 0);
        lifeTimeKills = PlayerPrefs.GetInt("LifeTimeEnemiesKilled", 0);
        lifeTimeDeath = PlayerPrefs.GetInt("LifeTimeDeath", 0);

        CalculateScore();
    }

    void PrintScore()
    {
        gameLengthText.text = $"單局遊戲時長 {gameLength:F1}s";
        shotsFiredText.text = $"射擊次數 {shotsFired}";
        accuracyText.text = $"命中率 {accuracy:F1} %";
        enemiesKilledText.text = $"擊殺數 {enemiesKilled}";
        lifeLossText.text = $"受到傷害 {lifeLoss:F1}";
        lifeRecoverText.text = $"恢復生命 {lifeRecover:F1}";
        var ts = TimeSpan.FromSeconds(Mathf.RoundToInt(lifeTimeGameLength)); // 取整秒，可改成 Floor/Ceil
        int hours = (int)ts.TotalHours; // 不讓小時數在 24 之後歸零
        lifeTimeGameLengthText.text = $"總遊戲時長 {hours:00}hr {ts.Minutes:00}min {ts.Seconds:00}s";
        lifeTimeKillsText.text = $"總擊殺數 {lifeTimeKills}";
        lifeTimeDeathText.text = $"總死亡數 {lifeTimeDeath}";
        commentText.text = comment;
    }

    public void ResetScore()
    {
        PlayerPrefs.SetFloat("GameLength", 0);
        PlayerPrefs.SetInt("ShotsFired", 0);
        PlayerPrefs.SetInt("ShotsHit", 0);
        PlayerPrefs.SetInt("EnemiesKilled", 0);
        PlayerPrefs.SetFloat("Accuracy", 0);
        PlayerPrefs.SetFloat("LifeLoss", 0);
        PlayerPrefs.SetFloat("LifeRecover", 0);
        PlayerPrefs.SetFloat("LifeTimeGameLength", 0);
        PlayerPrefs.SetInt("LifeTimeEnemiesKilled", 0);
        PlayerPrefs.SetInt("LifeTimeDeath", 0);

        PlayerPrefs.Save();

        LoadStatistics();
        PrintScore();
    }

    void CalculateScore()
    {
        float finalScore = (enemiesKilled * 15) + (accuracy * 500) - (lifeRecover * 5) - gameLength * 0.2f;

        if (finalScore >= 300) comment = "評語：戰神再世";
        else if (finalScore >= 200) comment = "評語：鎮國將領";
        else if (finalScore >= 100) comment = "評語：百戰老兵";
        else comment = "評語：觀光客";
    }
}