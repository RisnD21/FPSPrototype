using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem.BGM
{
    public class BGMManager : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] AudioSource defaultTrackSrc;   // 探索曲（常駐）
        [SerializeField] AudioSource battleTrackSrc;    // 戰鬥曲（需要時淡入）

        [Header("Mixing")]
        [SerializeField] float crossfadeDuration = 3.0f; // 淡換秒數
        [SerializeField] float linger = 3f;            // 離戰後延遲回探索，避免抖
        [SerializeField, Range(0f, 1f)] float initVolume = 0.5f;

        HashSet<AIAgent> engagingEnemies = new();
        public static BGMManager Instance { get; private set; }

        Coroutine fadeRoutine;
        Coroutine lingerRoutine;

        void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); return; }
        }

        void Start()
        {
            // 確保兩條都在播，戰鬥曲先靜音
            if (defaultTrackSrc != null)
            {
                defaultTrackSrc.loop = true;
                defaultTrackSrc.volume = initVolume;
                if (!defaultTrackSrc.isPlaying) defaultTrackSrc.Play();
            }

            if (battleTrackSrc != null)
            {
                battleTrackSrc.loop = true;
                battleTrackSrc.volume = 0f;
                if (!battleTrackSrc.isPlaying) battleTrackSrc.Play();
            }
        }

        // —— 外部事件 —— //
        public void OnEnemyEngage(AIAgent enemy)
        {
            if (enemy == null) return;
            engagingEnemies.Add(enemy);
            engagingEnemies.RemoveWhere(e => e == null);

            if (engagingEnemies.Count > 0)
            {
                // 進戰：立刻切戰鬥；若有等待退回的 linger，取消它
                if (lingerRoutine != null) { StopCoroutine(lingerRoutine); lingerRoutine = null; }
                CrossToBattle();
            }
        }

        public void OnEnemyDisengage(AIAgent enemy)
        {
            engagingEnemies.Remove(enemy);
            engagingEnemies.RemoveWhere(e => e == null);

            if (engagingEnemies.Count == 0)
            {
                // 離戰：等個 linger 秒再回探索，期間若又進戰就取消
                if (lingerRoutine != null) StopCoroutine(lingerRoutine);
                lingerRoutine = StartCoroutine(LingerThenBack());
            }
        }

        IEnumerator LingerThenBack()
        {
            float t = 0f;
            while (t < linger)
            {
                // 若在等待期間又進戰，就中止
                if (engagingEnemies.Count > 0) yield break;
                t += Time.deltaTime;
                yield return null;
            }
            CrossToDefault();
        }

        // —— 切換實作 —— //
        void CrossToBattle() => StartCrossfade(toBattle: true);
        void CrossToDefault() => StartCrossfade(toBattle: false);

        void StartCrossfade(bool toBattle)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(CrossfadeCoroutine(toBattle, crossfadeDuration));
        }

        IEnumerator CrossfadeCoroutine(bool toBattle, float duration)
        {
            var from = toBattle ? defaultTrackSrc : battleTrackSrc;
            var to = toBattle ? battleTrackSrc : defaultTrackSrc;

            if (from == null || to == null) yield break;

            if (!from.isPlaying) from.Play(); // 安全起見
            if (!to.isPlaying) to.Play();

            float t = 0f;
            float fromStart = from.volume;
            float toStart = to.volume;
            float toEnd = initVolume;

            while (t < duration)
            {
                t += Time.deltaTime;
                float k = t / duration;
                from.volume = Mathf.Lerp(fromStart, 0f, 2 * k);
                to.volume = Mathf.Lerp(toStart, toEnd, k);
                yield return null;
            }

            from.volume = 0f;
            to.volume = toEnd;
            fadeRoutine = null;
        }

        void OnDestroy()
        {
            engagingEnemies.Clear();
        }

        public float GetVolume()
        {
            return 0;
        }

        public void SetVolume(float val)
        {

        }
    }
}