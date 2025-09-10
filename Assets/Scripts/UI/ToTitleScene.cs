using UnityEngine;
using DG.Tweening;

public class ToTitleScene : MonoBehaviour
{
    public void Load()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}