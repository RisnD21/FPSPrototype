using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour, IHealthListener
{
    [SerializeField] Volume volume;
    [SerializeField] float maxIntensity = 0.4f;

    Vignette vignette;

    void Awake() 
    {
        if (!volume.profile.TryGet(out vignette)) Debug.LogError("Vignette not found in volume profile");
    }

    void Start() 
    {
        vignette.intensity.value = 0f;
    }


    public void OnHealthChanged(int current, int max, bool hide = false)
    {
        if (hide) return;
        float ratio = 1f - (float)current / max;
        if(vignette != null) vignette.intensity.value = ratio * maxIntensity;
    }

    public void OnDamaged(int x, float y, bool z){}
    public void OnHealed(int x, float y, bool z){}
    public void OnDeath(){}
}
