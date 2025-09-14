using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class StandaloneButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        // audioSource.PlayOneShot(selectSFX);  I feel like this is annoying
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1f; 
    }
}