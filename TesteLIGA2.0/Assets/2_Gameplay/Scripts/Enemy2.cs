using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D _coll;
    [SerializeField] private Collider2D _triggerColl;
    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }
    
    [Header("Attributes")]
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private int _maxHealth;
    public int CurrHealth { get; private set; }
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private int _scorePoints;
    public int ScorePoints { get { return _scorePoints; } }
    [SerializeField] private float _knockBackForce;
    public float KnockBackForce { get { return _knockBackForce; } }
    [Header("Behaviour Variables")]
    [SerializeField] private float _patrolDelay;
    [SerializeField] private float _targetMaxDistance;
    [Header("Raycast Variables")]
    [SerializeField] private Vector3 _wallCheckSize;
    [SerializeField] private Vector3 _groundCheckSize;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private float _playerCheckSize;
    [SerializeField] private float _playerCheckOffset;
    [Header("Debug")]
    [SerializeField] private bool _drawWallCheck;
    [SerializeField] private bool _drawGroundCheck;
    [SerializeField] private bool _drawPlayerCheck;
    [SerializeField] private bool _canMove = true;

    private Transform _targetPosition;
    private bool _moveRequest = true;
    private Vector3 _moveVector = Vector3.right;
    private EnemyStates _currState = EnemyStates.Patrol;
    private delegate void StateMachine();
    private StateMachine[] _enemyStates;
    public event Action<Enemy2> OnDeath;

    protected void Awake()
    {
        CurrHealth = _maxHealth;
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        
        _enemyStates = new StateMachine[(int)EnemyStates.Count];
        _enemyStates[(int)EnemyStates.Patrol] = Patrol;
        _enemyStates[(int)EnemyStates.Follow] = Follow;
    }

    protected void FixedUpdate()
    {
        if (_canMove && _moveRequest)
        {
            transform.Translate(_moveVector * _moveSpeed * Time.deltaTime);
        }
    }

    protected void Update()
    {
        if (_targetPosition == null)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(transform.position + (_moveVector * _playerCheckOffset), _moveVector, _playerCheckSize, LayerMask.GetMask("Player"));
            if (hitResult)
            {
                _targetPosition = hitResult.transform;
                if (_App.SoundManager)
                    _App.SoundManager.Play(_sfx.Get("Alerted").audio);
                _currState = EnemyStates.Follow;
                AllowMovement();
            }
            else
            {
                _targetPosition = null;
                _currState = EnemyStates.Patrol;
            }
        }
        else if (Vector2.Distance(transform.position, _targetPosition.position) >= _targetMaxDistance)
        {
            _targetPosition = null;
            _moveVector = (UnityEngine.Random.Range(0, 2) == 0) ? Vector3.right : Vector3.left;
            _currState = EnemyStates.Patrol;
        }
        
        if (_enemyStates[(int)_currState] != null)
            _enemyStates[(int)_currState]();
    }

    private void LateUpdate()
    {
        if (Anim)
            Anim.SetBool("IsMoving", _moveRequest);
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

    protected void OnDrawGizmos()
    {
        if (_drawWallCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (_moveVector * _wallCheckSize.x), _wallCheckSize);
        }
        if (_drawGroundCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + (_moveVector * (_groundCheckSize.x + _groundCheckOffset.x)) + (Vector3.up * _groundCheckOffset.y), _groundCheckSize);
        }
        if (_drawPlayerCheck)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + (_playerCheckOffset * _moveVector), transform.position + _moveVector * _playerCheckSize);
        }
    }
    
    public void TakeDamage(int damage, Vector2 knockBackDir, float knockBackForce)
    {
        CurrHealth -= damage;
        
        if (CurrHealth <= 0)
        {
            Die();
        }
        else
        {
            RB.velocity = Vector2.zero;
            RB.AddForce(knockBackDir * knockBackForce, ForceMode2D.Impulse);
        }
        if (_App.SoundManager)
            _App.SoundManager.Play(_sfx.Get("Hurt").audio);

        if (Anim)
            Anim.SetTrigger("Hit");
        
        if (_App.Instance)
            _App.Instance.Snooze();
    }
    protected void Die()
    {
        _canMove = false;
        
        if (_App.SoundManager)
            _App.SoundManager.Play(_sfx.Get("Death").audio);
        RB.constraints = RigidbodyConstraints2D.FreezeAll;
        _coll.enabled = false;
        _triggerColl.enabled = false;

        if (OnDeath != null)
            OnDeath(this);
        
        Anim.SetTrigger("Death");
        Destroy(gameObject, 1.5f);
    }
    protected void Patrol()
    {
        if (_moveRequest)
        {
            RaycastHit2D hitResult = Physics2D.BoxCast(transform.position + (_moveVector * _wallCheckSize.x), _wallCheckSize, 0, transform.forward, _wallCheckSize.x, LayerMask.GetMask("Ground"));
            RaycastHit2D groundCheck = Physics2D.BoxCast(transform.position + (_moveVector * (_groundCheckSize.x +_groundCheckOffset.x)) + (Vector3.up * _groundCheckOffset.y), _groundCheckSize, 0, transform.forward, _groundCheckSize.x, LayerMask.GetMask("Ground"));
            if (hitResult || !groundCheck)
            {
                StopMovement();
                _moveVector *= -1;
                Invoke("AllowMovement", _patrolDelay);
            }
        }
    }
    protected void Follow()
    {
        _moveVector = _targetPosition.position - transform.position;
        _moveVector.Normalize();
    }
    protected void AllowMovement() { _moveRequest = true; }
    protected void StopMovement() {  _moveRequest = false;  }
}