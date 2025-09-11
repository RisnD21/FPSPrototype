using UnityEngine;
using AudioSystem;

//simple version of sfx Manager, adjust all sfx source volume at once
public class SFXPlayer : MonoBehaviour, IAudioHandler
{
    [SerializeField] AudioSource[] sfxSource;
    [SerializeField] float defaultVolume = 0.5f;
    float volume;
    void Awake()
    {
        volume = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);
        AdjustVolume();
    }

    void Start()
    {
        if (sfxSource.Length == 0)
        {
            Debug.LogWarning("No AudioSrc is found in SFXPlayer");
            return;
        }
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        AdjustVolume();
    }

    void AdjustVolume()
    {
        foreach(var src in sfxSource)
        {
            src.volume = volume;
        }
    }

    void OnDisable() 
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}