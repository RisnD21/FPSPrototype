using UnityEngine;
using DG.Tweening;

public class InGameMenu : MonoBehaviour
{
    public void OnEnable()
    {
        PopUpMenuAnim();
        PauseGame();
    }

    public void OnDisable()
    {
        ResumeGame();
    }

    void PopUpMenuAnim()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Paused");
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
    }
}