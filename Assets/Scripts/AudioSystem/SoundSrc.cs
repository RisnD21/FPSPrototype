using System.Collections;
using UnityEngine;

namespace AudioSystem.SFX
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSrc : MonoBehaviour
    {
        AudioSource src;
        SoundID id;
        AudioClip clip;
        float volume;
        Vector3? pos;
        float initSpatialBlend;
        bool isDebugMode = false;

        void Awake()
        {
            src = GetComponent<AudioSource>();
            initSpatialBlend = src.spatialBlend;
        }

        public void Set(SoundID id, AudioClip clip, Vector3? pos, float vol)
        {
            this.id = id;
            this.clip = clip;
            volume = vol;
            this.pos = pos;
        }
        
        public void Play()
        {
            if(pos.HasValue)
            {
                src.spatialBlend = initSpatialBlend;
                transform.position = pos.Value;
            }else src.spatialBlend = 0;
            src.volume = volume;
            src.PlayOneShot(clip);

            StartCoroutine(FinishWatcher());
            if(isDebugMode) Debug.Log($"[SoundSrc] Playing {id}");
        }

        IEnumerator FinishWatcher()
        {
            while(src != null && src.isPlaying) yield return null;
            ReportStopOnce();
        }

        void ReportStopOnce()
        {
            SFXManager.Instance.StopReport(id);
            if(isDebugMode) Debug.Log($"[SoundSrc] Finish Playing {id}");
        }
    }
}