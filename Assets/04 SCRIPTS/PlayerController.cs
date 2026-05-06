using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Lumin;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D _rigi;
    Vector2 _move = Vector2.zero;
    [Header("Movement")]
    [SerializeField] private float _speed = 1.0f;

    [Header("Jump")]
    [SerializeField] private int _maxJump = 2;
    [SerializeField] private Vector2 _jumFore;
    private int _jumpCount = 0;

    [Header("GrondCheck")]
    [SerializeField] private GameObject _isGround;
    [SerializeField] private LayerMask _layerGround;
    [SerializeField] private float _radius;

    [Header("References")]
    [SerializeField] GunFire _gun;
    [SerializeField] private UpdateAnimation _anim;

    [Header("IsOnSlope")]
    [SerializeField] private bool IsOnLope;
    [SerializeField] private float angle;
    [SerializeField] private List<PhysicsMaterial2D> _material;

    [Header("HP")]
    [SerializeField] public float _HP = 100f;
    public float currenHP;

 

    [SerializeField] private PlayerState _playerState = PlayerState.idle;
    public PlayerState playerState => _playerState;
    Vector2 _flip = Vector2.zero;
    private float _inputX;
   
    
    void Start()
    {
        _rigi = this.GetComponent<Rigidbody2D>();
        _flip = this.transform.localScale;
        currenHP = _HP;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        HandleState();
        JumpFore();
        HandleInput();
        HandleGunFire();
        IsOnLope_Platformer();
    }
    private void IsOnLope_Platformer()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, 1.5f, _layerGround);
        Debug.DrawRay(hit.point , hit.normal * 1.5f, Color.yellow);
        Debug.DrawRay(this.transform.position, Vector2.down * 1.5f , color : Color.red);
        angle = Mathf.Round(Vector2.Angle(hit.normal , Vector2.up));
        if (angle > 0)
        {
            IsOnLope = true;
        }
        else IsOnLope = false;
    }
    private void FixedUpdate()
    {
        MovePlayer();

    }
    private void HandleState()
    {
        _anim.UpdateAnimationState(_playerState);
       
    }
    private void HandleGunFire()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _gun.GunFireBullet();
            AudioController.instance.PlaySound("bullet");
        }

    }
    private void HandleInput()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
    }
    private void JumpFore()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            if (IsGround())
            {
                _jumpCount = 0;
            }

            if (_jumpCount < _maxJump)
            {
                AudioController.instance.PlaySound("jump");
                _rigi.velocity = new Vector2(_rigi.velocity.x, 0);
                _rigi.AddForce(_jumFore);
                _jumpCount++;
            }
        }
    }
    private bool IsGround()
    {
        return Physics2D.OverlapCircle(_isGround.transform.position, _radius, _layerGround);

    }
    private void MovePlayer()
    {
        _move = _rigi.velocity;
        if (!IsOnLope)
        {
            _move.x = _inputX * _speed;
        }
        else if(IsGround() && IsOnLope)
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
        else _rigi.sharedMaterial = _material[1];
        _rigi.velocity = _move;
        this.transform.localScale = _flip;
    }
    public void UpdateState()
    {
        if (IsGround())
        {
            if (_rigi.velocity.x >= 0.1f || _rigi.velocity.x <= -0.1f)
            {
                _playerState = PlayerState.run;
            }
            else if (_rigi.velocity == Vector2.zero) _playerState = PlayerState.idle;
        }
        else
        {
            _playerState = PlayerState.jump;
        }

    }
    public enum PlayerState{
        idle,
        run,
        jump
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_isGround.transform.position, _radius);

    }
    public void TakeDamge(float damge)
    {
        currenHP -= damge;
        Observer.instance.Notify(CONSTANT.UIDamge);

    }
    public void HeartPicKup(float hp)
    {
        currenHP += hp;
        if(currenHP >= _HP) {  currenHP = _HP; }
        Observer.instance.Notify(CONSTANT.UIDamge);
    }
    
}
