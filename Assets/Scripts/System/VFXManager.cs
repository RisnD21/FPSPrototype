using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance {get; private set;}

    [SerializeField] ParticleSystem impactEffect;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        PoolSystem.Instance.InitPool(impactEffect, 10);
    }

    public void SpawnImpactEffect(Vector2 pos, Vector2 normal, string tag)
    {
        // we spawn different effects based on material later on
        // if this project ever reaches refactor stage
        if(tag == "NPC") Debug.Log("Apply damage!");

        var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(impactEffect);
        effect.gameObject.transform.position = pos;
        effect.gameObject.transform.forward = normal;
        effect.gameObject.SetActive(true);

        effect.Play();

        
        // var source = WorldAudioPool.GetWorldSFXSource();

        // source.transform.position = position;
        // source.pitch = Random.Range(0.8f, 1.1f);
        // source.PlayOneShot(setting.ImpactSound);
    }
}
