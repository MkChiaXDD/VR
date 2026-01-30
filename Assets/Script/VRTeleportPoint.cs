using UnityEngine;

public class VRTeleportPoint : MonoBehaviour
{
    public Transform playerRoot;
    Renderer r;
    private Material originalColor;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private GameObject clickImage;

    void Start()
    {
        clickImage.SetActive(false);
        r = GetComponent<Renderer>();
        originalColor = r.material;
    }

    void OnPointerEnter()
    {
        r.material.color = Color.green;
        clickImage.SetActive(true);
    }

    void OnPointerExit()
    {
        r.material.color = originalColor.color;
        clickImage.SetActive(false);
    }

    void OnPointerClick()
    {
        AudioManager.Instance.PlaySFX("Tp");
        playerRoot.position = transform.position + positionOffset;
        r.material.color = originalColor.color;
        clickImage.SetActive(true);
    }
}
