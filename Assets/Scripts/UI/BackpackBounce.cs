using UnityEngine;

public class BackpackBounce : MonoBehaviour
{
    Animation anim;
    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void PlayBounce()
    {
        if(anim.isPlaying) anim.Rewind();
        else anim.Play();
    }
}
