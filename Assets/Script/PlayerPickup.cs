using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    [Header("Hold & Fire Points")]
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform shootPoint;

    [Header("UI")]
    [SerializeField] private GameObject chargeUI;
    [SerializeField] private Image forceFillImage;
    [SerializeField] private TMP_Text text;

    [Header("Charge Settings")]
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float maxThrowForce = 12f;

    private PickupObject heldObject;

    private bool isCharging;
    private float chargeTimer;

    private bool lastTriggerState;

    public bool IsHolding => heldObject != null;

    private void Start()
    {
        chargeUI.SetActive(false);
        forceFillImage.fillAmount = 0f;
    }

    void Update()
    {
        bool triggerNow = Google.XR.Cardboard.Api.IsTriggerPressed;

        // Detect TAP (press this frame)
        if (triggerNow && !lastTriggerState)
        {
            HandleTap();
        }

        lastTriggerState = triggerNow;

        // Auto-charge over time (no holding)
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            float normalized = Mathf.Clamp01(chargeTimer / maxChargeTime);
            forceFillImage.fillAmount = normalized;
        }
    }

    void HandleTap()
    {
        if (!IsHolding) return;

        // FIRST TAP ? START CHARGING
        if (!isCharging)
        {
            StartCharging();
            return;
        }

        // SECOND TAP ? THROW
        ThrowChargedObject();
    }

    public void PickUp(PickupObject obj)
    {
        if (IsHolding) return;

        heldObject = obj;
        obj.OnPickUp(pickupPoint);

        chargeUI.SetActive(true);
        forceFillImage.fillAmount = 0f;
        chargeTimer = 0f;
        isCharging = false;
        text.text = "Tap to charge";
    }

    // ---------------- CHARGE ----------------

    void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f;
        forceFillImage.fillAmount = 0f;
        text.text = "Tap again to shoot";

        AudioManager.Instance?.PlaySFX("ChargeStart");
    }

    // ---------------- THROW ----------------

    void ThrowChargedObject()
    {
        isCharging = false;

        float normalized = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float force = normalized * maxThrowForce;

        Vector3 dir = Camera.main.transform.forward;

        heldObject.Throw(dir, force);
        heldObject = null;

        chargeTimer = 0f;
        forceFillImage.fillAmount = 0f;
        chargeUI.SetActive(false);

        AudioManager.Instance?.PlaySFX("Throw");
    }
}
