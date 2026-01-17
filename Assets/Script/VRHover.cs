using UnityEngine;

public class VRHover : MonoBehaviour
{
    private Renderer rend;

    [Header("Movement")]
    private float speed;
    private Vector3 direction;
    private Vector3 startPos;

    [Header("Lifetime")]
    [SerializeField] private float maxDistance = 5f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    private HoverPool pool;

    // ================= INIT =================
    public void Init(float speed, Vector3 direction, Transform startPos, HoverPool pool)
    {
        this.speed = speed;
        this.direction = direction.normalized;
        this.startPos = startPos.position;
        this.pool = pool;

        transform.position = this.startPos;
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        // Reset visuals when reused
        if (rend != null)
            rend.material.color = Color.white;
    }

    // ================= UPDATE =================
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            ReturnToPool();
        }
    }

    // ================= INTERACTION =================
    void OnPointerEnter()
    {
        rend.material.color = Color.yellow;
    }

    void OnPointerExit()
    {
        rend.material.color = Color.white;
    }

    void OnPointerClick()
    {
        ReturnToPool();
    }

    // ================= PLAYER HIT =================
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        ReturnToPool();
    }

    // ================= POOL =================
    void ReturnToPool()
    {
        pool.ReturnObject(gameObject);
    }
}
