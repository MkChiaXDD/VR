using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Hold & Fire Points")]
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private GameObject DoubleTapText;

    [Header("Double Tap")]
    [SerializeField] private float doubleTapTime = 0.4f;

    private PickupObject heldObject;

    float lastTapTime;
    bool waitingForSecondTap;
    bool lastTriggerState;

    public bool IsHolding => heldObject != null;

    private void Start()
    {
        DoubleTapText.SetActive(false);
    }

    void Update()
    {
        bool triggerNow = Google.XR.Cardboard.Api.IsTriggerPressed;

        if (triggerNow && !lastTriggerState)
        {
            HandleTap();
        }

        lastTriggerState = triggerNow;

        if (waitingForSecondTap && Time.time - lastTapTime > doubleTapTime)
            waitingForSecondTap = false;
    }

    void HandleTap()
    {
        if (!IsHolding) return;

        if (!waitingForSecondTap)
        {
            waitingForSecondTap = true;
            lastTapTime = Time.time;
            return;
        }

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
        DoubleTapText.SetActive(true);
        waitingForSecondTap = false;
    }

    void ThrowHeldObject()
    {
        Vector3 dir = Camera.main.transform.forward;
        DoubleTapText.SetActive(false);
        heldObject.Throw(dir, shootPoint);
        heldObject = null;
    }
}
