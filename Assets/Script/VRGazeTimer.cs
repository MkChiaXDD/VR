using UnityEngine;
using UnityEngine.UI;

public class VRGazeTimer : MonoBehaviour
{
    public Image ring;
    public float gazeDuration = 2f;

    float timer;
    bool gazing;

    void OnPointerEnter()
    {
        gazing = true;
        timer = 0f;
        ring.fillAmount = 0f;
    }

    void OnPointerExit()
    {
        gazing = false;
        ring.fillAmount = 0f;
    }

    void Update()
    {
        if (!gazing) return;

        timer += Time.deltaTime;
        ring.fillAmount = timer / gazeDuration;

        if (timer >= gazeDuration)
        {
            Destroy(gameObject);
        }
    }
}
