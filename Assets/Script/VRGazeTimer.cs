using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VRGazeTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image ring;

    [Header("Gaze Settings")]
    [SerializeField] private float gazeDuration = 2f;

    [Header("Bullet Settings")]
    [SerializeField] private float maxDistance = 30f;

    float timer;
    bool gazing;
    bool completed;

    float speed;
    Vector3 direction;
    Vector3 startPos;

    Collider col;
    TimerPool pool;

    // ================= INIT =================
    public void Init(float speed, Vector3 direction, Transform startPos, TimerPool pool)
    {
        this.speed = speed;
        this.direction = direction.normalized;
        this.startPos = startPos.position;
        this.pool = pool;

        transform.position = this.startPos;
    }

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    void OnEnable()
    {
        // Reset state when reused
        timer = 0f;
        gazing = false;
        completed = false;

        if (ring) ring.fillAmount = 0f;

        gameObject.layer = 6; // Interactive
        if (col) col.enabled = true;
    }

    // ================= UPDATE =================
    void Update()
    {
        // MOVE BULLET
        transform.position += direction * speed * Time.deltaTime;

        // AUTO EXPIRE (distance)
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            ReturnToPool();
            return;
        }

        // GAZE TIMER
        if (!gazing || completed) return;

        timer += Time.deltaTime;
        ring.fillAmount = timer / gazeDuration;

        if (timer >= gazeDuration)
        {
            CompleteInstant();
        }
    }

    // ================= GAZE =================
    void OnPointerEnter()
    {
        if (completed) return;

        gazing = true;
        timer = 0f;
        ring.fillAmount = 0f;
    }

    void OnPointerExit()
    {
        gazing = false;
        ring.fillAmount = 0f;
    }

    // ================= COMPLETE =================
    void CompleteInstant()
    {
        completed = true;
        gazing = false;

        // REMOVE FROM INTERACTION IMMEDIATELY (Cardboard-safe)
        gameObject.layer = 0;
        if (col) col.enabled = false;

        // RETURN NEXT FRAME
        StartCoroutine(ReturnNextFrame());
    }

    IEnumerator ReturnNextFrame()
    {
        yield return null; // 1 frame delay for reticle safety
        ReturnToPool();
    }

    void ReturnToPool()
    {
        pool.ReturnObject(gameObject);
    }
}
