using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwSpeed = 6f;
    [SerializeField] private float throwDuration = 1.2f;

    private bool isHeld;
    private Transform followTarget;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (isHeld && followTarget != null)
        {
            transform.position = followTarget.position;
        }
    }

    void OnPointerClick()
    {
        if (isHeld) return;

        FindFirstObjectByType<PlayerPickup>().PickUp(this);
    }

    public void OnPickUp(Transform pickupPoint)
    {
        isHeld = true;
        followTarget = pickupPoint;

        gameObject.layer = 0;
        if (col) col.enabled = false;
    }

    public void Throw(Vector3 direction)
    {
        isHeld = false;
        followTarget = null;

        StartCoroutine(ThrowRoutine(direction.normalized));
    }

    IEnumerator ThrowRoutine(Vector3 dir)
    {
        float timer = 0f;

        while (timer < throwDuration)
        {
            transform.position += dir * throwSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // TODO: hit boss or return to pool
        Destroy(gameObject);
    }
}
