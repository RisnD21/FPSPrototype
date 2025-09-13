using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        ScreenFader.Instance.FadeOut();
    }
    public void LoadScene(string sceneName, float fadeDuration = 0.35f)
    {
        ScreenFader.Instance.FadeIn(fadeDuration).OnComplete(() =>
        {
            StartCoroutine(LoadRoutine(sceneName, fadeDuration));
        });
    }

    IEnumerator LoadRoutine(string sceneName, float fadeDuration)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        ScreenFader.Instance.FadeOut(fadeDuration);
    }
}