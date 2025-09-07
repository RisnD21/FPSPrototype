using UnityEngine;
using System.Collections.Generic;

namespace AudioSystem.SFX
{
    [CreateAssetMenu(fileName = "Footsteps", menuName = "Database/FootstepDatabase")]
    public class FootstepDatabase : ScriptableObject
    {
        public List<AudioClip> footsteps = new();
    }
}