using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;
    public AudioSource AudioSource {get; private set;}
    [SerializeField] private List<AudioClip> _musics;
    [SerializeField] private string _lyricsPath;
    [SerializeField] private List<TextAsset> _textLyrics;
    string currentMusicName = "";
    const string FILE_EXTENSION = ".txt";
    float musicTime = 0;
    int i = 0;
    List<string> _lyrics;
    List<float> _timeLyrics;
    //float time, pollingTime = 1.0f;
    WaitForSecondsRealtime wait;
    private TMP_Text _tmpSongName;
    public float currentVolume {get; set;}
    private Canvas _canvas;
    private TMP_Text _tmpLyrics;
    private GameObject _lyricsPanel;
    private Button _btNote;
    private Button _btClose;
    private Image _imgLyricsPanel;
    private AudioClip currentClip;

    void Awake()
    {
        Instance = this;
        AudioSource = this.GetComponent<AudioSource>();
        _lyrics = new List<string>();
        _timeLyrics = new List<float>();
        wait = new WaitForSecondsRealtime(10.0f);
        _tmpSongName = GameObject.Find("tmpSongName").GetComponent<TMP_Text>();
        _btNote = GameObject.Find("note").GetComponent<Button>();
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //_lyricsPanel = _canvas.gameObject.transform.Find("LyricsPanel(Clone)").gameObject;
        for(int i = 0; i < _musics.Count; i++)
        {
            if(!_musics[i].UnloadAudioData())
            {
                Debug.Log("Unload audio data " + _musics[i] + " failed");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       randomBackgroundMusic();
       _btNote.onClick.AddListener(OnClickButtonNote);
       currentVolume = AudioSource.volume;
    }

    void Update()
    {
        // if(AudioSource.isPlaying && _lyrics.Count > 0)
        // {
        //     displayLyrics();
        // }
    }

    private void playCurrentMusic()
    {
        StopCoroutine(PlayAndWait());
        AudioSource.Stop();
        AudioSource.clip = _musics[_musics.Count-1];
    }

    private void randomBackgroundMusic()
    {
        if(_musics.Count == 0) return;
        currentClip = _musics[Random.Range(0,_musics.Count)];
        currentClip.LoadAudioData();
        StartCoroutine(ReadToPlay());
    }

    private IEnumerator ReadToPlay()
    {
        while(currentClip.loadState != AudioDataLoadState.Loaded)
        {
            yield return null;
        }
        AudioSource.clip = currentClip;
        //AudioSource.clip = _musics[3];
        currentMusicName = AudioSource.clip.name;
        _tmpSongName.text = currentMusicName.Substring(0, 1).ToUpper() + currentMusicName.Substring(1, currentMusicName.Length - 1) + " - " + "Memento Mori";
        //addLyricsTimer();
        if (!AudioSource.isPlaying)
        {
            StartCoroutine(PlayAndWait());
        }
    }

    private IEnumerator PlayAndWait()
    {
        musicTime = 0;
        i = 0;
        AudioSource.Play();
        //InvokeRepeating("displayLyrics",1.0f,1.0f);
        yield return new WaitForSecondsRealtime(AudioSource.clip.length);
        currentClip.UnloadAudioData();
        AudioSource.Stop();
        //Popup.Instance.ResetPosition();
        randomBackgroundMusic();
    }

    private void addLyricsTimer()
    {
        _lyrics.Clear();
        _timeLyrics.Clear();
        //string filePath = "";
        try
        {
            // filePath = _lyricsPath+currentMusicName+FILE_EXTENSION;
            
            // if(!File.Exists(filePath))
            // {
            //     filePath = "";
            //     Debug.Log(currentMusicName+FILE_EXTENSION + " not found");
            // }

            // if (!string.IsNullOrEmpty(filePath))
            // {
            //     foreach (string line in File.ReadLines(_lyricsPath + currentMusicName + FILE_EXTENSION))
            //     {
            //         // line split ':' first element is time, second element is lyric
            //         _timeLyrics.Add(float.Parse(line.Split(':')[0]));
            //         _lyrics.Add(line.Split(':')[1]);
            //     }
            // }
            string[] linesFromfile = GetCurrentSongLyricsText().text.Split("\n"[0]);
            if(linesFromfile.Length > 0)
            {
                foreach (string line in linesFromfile)
                {
                    // line split ':' first element is time, second element is lyric
                    _timeLyrics.Add(float.Parse(line.Split(':')[0]));
                    _lyrics.Add(line.Split(':')[1]);
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("No Current Song Lyrics: " + e);
        }

        
    }

    private TextAsset GetCurrentSongLyricsText()
    {
        for(int i = 0; i < _textLyrics.Count; i++)
        {
            if(_textLyrics[i].name.Equals(currentMusicName))
            {
                return _textLyrics[i];
            }
        }
        return null;
    }

    private void displayLyrics()
    {
        if (i < _timeLyrics.Count)
        {
            if (Mathf.Floor(AudioSource.time) == _timeLyrics[i])
            {
                switch(_timeLyrics[i])
                {
                    case 215:
                    case 220:
                    case 225:
                    case 230:
                        Popup.Instance.showPopupWithTextWriting(_lyrics[i], 5.0f, Popup.ANIMATE.GO_UP_THEN_DISAPPEAR, 0.001f);
                        break;
                    default:
                        Popup.Instance.showPopupWithTextWriting(_lyrics[i], 10.0f, Popup.ANIMATE.TRANSPARENT, 0.001f);
                        break;
                }
                i++;
            }
        }

        if(i == _timeLyrics.Count) StartCoroutine(WaitCouroutine());
    }

    IEnumerator WaitCouroutine()
    {
        yield return wait;
        Popup.Instance.SetActive(false);
    }

    public IEnumerator LoadMusic(string songName)
    {
        string key = songName;
        // Clear all cached AssetBundles
        // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
        // Addressables.ClearDependencyCacheAsync(key);
        Debug.Log(key);
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(key);
        yield return handle;
        if (handle.Result != null)
        {
            _musics.Add(handle.Result);
            Debug.Log("Added new song");
        }
        else
        {
            //Check the download size
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
            yield return getDownloadSize;

            //If the download size is greater than 0, download all the dependencies.
            if (getDownloadSize.Result > 0)
            {
                Debug.Log("Downloading...");
                AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(key);
                yield return downloadDependencies;
            }
            else
            {
                Debug.Log("File not found! " + getDownloadSize);
            }
        }
    }

    public void LoadSong(string songName)
    {
        StopAllCoroutines();
        AudioSource.Stop();
        for(int i = 0; i < _musics.Count; i++)
        {
            if(_musics[i].name.Equals(songName))
            {
                AudioSource.clip = _musics[i];
            }
        }
        
    }


    // public void MusicSlider()
    // {
    //     AS.time = AC[currentSong].length * slider.value;
    //     slider.value = AS.time / AC[currentSong].length;
    //     Debug.Log(AS.time);
    // }

    private void OnClickButtonNote()
    {
        showLyricsPanel();
    }

    public void OnClickButtonCloseNote()
    {
        closeLyricsPanel();
    }

    public void TurnDownVolume()
    {
        AudioSource.volume = Mathf.Lerp(AudioSource.volume, 0, 1.0f * Time.unscaledDeltaTime);
    }

    public void TurnUpVolume()
    {
        AudioSource.volume = Mathf.Lerp(AudioSource.volume, currentVolume, 1.0f * Time.unscaledDeltaTime);
    }

    private void showLyricsPanel()
    {
        if(_lyricsPanel == null)
        {
            StartCoroutine(LoadLyricsPanelAsync());
        }
        else
        {
            _btClose.onClick.AddListener(OnClickButtonCloseNote);
            _tmpLyrics.text = GetCurrentSongLyricsText().text;
        }
        if(_lyricsPanel != null && !_lyricsPanel.activeSelf) 
        {
            _imgLyricsPanel.material.SetFloat("_Progress",0);
            StopCoroutine(WaitLyricsPanelDisappear());
            StartCoroutine(WaitLyricsPanelAppear());
            //_lyricsPanel.gameObject.SetActive(true);
        }
    }

    private void closeLyricsPanel()
    {
        _btClose.onClick.RemoveAllListeners();
        _imgLyricsPanel.material.SetFloat("_Progress",1);
        StopCoroutine(WaitLyricsPanelAppear());
        StartCoroutine(WaitLyricsPanelDisappear());
        //_lyricsPanel.gameObject.SetActive(false);
    }

    IEnumerator LoadLyricsPanelAsync()
    {       
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("LyricsPanel");
        yield return handle;
        if (handle.Result != null)
        {
            _lyricsPanel = Instantiate(handle.Result, _canvas.transform);
            _btClose = _lyricsPanel.transform.Find("close").GetComponent<Button>();
            _tmpLyrics = GameObject.Find("tmpLyrics").GetComponent<TMP_Text>();
            _btClose.onClick.AddListener(OnClickButtonCloseNote);
            _tmpLyrics.text = "";
            _tmpLyrics.text = GetCurrentSongLyricsText().text;
            _imgLyricsPanel = _lyricsPanel.transform.Find("Panel").GetComponent<Image>();
            _imgLyricsPanel.material.SetFloat("_Progress",0);
            StartCoroutine(WaitLyricsPanelAppear());
            //_lyricsPanel.gameObject.SetActive(true);
        }           
    }

    private void Transparent(float time, float totalTime)
    {
        Color tempColor;
        tempColor = _tmpLyrics.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, time/totalTime);
        _imgLyricsPanel.material.SetFloat("_Progress", 1 - time/totalTime);
        _tmpLyrics.color = tempColor;
    }

    private void Opaque(float time,float totalTime)
    {
        if (time >= totalTime / 2)
        {
            Color tempColor;
            tempColor = _tmpLyrics.color;
            tempColor.a = Mathf.Lerp(tempColor.a, 1, time / totalTime / 2);
            _tmpLyrics.color = tempColor;
        }
        //tempColor.a = 1;
        //_imgLyricsPanel.material.SetFloat("_Progress", Mathf.MoveTowards(_imgLyricsPanel.material.GetFloat("_Progress"), 1, time));
        _imgLyricsPanel.material.SetFloat("_Progress", time/totalTime);
        
    }

    IEnumerator WaitLyricsPanelDisappear()
    {
        float t = 0;
        float duration = 2.0f;
        while(t < duration)
        {
            Transparent(t, duration);
            t += Time.deltaTime;
            yield return null;
        }
        _lyricsPanel.gameObject.SetActive(false);
    }

    IEnumerator WaitLyricsPanelAppear()
    {
        _lyricsPanel.gameObject.SetActive(true);
        float t = 0;
        float duration = 2.0f;
        while(t < duration)
        {
            Opaque(t, duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
