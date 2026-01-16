using UnityEngine;

public class VRTeleportPoint : MonoBehaviour
{
    public Transform playerRoot;
    Renderer r;
    private Material originalColor;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 1.6f, 0f);

    void Start()
    {
        r = GetComponent<Renderer>();
        originalColor = r.material;
    }

    void OnPointerEnter()
    {
        r.material.color = Color.green;
    }

    void OnPointerExit()
    {
        r.material.color = originalColor.color;
    }

    void OnPointerClick()
    {
        playerRoot.position = transform.position + positionOffset;
    }
}
