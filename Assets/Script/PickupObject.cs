using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float throwSpeed = 6f;
    [SerializeField] private float throwDuration = 1.2f;

    private bool isHeld;
    private Transform followTarget;
    private BulletPool pool;

    private void Start()
    {
        pool = FindFirstObjectByType<BulletPool>();
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

        FindFirstObjectByType<PlayerBallSpawner>()
            ?.OnBallPickedUp(gameObject);

        FindFirstObjectByType<PlayerPickup>().PickUp(this);
    }


    public void OnPickUp(Transform pickupPoint)
    {
        isHeld = true;
        followTarget = pickupPoint;

        gameObject.layer = 0;
    }

    public void Throw(Vector3 direction, Transform shootPoint)
    {
        isHeld = false;
        followTarget = null;

        AudioManager.Instance.PlaySFX("Throw");
        // SNAP TO SHOOT POINT
        transform.position = shootPoint.position;

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

        gameObject.layer = 6;
        pool.ReturnObject(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Weak"))
    //    {
    //        //Return to pool
    //        pool.ReturnObject(gameObject);
    //    }
    //}
}
