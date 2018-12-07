using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    protected void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }

    protected void FixedUpdate()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
    }

    protected void OnCollisionEnter2D(Collision2D coll)
    {
        Enemy2 enemy = coll.transform.GetComponent<Enemy2>();
        if (enemy)
        {
            enemy.TakeDamage(_damage, transform.right, enemy.KnockBackForce);
        }

        if (coll.gameObject.layer == LayerMask.GetMask("Ground"))
        {
            if (_App.SoundManager)
                _App.SoundManager.Play(_sfx.Get("Impact").audio);
        }
        
        Destroy(gameObject);
    }
}