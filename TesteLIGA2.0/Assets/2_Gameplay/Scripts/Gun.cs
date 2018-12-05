using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun
{
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private Projectile _bulletPrefab;
    [SerializeField] private float _fireRate;
    private float timer;

    public void Fire(Vector3 firePoint, Quaternion bulletRot)
    {
        if (Time.time > timer)
        {
            if (_bulletPrefab != null)
            {
                GameObject.Instantiate(_bulletPrefab, firePoint, bulletRot);
                timer = Time.time + 1/_fireRate;
            }
            else
            {
                Debug.Log("There are null references in the editor!");
            }
        }
    }
}