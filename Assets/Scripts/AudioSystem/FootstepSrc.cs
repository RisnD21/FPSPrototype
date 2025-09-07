using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem.SFX
{
    [RequireComponent(typeof(AudioSource))]
    public class FootstepSrc : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Rigidbody2D body;

        [Header("Tuning")]
        [SerializeField] float moveEpsilon = 1f;   // 多小算不動，避免抖動觸發
        [SerializeField] float runSpeedThreshold = 10f; // 超過這速度算跑步（依你專案單位調整）
        [SerializeField] float walkInterval = 0.5f;
        [SerializeField] float runInterval = 0.3f;

        [Header("Audio")]
        [SerializeField] FootstepDatabase footstepDB;
        List<AudioClip> footstepClips = new();
        [SerializeField] float baseVolume = 0.9f;
        [SerializeField] Vector2 volumeJitter = new(0.9f, 1.1f);
        [SerializeField] Vector2 pitchJitter = new(0.95f, 1.06f);

        AudioSource src;
        Coroutine routine;
        int lastIndex = -1;

        bool IsMoving => body && body.linearVelocity.sqrMagnitude > moveEpsilon * moveEpsilon;

        void Awake()
        {
            src = GetComponent<AudioSource>();
            if (!body) body = GetComponentInParent<Rigidbody2D>();
            // 預設 2D 聲音（Top-down 常見）
            src.spatialBlend = 0f;

            if (footstepDB != null) footstepClips = footstepDB.footsteps;
        }

        void OnEnable()
        {
            routine ??= StartCoroutine(FootstepRoutine());
        }

        void OnDisable()
        {
            if (routine != null) { StopCoroutine(routine); routine = null; }
        }

        IEnumerator FootstepRoutine()
        {
            if (footstepClips == null || footstepClips.Count == 0)
            {
                Debug.Log("[FootstepSrc] No footstep clips assigned");
                yield break;
            }

            while (true)
                {
                    if (IsMoving)
                    {
                        PlayFootstep();

                        float speed = body.linearVelocity.magnitude;
                        float interval = speed >= runSpeedThreshold ? runInterval : walkInterval;
                        Debug.Log($"speed={speed} interval={interval}");
                        interval += Random.Range(-0.05f, 0.05f); // 微隨機化間隔，避免機械感

                        yield return new WaitForSeconds(interval);
                    }
                    else yield return null;
                }
        }

        void PlayFootstep()
        {
            if (footstepClips.Count == 0) return;

            // 避免連播同一段：抽到跟上次同一個就重抽一次（O(1) 小成本）
            int index = Random.Range(0, footstepClips.Count);
            if (footstepClips.Count > 1 && index == lastIndex)
                index = (index + 1) % footstepClips.Count;
            lastIndex = index;

            // 輕微隨機化
            src.pitch = Random.Range(pitchJitter.x, pitchJitter.y);
            float vol = baseVolume * Random.Range(volumeJitter.x, volumeJitter.y);

            src.PlayOneShot(footstepClips[index], vol);
        }
    }
}