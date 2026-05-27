using System.Collections; // Bắt buộc phải có để chạy Coroutine
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region VARIABLES
    [Header("Speed")]
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private float _chaseSpeed;

    [Header("Detect")]
    [SerializeField] private float _detectRange;
    [SerializeField] private float _attackRange;

    [Header("Patrol Points")]
    [SerializeField] private float distance;

    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    [Header("Hit Effects & Stun")]
    [SerializeField] private GameObject _hitVFXPrefab;       // Prefab hiệu ứng dính đòn
    [SerializeField] private float _knockbackHorizontal = 1.5f; // Lực đẩy lùi ngang
    [SerializeField] private float _knockbackVertical = 2f;    // Lực nẩy lên nhẹ
    [SerializeField] private float _stunDuration = 1.5f;       // Thời gian quái bị CHOÁNG
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.15f;

    private bool _isStunned = false; // Trạng thái đang bị choáng/khựng

    // 🔥 CẬP NHẬT: Cấu hình gây sát thương liên tục lên Player
    [Header("Damage to Player Settings")]
    [SerializeField] private float _damageToPlayer = 10f;       // Sát thương gây ra cho Player
    [SerializeField] private float _damageCooldown = 0.5f;     // Cứ mỗi 0.5 giây ở cạnh quái sẽ bị trừ máu tiếp
    private float _nextDamageTime = 0f;                         // Bộ đếm thời gian cho đợt sát thương kế tiếp

    Vector2 _pointA;
    Vector2 _pointB;
    Transform _player;
    Rigidbody2D _rigi;
    SpriteRenderer _spriteRenderer;
    Vector2 _target;
    private Vector2 _flipScale;

    enum EnemyState
    {
        Patrol, Chese, Attack
    }
    [SerializeField] private EnemyState _enemyState = EnemyState.Patrol;
    #endregion

    #region UNITY LIFECYCLE
    private void Start()
    {
        _pointA = new Vector2(this.transform.position.x + distance, this.transform.position.y);
        _pointB = new Vector2(this.transform.position.x - distance, this.transform.position.y);
        _target = _pointA;
        _rigi = this.GetComponent<Rigidbody2D>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _player = GameManager.instance.player.transform;

        _currentHealth = _maxHealth;
        _flipScale = this.transform.localScale;
    }

    private void Update()
    {
        if (_currentHealth <= 0) return;
        if (_isStunned) return; // Nếu đang bị choáng thì đứng im, không quét tìm Player

        float distanceToPlayer = Vector2.Distance(this.transform.position, _player.position);

        if (distanceToPlayer <= _attackRange)
        {
            _enemyState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= _detectRange)
        {
            _enemyState = EnemyState.Chese;
        }
        else
        {
            _enemyState = EnemyState.Patrol;
        }
    }

    private void FixedUpdate()
    {
        if (_isStunned)
        {
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
            return;
        }
        if (_currentHealth <= 0) return;

        switch (_enemyState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chese:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }
    #endregion

    #region MOVEMENT LOGIC
    private void MoveEnemyAI(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)(this.transform.position)).normalized;
        _rigi.velocity = new Vector2(dir.x * speed, _rigi.velocity.y);

        if (_rigi.velocity.x > 0.1f)
        {
            _flipScale.x = Mathf.Abs(_flipScale.x);
        }
        else if (_rigi.velocity.x < -0.1f)
        {
            _flipScale.x = -Mathf.Abs(_flipScale.x);
        }
        this.transform.localScale = _flipScale;
    }

    private void Patrol()
    {
        MoveEnemyAI(_target, _patrolSpeed);

        if (Vector2.Distance(this.transform.position, _target) < 0.5f)
        {
            _target = _target.Equals((Vector2)_pointA) ? _pointB : _pointA;
        }
    }

    private void Chase()
    {
        MoveEnemyAI(_player.position, _chaseSpeed);
    }

    private void Attack()
    {
        _rigi.velocity = new Vector2(0, _rigi.velocity.y);
    }
    #endregion

    #region COMBAT & DAMAGE SYSTEM
    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damageAmount;
        Debug.Log(this.gameObject.name + " bị chém! Máu còn: " + _currentHealth);

        SpawnHitVFX();

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(StunAndKnockbackRoutine());
            StartCoroutine(FlashColorCoroutine());
        }
    }

    private void SpawnHitVFX()
    {
        if (_hitVFXPrefab != null)
        {
            Vector3 spawnPosition = this.transform.position;
            spawnPosition.z = 0f;
            GameObject vfx = Instantiate(_hitVFXPrefab, spawnPosition, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }

    private IEnumerator StunAndKnockbackRoutine()
    {
        _isStunned = true;

        float knockbackDirection = this.transform.position.x - _player.position.x;
        knockbackDirection = Mathf.Sign(knockbackDirection);

        _rigi.velocity = new Vector2(knockbackDirection * _knockbackHorizontal, _knockbackVertical);

        yield return new WaitForSeconds(0.15f);
        _rigi.velocity = new Vector2(0, _rigi.velocity.y);

        yield return new WaitForSeconds(_stunDuration - 0.15f);

        _isStunned = false;
    }

    private IEnumerator FlashColorCoroutine()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _flashColor;
            yield return new WaitForSeconds(_flashDuration);
            _spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        Debug.Log(this.gameObject.name + " đã bị tiêu diệt!");
        _rigi.velocity = Vector2.zero;

        GameManager.instance.updateCoin(1f);
        Destroy(this.gameObject);
    }

    // 1. Khi vừa mới chạm vào quái (Gây sát thương ngay lập tức phát đầu tiên)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.CompareTag(CONSTANT.BulletTAG))
        {
            TakeDamage(20f);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag(CONSTANT.PlayerTAG))
        {
            // Kiểm tra nếu đã hết thời gian hồi sát thương thì trừ máu
            if (Time.time >= _nextDamageTime)
            {
                GameManager.instance.player.TakeDamge(_damageToPlayer);
                _nextDamageTime = Time.time + _damageCooldown; // Thiết lập mốc thời gian cho lần trừ máu kế tiếp
            }
        }
    }

    // 2. 🔥 CẬP NHẬT: Khi tiếp tục đứng dính (ôm) vào quái, máu sẽ liên tục bị trừ theo chu kỳ thời gian
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.CompareTag(CONSTANT.PlayerTAG))
        {
            // Nếu người chơi vẫn đứng dính vào quái và bộ đếm thời gian cooldown đã hết
            if (Time.time >= _nextDamageTime)
            {
                GameManager.instance.player.TakeDamge(_damageToPlayer);
                _nextDamageTime = Time.time + _damageCooldown; // Cập nhật lại mốc thời gian hồi chiêu mới
            }
        }
    }
    #endregion

    #region EDITOR GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, _detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _attackRange);
    }
    #endregion
}