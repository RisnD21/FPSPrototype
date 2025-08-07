using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject shieldRippleEffect;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            var ripple = Instantiate(shieldRippleEffect, transform);
            var pSR = ripple.GetComponent<ParticleSystemRenderer>();
            var mat = pSR.material;
            mat.SetVector("_Center", collision.GetContact(0).point);

            Destroy(ripple,2);
        }
    }
}
