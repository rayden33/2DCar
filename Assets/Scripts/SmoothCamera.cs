using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public float dampTime = 0.15f;
	public Transform target;
	public float MaxOrthographicSize = 5f;


	private Vector3 velocity = Vector3.zero;
	private Rigidbody2D _rigidbodyTarget;
	private CarController _carController;
	private float _minOrthographicSize;
	private float _offsetOrthographicSize;
	private float _targetSpeed;
	private float _targetMaxSpeed;
	private float _scaleProportion;

	void Start()
	{
		_carController = target.GetComponent<CarController>();
		_rigidbodyTarget = target.gameObject.GetComponent<Rigidbody2D>();
		_targetMaxSpeed = _carController.MaxSpeed;
		_minOrthographicSize = Camera.main.orthographicSize;
		_offsetOrthographicSize = MaxOrthographicSize - _minOrthographicSize;
	}
	
	void FixedUpdate () 
	{
		if (target)
		{
			_targetSpeed = _rigidbodyTarget.velocity.magnitude * 250f;
			_scaleProportion = _targetSpeed / _targetMaxSpeed;
			Camera.main.orthographicSize = Mathf.Clamp(_minOrthographicSize + _offsetOrthographicSize * _scaleProportion, _minOrthographicSize, MaxOrthographicSize);

			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(new Vector3(target.position.x, target.position.y+0.75f,target.position.z));
			Vector3 delta = new Vector3(target.position.x, target.position.y+0.75f,target.position.z) - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta;


			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
		
	}
}
