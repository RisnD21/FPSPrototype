using UnityEngine;

public class ScoreRecorder : MonoBehaviour
{
    float initTime;
    float gameLength;
    int shotsFired;
    int shotsHit;
    int enemiesKilled;
    float accuracy;
    float lifeLoss;
    float lifeRecover;

    float lifeTimeGameLength;
    int lifeTimeKills;
    int lifeTimeDeath;

    [SerializeField] Weapon firstWeaponToRecord;
    [SerializeField] Weapon secondWeaponToRecord;
    [SerializeField] Damageable bodyToRecord;
    [SerializeField] Damageable shieldToRecord;

    public void HasShot() => shotsFired++;
    public void HasHit() => shotsHit++;
    public void HasKilled() => enemiesKilled++;
    public void HasLostLife(float value) => lifeLoss+=value;
    public void HasRecoveredLife(float value) => lifeRecover+=value;

    public void HasDead() => lifeTimeDeath++;

    void OnEnable()
    {
        initTime = Time.time;

        lifeTimeGameLength = PlayerPrefs.GetFloat("LifeTimeGameLength", 0f);
        lifeTimeKills = PlayerPrefs.GetInt("LifeTimeEnemiesKilled", 0);
        lifeTimeDeath = PlayerPrefs.GetInt("LifeTimeDeath", 0);

        ResetSessionCounters();

        AIAgent.RecordHit += HasHit; 
        AIAgent.RecordDeath += HasKilled;
        firstWeaponToRecord.RecordShot += HasShot;
        secondWeaponToRecord.RecordShot += HasShot;
        bodyToRecord.RecordRecover += HasRecoveredLife;
        bodyToRecord.RecordLoss += HasLostLife;
        bodyToRecord.RecordDeath += HasDead;
        shieldToRecord.RecordLoss += HasLostLife;
    }



    void ResetSessionCounters()
    {
        gameLength = 0f;
        shotsFired = 0;
        shotsHit = 0;
        enemiesKilled = 0;
        accuracy = 0f;
        lifeLoss = 0f;
        lifeRecover = 0f;
    }

    void CalculateStatistics()
    {
        gameLength = Time.time - initTime;
        accuracy = (shotsFired == 0) ? 0 : (float)shotsHit / shotsFired * 100f;
        lifeTimeGameLength += gameLength;
        lifeTimeKills += enemiesKilled;
    }

    void SaveStatistics()
    {
        PlayerPrefs.SetFloat("GameLength", gameLength);
        PlayerPrefs.SetInt("ShotsFired", shotsFired);
        PlayerPrefs.SetInt("ShotsHit", shotsHit);
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        PlayerPrefs.SetFloat("Accuracy", accuracy);
        PlayerPrefs.SetFloat("LifeLoss", lifeLoss);
        PlayerPrefs.SetFloat("LifeRecover", lifeRecover);
        PlayerPrefs.SetFloat("LifeTimeGameLength", lifeTimeGameLength);
        PlayerPrefs.SetInt("LifeTimeEnemiesKilled", lifeTimeKills);
        PlayerPrefs.SetInt("LifeTimeDeath", lifeTimeDeath);

        PlayerPrefs.Save();
    }

    void OnDisable()
    {
        CalculateStatistics();
        SaveStatistics();

        AIAgent.RecordHit -= HasHit;
        AIAgent.RecordDeath -= HasKilled;
        firstWeaponToRecord.RecordShot -= HasShot;
        secondWeaponToRecord.RecordShot -= HasShot;
        bodyToRecord.RecordRecover -= HasRecoveredLife;
        bodyToRecord.RecordLoss -= HasLostLife;
        shieldToRecord.RecordLoss -= HasLostLife;
        bodyToRecord.RecordDeath -= HasDead;
    }
}
