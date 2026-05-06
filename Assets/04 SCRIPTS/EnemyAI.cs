using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    [Header("Speed")]
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private float _chaseSpeed;

    [Header("Detect")]
    [SerializeField] private float _detectRange;
    [SerializeField] private float _attackRange;

    [Header("Patol Points")] 
    [SerializeField] private float distance;

    Vector2 _pointA;
    Vector2 _pointB;
    Transform _player;
    Rigidbody2D _rigi;
    Vector2 _target;
    enum EnemyState{
        Patrol, Chese , Attack
    }
    [SerializeField] private EnemyState _enemyState = EnemyState.Patrol;

    private void Start()
    {
        _pointA = new Vector2(this.transform.position.x + distance , this.transform.position.y);
        _pointB = new Vector2(this.transform.position.x - distance, this.transform.position.y);
        _target = _pointA;
        _rigi = this.GetComponent<Rigidbody2D>();
        _player = GameManager.instance.player.transform;
    }
    private void Update()
    {
        float distance = Vector2.Distance(this.transform.position, _player.position);
       // Debug.Log(distance);
        if (distance <= _attackRange)
        {
            _enemyState = EnemyState.Attack;
        }
        else if (distance <= _detectRange) _enemyState = EnemyState.Chese;
        else _enemyState = EnemyState.Patrol;

    }
    private void FixedUpdate()
    {
        switch (_enemyState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chese:
                Patrol();//Chase();
                break;
            case EnemyState.Attack:
                Patrol();//Attack();
                break;
        }
    }
    private void MoveEnemyAI(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)(this.transform.position)).normalized;
        _rigi.velocity = dir * speed;
    }
    private void Patrol()
    {
        MoveEnemyAI(_target , _patrolSpeed);
       
        if(Vector2.Distance(this.transform.position , _target) < 0.5f){
            _target = _target.Equals((Vector2)_pointA) ? _pointB : _pointA;
        }
    }
    private void Chase()
    {
        MoveEnemyAI(_player.transform.position, _chaseSpeed);
    }
    private void Attack()
    {
        _rigi.velocity = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) return;
        if (collision.gameObject.CompareTag(CONSTANT.BulletTAG))
        {
            GameManager.instance.updateCoin(1f);
            Destroy(this.gameObject);

        }
        if (collision.gameObject.CompareTag(CONSTANT.PlayerTAG))
        {
            GameManager.instance.player.TakeDamge(10f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position , _detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _attackRange);
    }

}