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

    protected void OnTriggerEnter2D(Collider2D coll)
    {
        Enemy enemy = coll.GetComponent<Enemy>();
        if (enemy && coll.isTrigger == false)
        {
            enemy.TakeDamage(_damage, transform.right, enemy.KnockBackForce);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (coll.gameObject.layer == LayerMask.GetMask("Ground"));
        {
            if (_App.SoundManager)
                _App.SoundManager.Play(_sfx.Get("Impact").audio);
        }
    }
}