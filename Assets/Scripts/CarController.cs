using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Car
{
    public CarStatuses status;
}
public enum CarStatuses
{
    gas,
    neutral,
    braking
}
public class CarController : MonoBehaviour
{
    WheelJoint2D[] wheelJoints;
    JointMotor2D frontWheel;
    JointMotor2D midWheel;
    JointMotor2D backWheel;

    public ParticleSystem psSlip;
    public float maxSpeed = 1000f;
    public float angleMinSensitive = 1f;

    private float acceleration = 100f;
    private float deacceleration = 100f;
    private float gravity = 9.8f;
    private float angleCar = 0;
    private float speed = 0;
    private float mousePosPercentageX;
    private Car myCar = new Car();
    private Rigidbody2D rgbCar;

    void Braking()
    {
        frontWheel.motorSpeed = 0;
    }

    void ParticleSlip()
    {
        // Slip particle effect
        if(Mathf.Abs(speed - Mathf.Abs(frontWheel.motorSpeed)) > 0.3f * maxSpeed)
        {
            PsChangeRotation();

            if (!psSlip.isPlaying)
                psSlip.Play();
        }
        else
        {
            if (psSlip.isPlaying)
                psSlip.Stop();
        }
    }

    void PsChangeRotation()
    {
        Transform goTmp;
        ParticleSystem psTmp;
        for (int i = 0; i < psSlip.transform.childCount; i++)
        {
            goTmp = psSlip.transform.Find(i.ToString());
            psTmp = goTmp.gameObject.GetComponent<ParticleSystem>();
            var psShape = psTmp.shape;

            if (rgbCar.velocity.x < 0)
                psShape.rotation = new Vector3(180, -90, 0);
                //goTmp.rotation = Quaternion.Euler(180,-90,0);
            else
                psShape.rotation = new Vector3(0, -90, 0);
                //goTmp.rotation = Quaternion.Euler(0,-90,0);

        }
    }
    
    void Start()
    {
        wheelJoints = gameObject.GetComponents<WheelJoint2D>();
        frontWheel = wheelJoints[0].motor;
        midWheel = wheelJoints[1].motor;
        backWheel = wheelJoints[2].motor;
        myCar.status = CarStatuses.neutral;
        rgbCar = gameObject.GetComponent<Rigidbody2D>();
        speed = rgbCar.velocity.magnitude * 250;               /// (velocity.magnitude) 4.0 <> 1000.0 (JointMotor2D.motorSpeed) 
    }

    void FixedUpdate()
    {
        
        speed = rgbCar.velocity.magnitude * 250;
        angleCar = transform.localEulerAngles.z;
        if (angleCar > 180)
            angleCar -= 360;

        if (angleCar < angleMinSensitive && angleCar > -angleMinSensitive)
            angleCar = 0;
        
        if (Input.GetButton("Fire1"))
        {
            if (myCar.status == CarStatuses.braking)
                Braking();
            else
            {
                mousePosPercentageX = Input.mousePosition.x / Screen.width;
                mousePosPercentageX -= 0.5f;
                mousePosPercentageX *= 2;

                if (speed < 0.2f * maxSpeed)
                    myCar.status = CarStatuses.neutral;
                else
                    myCar.status = CarStatuses.gas;

                if (myCar.status == CarStatuses.gas && mousePosPercentageX * frontWheel.motorSpeed > 0)
                    myCar.status = CarStatuses.braking;
                else
                    myCar.status = CarStatuses.gas;

                if (myCar.status == CarStatuses.braking)
                    Braking();
                else
                    frontWheel.motorSpeed = Mathf.Clamp(frontWheel.motorSpeed - (acceleration - gravity * (angleCar / 180) * 500), -maxSpeed * mousePosPercentageX, -maxSpeed * mousePosPercentageX) ;
            }    

            ParticleSlip();
        }
        else
        {
            myCar.status = CarStatuses.neutral;
            if (angleCar < 0 || (angleCar == 0 && frontWheel.motorSpeed < 0))
            {
                frontWheel.motorSpeed = Mathf.Clamp(frontWheel.motorSpeed - (gravity * (-angleCar / 180) * 500 - deacceleration) * Time.deltaTime, -maxSpeed, 0);
            }
            else if (angleCar > 0 || (angleCar == 0 && frontWheel.motorSpeed > 0))
            {
                frontWheel.motorSpeed = Mathf.Clamp(frontWheel.motorSpeed + (gravity * (angleCar / 180) * 500 - deacceleration) * Time.deltaTime, 0, maxSpeed);
            }

        }
        
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
        wheelJoints[0].motor = frontWheel;
        wheelJoints[1].motor = frontWheel;
        wheelJoints[2].motor = frontWheel;
    }
}
