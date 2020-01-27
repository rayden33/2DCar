using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotEffect : MonoBehaviour
{
    [SerializeField] private float _screenShotEffectSpeed = 3f;
    private Image _whiteImage;
    private bool _fullWhite = false;
    private bool _screenShotMode = false;
    public bool ScreenShotMode
    {
        set { _screenShotMode = value; }
    }
    void Start()
    {
        _whiteImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_screenShotMode)
        {
            if (!_fullWhite)
            {
                _whiteImage.color = new Color (_whiteImage.color.r, _whiteImage.color.g, _whiteImage.color.b, _whiteImage.color.a + _screenShotEffectSpeed * Time.deltaTime);
                if (_whiteImage.color.a >= 1f)
                    _fullWhite = true;
            }
            else
            {
                _whiteImage.color = new Color (_whiteImage.color.r, _whiteImage.color.g, _whiteImage.color.b, _whiteImage.color.a - _screenShotEffectSpeed * Time.deltaTime);
                if (_whiteImage.color.a <= 0f)
                {
                    _fullWhite = false;
                    _screenShotMode = false;
                }
            }
        }
        
    }
}
