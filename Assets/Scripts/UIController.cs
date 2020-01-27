using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private float _screenShotTimer = 2f;
    [SerializeField] private float _screenMoveUpSpeed = 500f;
    [SerializeField] private RawImage _screenShotViewRawImage;
    [SerializeField] private RectTransform _mainMenuPanel;
    [SerializeField] private RectTransform _mainInfoPanel;
    [SerializeField] private GameObject _screenShotEffectPanel;

    private bool _isEndWay = false;
    private bool _screenShoted = false;
    private float _tmpTimer;
    private ScreenShotEffect _screenShotEffect;

    public void EndWay()
    {
        _isEndWay = true;
    }
    public void ScreenShot()
    {
        _screenShotEffect.ScreenShotMode = true;
        _screenShotViewRawImage.texture = ScreenCapture.CaptureScreenshotAsTexture();
        _screenShoted = true;
    }

    void Start()
    {
        _screenShotEffect = _screenShotEffectPanel.GetComponent<ScreenShotEffect>();
        _screenShotViewRawImage.transform.position = new Vector2(_screenShotViewRawImage.transform.position.x, -Screen.height/2f);
        _mainMenuPanel.anchoredPosition = new Vector2(0, -Screen.height);
        _mainInfoPanel.anchoredPosition = new Vector2(Screen.width, 0);
        _tmpTimer = _screenShotTimer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_screenShoted)
        {
            if (_screenShotViewRawImage.transform.position.y < Screen.height/2f)
                _screenShotViewRawImage.transform.Translate(Vector2.up * _screenMoveUpSpeed * Time.deltaTime);
        }
        else if (_isEndWay)
        {
            if (_tmpTimer > 0)
                _tmpTimer -= Time.deltaTime;
            else
                ScreenShot();
        }
    }
}
