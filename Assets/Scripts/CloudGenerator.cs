using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{   
    [Header("Cloud generate in scene interval")]
    public float _minCount;
    public float _maxCount;
    [Header("Cloud generate height interval")]
    public float _minHeight;
    public float _maxHeight;
    [Header("Cloud speed interval")]
    public float _minSpeed;
    public float _maxSpeed;
    public float _cloudHideCreateOffset = 2f;

    private float _cloudDistance;
    private List<Transform> _cloudPoolList = new List<Transform>();
    private Transform _lastCloud;
    private CloudScript _cloud;

    private Transform GetCloudFromPool()
    {
        foreach (var cloud in _cloudPoolList)
            if (!cloud.gameObject.activeInHierarchy) return cloud;

        Debug.Log("not enough clouds");
        return null;
    }
    private void PoolCloud()
    {
        float _cloudHeight = Random.Range(_minHeight, _maxHeight);
        float _cloudSpeed = Random.Range(_minSpeed, _maxSpeed);
        Transform _tmpCloud = GetCloudFromPool();
        _tmpCloud.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        _tmpCloud.position = new Vector3(_tmpCloud.position.x + _cloudHideCreateOffset, _cloudHeight,0);
        _tmpCloud.gameObject.SetActive(true);
        _cloud = _tmpCloud.GetComponent<CloudScript>();
        _cloud.Speed = _cloudSpeed;
        _cloud.HideOffset = _cloudHideCreateOffset;
        _lastCloud = _tmpCloud;
        /// distance between last cloud and next cloud that will be generated
        _cloudDistance = Random.Range(1f/_maxCount, 1f/_minCount);
        _cloudDistance *= Screen.width;
    }
    private void PoolManager()
    {
        if (Screen.width - Camera.main.WorldToScreenPoint(_lastCloud.position).x > _cloudDistance)
        {
            PoolCloud();
        }
    }
    void Start()
    {
        foreach (Transform _child in transform)
        {
            _cloudPoolList.Add(_child);
            _child.gameObject.SetActive(false);
        }

        PoolCloud();
    }

    void FixedUpdate()
    {
        PoolManager();
    }
}
