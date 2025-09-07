using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem.SFX
{
    [System.Serializable]
    public class SoundConfig
    {
        public SoundID soundID;
        public AudioClip clip;
        public int maxInstances = 5;
        public float cooldown = 0;
        public float volume = 1;
    }

    [CreateAssetMenu(fileName = "SFXDatabase", menuName = "Database/SFXDatabase")]
    public class SFXDatabase : ScriptableObject
    {
        public List<SoundConfig> configs;
    }
}
