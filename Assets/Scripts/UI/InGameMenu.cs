using UnityEngine;
using DG.Tweening;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject controls;
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

    public void RestartGame()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowControls()
    {
        controls.SetActive(true);
    }

    public void ToTitleScene()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}