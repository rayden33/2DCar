using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScript : MonoBehaviour
{
    [SerializeField] private GameObject Car;
    private TrackGenerator _trackGenerator;
    private float _trackLenght;
    void Start()
    {
        _trackGenerator = gameObject.GetComponentInParent<TrackGenerator>();
        _trackLenght = _trackGenerator.TrackLenght;
    }

    
    void FixedUpdate()
    {
        if (Car.transform.position.x - transform.position.x > 1.5f * _trackLenght || transform.position.x - Car.transform.position.x > 0.5f * _trackLenght)
        {
            gameObject.SetActive(false);
        }
    }
}
