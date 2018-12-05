using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;

    [Header("Attributes")]
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _maxBombUses;
    [SerializeField] private int _invencibilityTime;
    private int _score;
    public int Score { get { return _score; } set { _score = (value < 0) ? 0 : value; } }
    private int _currHealth;
    public int CurrHealth { get { return _currHealth; } set { _currHealth = Mathf.Clamp(value, 0, _maxHealth); } }
    private int _currBombUses;
    public int CurrBombUses { get { return _currBombUses; } set { _currBombUses = value; _currBombUses = Mathf.Clamp(_currBombUses, 0, _maxBombUses); } }

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _midJumpFallSpeed;
    [SerializeField] private float _fallSpeed;
    [SerializeField] private Vector2 _characterSize;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _groundMask;

    [Header("Weapons")]
    [SerializeField] private Gun _equippedGun;
    [SerializeField] private Bomb _bombPrefab;

    [Header("Debug")]
    [SerializeField] private bool _drawGroundCheck;

    private Vector3 _moveVector;
    private bool _moveRequest = true;
    private bool _jumpRequest = false;
    private bool _isGrounded = false;
    private bool _isFiring = false;
    private bool _isPlantingBomb = false;

    public event Action OnTakeDamage;
    public event Action OnPlayerDeath;
    public event Action OnUseBomb;
    public event Action OnPickupBombs;

    protected void Awake()
    {
        CurrHealth = _maxHealth;
        CurrBombUses = _maxBombUses;
        EndInvencibility();
    }

    protected void FixedUpdate()
    {
        if (_moveRequest)
        {
            if (_rigidBody)
            {
                if (_jumpRequest)
                {
                    Jump();
                }
                if (_rigidBody.velocity.y < 0)
                {
                    _rigidBody.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime * (_fallSpeed - 1);
                }
                else if (!Input.GetButton("Jump") && _rigidBody.velocity.y > 0)
                {
                    _rigidBody.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime * (_midJumpFallSpeed - 1);
                }
            }
            else
            {
                Debug.Log("RigidBody reference not set in inspector!");
            }

            if (!_jumpRequest)
            {
                GroundCheck();
            }

            _moveVector = Vector3.right * Input.GetAxis("Horizontal") * _moveSpeed;
            Move(_moveVector);
        }
    }

    protected void Update()
    {
        InputCheck();
        Flip();

        //Animator Methods
        if (_animator)
        {
            _animator.SetFloat("NormalizedRunSpeed", Mathf.Abs(_moveVector.x) / _moveSpeed);
            _animator.SetBool("IsGrounded", _isGrounded);
            _animator.SetBool("IsFiring", _isFiring);
        }
        else
        {
            Debug.Log("Animator reference not set in inspector!");
        }
    }

    protected void OnDrawGizmos()
    {
        if (_drawGroundCheck)
        {
            Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCenter, _groundCheckSize);
        }
    }

    protected void Move(Vector3 moveVector)
    {
        transform.Translate(moveVector * Time.deltaTime);
    }

    protected void AllowMovement()
    {
        _moveRequest = true;
    }

    protected void StopMovement()
    {
        _moveRequest = false;
    }

    protected void PlantBomb()
    {
        if (_bombPrefab && _animator)
        {
            if (CurrBombUses > 0)
            {
                float fowardDir = (Mathf.Sign(transform.localScale.x));
                _isPlantingBomb = true;
                Instantiate(_bombPrefab, (transform.position + (Vector3.right * 0.1f * fowardDir)), Quaternion.identity);
                CurrBombUses--;
                if (OnUseBomb != null)
                    OnUseBomb();
                _animator.SetTrigger("IsPlantingBomb");
                if (AudioManager.instance)
                    AudioManager.instance.Play(_sfx.Get("PlantBomb").audio);
            }
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    protected void Jump()
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.AddForce(Vector2.up * _jumpForce);

        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("Jump").audio);

        _jumpRequest = false;
        _isGrounded = false;
    }

    public void TakeDamage(int damage)
    {
        CurrHealth -= damage;
        if (OnTakeDamage != null)
            OnTakeDamage();
        if (CurrHealth <= 0)
            Die();

        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("Hurt").audio);

        StopMovement();
        StartInvencibility();
        Invoke("EndInvencibility", _invencibilityTime);

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
        if (OnTakeDamage != null)
            OnTakeDamage();
        if (CurrHealth <= 0)
            Die();

        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("Hurt").audio);

        StopMovement();
        StartInvencibility();
        Invoke("EndInvencibility", _invencibilityTime);
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.AddForce(knockBackDir * knockBackForce, ForceMode2D.Impulse);

        if (_animator)
        {
            _animator.SetTrigger("IsTakingDamage");
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    protected void StartInvencibility()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    }

    protected void EndInvencibility()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    protected void GroundCheck()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
        _isGrounded = (Physics2D.OverlapBox(boxCenter, _groundCheckSize, 0f, _groundMask) != null);
    }

    protected void Die()
    {
        if (AudioManager.instance)
            AudioManager.instance.Play(_sfx.Get("GameOver").audio);
        if (OnPlayerDeath != null)
            OnPlayerDeath();
        Destroy(gameObject);
    }

    protected void InputCheck()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _jumpRequest = true;
        }

        if (Input.GetButton("Fire1") && _isGrounded && !_isPlantingBomb)
        {
            _isFiring = true;
            float rotAmount = (Mathf.Sign(transform.localScale.x) == 1) ? 0 : 180; //if the player's facing left it will rotate the projectile
            Quaternion bulletRot = Quaternion.Euler(Vector3.up * rotAmount);
            _equippedGun.Fire(_firePoint.position, bulletRot);
        }
        else
        {
            _isFiring = false;
        }

        if (Input.GetButtonDown("Fire2") && _isGrounded && !_isPlantingBomb && !_isFiring)
        {
            PlantBomb();
        }
        else if (_moveRequest && _isPlantingBomb)
        {
            _isPlantingBomb = false;
        }
    }

    protected void Flip()
    {
        if (_moveVector.x != 0)
            transform.localScale = (Vector3.right * Mathf.Sign(_moveVector.x) * Mathf.Abs(transform.localScale.x)) +  (Vector3.up * transform.localScale.y) + (Vector3.forward * transform.localScale.z);
    }

    public bool PickupBomb(int quantity)
    {
        if ((CurrBombUses+quantity) <= _maxBombUses)
        {
            CurrBombUses += quantity;
            if (OnPickupBombs != null)
                OnPickupBombs();
            return true;
        }
        else
        {
            return false;
        }
    }
}