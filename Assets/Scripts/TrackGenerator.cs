using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] private Transform _carTransform;
    [Header("Tracks")]
    [SerializeField] private Transform[] _trackPoolList;
    [SerializeField] private Transform _finishTrack;
    public Transform FinishTrack
    {
        get { return _finishTrack; }
    }
    [SerializeField] private int _trackTranspositionCount = 1;

    private bool _leftTrack = true;
    private bool _rightTrack = false;
    private bool _isEndOfWay = false;
    private int _currentTrackIndex = 0;
    private int _moveDirection = 0;
    private float _trackLenght = 50f;
    public float TrackLenght
    {
        get { return _trackLenght; }
    }
    private Transform _newTrack;
    private CarController _carController;

    private Transform GetTrackFromPool()
    {
        foreach (var item in _trackPoolList)
        {
            if (!item.gameObject.activeInHierarchy)
                return item;
        }

        return null;
    }
    private void PoolTrack(float _newTrackIndex)
    {
        _newTrack = GetTrackFromPool();
        _newTrack.position = new Vector2(_newTrackIndex * _trackLenght, 0);
        if(_newTrack.position.x >= _finishTrack.position.x || _newTrack.position.x <= 0)
            return;
        _newTrack.gameObject.SetActive(true);
    }
    private void PoolManager()
    {
        if (_carTransform.position.x / _trackLenght - _currentTrackIndex > 0.5f)
        {
            if (_moveDirection > 0)
            {
                _rightTrack = true;
                _leftTrack = false;
            }
            if (!_rightTrack)
            {
                PoolTrack(_currentTrackIndex + 1);
                _rightTrack = true;
                _leftTrack = false;
            }
        }
        else
        {
            if (_moveDirection < 0)
            {
                _rightTrack = false;
                _leftTrack = true;
            }
            if (!_leftTrack)
            {
                PoolTrack(_currentTrackIndex - 1);
                _leftTrack = true;
                _rightTrack = false;
            }
        }
    }
    void Start()
    {
        _carController = _carTransform.gameObject.GetComponent<CarController>();
        _finishTrack.position = new Vector2((_trackTranspositionCount + 1) * _trackLenght, 0);
        _finishTrack.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (_isEndOfWay)
            return;

        if (_carTransform.position.x > _finishTrack.position.x + _trackLenght * 0.8f)
        {
            _isEndOfWay = true;
            _carController.FinishScene();
        }
        else if (_carTransform.position.x > _finishTrack.position.x)
        {
            _carController.TakeControl();
        }
        _moveDirection = _currentTrackIndex;
        _currentTrackIndex = Mathf.FloorToInt(_carTransform.position.x / _trackLenght);
        _moveDirection -= _currentTrackIndex;
        PoolManager();
    }
}
