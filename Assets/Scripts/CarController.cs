using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public CarState _state;
    public bool _onGround = false;
}
public enum CarState
{
    drag_drop,
    gas,
    neutral,
    braking
}
public class CarController : MonoBehaviour
{

    
    public GameObject[] gameObjectWheels;
    public ParticleSystem particleSlip;
    public float maxSpeed = 1000f;
    public float angleMinSensitive = 1f;

    private float _acceleration = 100f;
    private float _deacceleration = 100f;
    private float _gravity = 9.8f;
    private float _angleCar = 0;
    private float _speed = 0;
    private float _mousePosPercentageX;
    private WheelJoint2D[] _wheelJoints;
    private JointMotor2D _frontWheel;
    private JointMotor2D _midWheel;
    private JointMotor2D _backWheel;
    private Rigidbody2D _rgbCar;
    private TargetJoint2D _tjDragDrop;
    private WheelScript[] _wheelScript;
    private Car _myCar;

    public void ChangeOnGroundState()
    {
        foreach (var item in _wheelScript)
        {
            if(item.onGround)
            {
                _myCar._onGround = true;
                return;
            }
        }
        _myCar._onGround = false;
    }

    void DragDrop()
    {
        _tjDragDrop.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void Moving()
    {
        if (_speed < 0.2f * maxSpeed)
            _myCar._state = CarState.neutral;
        else
            _myCar._state = CarState.gas;

        if (_myCar._state == CarState.gas && _mousePosPercentageX * _frontWheel.motorSpeed > 0)
            _myCar._state = CarState.braking;
        else
            _myCar._state = CarState.gas;

        _frontWheel.motorSpeed = Mathf.Clamp(_frontWheel.motorSpeed - (_acceleration - _gravity * (_angleCar / 180) * 500), -maxSpeed * _mousePosPercentageX, -maxSpeed * _mousePosPercentageX) ;

    }
    void Braking()
    {
        _frontWheel.motorSpeed = 0;
    }

    void ParticleSlip()
    {
        // Slip particle effect
        if(Mathf.Abs(_speed - Mathf.Abs(_frontWheel.motorSpeed)) > 0.3f * maxSpeed)
        {
            if (_myCar._onGround)
            {
                PSlipChangeRotation();
                if (!particleSlip.isPlaying) particleSlip.Play();
            }
            else
            {
                if (particleSlip.isPlaying) particleSlip.Stop();
            }
        }
        else
        {
            if (particleSlip.isPlaying) particleSlip.Stop();
        }
    }

    void PSlipChangeRotation()
    {
        Transform goTmp;
        ParticleSystem psTmp;
        for (int i = 0; i < particleSlip.transform.childCount; i++)
        {
            goTmp = particleSlip.transform.Find(i.ToString());
            psTmp = goTmp.gameObject.GetComponent<ParticleSystem>();
            var psShape = psTmp.shape;

            if (_rgbCar.velocity.x < 0)
                psShape.rotation = new Vector2(180, -90);
            else
                psShape.rotation = new Vector2(0, -90);
        }
    }

    void CalibrationCarRotation()
    {
        if (Mathf.Abs(_angleCar) > 30f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,0), Mathf.Abs(_angleCar) * 2f * Time.deltaTime);
        }
    }
    
    void Start()
    {
        _myCar = new Car();
        _wheelJoints = gameObject.GetComponents<WheelJoint2D>();
        _rgbCar = gameObject.GetComponent<Rigidbody2D>();
        _tjDragDrop = gameObject.GetComponent<TargetJoint2D>();
        _wheelScript = gameObject.GetComponentsInChildren<WheelScript>();
        _frontWheel = _wheelJoints[0].motor;
        _midWheel = _wheelJoints[1].motor;
        _backWheel = _wheelJoints[2].motor;
        _myCar._state = CarState.neutral;
        _speed = _rgbCar.velocity.magnitude * 250;               /// (velocity.magnitude) 4.0 = 1000.0 (JointMotor2D.motorSpeed) 
    }

    void FixedUpdate()
    {
        
        _speed = _rgbCar.velocity.magnitude * 250;
        _angleCar = transform.localEulerAngles.z;

        if (_angleCar > 180)
            _angleCar -= 360;

        if (_angleCar < angleMinSensitive && _angleCar > -angleMinSensitive)
            _angleCar = 0;
        
        if (Input.GetButton("Fire1"))
        {
            _mousePosPercentageX = Input.mousePosition.x / Screen.width;
            _mousePosPercentageX -= 0.5f;
            _mousePosPercentageX *= 2;
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Camera.main.transform.forward);
            
            if (hitInfo)
            {
                if (hitInfo.transform.tag == "Player")
                {
                    _myCar._state = CarState.drag_drop;
                    _tjDragDrop.enabled = true;
                }
            }

            switch(_myCar._state)
            {
                case CarState.drag_drop: DragDrop();
                    break;
                case CarState.braking: Braking(); 
                    break;
                default: Moving();
                    break;
            }
        }
        else
        {
            _myCar._state = CarState.neutral;
            _tjDragDrop.enabled = false;
            if (_angleCar < 0 || (_angleCar == 0 && _frontWheel.motorSpeed < 0))
            {
                _frontWheel.motorSpeed = Mathf.Clamp(_frontWheel.motorSpeed - (_gravity * (-_angleCar / 180) * 500 - _deacceleration) * Time.deltaTime, -maxSpeed, 0);
            }
            else if (_angleCar > 0 || (_angleCar == 0 && _frontWheel.motorSpeed > 0))
            {
                _frontWheel.motorSpeed = Mathf.Clamp(_frontWheel.motorSpeed + (_gravity * (_angleCar / 180) * 500 - _deacceleration) * Time.deltaTime, 0, maxSpeed);
            }

        }
        if (_myCar._state != CarState.drag_drop) 
            ParticleSlip();
        else
            CalibrationCarRotation();
            
        
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
        _wheelJoints[0].motor = _frontWheel;
        _wheelJoints[1].motor = _frontWheel;
        _wheelJoints[2].motor = _frontWheel;
    }

}
