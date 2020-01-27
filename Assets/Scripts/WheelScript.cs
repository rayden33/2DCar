using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem SlipParticleSystem;
    [Range(0,1)]
    [SerializeField] private float SlipSensitive;

    private bool _onGround = false;
    private CarController _carController;


    public void Slipping(float _speed, float _motorSpeed, float _maxSpeed, bool _isDrag)
    {
        if(Mathf.Abs(_speed - Mathf.Abs(_motorSpeed)) > (1 - SlipSensitive) * _maxSpeed && !_isDrag)
        {
            if (_onGround && Input.GetButton("Fire1"))
            {
                if (!SlipParticleSystem.isPlaying) SlipParticleSystem.Play();
            }
            else if (SlipParticleSystem.isPlaying) SlipParticleSystem.Stop();
        }
        else
        {
            if (SlipParticleSystem.isPlaying) SlipParticleSystem.Stop();
        }
    }

    void Start()
    {
        _carController = gameObject.GetComponentInParent<CarController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _onGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _onGround = false;
        }
    }
}
