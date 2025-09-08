using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace AudioSystem.SFX
{
    public class SFXManager : MonoBehaviour
    {
        class GlobalGate
        {
            int activeInstances = 0;
            float lastPlayedTime = -Mathf.Infinity;
            bool isDebugMode = false;
            public bool CanPlay(SoundConfig cfg)
            {
                if (Time.time - lastPlayedTime < cfg.cooldown)
                {
                    if (isDebugMode) Debug.Log($"[GlobalGate] {cfg.soundID} is in cooldown");
                    return false;
                }
                if (activeInstances >= cfg.maxInstances)
                {
                    if (isDebugMode) Debug.Log($"[GlobalGate] {cfg.soundID} reach max instances");
                    return false;
                }
                return true;
            }

            public void OnPlay()
            {
                lastPlayedTime = Time.time;
                activeInstances++;
            }

            public void OnStop()
            {
                activeInstances = Mathf.Max(activeInstances - 1, 0);
            }
        }

        public static SFXManager Instance { get; private set; }
        Dictionary<SoundID, SoundConfig> configs;
        Dictionary<SoundID, GlobalGate> gates;
        [SerializeField] SFXDatabase SFXDB;
        [SerializeField] SoundSrc sourcePrefab;
        [SerializeField] Transform sourceParent;
        float masterVolume = 1f;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            configs = new();
            foreach (var c in SFXDB.configs)
            {
                if (!configs.TryAdd(c.soundID, c))
                {
                    Debug.LogWarning($"[SFXManager] Duplicate SoundID: {c.soundID}");
                    continue;
                }
            }

            gates = new();
            foreach (var config in configs) gates[config.Key] = new GlobalGate();
        }

        void Start()
        {
            PoolSystem.Instance.InitPool(sourcePrefab, 20, sourceParent);
        }

        public void PlaySound(SoundID sfx, Vector3? pos = null)
        {
            if (!configs.TryGetValue(sfx, out var cfg))
            {
                Debug.LogError($"[SFXManager] Invalid SFX ID {sfx}");
                return;
            }

            if (!cfg.clip)
            {
                Debug.LogWarning($"[SFXManager] {sfx} clip is null");
                return;
            }

            var gate = gates[sfx];
            if (!gate.CanPlay(cfg)) return;

            if (!PoolSystem.Instance.Contains(sourcePrefab)) return;

            var soundSrc = PoolSystem.Instance.GetInstance<SoundSrc>(sourcePrefab);

            gate.OnPlay();
            soundSrc.Set(sfx, cfg.clip, pos, cfg.volume * masterVolume);

            soundSrc.Play();
        }

        public void StopReport(SoundID id) => gates[id].OnStop();
        
        public float GetVolume()
        {
            return masterVolume;
        }

        public void SetVolume(float val)
        {
            masterVolume = Mathf.Clamp01(val);
        }
    }
}