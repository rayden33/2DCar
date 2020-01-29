using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    public UnityAction onPickStar;

    private int _score = 0;
    public int Score
    {
        get { return _score; }
    }

    private void PickStar()
    {
        _score++;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void OnEnable()
    {
        onPickStar += PickStar;
    }

}
