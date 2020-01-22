using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] private Transform CarTransform;
    [Header("Tracks")]
    [SerializeField] private Transform[] TrackPoolList;
    [SerializeField] private Transform FinishTrack;
    [SerializeField] private int TrackTranspositionCount = 1;

    private bool _leftTrack = true;
    private bool _rightTrack = false;
    private int _currentTrackIndex = 0;
    private int _moveDirection = 0;
    private float _trackLenght = 50f;
    private float _leftBorderScreenXToWorldX;
    private float _rightBorderScreenXToWorldX;
    private float _screenWidthToWorldDistance;
    private Transform _newTrack;

    public float TrackLenght
    {
        get { return _trackLenght; }
    }
    private Transform GetTrackFromPool()
    {
        foreach (var item in TrackPoolList)
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
        if(_newTrack.position.x >= FinishTrack.position.x || _newTrack.position.x <= 0)
            return;
        _newTrack.gameObject.SetActive(true);
    }
    private void PoolManager()
    {
        // Debug.Log(CarTransform.position.x / _trackLenght - _currentTrackIndex);
        // Debug.Log(_moveDirection);
        if (CarTransform.position.x / _trackLenght - _currentTrackIndex > 0.5f)
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
        // _leftBorderScreenXToWorldX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        // _rightBorderScreenXToWorldX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        // _screenWidthToWorldDistance = _rightBorderScreenXToWorldX - _leftBorderScreenXToWorldX;

        FinishTrack.position = new Vector2((TrackTranspositionCount + 1) * _trackLenght, 0);
        FinishTrack.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        _moveDirection = _currentTrackIndex;
        _currentTrackIndex = Mathf.FloorToInt(CarTransform.position.x / _trackLenght);
        _moveDirection -= _currentTrackIndex;
        PoolManager();
        //Debug.Log(_currentTrackIndex);

        // _leftBorderScreenXToWorldX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        // _rightBorderScreenXToWorldX = _leftBorderScreenXToWorldX + _screenWidthToWorldDistance;
    }
}
