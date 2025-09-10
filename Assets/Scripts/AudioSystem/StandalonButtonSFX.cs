using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class StandaloneButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip selectSFX;
    AudioSource audioSource;
    Button btn;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        btn = GetComponent<Button>();
    }
    
    void Start()
    {
        btn.onClick.AddListener(() => audioSource.PlayOneShot(clickSFX) );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(selectSFX);  
    }
}
