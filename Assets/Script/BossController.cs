using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    // ================= POOLS =================
    [Header("Pools")]
    [SerializeField] private HoverPool hoverPool;
    [SerializeField] private TimerPool timerPool;

    // ================= ATTACK POSITIONS =================
    [Header("Attack Positions")]
    [SerializeField] private List<Transform> teleportPoints;

    // ================= MOVEMENT =================
    [Header("Movement")]
    [SerializeField] private List<Transform> movePoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int attacksBeforeMove = 3;

    private int attackCount;
    private Transform currentMoveTarget;
    private bool isMoving;

    // ================= HEALTH =================
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    public int currentHp;

    // ================= ATTACK SETTINGS =================
    [Header("Attack Settings")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float bulletSpeed = 2f;
    [SerializeField] private float attackCooldown = 3f;
    private float attackTimer;

    // ================= ANGRY MODE =================
    [Header("Angry Mode Settings")]
    [SerializeField] private int angryHpThreshold = 5;
    [SerializeField] private float angryAttackCooldown = 1.5f;
    [SerializeField, Range(0f, 1f)] private float shootAllChance = 0.4f;
    [SerializeField] private ParticleSystem angryAura;
    [SerializeField] private ParticleSystem angryAura2;

    private bool isAngry;

    // ================= DAMAGE FLASH =================
    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;

    private Renderer bossRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    // ================= UI =================
    [Header("UI")]
    [SerializeField] private Image healthFill;

    // ================= FACING =================
    [Header("Facing")]
    [SerializeField] private Transform boatTarget;
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private Transform bossRoot;

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

    [SerializeField] private Animator anim;

    // ================= UNITY =================
    void Start()
    {
        currentHp = maxHealth;

        bossRenderer = GetComponentInChildren<Renderer>();
        originalColor = bossRenderer.material.color;

        UpdateHealthUI();

        foreach (GameObject ws in weakSpots)
            ws.SetActive(false);
    }

    void Update()
    {
        HandleMovement();

        if (!isMoving)
        {
            FaceBoat();
        }

        if (!isMoving)
            HandleAttacks();

        HandleWeakSpots();

        if (isAngry)
        {
            if (!angryAura.isPlaying) angryAura.Play();
            if (!angryAura2.isPlaying) angryAura2.Play();
        }
    }

    // ================= ATTACK =================
    void HandleAttacks()
    {
        // ❌ DO NOT ATTACK WHILE MOVING
        if (isMoving) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            StartCoroutine(ThrowThenAttack());
            attackTimer = 0f;
        }
    }


    IEnumerator ThrowThenAttack()
    {
        anim.SetTrigger("Throw");
        AudioManager.Instance.PlaySFX("roar", 0.2f);

        yield return new WaitForSeconds(0.75f);

        Attack();
    }

    void Attack()
    {
        bool shootAll = isAngry && Random.value < shootAllChance;
        List<Transform> attackPoints = shootAll
            ? new List<Transform>(teleportPoints)
            : GetAttackPositions(2);

        foreach (Transform targetPoint in attackPoints)
        {
            Vector3 dir = (targetPoint.position - firingPoint.position).normalized;
            int bulletType = Random.Range(0, 2);

            if (bulletType == 0)
            {
                GameObject b = hoverPool.GetObject();
                b.GetComponent<VRHover>().Init(bulletSpeed, dir, firingPoint, hoverPool);
            }
            else
            {
                GameObject b = timerPool.GetObject();
                b.GetComponent<VRGazeTimer>().Init(bulletSpeed, dir, firingPoint, timerPool);
            }
        }

        attackCount++;

        if (attackCount >= attacksBeforeMove && !isMoving)
        {
            attackCount = 0;
            StartCoroutine(DelayMove());
        }
    }

    private IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(2f);

        ChooseNewMoveTarget();
    }

    List<Transform> GetAttackPositions(int count)
    {
        List<Transform> available = new List<Transform>(teleportPoints);
        List<Transform> chosen = new List<Transform>();

        for (int i = 0; i < Mathf.Min(count, available.Count); i++)
        {
            int idx = Random.Range(0, available.Count);
            chosen.Add(available[idx]);
            available.RemoveAt(idx);
        }

        return chosen;
    }

    // ================= MOVEMENT =================
    void ChooseNewMoveTarget()
    {
        if (movePoints == null || movePoints.Count == 0 || isMoving)
            return;

        Transform candidate = movePoints[Random.Range(0, movePoints.Count)];

        // 🔥 CHECK: already at this point?
        Vector3 flatBossPos = new Vector3(
            bossRoot.position.x,
            0f,
            bossRoot.position.z
        );

        Vector3 flatTargetPos = new Vector3(
            candidate.position.x,
            0f,
            candidate.position.z
        );

        if (Vector3.Distance(flatBossPos, flatTargetPos) < 0.2f)
        {
            // Too close → ignore movement completely
            return;
        }

        currentMoveTarget = candidate;
        isMoving = true;

        // ✅ ONLY trigger animation if actually moving
        anim.SetTrigger("MoveStart");
        AudioManager.Instance.PlaySFX("BossMove");
    }


    void HandleMovement()
    {
        if (!isMoving || currentMoveTarget == null) return;

        Vector3 targetPos = new Vector3(
            currentMoveTarget.position.x,
            bossRoot.position.y,
            currentMoveTarget.position.z
        );

        // MOVE
        bossRoot.position = Vector3.MoveTowards(
            bossRoot.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // FACE MOVE DIRECTION
        Vector3 moveDir = targetPos - bossRoot.position;
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion moveRot = Quaternion.LookRotation(moveDir);
            bossRoot.rotation = Quaternion.Slerp(
                bossRoot.rotation,
                moveRot,
                Time.deltaTime * rotateSpeed
            );
        }

        // ARRIVAL CHECK
        if (Vector3.Distance(bossRoot.position, targetPos) < 0.1f)
        {
            isMoving = false;
            currentMoveTarget = null;

            anim.SetTrigger("MoveStop");
        }
    }


    // ================= FACING =================
    void FaceBoat()
    {
        if (!boatTarget || !bossRoot) return;

        Vector3 dir = boatTarget.position - bossRoot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        bossRoot.rotation = Quaternion.Slerp(
            bossRoot.rotation,
            rot,
            Time.deltaTime * rotateSpeed
        );
    }

    // ================= WEAK SPOTS =================
    void HandleWeakSpots()
    {
        if (weakSpotActive)
        {
            weakSpotTimer -= Time.deltaTime;
            if (weakSpotTimer <= 0f)
            {
                DeactivateWeakSpot();
                StartWeakSpotCooldown();
            }
        }
        else if (coolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                coolingDown = false;
                ActivateRandomWeakSpot();
            }
        }
        else ActivateRandomWeakSpot();
    }

    void ActivateRandomWeakSpot()
    {
        if (weakSpots.Count == 0) return;

        currentWeakSpot = weakSpots[Random.Range(0, weakSpots.Count)];
        currentWeakSpot.SetActive(true);
        weakSpotTimer = weakSpotActiveTime;
        weakSpotActive = true;
    }

    void DeactivateWeakSpot()
    {
        if (!currentWeakSpot) return;
        currentWeakSpot.SetActive(false);
        currentWeakSpot = null;
        weakSpotActive = false;
    }

    void StartWeakSpotCooldown()
    {
        coolingDown = true;
        cooldownTimer = weakSpotCooldown;
    }

    public void OnWeakSpotHit(GameObject hitSpot)
    {
        if (hitSpot != currentWeakSpot) return;

        Damage();
        DeactivateWeakSpot();
        StartWeakSpotCooldown();
    }

    // ================= DAMAGE =================
    public void Damage()
    {
        currentHp = Mathf.Clamp(--currentHp, 0, maxHealth);
        UpdateHealthUI();

        if (!isAngry && currentHp <= angryHpThreshold)
            EnterAngryMode();

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        AudioManager.Instance.PlaySFX("BossDamage", 0.5f);
        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHp <= 0)
        {
            FindFirstObjectByType<GameManager>()?.EndGame();
            Destroy(gameObject);
        }
    }

    void EnterAngryMode()
    {
        isAngry = true;
        attackCooldown = angryAttackCooldown;
        angryAura?.Play();
        angryAura2?.Play();
    }

    void UpdateHealthUI()
    {
        if (healthFill)
            healthFill.fillAmount = (float)currentHp / maxHealth;
    }

    IEnumerator DamageFlash()
    {
        bossRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        bossRenderer.material.color = originalColor;
    }
}
