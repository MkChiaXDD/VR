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

    private HoverPool pool;

    // Called by BossController AFTER getting from pool
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

    void Update()
    {
        // Move bullet
        transform.position += direction * speed * Time.deltaTime;

        // Return to pool if too far
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            ReturnToPool();
        }
    }

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

    void ReturnToPool()
    {
        pool.ReturnObject(gameObject);
    }
}
