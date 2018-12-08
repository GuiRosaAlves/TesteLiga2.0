using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun
{
    [SerializeField] private AudioClip _gunshotFX;
    [SerializeField] private Projectile _bulletPrefab;
    [SerializeField] private float _fireRate;
    private float _timer;

    public void Fire(Vector3 firePoint, Quaternion bulletRot)
    {
        if (Time.time > _timer)
        {
            if (_bulletPrefab != null)
            {
                if (_App.SoundManager)
                    _App.SoundManager.Play(_gunshotFX);
                if (CameraController.instance)
                    CameraController.instance.ScreenShake();
                
                GameObject.Instantiate(_bulletPrefab, firePoint, bulletRot);
                _timer = Time.time + 1/_fireRate;
            }
            else
            {
                Debug.Log("There are null references in the editor!");
            }
        }
    }
}