using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;
    public AudioSource AudioSource;
    [SerializeField] private List<AudioClip> _musics;
    [SerializeField] private string _lyricsPath;
    [SerializeField] private ParticleSystem _particleText;
    ParticleSystem _particleTextPrefab;
    string currentMusicName = "";
    const string FILE_EXTENSION = ".txt";
    float musicTime = 0;
    int i = 0;
    List<string> _lyrics;
    List<float> _timeLyrics;
    float time, pollingTime = 1.0f;
    WaitForSecondsRealtime wait;

    void Awake()
    {
        Instance = this;
        AudioSource = this.GetComponent<AudioSource>();
        _lyrics = new List<string>();
        _timeLyrics = new List<float>();
        AudioSource.ignoreListenerPause = true;
        _particleTextPrefab = Instantiate(_particleText,this.transform);
        wait = new WaitForSecondsRealtime(10.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
       randomBackgroundMusic();
    }

    void Update()
    {
        if(AudioSource.isPlaying && _lyrics.Count > 0)
        {
            displayLyrics();
        }
    }

    private void randomBackgroundMusic()
    {
        if(_musics.Count == 0) return;
        AudioSource.clip = _musics[Random.Range(0,_musics.Count)];
        //AudioSource.clip = _musics[3];
        currentMusicName = AudioSource.clip.name;
        addLyricsTimer();
        if(!AudioSource.isPlaying) 
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
        AudioSource.Stop();
        Popup.Instance.ResetPosition();
        randomBackgroundMusic();
    }

    private void addLyricsTimer()
    {
        _lyrics.Clear();
        _timeLyrics.Clear();
        string filePath = "";
        try
        {
            filePath = _lyricsPath+currentMusicName+FILE_EXTENSION;
            if(!File.Exists(filePath))
            {
                filePath = "";
                Debug.Log(currentMusicName+FILE_EXTENSION + " not found");
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("No Current Song Lyrics: " + e);
        }

        if(!string.IsNullOrEmpty(filePath))
        {
            foreach (string line in File.ReadLines(_lyricsPath + currentMusicName + FILE_EXTENSION))
            {
                // line split ':' first element is time, second element is lyric
                _timeLyrics.Add(float.Parse(line.Split(':')[0]));
                _lyrics.Add(line.Split(':')[1]);
            }
        }
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

    // public void MusicSlider()
    // {
    //     AS.time = AC[currentSong].length * slider.value;
    //     slider.value = AS.time / AC[currentSong].length;
    //     Debug.Log(AS.time);
    // }


}
