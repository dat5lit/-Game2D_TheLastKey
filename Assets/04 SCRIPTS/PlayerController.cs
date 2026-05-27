using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region ENUMS & STATE
    public enum PlayerState
    {
        Grounded,
        Jump,
        Attack,
        Dash,
        Hit,
        Dead
    }

    [Header("State")]
    [SerializeField] private PlayerState _playerState = PlayerState.Grounded;
    public PlayerState playerState => _playerState;
    #endregion

    #region VARIABLES
    [Header("Components")]
    private Rigidbody2D _rigi;
    [SerializeField] private UpdateAnimation _anim;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    private float _speed;
    private float _inputX;
    private Vector2 _move = Vector2.zero;
    private Vector2 _flip = Vector2.zero;

    public float Speed => _speed;

    [Header("Jump")]
    [SerializeField] private int _maxJump = 2;
    [SerializeField] private Vector2 _jumFore;

    private int _jumpCount = 0;

    [Header("Dash")]
    [SerializeField] private float _dashForce = 20f;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    private bool _canDash = true;
    private bool _isDashing = false;

    [Header("Ground Check")]
    [SerializeField] private GameObject _isGround;
    [SerializeField] private LayerMask _layerGround;
    [SerializeField] private float _radius;

    private bool _isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private bool IsOnLope;
    [SerializeField] private float angle;
    [SerializeField] private List<PhysicsMaterial2D> _material;

    [Header("Health Points")]
    [SerializeField] public float _HP = 100f;
    public float currenHP;

    [Header("Invincibility Settings")]
    [SerializeField] private float _invincibilityDuration = 0.2f;

    private bool _isInvincible = false;
    #endregion

    #region ATTACK SETTINGS
    [Header("Attack Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange = 0.5f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _attackDamage = 20f;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _attackDuration = 0.3f;

    private bool _canAttack = true;
    private bool _isAttacking = false;
    #endregion

    #region UNITY LIFECYCLE
    void Start()
    {
        _rigi = GetComponent<Rigidbody2D>();
        _flip = transform.localScale;
        currenHP = _HP;
    }

    void Update()
    {
        CheckGround();

        // Nếu chết thì chỉ update animation
        if (_playerState == PlayerState.Dead)
        {
            UpdateAnimation();
            return;
        }

        UpdateState();
        HandleInput();
        JumpFore();
        HandleAttack();
        HandleDash();
        IsOnLope_Platformer();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (_playerState == PlayerState.Dead) return;

        if (_isDashing || _isAttacking) return;

        MovePlayer();
    }
    #endregion

    #region INPUT & CORE LOGIC
    private void HandleInput()
    {
        if (_playerState == PlayerState.Dead) return;

        _inputX = Input.GetAxisRaw("Horizontal");

        if (_inputX == 0)
        {
            _speed = 0;
        }
        else
        {
            bool isRun = Input.GetKey(KeyCode.LeftShift);
            _speed = isRun ? runSpeed : walkSpeed;
        }
    }

    private void JumpFore()
    {
        if (_playerState == PlayerState.Dead || _isDashing || _isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded)
            {
                _jumpCount = 0;
            }

            if (_jumpCount < _maxJump)
            {
                AudioController.instance.PlaySound("jump");

                _anim.Jump();

                _rigi.velocity = new Vector2(_rigi.velocity.x, 0);

                _rigi.AddForce(_jumFore);

                _jumpCount++;

                _canDash = true;
            }
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Q)
            && _canDash
            && !_isDashing
            && !_isAttacking
            && _playerState != PlayerState.Dead)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private void HandleAttack()
    {
        if (_playerState == PlayerState.Dead
            || _isDashing
            || !_canAttack)
            return;

        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(AttackRoutine(1));

        else if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(AttackRoutine(2));

        else if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(AttackRoutine(3));
    }
    #endregion

    #region PHYSICS & MOVEMENT
    private void MovePlayer()
    {
        _move = _rigi.velocity;

        if (!IsOnLope)
        {
            _move.x = _inputX * _speed;
        }
        else if (_isGrounded && IsOnLope)
        {
            _move.x = Mathf.Cos(angle * Mathf.Deg2Rad) * _inputX * _speed;
            _move.y = Mathf.Sin(angle * Mathf.Deg2Rad) * _inputX * _speed;
        }

        if (_inputX > 0)
        {
            _flip.x = Mathf.Abs(_flip.x);
            _rigi.sharedMaterial = _material[0];
        }
        else if (_inputX < 0)
        {
            _flip.x = -Mathf.Abs(_flip.x);
            _rigi.sharedMaterial = _material[0];
        }
        else
        {
            _rigi.sharedMaterial = _material[1];
        }

        _rigi.velocity = _move;

        transform.localScale = _flip;
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(
            _isGround.transform.position,
            _radius,
            _layerGround
        );

        if (_isGrounded)
        {
            _canDash = true;
        }
    }

    private void IsOnLope_Platformer()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            1.5f,
            _layerGround
        );

        Debug.DrawRay(hit.point, hit.normal * 1.5f, Color.yellow);
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);

        angle = Mathf.Round(Vector2.Angle(hit.normal, Vector2.up));

        IsOnLope = angle > 0;
    }
    #endregion

    #region ACTION & SKILL EXECUTION
    private IEnumerator DashRoutine()
    {
        _canDash = false;
        _isDashing = true;

        _playerState = PlayerState.Dash;

        _anim.Dash();

        _rigi.velocity = Vector2.zero;

        float originalGravity = _rigi.gravityScale;

        _rigi.gravityScale = 0f;

        float dashDirection = Mathf.Sign(transform.localScale.x);

        _rigi.velocity = new Vector2(dashDirection * _dashForce, 0f);

        yield return new WaitForSeconds(_dashTime);

        _rigi.velocity = new Vector2(0f, _rigi.velocity.y);

        _rigi.gravityScale = originalGravity;

        _isDashing = false;

        _playerState = _isGrounded
            ? PlayerState.Grounded
            : PlayerState.Jump;

        yield return new WaitForSeconds(_dashCooldown);

        _canDash = true;
    }

    private IEnumerator AttackRoutine(int comboStep)
    {
        _canAttack = false;

        _isAttacking = true;

        _playerState = PlayerState.Attack;

        _anim.SetCombo(comboStep);

        _anim.Attack();

        if (_isGrounded)
        {
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
        }
        else
        {
            _rigi.velocity = new Vector2(_rigi.velocity.x * 0.2f, _rigi.velocity.y);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            _attackPoint.position,
            _attackRange,
            _enemyLayer
        );

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("enemy"))
            {
                EnemyAI enemy = enemyCollider.GetComponent<EnemyAI>();

                if (enemy != null)
                {
                    enemy.TakeDamage(_attackDamage);
                }
            }
        }

        yield return new WaitForSeconds(_attackDuration);

        _isAttacking = false;

        _playerState = _isGrounded
            ? PlayerState.Grounded
            : PlayerState.Jump;

        float remainingCooldown =
            Mathf.Max(0, _attackCooldown - _attackDuration);

        yield return new WaitForSeconds(remainingCooldown);

        _canAttack = true;
    }

    // =========================
    // DEAD ROUTINE
    // =========================
    private IEnumerator DeadRoutine()
    {
        _playerState = PlayerState.Dead;

        // khóa toàn bộ action
        _isAttacking = false;
        _isDashing = false;

        _canAttack = false;
        _canDash = false;

     
        // chạy animation chết
        _anim.Dead();

        // đợi animation chạy xong
        yield return new WaitForSeconds(1.5f);

        // giữ nguyên xác
        this.enabled = false;
        // dừng di chuyển
        _rigi.velocity = Vector2.zero;

        // khóa gravity nếu muốn xác đứng yên
        _rigi.gravityScale = 0;

    }
    public void TakeDamge(float damge)
    {
        if (_isInvincible || _playerState == PlayerState.Dead)
            return;

        currenHP -= damge;

        if (currenHP <= 0)
        {
            currenHP = 0;

            StartCoroutine(DeadRoutine());
        }
        else
        {
            _anim.Hit();

            StartCoroutine(InvincibilityRoutine());
        }

        Observer.instance.Notify(CONSTANT.UIDamge);
    }

    private IEnumerator InvincibilityRoutine()
    {
        _isInvincible = true;

        _playerState = PlayerState.Hit;

        yield return new WaitForSeconds(_invincibilityDuration);

        _playerState = _isGrounded
            ? PlayerState.Grounded
            : PlayerState.Jump;

        _isInvincible = false;
    }

    public void HeartPicKup(float hp)
    {
        currenHP = Mathf.Clamp(currenHP + hp, 0, _HP);

        Observer.instance.Notify(CONSTANT.UIDamge);
    }
    #endregion

    #region ANIMATION & STATE HANDLING
    public void UpdateState()
    {
        if (_playerState == PlayerState.Dead)
            return;

        if (_isDashing)
        {
            _playerState = PlayerState.Dash;
            return;
        }

        if (_isAttacking)
        {
            _playerState = PlayerState.Attack;
            return;
        }

        if (_isInvincible)
        {
            _playerState = PlayerState.Hit;
            return;
        }

        _playerState = !_isGrounded
            ? PlayerState.Jump
            : PlayerState.Grounded;
    }

    private void UpdateAnimation()
    {
        _anim.SetGrounded(_isGrounded);

        _anim.SetSpeed(Mathf.Abs(_inputX) * _speed);
    }

    public void EndAttack()
    {

    }
    #endregion

    #region EDITOR GIZMOS
    private void OnDrawGizmos()
    {
        if (_isGround != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(
                _isGround.transform.position,
                _radius
            );
        }

        if (_attackPoint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                _attackPoint.position,
                _attackRange
            );
        }
    }
    #endregion
}