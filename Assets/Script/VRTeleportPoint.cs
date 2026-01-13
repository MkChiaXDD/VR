using UnityEngine;

public class VRTeleportPoint : MonoBehaviour
{
    public Transform playerRoot;
    Renderer r;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 1.6f, 0f);

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void OnPointerClick()
    {
        r.material.color = Color.green; // visual proof
        playerRoot.position = transform.position + positionOffset;
    }
}
