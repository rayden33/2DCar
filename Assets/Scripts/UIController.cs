using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField] private float _screenShotTimer = 2f;
    [SerializeField] private float _screenMoveUpSpeed = 500f;
    [SerializeField] private float _panelMoveDuration = 2f;
    [SerializeField] private Sprite[] _mainMenuFigureSprites;
    [SerializeField] private RawImage _screenShotViewRawImage;
    [SerializeField] private Image _figureImage;
    [SerializeField] private Text _scoreText;
    [SerializeField] private RectTransform _mainMenuPanel;
    [SerializeField] private RectTransform _mainInfoPanel;
    [SerializeField] private GameObject _screenShotEffectPanel;

    private bool _isEndWay = false;
    private bool _screenShoted = false;
    private float _tmpTimer;
    private int _nextMainMenuSpriteIndex = 1;
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
    public void ChangeFigure()
    {
        _figureImage.sprite = _mainMenuFigureSprites[_nextMainMenuSpriteIndex];
        _nextMainMenuSpriteIndex = (_nextMainMenuSpriteIndex + 1) % _mainMenuFigureSprites.Length;
    }
    public void OpenMainMenu()
    {
        _mainMenuPanel.DOAnchorPos(Vector2.zero, _panelMoveDuration);
    }
    public void CloseMainMenu()
    {
        _mainMenuPanel.DOAnchorPos(new Vector2(0, -Screen.height),_panelMoveDuration);
    }

    public void OpenMainInfo()
    {
        _mainInfoPanel.DOAnchorPos(Vector2.zero, _panelMoveDuration);
        CloseMainMenu();
    }
    public void CloseMainInfo()
    {
        _mainInfoPanel.DOAnchorPos(new Vector2(Screen.width, 0),_panelMoveDuration);
    }
    public void ClickOKButton()
    {
        CloseMainInfo();
        OpenMainMenu();
    }

    private void SyncScoreText()
    {
        _scoreText.text = GameController.instance.Score.ToString();
    }

    void OnEnable()
    {
        GameController.instance.onPickStar += SyncScoreText;
    }
    void Start()
    {
        _screenShotEffect = _screenShotEffectPanel.GetComponent<ScreenShotEffect>();
        _screenShotViewRawImage.transform.position = new Vector2(Screen.width/2f, -Screen.height/2f);
        _mainMenuPanel.anchoredPosition = new Vector2(0, -Screen.height);
        _mainInfoPanel.anchoredPosition = new Vector2(Screen.width, 0);
        _tmpTimer = _screenShotTimer;
    }

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
