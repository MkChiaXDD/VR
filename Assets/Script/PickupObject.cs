using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float throwSpeed = 6f;
    [SerializeField] private float throwDuration = 1.2f;

    [Header("Hover Highlight")]
    [SerializeField] private Color hoverEmissionColor = Color.cyan;
    [SerializeField] private float hoverEmissionIntensity = 1.5f;

    private bool isHeld;
    private Transform followTarget;
    private BulletPool pool;

    // Highlight
    private Renderer rend;
    private Material mat;
    private Color baseColor;
    private Color baseEmission;

    private void Start()
    {
        pool = FindFirstObjectByType<BulletPool>();

        // Grab renderer from child model (Rock)
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            mat = rend.material; // instance material
            baseColor = mat.color;
            baseEmission = mat.GetColor("_EmissionColor");
        }
    }

    private void Update()
    {
        if (isHeld && followTarget != null)
        {
            transform.position = followTarget.position;
        }
    }

    // ---------------- INTERACTION ----------------

    void OnPointerEnter()
    {
        OnHoverEnter();
    }

    void OnPointerExit()
    {
        OnHoverExit();
    }

    void OnPointerClick()
    {
        if (isHeld) return;

        FindFirstObjectByType<PlayerBallSpawner>()
            ?.OnBallPickedUp(gameObject);

        FindFirstObjectByType<PlayerPickup>()
            ?.PickUp(this);
    }

    // ---------------- PICKUP ----------------

    public void OnPickUp(Transform pickupPoint)
    {
        isHeld = true;
        followTarget = pickupPoint;

        OnHoverExit(); // stop glow when picked up
        gameObject.layer = 0;
    }

    public void Throw(Vector3 direction, Transform shootPoint)
    {
        isHeld = false;
        followTarget = null;

        OnHoverExit();

        AudioManager.Instance.PlaySFX("Throw");

        // Snap to shoot point
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

    // ---------------- HOVER VISUAL ----------------

    public void OnHoverEnter()
    {
        if (isHeld || mat == null) return;

        mat.color = baseColor * 1.1f;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", hoverEmissionColor * hoverEmissionIntensity);
    }

    public void OnHoverExit()
    {
        if (mat == null) return;

        mat.color = baseColor;
        mat.SetColor("_EmissionColor", baseEmission);
    }
}
