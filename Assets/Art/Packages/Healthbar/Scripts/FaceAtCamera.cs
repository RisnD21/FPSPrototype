using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FaceAtCamera : MonoBehaviour
{
    float timer = 0.01f;
    // Update is called once per frame

    void Start()
    {
        
    }

    void FaceAtPlayer()
    {
        transform.forward = Camera.main.transform.forward;
        transform.position = GetComponentInParent<Transform>().position + new Vector3(0,2,0);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.01f)
        {
            FaceAtPlayer();
            timer = 0;
        }
    }
}
