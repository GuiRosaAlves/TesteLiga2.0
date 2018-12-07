using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyStates { Patrol, Follow, Charge, Count }
public class Enemy : MonoBehaviour
{
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    
    [Header("References")]
    [SerializeField] private Collider2D _coll;
    [SerializeField] private Collider2D _triggerColl;
    [Header("Stats")]
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private float _maxHP;
    [SerializeField] private float  _knockBackForce;
    [SerializeField] private int _scorePoints;
    [SerializeField] private int _damage;
    
    public float CurrHP { get; private set; }
    public float KnockBackForce { get; private set; }
    public float ScorePoints { get; private set; }

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 10f;
    [SerializeField] private float _acceleration = 60f;

    public float CurrSpeed { get; private set; }
    
    [Header("Raycasting")]
    [SerializeField] private Collider2D _playerCheck;
    [SerializeField] private Collider2D _wallCheck;
    [SerializeField] private Collider2D _fallCheck;
    [SerializeField] private LayerMask _groundMask;

    [Header("Behaviour")] 
    [SerializeField] private float _patrolDelay;
    [SerializeField] private float _targetMaxDistance;
    
    [Header("Triggers")] 
    [SerializeField] private bool _canMove = true;
    private bool _moveRequest = false;
    
    public event Action<int> OnDeath;
    private Vector2 _moveVector;
    public Vector2 MoveVector { get { return _moveVector; } private set { _moveVector = value; } }
    
    private EnemyStates _currState = EnemyStates.Patrol;
    private delegate void StateMachine();
    private StateMachine[] _enemyStates;
    
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CurrHP = _maxHP;
        CurrSpeed = _moveSpeed;
        KnockBackForce = _knockBackForce;
        ScorePoints = _scorePoints;
        
        _enemyStates = new StateMachine[(int)EnemyStates.Count];
        _enemyStates[(int)EnemyStates.Patrol] = Patrol;
        _enemyStates[(int)EnemyStates.Follow] = Follow;
        _enemyStates[(int)EnemyStates.Charge] = Charge;
        _moveVector = (Random.Range(0, 2) == 0) ? Vector2.right : Vector2.left;
        
    }

    private void FixedUpdate()
    {
        if (_canMove && _moveRequest)
        {
            RB.AddForce(_moveVector * _acceleration);
            RB.velocity = Vector2.right * Mathf.Clamp(RB.velocity.x, 0, CurrSpeed) + Vector2.up * RB.velocity.y;
        }
    }
    
    private void Update()
    {
//        if (CurrTarget == null)
//        {
//            var hit = Physics2D.Raycast(transform.position, transform.right, _rayDistance, LayerMask.GetMask("Player"));
//            if (hit)
//            {
//                CurrTarget = hit.transform.GetComponent<Character>();
//                if (_App.SoundManager)
//                    _App.SoundManager.Play(_sfx.Get("Alerted").audio);
//                _currState = EnemyStates.Follow;
//                AllowMovement();
//            }
//            else
//            {
//                CurrTarget = null;
//                _currState = EnemyStates.Patrol;
//            }
//        }else if (Vector2.Distance(transform.position, CurrTarget.transform.position) >= _targetMaxDistance)
//        {
//            CurrTarget = null;
//            MoveVector = (UnityEngine.Random.Range(0, 2) == 0) ? Vector3.right : Vector3.left;
//            _currState = EnemyStates.Patrol;
//        }
        
        if (_enemyStates[(int)_currState] != null)
            _enemyStates[(int)_currState]();
    }

    private void LateUpdate()
    {
        Flip();
        Anim.SetFloat("NormalizedSpeed", Mathf.Abs(_moveVector.x));
    }
    
    private void Patrol()
    {
        if (_playerCheck)
        {
            
        }
    }
    
    private void Follow()
    {
        
    }

    private void Charge()
    {
        
    }
    
//    public void AllowMovement() { _canMove = true; }
//    public void StopMovement() { _canMove = false; }
//    private bool GroundCheck()
//    {
//        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
//        return (Physics2D.OverlapBox(boxCenter, _groundCheckSize, 0f, _groundMask) != null);
//    }
    private void Flip()
    {
        if (MoveVector.x != 0)
            transform.localScale = (Vector3.right * Mathf.Sign(_moveVector.x) * Mathf.Abs(transform.localScale.x)) +  (Vector3.up * transform.localScale.y) + (Vector3.forward * transform.localScale.z);
    }
    
    protected void OnTriggerEnter2D(Collider2D coll)
    {
        Character player = coll.GetComponent<Character>();
        if (player)
        {
            var knockBackDir = transform.position - player.transform.position;
            knockBackDir.Normalize();
            //TODO: Fix the knockback direction
            player.TakeDamage(_damage, -knockBackDir, _knockBackForce);
            if (RB)
            {
                RB.velocity = Vector2.zero;
                RB.AddForce(knockBackDir * _knockBackForce, ForceMode2D.Impulse);
            }
        }
    }

//    public void TakeDamage(int damage, Vector2 knockBackDir, float knockBackForce)
//    {
//        CurrHP -= damage;
//        if (CurrHP <= 0)
//        {
//            Die();
//        }
//        else
//        {
//            RB.velocity = Vector2.zero;
//            RB.AddForce(knockBackDir * knockBackForce, ForceMode2D.Impulse);
//        }
//        if (_App.SoundManager)
//            _App.SoundManager.Play(_sfx.Get("Hurt").audio);
//
//        if (Anim)
//            Anim.SetTrigger("Hit");
//    }
//
//    private void Die()
//    {
//        if (_App.SoundManager)
//            _App.SoundManager.Play(_sfx.Get("Death").audio);
//        RB.constraints = RigidbodyConstraints2D.FreezeAll;
//        _coll.enabled = false;
//        _triggerColl.enabled = false;
//
//        if (OnDeath != null)
//            OnDeath(_scorePoints);
//        
//        Anim.SetTrigger("Death");
//        Destroy(gameObject, 1.5f);
//    }
}