using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorkScript : MonoBehaviour
{
    private ParticleSystem[] _fireCores;
    private ParticleSystem _fireExplosion;
    private bool _particlePalying = false;
    private int _randomFireCoreNum = 0;
    private int _maxRandomFireCoreNum;
    void Start()
    {
        _fireCores = gameObject.GetComponentsInChildren<ParticleSystem>(false);
        _maxRandomFireCoreNum = _fireCores.Length;
        foreach (var item in _fireCores)
        {
            item.Play();
            _fireExplosion =  item.subEmitters.GetSubEmitterSystem(0);
            _fireExplosion.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (!_fireCores[_randomFireCoreNum].isPlaying)
        {
            _randomFireCoreNum = Random.Range(0, _maxRandomFireCoreNum - 1);
            _fireCores[_randomFireCoreNum].Play();
        }
    }
}
