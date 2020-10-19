using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistButton : MonoBehaviour
{
    private Button _button;

    [SerializeField]
    private Text _buttonText;

    public Action OnPlaylistClicked;
    public Playlist playlist;

    private void Start()
    {
        _button?.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnPlaylistClicked?.Invoke();
    }

    public void SetButtonText(string text)
    {
        _buttonText.text = text;
    }
}
