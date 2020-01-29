using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StarScript : MonoBehaviour
{
    public UnityEvent OnDestroyStar;

    public ParticleSystem StarDestroyEffect;
    private bool _picked = false;
    public void HideStar()
    {
        StarDestroyEffect.transform.parent = null;
        Destroy(gameObject);
        // gameObject.SetActive(false);         /// Need to use ObjectPool in final
    }
    void Awake()
    {
        OnDestroyStar.AddListener(HideStar);
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameController.instance.onPickStar.Invoke();
            OnDestroyStar.Invoke();
        }
    }
    
}
