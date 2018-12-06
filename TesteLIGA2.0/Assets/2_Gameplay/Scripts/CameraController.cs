using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }
    
    [SerializeField] private Transform _cameraBounds;
//    [SerializeField] private Vector4 _offsetBounds;
//    [SerializeField] private float _screenShakeDuration;
    
    private Vector2 _size;
    private Vector2 _offset;

    private float _left;
    private float _right;
    private float _top;
    private float _bottom;

    [SerializeField] private Transform target;
    [SerializeField] private float _smoothTimeY;
    [SerializeField] private float _smoothTimeX;
    private Vector2 velocity;
//    private float _shakeAmt;
//    private bool _screenIsShaking = false;

    protected void Awake()
    {
        instance = this;
    }
    
    protected void Start()
    {
        Sprite sprite = _cameraBounds.transform.GetComponent<SpriteRenderer>().sprite;

        float pixelPerUnits = sprite.rect.width / sprite.bounds.size.x;

        CalculateSize(sprite, pixelPerUnits);
        RefreshBounds();
    }

    protected void FixedUpdate()
    {
        if (target != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, _smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity.y, _smoothTimeY);
            
            transform.position = (Vector3.right * posX) + (Vector3.up * posY) + (Vector3.forward * transform.position.z);
        }
    }

//    private void Update()
//    {
//        if(_shakeAmt>0) 
//        {
//            float quakeAmt = Random.value*_shakeAmt*2 - _shakeAmt;
//            Vector3 pp = transform.position;
//            pp.x += quakeAmt;
//            pp.y += quakeAmt;
//            transform.position = pp;
//            _screenIsShaking = true;
//            _shakeAmt -= Mathf.Lerp(_shakeAmt, 0, _screenShakeDuration);
//        }
//        else
//        {
//            _screenIsShaking = false;
//        }
//    }

    protected void LateUpdate()
    {
        Vector3 pos = transform.position;
        
//        if (_screenIsShaking)
//        {
            pos.x = Mathf.Clamp(pos.x, _left, _right);
            pos.y = Mathf.Clamp(pos.y, _bottom, _top);
            transform.position = pos;
//        }
//        else
//        {
//            if (Math.Abs(pos.x - (_left + _offsetBounds.x)) < 0.3f)
//            {
//                pos.x = Mathf.Lerp(pos.x, _left+_offsetBounds.x, _smoothTimeX);
//                Debug.Log("Left Lerping"); 
//            }
//            else if (pos.x > _right + _offsetBounds.y)
//            {
//                pos.x = Mathf.Lerp(pos.x, _right+_offsetBounds.y, _smoothTimeX);
//                Debug.Log("Right Lerping");
//            }
//
//            if (Math.Abs(pos.y - (_bottom + _offsetBounds.z)) < 0.3f)
//            {
//                pos.y = Mathf.Lerp(pos.y, _bottom+_offsetBounds.z, _smoothTimeY);
//                Debug.Log("Bottom Lerping");
//            }
//            else if (pos.y > _top + _offsetBounds.w)
//            {
//                pos.y = Mathf.Lerp(pos.y, _top+_offsetBounds.w, _smoothTimeY);
//                Debug.Log("Top Lerping");
//            }
//            
//            transform.position = pos;
//        }
    }
    
//    public void ScreenShake(float amount)
//    {
//        _shakeAmt = amount;
//    }
//    public void StopScreenShake()
//    {
//        _shakeAmt = 0;
//    }

    protected void CalculateSize(Sprite sprite, float pixelPerUnits)
    {
        _size = new Vector2(_cameraBounds.transform.parent.localScale.x * sprite.texture.width / pixelPerUnits,
                            _cameraBounds.transform.parent.localScale.y * sprite.texture.height / pixelPerUnits);

        _offset = _cameraBounds.transform.position;
    }

    protected void RefreshBounds()
    {
        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        _left = horzExtent - _size.x / 2.0f + _offset.x;
        _right = _size.x / 2.0f - horzExtent + _offset.x;
        _bottom = vertExtent - _size.y / 2.0f + _offset.y;
        _top = _size.y / 2.0f - vertExtent + _offset.y;
    }
}