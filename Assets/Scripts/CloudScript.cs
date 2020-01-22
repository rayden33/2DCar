using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    public float Speed
    {
        set { _speed = value; }
    }
    public float HideOffset
    {
        set { _hideOffset = value; }
    }

    private float _speed;
    private float _hideOffset;
    private CloudGenerator cloudGenerator;
    void Start()
    {
        cloudGenerator = gameObject.GetComponentInParent<CloudGenerator>();
    }

    void FixedUpdate()
    {
        if (transform.position.x >= Camera.main.ScreenToWorldPoint(new Vector3(0,0,0)).x - _hideOffset)
        {
            transform.Translate(new Vector3(-1,0,0) * _speed * Time.deltaTime);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
