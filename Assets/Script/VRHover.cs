using UnityEngine;

public class VRHover : MonoBehaviour
{
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnPointerEnter()
    {
        rend.material.color = Color.yellow;
    }

    void OnPointerExit()
    {
        rend.material.color = Color.white;
    }
}
