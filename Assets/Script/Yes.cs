using UnityEngine;

public class Yes : MonoBehaviour
{
    [Header("Float (Up & Down)")]
    [SerializeField] private float floatHeight = 0.2f;
    [SerializeField] private float floatSpeed = 1f;

    [Header("Sway (Rotation)")]
    [SerializeField] private float swayAngle = 5f;
    [SerializeField] private float swaySpeed = 0.5f;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Update()
    {
        // FLOAT (Sin wave)
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + Vector3.up * yOffset;

        // SWAY (Gentle rotation)
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
        float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAngle;

        Quaternion swayRotation = Quaternion.Euler(swayX, 0f, swayZ);
        transform.localRotation = startRot * swayRotation;
    }
}
