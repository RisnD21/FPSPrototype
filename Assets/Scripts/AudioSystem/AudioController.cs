using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AudioSystem
{
    public class AudioController : MonoBehaviour
    {
        enum IconState { Mute, Play };
        [SerializeField] List<Sprite> audioImages;    //index 0: Mute, index 1: Play
        [SerializeField] Slider volumeSlider;
        [SerializeField] Button toggleButton;
        [SerializeField] Image iconImage;
        [SerializeField] MonoBehaviour audioHandlerBehaviour; // Assign a MonoBehaviour that implements IAudioHandler in the Inspector
        IAudioHandler audioHandler;

        bool IsMute => volumeSlider.value <= 0.01f;
        float volumeBefMute;
        bool isDebugMode = false;

        void Awake() 
        {
            if (audioHandlerBehaviour is IAudioHandler handler)
            {
                audioHandler = handler;
            }
            else
            {
                Debug.LogError("Assigned MonoBehaviour does not implement IAudioHandler interface.");
            }
        }

        void Start()
        {
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
            toggleButton.onClick.AddListener(ToggleAudio);
        
            if(audioHandler != null )
                volumeBefMute = audioHandler.GetVolume();

            volumeSlider.value = volumeBefMute;
        }

        void ToggleAudio()
        {
            if (IsMute)
            {
                volumeSlider.value = volumeBefMute;
                if (isDebugMode) Debug.Log($"Restored volume to {volumeBefMute}");
            }
            else
            {
                volumeBefMute = volumeSlider.value;
                volumeSlider.value = 0;
                if (isDebugMode) Debug.Log("Muted audio");
            }
        }

        void UpdateVolume(float value)
        {
            if (IsMute && iconImage.sprite != audioImages[0]) UpdateIcon(IconState.Mute);
            else if (!IsMute && iconImage.sprite != audioImages[1]) UpdateIcon(IconState.Play);

            if (isDebugMode) Debug.Log($"Set volume to {value}");

            if (audioHandler != null)
                audioHandler.SetVolume(Mathf.Clamp01(volumeSlider.value));
        }

        void UpdateIcon(IconState state)
        {
            iconImage.sprite = state == IconState.Mute ? audioImages[0] : audioImages[1];
            if (isDebugMode) Debug.Log($"Set icon to {state}");
        }
    }
}