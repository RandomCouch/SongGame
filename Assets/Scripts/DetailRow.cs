using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class DetailRow : MonoBehaviour
{
    [SerializeField]
    private Image _progressImage;

    [SerializeField]
    private Image _albumCover;

    [SerializeField]
    private Text _songTitle;

    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Image _resultImage;

    [SerializeField]
    private Sprite _winSprite;

    [SerializeField]
    private Sprite _loseSprite;

    private RoundResult _result;
    public RoundResult result
    {
        get
        {
            return _result;
        }
        set
        {
            _result = value;
            OnResultSet();
        }
    }

    private void OnResultSet()
    {
        _progressImage.fillAmount = result.time / result.song.sampleClip.length;
        _albumCover.sprite = result.song.pictureSprite;
        _albumCover.preserveAspect = true;
        _songTitle.text = result.song.artist + " - " + result.song.title;
        SetWin(result.won);
        if (result.won)
        {
            _timeText.text = result.time.ToString("F2") + "s";
        }
    }

    private void SetWin(bool win)
    {
        if (win)
        {
            _resultImage.sprite = _winSprite;
        }
        else
        {
            _resultImage.sprite = _loseSprite;
        }
    }
}
