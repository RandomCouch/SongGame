using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField]
    private DetailRow _detailRowPrefab;

    [SerializeField]
    private Transform _detailsContainer;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _accuracyText;

    [SerializeField]
    private Button _continueButton;

    private GameResults _results;
    public GameResults results
    {
        get
        {
            return _results;
        }
        set
        {
            _results = value;
            OnResultsSet();
        }
    }

    private void Start()
    {
        _continueButton?.onClick.AddListener(OnContinueButtonClicked);
    }

    private void OnResultsSet()
    {
        //clear details list first
        foreach(Transform t in _detailsContainer)
        {
            Destroy(t.gameObject);
        }
        //score is based on this
        //one song is worth 500 points
        //actual result for getting a song right is 500 * (1 - timeAnswered)
        int finalScore = 0;
        int roundsWon = 0;
        foreach(RoundResult roundResult in results.rounds)
        {
            if (roundResult.won)
            {
                float scoreCalc = 500f * (1f - (roundResult.time / roundResult.song.sampleClip.length));
                finalScore += Mathf.RoundToInt(scoreCalc);
                roundsWon++;
            }

            DetailRow detailRow = Instantiate(_detailRowPrefab, _detailsContainer);
            detailRow.result = roundResult;
        }

        float accuracy = (float)roundsWon / (float)results.rounds.Length * 100f;
        _scoreText.text = finalScore.ToString();
        _accuracyText.text = Mathf.RoundToInt(accuracy).ToString() + "%";

    }

    private void OnContinueButtonClicked()
    {
        GameManager.Instance.ResetGame();
    }
}
