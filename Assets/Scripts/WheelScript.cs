using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public bool onGround = false;

    private CarController _carController;

    void Start()
    {
        _carController = gameObject.GetComponentInParent<CarController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = true;
            _carController.ChangeOnGroundState();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = false;
            _carController.ChangeOnGroundState();
        }
    }
}
