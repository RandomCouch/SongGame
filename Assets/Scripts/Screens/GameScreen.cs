using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField]
    private QuestionItem _questionItemPrefab;

    [SerializeField]
    private ChoiceButton _choiceButtonPrefab;

    [SerializeField]
    private Transform _questionsContainer;

    [SerializeField]
    private Transform _choicesContainer;

    [SerializeField]
    private Text _title;

    [SerializeField]
    private Text _countdownText;

    [SerializeField]
    private int _countdownSeconds;

    private int _currentSongIndex = 0;

    private List<QuestionItem> _questionItems;

    private Playlist _playlist;

    private Coroutine _roundCoroutine;

    private bool _playlistCompleted = false;

    public Playlist playlist
    {
        get
        {
            return _playlist;
        }
        set
        {
            _playlist = value;
            SetupPlaylist();
        }
    }

    private void SetupPlaylist()
    {
        if(_questionItems != null)
        {
            _questionItems.Clear();
        }
        else
        {
            _questionItems = new List<QuestionItem>();
        }
        
        _title.text = playlist.playlist;
        //clear questions
        foreach(Transform t in _questionsContainer)
        {
            Destroy(t.gameObject);
        }
        int questionIndex = 0;
        foreach(Question q in playlist.questions)
        {
            questionIndex++;
            QuestionItem questionGO = Instantiate(_questionItemPrefab, _questionsContainer);
            questionGO.question = q;
            questionGO.SetProgress(0f);
            questionGO.SetQuestionText(questionIndex.ToString());
            questionGO.state = QuestionItem.QuestionState.Inactive;
            _questionItems.Add(questionGO);
        }

        StartCoroutine(CountdownCR());
    }


    private IEnumerator CountdownCR()
    {
        _countdownText.gameObject.SetActive(true);
        _countdownText.text = _countdownSeconds.ToString();
        int elapsedSeconds = 0;
        while(elapsedSeconds < _countdownSeconds)
        {
            yield return new WaitForSeconds(1f);
            elapsedSeconds++;
            int remainingTime = _countdownSeconds - elapsedSeconds;
            _countdownText.text = remainingTime.ToString();
        }
        _countdownText.gameObject.SetActive(false);
        //start game
        
        _roundCoroutine = StartCoroutine(RoundCR());
    }

    private IEnumerator RoundCR()
    {
        _currentSongIndex = Mathf.Clamp(_currentSongIndex, 0, _questionItems.Count - 1);
        QuestionItem  currentQuestion = _questionItems[_currentSongIndex];
        SetupChoices(currentQuestion.question);
        AudioClip clipToPlay = currentQuestion.question.song.sampleClip;
        GameManager.Instance.PlayAudio(clipToPlay);
        

        float elapsedSeconds = 0f;
        while(elapsedSeconds < clipToPlay.length)
        {
            elapsedSeconds += Time.deltaTime;
            float progress = elapsedSeconds / clipToPlay.length;
            currentQuestion.SetProgress(progress);
            currentQuestion.state = QuestionItem.QuestionState.Active;
            yield return null;
        }


        currentQuestion.state = QuestionItem.QuestionState.Lost;
        _currentSongIndex++;

        if (_currentSongIndex < _questionItems.Count)
        {
            _roundCoroutine = StartCoroutine(RoundCR());
        }
        else
        {
            _playlistCompleted = true;
            EndGame();
        }
    }

    private void SetupChoices(Question q)
    {
        //clear choices 
        foreach(Transform t in _choicesContainer)
        {
            Destroy(t.gameObject);
        }

        int cIndex = 0;

        foreach(Choice c in q.choices)
        {
            ChoiceButton choiceButton = Instantiate(_choiceButtonPrefab, _choicesContainer);
            choiceButton.SetButtonText(c.artist + " - " + c.title);
            choiceButton.choiceIndex = cIndex;
            choiceButton.OnChoiceSelected += (choiceIndex) =>
            {
                if(_roundCoroutine != null)
                {
                    StopCoroutine(_roundCoroutine);
                }

                if(choiceIndex == q.answerIndex)
                {
                    _questionItems[_currentSongIndex].state = QuestionItem.QuestionState.Won;
                }
                else
                {
                    _questionItems[_currentSongIndex].state = QuestionItem.QuestionState.Lost;
                }

                _currentSongIndex++;

                if (_currentSongIndex < _questionItems.Count)
                {
                    _roundCoroutine = StartCoroutine(RoundCR());
                }
                else
                {
                    _playlistCompleted = true;
                    EndGame();
                }
            };

            cIndex++;
        }
    }

    private void EndGame()
    {
        //gather results
        GameResults gameResults = new GameResults()
        {
            playlistName = playlist.playlist,
            rounds = new RoundResult[_questionItems.Count]
        };

        for(int i = 0; i < _questionItems.Count; i++)
        {
            gameResults.rounds[i].song = _questionItems[i].question.song;
            gameResults.rounds[i].won = _questionItems[i].state == QuestionItem.QuestionState.Won;
            gameResults.rounds[i].time = _questionItems[i].timeCompleted;
        }

        GameManager.Instance.StopAudio();
        GameManager.Instance.ShowScore(gameResults);

        //clean up screen
        _currentSongIndex = 0;
        //clear questions
        foreach (Transform t in _questionsContainer)
        {
            Destroy(t.gameObject);
        }
        //clear choices 
        foreach (Transform t in _choicesContainer)
        {
            Destroy(t.gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
