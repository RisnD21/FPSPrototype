using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using AudioSystem.BGM;

namespace AudioSystem.UI
{
    public class AudioController : MonoBehaviour
    {
        //index 0: off, index 1: on
        public List<Sprite> audioImages;
        public Slider volumeSlider;
        public Button toggleButton;
        public Image iconImage;

        bool audioIsOn;
        float volume;

        void Start()
        {
            if (volumeSlider == null)
            {
                Debug.LogError("Reference Missing: audio volumeSlider reference is missing");
            } else
            {
                volumeSlider.onValueChanged.AddListener(UpdateVolume);
            }

            if(toggleButton == null)
            {
                Debug.LogError("Reference Missing: audio toggleButton reference is missing");
            }
            
            if (audioImages.Count < 2)
            {
                Debug.LogError("Reference Missing: audio sprite reference missing");
            }


            if (BGMManager.Instance == null)
            {
                Debug.LogError("Can't find BGM player Instance");
            }
            
            toggleButton.onClick.AddListener(ToggleAudio);
            
                audioIsOn = BGMManager.Instance.GetVolume() > 0;
                volumeSlider.value = BGMManager.Instance.GetVolume();
                UpdateIcon();
        }

        //aft toggle, if audio is off we save the current volume then set bgm's volume to 0
        //else we restore the bgm's value by modifying slider(which will trigger UpdateVol automatically)
        void ToggleAudio()
        {
            audioIsOn = !audioIsOn;

            if (audioIsOn)
            {
                volumeSlider.value = volume;
            } 
            else
            {
                volume = volumeSlider.value;
                volumeSlider.value = 0;
            }

            UpdateIcon();
        }


        void UpdateVolume(float value)
        {
            BGMManager.Instance.SetVolume(Mathf.Clamp01(volumeSlider.value));

            if (value > 0)
            {
                audioIsOn = true;
            } else
            {
                audioIsOn = false;
                UpdateIcon();
            }        
        }

        void UpdateIcon()
        {
            iconImage.sprite = audioIsOn? audioImages[1] : audioImages[0]; 
        }
    }
}