using UnityEngine;
using DG.Tweening;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject controls;
    [SerializeField] float scale = 0.7f;
    public void OnEnable()
    {
        PopUpMenuAnim();
        PauseGame();
    }

    public void OnDisable()
    {
        ResumeGame();
        controls.SetActive(false);
    }

    void PopUpMenuAnim()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * scale, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
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

    public void RestartGame()
    {
        SceneLoader.Instance.LoadScene("Main");
    }

    public void ShowControls()
    {
        controls.SetActive(true);
    }

    public void ToGameScene()
    {
        SceneLoader.Instance.LoadScene("Main");   
    }

    public void ToTitleScene()
    {
        SceneLoader.Instance.LoadScene("TitleScene");   
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}