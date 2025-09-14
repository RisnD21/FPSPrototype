using System.Collections;
using AudioSystem.SFX;
using UnityEngine;

public class SimpleAmbient : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] AudioSource droplet;
    [SerializeField] AudioSource windHowling;
    float masterSFXVolume = 1f;
    float dropletBaseVolume;
    float windHowlingBaseVolume;
    void Start()
    {
        StartCoroutine(DropletRoutine());
        StartCoroutine(WindHowlingRoutine());
    }

    IEnumerator DropletRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0f, 20f));
            dropletBaseVolume = Random.Range(0.1f, 0.3f);
            droplet.volume = dropletBaseVolume * masterSFXVolume;
            droplet.pitch = Random.Range(0.8f, 1.2f);
            droplet.Play();
        }
    }

    IEnumerator WindHowlingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0f, 15f));
            windHowlingBaseVolume = Random.Range(0.4f, 0.6f);
            windHowling.volume = dropletBaseVolume * masterSFXVolume;
            windHowling.pitch = Random.Range(0.8f, 1.2f);
            windHowling.Play();
        }
    }

    void Update()
    {
        if(SFXManager.Instance != null && masterSFXVolume != SFXManager.Instance.masterVolume)
        {
            masterSFXVolume = SFXManager.Instance.masterVolume;
            droplet.volume = masterSFXVolume * dropletBaseVolume;
            windHowling.volume = masterSFXVolume * windHowlingBaseVolume;
        }
    }
}
