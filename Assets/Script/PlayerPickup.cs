using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private Transform pickupPoint;

    [Header("Double Tap Settings")]
    [SerializeField] private float doubleTapTime = 0.4f;

    private PickupObject heldObject;

    float lastTapTime;
    bool waitingForSecondTap;

    bool lastTriggerState; // IMPORTANT

    public bool IsHolding => heldObject != null;

    void Update()
    {
        bool triggerNow = Google.XR.Cardboard.Api.IsTriggerPressed;

        // Detect trigger DOWN (edge)
        if (triggerNow && !lastTriggerState)
        {
            HandleTap();
        }

        lastTriggerState = triggerNow;

        // Timeout double tap
        if (waitingForSecondTap && Time.time - lastTapTime > doubleTapTime)
        {
            waitingForSecondTap = false;
        }
    }

    void HandleTap()
    {
        if (!IsHolding) return;

        // FIRST TAP
        if (!waitingForSecondTap)
        {
            waitingForSecondTap = true;
            lastTapTime = Time.time;
            return;
        }

        // SECOND TAP
        if (Time.time - lastTapTime <= doubleTapTime)
        {
            ThrowHeldObject();
            waitingForSecondTap = false;
        }
    }

    public void PickUp(PickupObject obj)
    {
        if (IsHolding) return;

        heldObject = obj;
        obj.OnPickUp(pickupPoint);

        // RESET TAP STATE AFTER PICKUP
        waitingForSecondTap = false;
    }

    void ThrowHeldObject()
    {
        Vector3 dir = Camera.main.transform.forward;

        heldObject.Throw(dir);
        heldObject = null;
    }
}
