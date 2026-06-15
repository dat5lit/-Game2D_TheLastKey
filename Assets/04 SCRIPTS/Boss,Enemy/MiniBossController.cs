using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MiniBossController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHP = 200f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackDamage = 20f;

    [Header("AI Range")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float distancePlayer = 14f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MiniBossAnimator bossAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    [Header("Drop Item")]
    [SerializeField] private GameObject keyPrefab;
   
    Vector2 _moveMinBoss = Vector2.zero;
    [SerializeField] private HealthBarWorld healthBar;
    [SerializeField] private EnemyHealthBar healthBarUI;

    [Header("Dialogue")]
    private bool hasShownMessage = false;
    public enum BossState { Idle, Walk, Attack, Hurt, Die }
    private BossState currentState = BossState.Idle;

    private float currentHP;
    private float attackTimer;
    private bool isDead;
    private bool isFacingRight = true;
    private bool isChasing;
  
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        healthBar.SetMaxHealth( maxHP);
        healthBarUI.SetTarget(transform);
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        // Bỏ qua Update AI nếu đang trong trạng thái bị khóa
        if (currentState == BossState.Attack && attackTimer > 0)
            return;
        float distToPlayer = Vector2.Distance((Vector2)this.transform.position,(Vector2)playerTransform.position); 
      
        UpdateAI(distToPlayer);
        HandleMovement(distToPlayer);
        FlipSprite();
    }

    private void UpdateAI(float distToPlayer)
    {
        // 2. Nếu thấy Player
        if(isDead) return;
        if (distToPlayer <= detectionRange)
        {
            if (!hasShownMessage)
            {
                hasShownMessage = true;
                Observer.instance.Notify(CONSTANT.BossMessage);
                rb.velocity = Vector3.zero;
                return;
            }
            isChasing = true;
            if (distToPlayer <= distancePlayer && attackTimer <= 0)
            {
                ChangeState(BossState.Attack);
                attackTimer = attackCooldown;
              
            }
            else if (distToPlayer > distancePlayer)
            {
                ChangeState(BossState.Walk);
            }
        }
        else
        {
            isChasing = false;
            ChangeState(BossState.Idle);
        }
    }
   
    private void HandleMovement(float distacetoPlayer)
    {
        if (!isChasing)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        if(distacetoPlayer <= distancePlayer)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        Vector2 dir = ((Vector2)playerTransform.transform.position - (Vector2)transform.position).normalized;
        _moveMinBoss.x = dir.x;
        _moveMinBoss.y =0;
        rb.velocity = _moveMinBoss * moveSpeed;
    }

    private void FlipSprite()
    { 
        float direction = transform.position.x- playerTransform.position.x;
        if (Mathf.Abs(direction) > 0.1f) SetFacing(direction > 0);
    }

    private void SetFacing(bool faceRight)
    {
        if (isFacingRight == faceRight) return;
        isFacingRight = faceRight;
        Vector3 s = transform.localScale;
        s.x = faceRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        bossAnimator?.OnStateChanged(currentState);

        if (newState == BossState.Attack)
        {
            attackTimer = attackCooldown;
          
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;

        healthBar.SetHealth(currentHP);

        isChasing = true;

        Debug.Log("Boss HP còn lại: " + currentHP);

        if (currentHP <= 0f)
            Die();
        else
            StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        ChangeState(BossState.Hurt);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f); // Thời gian khựng khi bị đánh
        ChangeState(BossState.Walk);
    }

    public void OnAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        Debug.Log("Hit Count: " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Hit: " + hit.name);

            PlayerController player = hit.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                player.TakeDamge(attackDamage);
            }
        }
    }
    private void Die()
    {
        isDead = true;

        healthBar.gameObject.SetActive(false);

        ChangeState(BossState.Die);

        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(SpawnKeyAfterDeath());
    }

    private IEnumerator SpawnKeyAfterDeath()
    {
        // Chờ thời gian animation Dead
        yield return new WaitForSeconds(1.5f);

        if (keyPrefab != null)
        {
            Instantiate(
                keyPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (attackPoint != null) { Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}