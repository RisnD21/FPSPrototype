using UnityEngine;
using AudioSystem;

//simple version of bgm Manager, control only 1 audio source
public class BGMPlayer : MonoBehaviour, IAudioHandler
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] float defaultVolume = 0.5f;

    void Awake() 
    {
        if (bgmSource == null)
        {
            Debug.LogError("AudioSource is not assigned in BGMPlayer.");
            return;
        }

        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", defaultVolume);
        bgmSource.volume = Mathf.Clamp01(savedVolume);
    }
    void Start()
    {
        bgmSource.volume = defaultVolume;
        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public float GetVolume()
    {
        if (bgmSource != null)
            return bgmSource.volume;
        return 0f;
    }

    public void SetVolume(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = Mathf.Clamp01(value);
    }

    void OnDisable() 
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmSource.volume);
    }
}