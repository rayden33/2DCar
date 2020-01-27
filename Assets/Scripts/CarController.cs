using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car
{
    public CarState _state;
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
    public float MaxSpeed
    {
        get { return _maxSpeed; }
    }

    [SerializeField] private float _maxSpeed = 2000;
    [SerializeField] private float _finishCarSpeed = 1000f;
    [SerializeField] private float _carAngleMinValue = 1f;
    [SerializeField] private float _acceleration = 100f;
    [SerializeField] private float _deacceleration = 100f;
    [SerializeField] private float _gravity = 9.8f;
    [SerializeField] private GameObject _fireWorkSystem;
    [SerializeField] private GameObject _uiControllerGameObject;
    
    private bool _calibratingCarRotate = false;
    private bool _isFinish = false;
    private float _angleCar = 0;
    private float _speed = 0;
    private float _neutralSpeedProportion = 0.2f;
    private float _mousePosPercentageX;
    private float _carMass;
    private WheelJoint2D[] _wheelJoints;
    private JointMotor2D _wheelMotor;
    private Rigidbody2D _rigidbodyCar;
    private TargetJoint2D _targetJointDragDrop;
    private WheelScript[] _wheelScripts;
    private UIController _uiController;
    private Car _myCar;

    void DragDrop()
    {
        _targetJointDragDrop.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void Moving()
    {
        if (_speed <= _neutralSpeedProportion * MaxSpeed)
            _myCar._state = CarState.neutral;
        else
            _myCar._state = CarState.gas;

        if (_myCar._state == CarState.gas && _mousePosPercentageX * _wheelMotor.motorSpeed > 0)
            _myCar._state = CarState.braking;
        else
            _myCar._state = CarState.gas;

        _wheelMotor.motorSpeed = Mathf.Clamp(_wheelMotor.motorSpeed - (_acceleration - _gravity * _carMass * (_angleCar / 180)), -MaxSpeed * _mousePosPercentageX, -MaxSpeed * _mousePosPercentageX) ;

    }
    void Braking()
    {
        _wheelMotor.motorSpeed = 0;
    }

    void ParticleSlip()
    {
        // Slip particle effect
        foreach (var item in _wheelScripts)
        {
            item.Slipping(_speed, _wheelMotor.motorSpeed, MaxSpeed, _myCar._state == CarState.drag_drop);
        }
    }

    void CalibrationCarRotation()
    {
        //Debug.Log(Mathf.Pow(_angleCar / 4f, 2f) * Time.deltaTime);
        if (_myCar._state == CarState.drag_drop)
        {
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,0), Mathf.Pow(_angleCar / 4f, 2f) * Time.deltaTime);     /// function y = x^2
        }
        else
        {
            if (Mathf.Abs(_angleCar) > 45f)
                _calibratingCarRotate = true;
            if (Mathf.Abs(_angleCar) < 30f)
                _calibratingCarRotate = false;
            if (_calibratingCarRotate)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,0), Mathf.Pow(_angleCar / 2f, 2f) * Time.deltaTime);
            }
        }
        
    }

    public void TakeControl()
    {
        _isFinish = true;
        _wheelMotor.motorSpeed = -_finishCarSpeed;
        _targetJointDragDrop.enabled = false;
    }
    public void FinishScene()
    {
        _fireWorkSystem.SetActive(true);
        _wheelMotor.motorSpeed = 0;
        _uiController.EndWay();
    } 
    
    void Start()
    {
        _myCar = new Car();
        _wheelJoints = gameObject.GetComponents<WheelJoint2D>();
        _rigidbodyCar = gameObject.GetComponent<Rigidbody2D>();
        _targetJointDragDrop = gameObject.GetComponent<TargetJoint2D>();
        _wheelScripts = gameObject.GetComponentsInChildren<WheelScript>();
        _uiController = _uiControllerGameObject.GetComponent<UIController>();
        _wheelMotor = _wheelJoints[0].motor;
        _myCar._state = CarState.neutral;
        _speed = _rigidbodyCar.velocity.magnitude * 250f;               /// (velocity.magnitude) 4.0 * 250f = 1000.0 (JointMotor2D.motorSpeed) 
        _carMass = _rigidbodyCar.mass;
    }

    void FixedUpdate()
    {   

        if (_isFinish)
        {
            ParticleSlip();
            CalibrationCarRotation();

            _wheelJoints[0].motor = _wheelMotor;
            _wheelJoints[1].motor = _wheelMotor;
            _wheelJoints[2].motor = _wheelMotor;
            return;
        }
        
        _speed = _rigidbodyCar.velocity.magnitude * 250f;
        _angleCar = transform.localEulerAngles.z;

        if (_angleCar > 180)
            _angleCar -= 360;

        if (_angleCar < _carAngleMinValue && _angleCar > -_carAngleMinValue)
            _angleCar = 0;
        
        if (Input.GetButton("Fire1"))
        {
            _mousePosPercentageX = ((Input.mousePosition.x / Screen.width) - 0.5f) * 2f;
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
            
            if (hitInfo)
            {
                if (hitInfo.transform.tag == "Player")
                {
                    _myCar._state = CarState.drag_drop;
                    _targetJointDragDrop.enabled = true;
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
            _targetJointDragDrop.enabled = false;

            if (_angleCar < 0 || (_angleCar == 0 && _wheelMotor.motorSpeed < 0))
            {
                _wheelMotor.motorSpeed = Mathf.Clamp(_wheelMotor.motorSpeed - (_gravity * _carMass * (-_angleCar / 180) - _deacceleration) * Time.deltaTime, -MaxSpeed, 0);
            }
            else if (_angleCar > 0 || (_angleCar == 0 && _wheelMotor.motorSpeed > 0))
            {
                _wheelMotor.motorSpeed = Mathf.Clamp(_wheelMotor.motorSpeed + (_gravity * _carMass * (_angleCar / 180) - _deacceleration) * Time.deltaTime, 0, MaxSpeed);
            }

        }

        ParticleSlip();
        CalibrationCarRotation();
        
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
        _wheelJoints[0].motor = _wheelMotor;
        _wheelJoints[1].motor = _wheelMotor;
        _wheelJoints[2].motor = _wheelMotor;
    }

}
