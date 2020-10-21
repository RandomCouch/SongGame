using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour
{
    [SerializeField]
    private Color _activeColor;
    [SerializeField]
    private Color _inactiveColor;
    [SerializeField]
    private Color _wonColor;
    [SerializeField]
    private Color _lostColor;

    [SerializeField]
    private Image _questionImg;

    [SerializeField]
    private Image _progress;

    [SerializeField]
    private Text _questionText;

    private QuestionState _state;
    public enum QuestionState
    {
        Inactive,
        Active,
        Won,
        Lost
    }
    public QuestionState state
    {
        get
        {
            return _state;
        }
        set
        {
            if(_state != value)
            {
                _state = value;
                OnStateChanged();
            }
        }
    }

    public float timeCompleted
    {
        get
        {
            return _progress.fillAmount * question.song.sampleClip.length;
        }
    }

    public Question question;

    private void OnStateChanged()
    {
        switch (state)
        {
            case QuestionState.Inactive:
                _questionImg.color = _inactiveColor;
                break;
            case QuestionState.Active:
                _questionImg.color = _activeColor;
                break;
            case QuestionState.Lost:
                _questionImg.color = _lostColor;
                break;
            case QuestionState.Won:
                _questionImg.color = _wonColor;
                break;
        }
    }

    public void SetProgress(float p)
    {
        p = Mathf.Clamp(p, 0f, 1f);
        _progress.fillAmount = p;
    }

    public void SetQuestionText(string text)
    {
        _questionText.text = text;
    }

}
