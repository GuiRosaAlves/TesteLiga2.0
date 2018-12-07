using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidBody;
    [Header("Attributes")]
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private int _damage;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionDelay;
    [SerializeField] private LayerMask[] _hitMask;
    [Header("Debug")]
    [SerializeField] private bool _drawRadius;

    private void Explode()
    {
        if (_App.SoundManager)
            _App.SoundManager.Play(_sfx.Get("explode").audio);
        
        List<GameObject> objectsInRange = new List<GameObject>();
        RaycastHit2D[] hitResult;

        for (int i = 0; i < _hitMask.Length; i++)
        {
            hitResult = Physics2D.CircleCastAll(transform.position, _explosionRadius, Vector2.up, _explosionRadius, _hitMask[i].value);
            for (int n = 0; n < hitResult.Length; n++)
            {
                if (!objectsInRange.Contains(hitResult[n].collider.gameObject))
                {
                    objectsInRange.Add(hitResult[n].collider.gameObject);
                }
            }
        }

        //TODO: Modify This Garbage code
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            Vector2 knockBackDir = objectsInRange[i].transform.position - transform.position;

            Character player = objectsInRange[i].GetComponent<Character>();
            Enemy2 enemy = objectsInRange[i].GetComponent<Enemy2>();

            if (objectsInRange[i].GetComponent<Character>())
            {
                player.TakeDamage(_damage, knockBackDir, _explosionForce);
            }
            else if (objectsInRange[i].GetComponent<Enemy2>())
            {
                enemy.TakeDamage(_damage, knockBackDir, _explosionForce);
            }
        }

        StartCoroutine("DestroyObject");
    }

    protected void Awake ()
    {
        Invoke("StartAnimation", _explosionDelay);
    }

    protected void StartAnimation()
    {
        if (_animator)
        {
            _animator.SetTrigger("explode");
        }
        else
        {
            Debug.Log("Animator reference not set in inspector!");
        }
    }

    protected void OnDrawGizmos()
    {
        if (_drawRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsTag("Explode"));
        Destroy(gameObject);
    }
}