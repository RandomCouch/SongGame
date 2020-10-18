using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset _jsonFile;

    [SerializeField]
    private LoadingScreen _loadingScreen;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        //process json data into objects
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
        }
        Debug.Log("Processed game data");
        yield return new WaitForSeconds(0.5f);
        _loadingScreen.gameObject.SetActive(false);
    }
}
