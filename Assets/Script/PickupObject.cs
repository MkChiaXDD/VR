using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float throwSpeed = 6f;
    [SerializeField] private float throwDuration = 1.2f;

    private bool isHeld;
    private Transform followTarget;
    private Collider col;

    private PlayerBallSpawner spawner;
    private BallPool ballPool;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    void Start()
    {
        spawner = FindFirstObjectByType<PlayerBallSpawner>();
        ballPool = FindFirstObjectByType<BallPool>(); // IMPORTANT
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

        spawner?.OnBallPickedUp(gameObject);

        FindFirstObjectByType<PlayerPickup>().PickUp(this);
    }

    public void OnPickUp(Transform pickupPoint)
    {
        isHeld = true;
        followTarget = pickupPoint;

        gameObject.layer = 0;
        if (col) col.enabled = false;
    }

    public void Throw(Vector3 direction, Transform shootPoint)
    {
        isHeld = false;
        followTarget = null;

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

        FinishBall();
    }

    // ================= FINISH =================
    void FinishBall()
    {
        spawner?.OnBallFinished(gameObject);

        // Reset interaction
        gameObject.layer = 6; // Interactive layer again
        if (col) col.enabled = true;

        ballPool.ReturnObject(gameObject);
    }
}
