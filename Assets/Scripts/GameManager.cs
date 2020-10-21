using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset _jsonFile;

    [SerializeField]
    private LoadingScreen _loadingScreen;

    [SerializeField]
    private Transform _playlistContainer;

    [SerializeField]
    private PlaylistButton _playlistButtonPrefab;

    [SerializeField]
    private List<Screen> _screens;

    private AudioSource _audioSource;

    public enum GameState
    {
        Loading,
        SelectPlaylist,
        GameScreen,
        ScoreScreen
    }

    private GameState _state;

    public GameState state
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

    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnStateChanged()
    {
        switch (state)
        {
            case GameState.Loading:
                SwitchScreen("loading");
                break;
            case GameState.SelectPlaylist:
                SwitchScreen("select");
                break;
            case GameState.GameScreen:
                SwitchScreen("game");
                break;
            case GameState.ScoreScreen:
                SwitchScreen("score");
                break;
        }
    }

    private void SwitchScreen(string id)
    {
        foreach(Screen screen in _screens)
        {
            if (screen.ScreenObject != null) {
                screen.ScreenObject.SetActive(false);
                if (screen.id.Equals(id))
                {
                    screen.ScreenObject.SetActive(true);
                }
            }
        }
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        //process json data into objects
        state = GameState.Loading;
        GameData data = JsonUtility.FromJson<GameData>("{\"playlists\":" + _jsonFile.text + "}");
        for(int p = 0; p < data.playlists.Length; p++)
        {
            Playlist playlist = data.playlists[p];
            for(int q = 0; q < playlist.questions.Length; q++)
            {
                Question question = playlist.questions[q];
                if (!string.IsNullOrEmpty(question.song.picture))
                {
                    //Download image
                    UnityWebRequest request = UnityWebRequestTexture.GetTexture(question.song.picture);
                    yield return request.SendWebRequest();

                    if(request.isNetworkError || request.isHttpError)
                    {
                        Debug.LogError("Request for image " + question.song.picture + " failed");
                    }
                    else
                    {
                        Texture2D tex2d = DownloadHandlerTexture.GetContent(request);
                        Sprite pSprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0f, 0f));
                        data.playlists[p].questions[q].song.pictureSprite = pSprite;
                    }
                }
                if (!string.IsNullOrEmpty(question.song.sample))
                {
                    UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(question.song.sample, AudioType.WAV);
                    yield return request.SendWebRequest();

                    if (request.isNetworkError || request.isHttpError)
                    {
                        Debug.LogError("Request for audio clip " + question.song.sample + " failed");
                    }
                    else
                    {
                        data.playlists[p].questions[q].song.sampleClip = DownloadHandlerAudioClip.GetContent(request);
                    }
                }

                float qProgress = (q + 1f) / (float)playlist.questions.Length;
                _loadingScreen.UpdateSecondaryCircle(qProgress);
            }
            Debug.Log("Processed playlist " + playlist.playlist);
            float pProgress = (p + 1f) / (float)data.playlists.Length;
            _loadingScreen.UpdateMainLoadingCircle(pProgress);

            PlaylistButton button = Instantiate(_playlistButtonPrefab, _playlistContainer);
            button.SetButtonText(playlist.playlist);
            button.OnPlaylistClicked += () =>
            {
                OnPlaylistClicked(playlist);
            };
        }
        Debug.Log("Processed game data");
        yield return new WaitForSeconds(0.5f);

        state = GameState.SelectPlaylist;
    }

    private void OnPlaylistClicked(Playlist playlist)
    {
        state = GameState.GameScreen;
        GameScreen gameScreen = _screens.Where(x => x.id == "game").FirstOrDefault().ScreenObject.GetComponent<GameScreen>();
        gameScreen.playlist = playlist;
        
    }

    public void ShowScore(GameResults results)
    {
        state = GameState.ScoreScreen;
        ScoreScreen scoreScreen = _screens.Where(x => x.id == "score").FirstOrDefault().ScreenObject.GetComponent<ScoreScreen>();
        scoreScreen.results = results;
    }

    public void PlayAudio(AudioClip clip)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void StopAudio()
    {
        _audioSource.Stop();
    }


    public void ResetGame()
    {
        state = GameState.SelectPlaylist;
    }
}
