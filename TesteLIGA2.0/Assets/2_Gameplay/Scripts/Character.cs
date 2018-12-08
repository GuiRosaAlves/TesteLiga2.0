using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Transform _firePoint;
	public Rigidbody2D RB { get; private set; }
	public Animator Anim { get; private set; }


	[Header("Attributes")]
	[SerializeField] private SFXDictionary _sfx;
	[SerializeField] private int _maxHP = 3;
	[SerializeField] private int _maxBombUses = 3;
	[SerializeField] private int _invencibilityTime;
	
	//Properties
	public int Score { get; private set; }
	private int _currHP;
	public int CurrHP
	{
		get { return _currHP; }
		private set { _currHP = Mathf.Clamp(value, 0, _maxHP); } 
	}
	public int CurrBombUses { get; private set; }
	public float CurrSpeed { get; private set; }
	
	[Header("Movement")]
	[SerializeField] private float _moveSpeed = 2.5f;
	[SerializeField] private float _jumpForce = 260f;
	[SerializeField] private float _midJumpFallSpeed = 5f;
	[SerializeField] private float _fallSpeed = 3f;
	
	[Header("Ground Check")]
	[SerializeField] private Vector2 _characterSize;
	[SerializeField] private Vector2 _groundCheckSize;
	[SerializeField] private LayerMask _groundMask;
	
	[Header("Weapons")]
	[SerializeField] private Gun _equippedGun;
	[SerializeField] private Bomb _bombPrefab;
	
	[Header("Triggers")]
	[SerializeField] private bool _canMove = true;
	[SerializeField] private bool _canJump = true;
	[SerializeField] private bool _canPlaceBomb = true;
	[SerializeField] private bool _canShoot = true;
	
	private bool _jumpRequest = false;
	private bool _placeBombRequest = false;
	private bool _shootRequest = false;
	
	public bool IsGrounded { get; private set; }
	public bool IsInvunerable { get; private set; }
	
	public event Action OnTakeDamage;
	public event Action OnPlayerDeath;
	public event Action OnUseBomb;
	public event Action OnPickupBombs;
	public event Action OnScorePoint;
	
	public float MoveDir { get; private set; }
	
	private void Awake()
	{
		EndInvencibility();
		RB = GetComponent<Rigidbody2D>();
		Anim = GetComponent<Animator>();
		CurrHP = _maxHP;
		CurrBombUses = _maxBombUses;
		CurrSpeed = _moveSpeed;
	}
	
	void FixedUpdate ()
	{
		if (_canMove)
		{
			UpdateMovement();
			Flip();
		}
		if (RB)
		{
			if (RB.velocity.y < 0)
				RB.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime * (_fallSpeed - 1);
			else if (!Input.GetButton("Jump") && RB.velocity.y > 0)
				RB.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime * (_midJumpFallSpeed - 1);
		}
	}
	
	void Update ()
	{
		IsGrounded = GroundCheck();
		MoveDir = Input.GetAxis("Horizontal");

		if (_jumpRequest)
			Jump();
		if (_placeBombRequest)
			PlantBomb();
		if (_shootRequest)
			FireWeapon();
		
		if (IsGrounded && _canJump && Input.GetButtonDown("Jump"))
			_jumpRequest = true;
		
		if (Input.GetButtonDown("Fire1"))
		{
			if (IsGrounded && _canShoot)
				_shootRequest = true;
		}
		else if(Input.GetButtonDown("PlaceBomb"))
		{
			if (IsGrounded && _canPlaceBomb)
				_placeBombRequest = true;
		}
	}

	private void LateUpdate()
	{
		Anim.SetFloat("NormalizedRunSpeed", Mathf.Abs(MoveDir));
		Anim.SetBool("Jumping", _jumpRequest);
		Anim.SetBool("PlantingBomb", (CurrBombUses > 0) && _placeBombRequest);
		Anim.SetBool("Firing", _shootRequest);
		Anim.SetBool("IsGrounded", IsGrounded);
	}

	private void UpdateMovement()
	{
		transform.position += Vector3.right * MoveDir * Time.deltaTime * _moveSpeed;
	}
	
	private void Jump()
	{
		if (_App.SoundManager)
			_App.SoundManager.Play(_sfx.Get("Jump").audio);
		RB.velocity = Vector2.zero;
		RB.AddForce(Vector2.up * _jumpForce);
		_jumpRequest = false;
		IsGrounded = false;
	}
	
	private void PlantBomb()
	{	
		if (_bombPrefab && Anim)
		{
			if (CurrBombUses > 0)
			{
				float fowardDir = (Mathf.Sign(transform.localScale.x));
				Instantiate(_bombPrefab, (transform.position + (Vector3.right * 0.1f * fowardDir)), Quaternion.identity);
				CurrBombUses--;
				if (OnUseBomb != null)
					OnUseBomb();
				if (_App.SoundManager)
					_App.SoundManager.Play(_sfx.Get("PlantBomb").audio);
			}
		}
		else
		{
			Debug.Log("There are null references in the editor!");
		}
		_placeBombRequest = false;
	}

	private void FireWeapon()
	{
		float rotAmount = (Mathf.Sign(transform.localScale.x) == 1) ? 0 : 180; //if the player's facing left it will rotate the projectile
		Quaternion bulletRot = Quaternion.Euler(Vector3.up * rotAmount);
		_equippedGun.Fire(_firePoint.position, bulletRot);
		_shootRequest = false;
	}
	
	public void TakeDamage(int damage)
	{
		CurrHP -= damage;
		if (OnTakeDamage != null)
			OnTakeDamage();
		if (CurrHP <= 0)
			Die();

		if (_App.SoundManager)
			_App.SoundManager.Play(_sfx.Get("Hurt").audio);

		StartInvencibility();
		Invoke("EndInvencibility", _invencibilityTime);

		if (Anim)
		{
			Anim.SetTrigger("TakeDamage");
		}
		else
		{
			Debug.Log("There are null references in the editor!");
		}
	}
	
	public void TakeDamage(int damage, Vector2 knockBackDir, float knockBackForce)
	{
		CurrHP -= damage;
		if (OnTakeDamage != null)
			OnTakeDamage();
		if (CurrHP <= 0)
			Die();

		if (_App.SoundManager)
			_App.SoundManager.Play(_sfx.Get("Hurt").audio);
		if (CameraController.instance)
			CameraController.instance.ScreenShake();

		StartInvencibility();
		Invoke("EndInvencibility", _invencibilityTime);
		RB.velocity = Vector2.zero;
		RB.AddForce(knockBackDir * knockBackForce, ForceMode2D.Impulse);

		if (Anim)
		{
			Anim.SetTrigger("TakeDamage");
		}
		else
		{
			Debug.Log("There are null references in the editor!");
		}
		if (_App.Instance)
			_App.Instance.Snooze();
	}
	
	protected void StartInvencibility()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
		IsInvunerable = true;
	}

	protected void EndInvencibility()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
		IsInvunerable = false;
	}
	
	protected void Die()
	{
		if (_App.SoundManager)
			_App.SoundManager.Play(_sfx.Get("GameOver").audio);
		if (OnPlayerDeath != null)
			OnPlayerDeath();
		Destroy(gameObject);
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
	
	public void ScorePoint(Enemy2 enemy)
	{
		Score += enemy.ScorePoints;
		if (OnScorePoint != null)
			OnScorePoint();
	}
	
	private void Flip()
	{
		if (MoveDir != 0)
			transform.localScale = (Vector3.right * Mathf.Sign(MoveDir) * Mathf.Abs(transform.localScale.x)) +  (Vector3.up * transform.localScale.y) + (Vector3.forward * transform.localScale.z);
	}
	private bool GroundCheck()
	{
		Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
		return (Physics2D.OverlapBox(boxCenter, _groundCheckSize, 0f, _groundMask) != null);
	}
	
	protected void OnDrawGizmos()
	{
		Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_characterSize.y + _groundCheckSize.y) * 0.5f;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(boxCenter, _groundCheckSize);
	}

	public void CanMoveState(int state) { _canMove = (state > 0) ? true : false; }
	public void CanPlaceBombState(int state) { _canPlaceBomb = (state > 0) ? true : false; }
	public void CanJumpState(int state) { _canJump = (state > 0) ? true : false; }
	public void CanShootState(int state) { _canShoot = (state > 0) ? true : false; }
}