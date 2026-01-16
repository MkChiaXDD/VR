using UnityEngine;

public class BossWeakSpotHit : MonoBehaviour
{
    private BossController boss;

    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    void OnPointerClick()
    {
        boss.OnWeakSpotHit(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Thrown"))
        {
            boss.OnWeakSpotHit(gameObject);
        }
    }
}
