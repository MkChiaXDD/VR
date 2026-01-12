using UnityEngine;

public class VRClick : MonoBehaviour
{
    void OnPointerClick()
    {
        Debug.Log("Clicked via Cardboard trigger");
        // shoot / pop / activate
        Destroy(gameObject);
    }
}
