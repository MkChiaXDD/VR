using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private HoverPool hoverPool;
    [SerializeField] private TimerPool timerPool;

    [Header("Attack Positions")]
    [SerializeField] private List<Transform> teleportPoints;

    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    private int currentHp;

    [Header("Attack Settings")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float bulletSpeed = 2f;
    [SerializeField] private float attackCooldown = 3f;
    private float attackTimer;

    [Header("Angry Mode Settings")]
    [SerializeField] private int angryHpThreshold = 5;
    [SerializeField] private float angryAttackCooldown = 1.5f;
    [SerializeField, Range(0f, 1f)] private float shootAllChance = 0.4f;

    private bool isAngry;


    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;

    private Renderer bossRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    [Header("UI")]
    [SerializeField] private Image healthFill;

    // ================= WEAK SPOTS =================

    [Header("Weak Spots")]
    [SerializeField] private List<GameObject> weakSpots;

    [SerializeField] private float weakSpotActiveTime = 10f;
    [SerializeField] private float weakSpotCooldown = 5f;

    private GameObject currentWeakSpot;
    private float weakSpotTimer;
    private float cooldownTimer;

    private bool weakSpotActive;
    private bool coolingDown;

    // ================= UNITY =================

    void Start()
    {
        currentHp = maxHealth;

        bossRenderer = GetComponentInChildren<Renderer>();
        originalColor = bossRenderer.material.color;

        UpdateHealthUI();

        // Make sure all weak spots start OFF
        foreach (GameObject ws in weakSpots)
        {
            ws.SetActive(false);
        }
    }

    void Update()
    {
        HandleAttacks();
        HandleWeakSpots();
    }

    // ================= ATTACK LOGIC =================

    void HandleAttacks()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    private List<Transform> GetAttackPositions(int count)
    {
        List<Transform> available = new List<Transform>(teleportPoints);
        List<Transform> chosen = new List<Transform>();

        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            chosen.Add(available[index]);
            available.RemoveAt(index);
        }

        return chosen;
    }

    private void Attack()
    {
        bool shootAll = isAngry && Random.value < shootAllChance;

        List<Transform> attackPoints;

        if (shootAll)
        {
            // Shoot from ALL attack positions
            attackPoints = new List<Transform>(teleportPoints);
        }
        else
        {
            // Normal behaviour (2 random positions)
            attackPoints = GetAttackPositions(2);
        }

        for (int i = 0; i < attackPoints.Count; i++)
        {
            Transform targetPoint = attackPoints[i];

            Vector3 direction =
                (targetPoint.position - firingPoint.position).normalized;

            int bulletType = Random.Range(0, 2);

            if (bulletType == 0)
            {
                GameObject bullet = hoverPool.GetObject();
                VRHover bulletScript = bullet.GetComponent<VRHover>();

                bulletScript.Init(bulletSpeed, direction, firingPoint, hoverPool);
            }
            else
            {
                GameObject bullet = timerPool.GetObject();
                VRGazeTimer bulletScript = bullet.GetComponent<VRGazeTimer>();

                bulletScript.Init(bulletSpeed, direction, firingPoint, timerPool);
            }
        }
    }


    // ================= WEAK SPOT LOGIC =================

    void HandleWeakSpots()
    {
        // Weak spot active ? countdown
        if (weakSpotActive)
        {
            weakSpotTimer -= Time.deltaTime;

            if (weakSpotTimer <= 0f)
            {
                DeactivateWeakSpot();
                StartWeakSpotCooldown();
            }
        }
        // Cooling down ? countdown
        else if (coolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                coolingDown = false;
                ActivateRandomWeakSpot();
            }
        }
        // Nothing active yet
        else
        {
            ActivateRandomWeakSpot();
        }
    }

    void ActivateRandomWeakSpot()
    {
        if (weakSpots.Count == 0) return;

        int index = Random.Range(0, weakSpots.Count);
        currentWeakSpot = weakSpots[index];

        currentWeakSpot.SetActive(true);

        weakSpotTimer = weakSpotActiveTime;
        weakSpotActive = true;
    }

    void DeactivateWeakSpot()
    {
        if (currentWeakSpot == null) return;

        currentWeakSpot.SetActive(false);
        currentWeakSpot = null;
        weakSpotActive = false;
    }

    void StartWeakSpotCooldown()
    {
        coolingDown = true;
        cooldownTimer = weakSpotCooldown;
    }

    // ================= CALLED BY WEAK SPOT =================

    public void OnWeakSpotHit(GameObject hitSpot)
    {
        if (!weakSpotActive) return;
        if (hitSpot != currentWeakSpot) return;

        Damage();
        DeactivateWeakSpot();
        StartWeakSpotCooldown();
    }

    // ================= DAMAGE =================

    public void Damage()
    {
        currentHp--;
        currentHp = Mathf.Clamp(currentHp, 0, maxHealth);

        // ENTER ANGRY MODE
        if (!isAngry && currentHp <= angryHpThreshold)
        {
            EnterAngryMode();
        }

        UpdateHealthUI();

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void EnterAngryMode()
    {
        isAngry = true;
        attackCooldown = angryAttackCooldown;

        Debug.Log("Boss entered ANGRY MODE");
    }


    void UpdateHealthUI()
    {
        if (healthFill == null) return;

        float normalizedHealth = (float)currentHp / maxHealth;
        healthFill.fillAmount = normalizedHealth;
    }

    IEnumerator DamageFlash()
    {
        bossRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        bossRenderer.material.color = originalColor;
    }

}
