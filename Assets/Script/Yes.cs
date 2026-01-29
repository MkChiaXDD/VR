using System.Collections.Generic;
using UnityEngine;

public class Yes : MonoBehaviour
{
    [Header("Float (Up & Down)")]
    [SerializeField] private float floatHeight = 0.2f;
    [SerializeField] private float floatSpeed = 1f;

    [Header("Sway (Rotation)")]
    [SerializeField] private float swayAngle = 5f;
    [SerializeField] private float swaySpeed = 0.5f;

    [Header("Boat Move Things")]
    [SerializeField] private List<Transform> waypointList;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float stayDuration = 2f;
    [SerializeField] private float waypointThreshold = 0.1f;

    [Header("Face Boss")]
    [SerializeField] private Transform bossTarget;
    [SerializeField] private float rotateSpeed = 2f;

    private int currentWaypointIndex;
    private float stayTimer;

    private Vector3 basePos;

    // IMPORTANT: base rotation that faces the boss
    private Quaternion baseFacingRotation;

    void Start()
    {
        basePos = transform.position;
        baseFacingRotation = transform.rotation;
    }

    void Update()
    {
        HandleWaypointMovement();
        FaceBoss();
        HandleFloatAndSway();
    }

    // ================= WAYPOINT MOVE =================

    void HandleWaypointMovement()
    {
        if (waypointList == null || waypointList.Count == 0)
            return;

        Transform target = waypointList[currentWaypointIndex];

        Vector3 targetPos = new Vector3(
            target.position.x,
            basePos.y,
            target.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) <= waypointThreshold)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayDuration)
            {
                stayTimer = 0f;
                currentWaypointIndex =
                    (currentWaypointIndex + 1) % waypointList.Count;
            }
        }
    }

    // ================= FACE BOSS (Y AXIS ONLY) =================

    void FaceBoss()
    {
        if (bossTarget == null) return;

        Vector3 direction = bossTarget.position - transform.position;
        direction.y = 0f; // Y-axis only (VR safe)

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction);

        baseFacingRotation = Quaternion.Slerp(
            baseFacingRotation,
            targetRot,
            Time.deltaTime * rotateSpeed
        );
    }

    // ================= FLOAT + SWAY =================

    void HandleFloatAndSway()
    {
        // FLOAT (vertical only)
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(
            transform.position.x,
            basePos.y + yOffset,
            transform.position.z
        );

        // SWAY (X/Z only)
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
        float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAngle;

        Quaternion swayRotation = Quaternion.Euler(swayX, 0f, swayZ);

        // COMBINE: face boss + sway
        transform.rotation = baseFacingRotation * swayRotation;
    }
}
