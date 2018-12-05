using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { Patrol, Follow, Count }
public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidBody;
    [Header("Attributes")]
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private int _maxHealth;
    private int _currHealth;
    public int CurrHealth { get { return _currHealth; } private set { _currHealth = Mathf.Clamp(value, 0, _maxHealth); } }
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

    private Transform _targetPosition;
    private bool _moveRequest = true;
    private Vector3 _moveVector = Vector3.right;
    private EnemyStates _currState = EnemyStates.Patrol;
    private delegate void StateMachine();
    private StateMachine[] _enemyStates;

    protected void Awake()
    {
        CurrHealth = _maxHealth;

        _enemyStates = new StateMachine[(int)EnemyStates.Count];
        _enemyStates[(int)EnemyStates.Patrol] = Patrol;
        _enemyStates[(int)EnemyStates.Follow] = Follow;
    }

    protected void FixedUpdate()
    {
        if (_moveRequest)
        {
            transform.Translate(_moveVector * _moveSpeed * Time.deltaTime);
        }
    }

    protected void Update()
    {
        if (_targetPosition == null)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(transform.position + (_moveVector * _playerCheckOffset), _moveVector, _playerCheckSize);
            if (hitResult && hitResult.transform.tag == "Player")
            {
                _targetPosition = hitResult.transform;
                if (AudioManager.instance)
                    AudioManager.instance.Play(_sfx.Get("Alerted").audio);
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
        

        if (_animator)
        {
            _animator.SetBool("IsMoving", _moveRequest);
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }

        if (_enemyStates[(int)_currState] != null)
        {
            _enemyStates[(int)_currState]();
        }
    }

    protected void OnTriggerEnter2D(Collider2D coll)
    {
        PlayerController player = coll.GetComponent<PlayerController>();
        if (player)
        {
            Vector2 knockBackDir = (_moveVector.x > 0) ? Vector2.left : Vector2.right;
            player.TakeDamage(_damage, -knockBackDir, _knockBackForce);
            if (_rigidBody)
            {
                _rigidBody.velocity = Vector2.zero;
                _rigidBody.AddForce(knockBackDir * _knockBackForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("There are null references in the editor!");
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
    public void TakeDamage(int damage)
    {
        CurrHealth -= damage;

        if (CurrHealth <= 0)
        {
            if (AudioManager.instance)
                AudioManager.instance.Play(_sfx.Get("Death").audio);
            _animator.SetBool("IsDead", true);
            _animator.SetTrigger("Die");
        }
        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("Hurt").audio);
        StopMovement();

        if (_animator)
        {
            _animator.SetTrigger("IsTakingDamage");
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }
    public void TakeDamage(int damage, Vector2 knockBackDir, float knockBackForce)
    {
        CurrHealth -= damage;
        if (CurrHealth <= 0)
        {
            if (AudioManager.instance)
                AudioManager.instance.Play(_sfx.Get("Death").audio);
            _animator.SetBool("IsDead", true);
            _animator.SetTrigger("Die");
        }
        else
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.AddForce(knockBackDir * knockBackForce, ForceMode2D.Impulse);
        }
        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("Hurt").audio);

        StopMovement();

        if (_animator)
        {
            _animator.SetTrigger("IsTakingDamage");
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }
    protected void Die()
    {
        if (EnemyManager.instance)
            EnemyManager.instance.EnemyKilled(this);
        Destroy(gameObject);
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
    protected void AllowMovement()
    {
        _moveRequest = true;
    }
    protected void StopMovement()
    {
        _moveRequest = false;
    }
    protected void Flip()
    {
        if (_moveVector.x != 0)
            transform.localScale = (Vector3.right * Mathf.Sign(_moveVector.x) * Mathf.Abs(transform.localScale.x)) + (Vector3.up * transform.localScale.y) + (Vector3.forward * transform.localScale.z);
    }
}