using System;
using UnityEngine;

public enum EnemyStates { Patrol, Follow, Count }
public class Enemy : MonoBehaviour
{
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    
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
    [SerializeField] private float _moveSpeed;
    //TODO: Acceleration variable

    [Header("Ground Check")]
    [SerializeField] private Vector3 _wallCheckSize;
    [SerializeField] private Vector2 _characterSize;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _groundMask;

    [Header("Player Check")] 
    [SerializeField] private float _patrolDelay;
    [SerializeField] private float _rayDistance;
    [SerializeField] private float _rayOffset;
    [SerializeField] private float _targetMaxDistance;
    
    [Header("Triggers")] 
    [SerializeField] private bool _canMove = true;
    
    public bool IsGrounded { get; private set; }
    
    public event Action<int> OnDeath;

    public Character CurrTarget { get; private set; }
    public Vector2 MoveVector { get; private set; }
    
    private EnemyStates _currState = EnemyStates.Patrol;
    private delegate void StateMachine();
    private StateMachine[] _enemyStates;
    
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CurrHP = _maxHP;
        KnockBackForce = _knockBackForce;
        ScorePoints = _scorePoints;
        
        _enemyStates = new StateMachine[(int)EnemyStates.Count];
        _enemyStates[(int)EnemyStates.Patrol] = Patrol;
        _enemyStates[(int)EnemyStates.Follow] = Follow;
    }

    private void FixedUpdate()
    {
        if (_canMove)
            transform.Translate(MoveVector * _moveSpeed * Time.deltaTime);
    }
    
    private void Update()
    {
        if (CurrTarget == null)
        {
            var hit = Physics2D.Raycast(transform.position, transform.right, _rayDistance, LayerMask.GetMask("Player"));
            if (hit)
            {
                CurrTarget = hit.transform.GetComponent<Character>();
                if (_App.SoundManager)
                    _App.SoundManager.Play(_sfx.Get("Alerted").audio);
                _currState = EnemyStates.Follow;
                AllowMovement();
            }
            else
            {
                CurrTarget = null;
                _currState = EnemyStates.Patrol;
            }
        }else if (Vector2.Distance(transform.position, CurrTarget.transform.position) >= _targetMaxDistance)
        {
            CurrTarget = null;
            MoveVector = (UnityEngine.Random.Range(0, 2) == 0) ? Vector3.right : Vector3.left;
            _currState = EnemyStates.Patrol;
        }
        
        if (_enemyStates[(int)_currState] != null)
            _enemyStates[(int)_currState]();
    }

    private void LateUpdate()
    {
        Flip();
        IsGrounded = GroundCheck();
        Anim.SetFloat("NormalizedSpeed", Mathf.Abs(MoveVector.x));
    }
    private void Patrol()
    {
        if (_canMove)
        {
            RaycastHit2D hitResult = Physics2D.BoxCast(transform.position + (Vector3)(MoveVector * _wallCheckSize.x), _wallCheckSize, 0, transform.forward, _wallCheckSize.x, LayerMask.GetMask("Ground"));
            if (hitResult || !IsGrounded)
            {
                StopMovement();
                MoveVector += (MoveVector.x * Vector2.right * -1);
                Invoke("AllowMovement", _patrolDelay);
            }
        }
    }
    
    private void Follow()
    {
        MoveVector = CurrTarget.transform.position - transform.position;
        MoveVector.Normalize();
    }
    
    public void AllowMovement() { _canMove = true; }
    public void StopMovement() { _canMove = false; }
    private bool GroundCheck()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
        return (Physics2D.OverlapBox(boxCenter, _groundCheckSize, 0f, _groundMask) != null);
    }
    private void Flip()
    {
        if (MoveVector.x != 0)
            transform.localScale = (Vector3.right * Mathf.Sign(MoveVector.x) * Mathf.Abs(transform.localScale.x)) +  (Vector3.up * transform.localScale.y) + (Vector3.forward * transform.localScale.z);
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
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, _groundCheckSize);
        
        Gizmos.color = Color.blue;
        var pos = transform.position + (Vector3.right * _rayOffset);
        Gizmos.DrawLine(pos, pos + transform.right * _rayDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)(MoveVector * _wallCheckSize.x), _wallCheckSize);
    }

    public void TakeDamage(int damage, Vector2 knockBackDir, float knockBackForce)
    {
        CurrHP -= damage;
        if (CurrHP <= 0)
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
    }

    private void Die()
    {
        if (_App.SoundManager)
            _App.SoundManager.Play(_sfx.Get("Death").audio);
        RB.constraints = RigidbodyConstraints2D.FreezeAll;
        _coll.enabled = false;
        _triggerColl.enabled = false;

        if (OnDeath != null)
            OnDeath(_scorePoints);
        
        Anim.SetTrigger("Death");
        Destroy(gameObject, 1.5f);
    }
}