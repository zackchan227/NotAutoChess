using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using LootLocker.Requests;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState
    {
        GenerateGrid = 0,
        SpawnAttacker = 1,
        SpawnDefender = 2,
        AttackerTurn = 3,
        DefenderTurn = 4
    }

 

    public Button btChangeSpeed;
    public Button btPause;
    public float gameSpeed = 1.0f;
    public bool _isPlaying {get; set ;}
    //[SerializeField] Transform backgroundTransform;
    [SerializeField] Toggle tgSwitchDimension;
    [SerializeField] TMP_Dropdown ddMusic;
    [SerializeField] Toggle tgSound;
    [SerializeField] Button btRestart;
    [SerializeField] Button btSettings;
    [SerializeField] TMP_Text tmpFPS;
    [SerializeField] public GameConfig gameConfig;
    [SerializeField] public GameObject containerDownload;
    [SerializeField] TMP_Text tmpMoveType;
    public bool _isReadyToSwitchIsometric {get; set ;}
    public MoveType _currentMoveType {get; set ;}

    float[] gameSpeeds = { 1.0f, 1.5f, 2.0f, 3.0f, 5.0f };
    string[] gameSpeedsTxt = { "x1", "x1.5", "x2", "x3", "x5" };
    public bool isPausing {get; set ;}
    public bool isLost {get; private set ;}
    public float _maxZoom;
    float time;
    int frameCount;
    float pollingTime = 1.0f;
    public byte _enemyCount {get; set ;}
    public ulong _moveCount {get; set ;}
    public ulong _killCount {get; set ;}
    private int _playCount = 0;
    public ulong _score {get; set ;}
    public bool isSoundsOn {get; set ;}
    public string _playTime {get; set ;}

    void Awake()
    {
        Instance = this;
        // TMP_Text[] texts = FindObjectsOfType<TMP_Text>();
        // foreach(TMP_Text txt in texts)
        // {
        //     txt.font = gameConfig.fontTMP;
        // }
        _maxZoom = 9.0f;
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        tgSwitchDimension.onValueChanged.AddListener(OnValueChangedToggleSwitchDimension);
        tgSound.onValueChanged.AddListener(OnValueChangedToggleSound);
        btRestart.onClick.AddListener(OnClickButtonRestart);
        btSettings.onClick.AddListener(OnClickButtonSettings);
        //ddMusic.onValueChanged.AddListener(delegate {OnValueChangedDropdownMusic(ddMusic);});
        _playCount = PlayerPrefs.GetInt("PlayCount");
        _playCount++;
        PlayerPrefs.SetInt("PlayCount", _playCount);
        Debug.Log(_playCount);
    }

    private void OnDisable()
    {
        tgSwitchDimension.onValueChanged.RemoveAllListeners();
        btRestart.onClick.RemoveAllListeners();
        // btChangeSpeed.onClick.RemoveAllListeners();
        btSettings.onClick.RemoveAllListeners();
        //ddMusic.onValueChanged.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        // for (int i = 0; i < gameSpeeds.Length; i++)
        // {
        //     if (gameSpeed == gameSpeeds[i])
        //     {
        //         hasInitGameSpeed = true;
        //         btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[i];
        //         break;
        //     }
        // }
        // if (!hasInitGameSpeed) gameSpeed = 1.0f;
        StartCoroutine(ConnectLootLocker());
        isLost = false;
        isPausing = false;
        _isReadyToSwitchIsometric = true;
        _isPlaying = false;
        ChangeState(GameState.GenerateGrid);
        ChangeState(GameState.SpawnAttacker);
        ChangeState(GameState.SpawnDefender);
        ChangeState(GameState.AttackerTurn);
        //Application.targetFrameRate = 30;
        int isOn = PlayerPrefs.GetInt("IsSoundsOn",1);
        if(isOn == 0) isSoundsOn = false;
        else isSoundsOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameDefine._isDeveloperBuild)
        {
            time += Time.unscaledDeltaTime;
            frameCount++;
            if (time > pollingTime)
            {
                int frameRate = Mathf.RoundToInt(frameCount / time);
                tmpFPS.text = frameRate.ToString() + "fps";
                time -= pollingTime;
                frameCount = 0;
            }
        }

        if(isPausing) return;
        
        if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
        {
            //Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(0);
         }

        if(isLost) return;

        //tmpTimer.text = Utils.formatTimer(currentTimeFromStart);

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            switchDimension(_isReadyToSwitchIsometric);
        }

        

        if (_enemyCount >= gameConfig.enemyCount)
        {
            Toast.Instance.showToast("MISSION FAILED", 5.0f, Toast.ANIMATE.TRANSPARENT);
            isLost = true;
            StartCoroutine(SubmitScore());
            btRestart.gameObject.SetActive(true);
        }
    }

    IEnumerator ConnectLootLocker()
    {
        bool isDone = false;
        LootLockerSDKManager.StartGuestSession((response) => 
        {
            if(response.success)
            {
                Debug.Log("Guest Login Success");
                //PlayerPrefs.SetString("PlayerID",response.player_id.ToString());
                string playerName = PlayerPrefs.GetString("PlayerName");
                if(string.IsNullOrEmpty(playerName))
                {
                    PlayerPrefs.SetString("PlayerName", SystemInfo.deviceName);
                }
                playerName = PlayerPrefs.GetString("PlayerName");
                isDone = true;
            }
            else 
            {
                Debug.Log("Guest Login Failed");
                isDone = true;
                StartCoroutine(ConnectLootLocker());
            }
        });
        yield return new WaitUntil(() => isDone);
    }

    IEnumerator SubmitScore()
    {
        bool isDone = false;
        string playerName = PlayerPrefs.GetString("PlayerName");
        string metaData = _moveCount.ToString() + "," 
                        + _killCount.ToString() + "," 
                        + _playCount + "," 
                        + gameConfig.gameLevel + "," 
                        + _playTime + "," 
                        + DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
        //NetworkManager.Instance.SaveScore(metaData);
        LootLockerSDKManager.SubmitScore(playerName, (int)_score, 7528, metaData, (response) =>
        {
            if (response.statusCode == 200)
            {
                //Toast.Instance.showToast("Success Submit Score: " + response.text, 5.0f, Toast.ANIMATE.TRANSPARENT);
                isDone = true;
            }
            else
            {
                Toast.Instance.showToast("Submit Score Failed: " + response.Error, 5.0f, Toast.ANIMATE.TRANSPARENT);
                isDone = true;
            }
        });
        yield return new WaitUntil(() => isDone);
    }    

    public void ChangeState(GameState newState)
    {
        //gameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateNormalGrid();
                //GridManager.Instance.GenerateIsometricGrid();
                break;
            case GameState.SpawnAttacker:
                UnitManager.Instance.SpawnAttackerRandomPos();
                break;
            case GameState.SpawnDefender:
                UnitManager.Instance.SpawnDefenderRandomPos();
                break;
            case GameState.AttackerTurn:
                _currentMoveType = getRandomMoveType(gameConfig.gameLevel);
                tmpMoveType.text = getMoveTypeName(_currentMoveType);
                // Player.Instance.Move(0, GridManager.Instance.GetPlayerMoveableTiles(!_isReadyToSwitchIsometric, Player.Instance._parentTransform.position, randomMoveType));
                Player.Instance.Move(0, GridManager.Instance.GetPlayerMoveableTiles(!_isReadyToSwitchIsometric, Player.Instance._parentTransform.position, _currentMoveType));
                break;
            case GameState.DefenderTurn:
                UnitManager.Instance.AllDefendersFlipX();
                break;
            default:
                Debug.Log(nameof(newState));
                break;
        }
    }

    private void switchDimension(bool value)
    {
        string tmpText = "2D";
        
        if (value)
        {
            StartCoroutine(GridManager.Instance.ConvertToIsometricTileWithDestroy());
            //backgroundTransform.position = new Vector2(0, 0.5f);
            Camera.main.transform.position = new Vector3(0, 0.5f, -10);
            _isReadyToSwitchIsometric = false;
            _maxZoom = 16.0f;
            tmpText = "Isometric";
        }
        else
        {
            StartCoroutine(GridManager.Instance.ConvertToNormalTileWithDestroy());
            //backgroundTransform.position = new Vector2(-0.5f, 0.5f);
            Camera.main.transform.position = new Vector3(-0.5f, 0.5f, -10);
            _isReadyToSwitchIsometric = true;
            _maxZoom = 9.0f;
            tmpText = "2D";
        }
        tgSwitchDimension.GetComponentInChildren<TMP_Text>().text = tmpText;
    }

    private void OnValueChangedToggleSwitchDimension(bool value)
    {
        switchDimension(value);
    }

    private void OnValueChangedToggleSound(bool value)
    {
        SoundsCheck(value);
    }

    public void OnValueChangedDropdownMusic(TMP_Dropdown change)
    {
        SoundsManager.Instance.LoadMusic(change.name.ToLower());
    }

    public void SoundsCheck(bool value)
    {
        string tmpText = value ? "ON" :  "OFF";
        tgSound.GetComponentInChildren<TMP_Text>().text = tmpText;
        Player.Instance.AudioSource.mute = !value;
        Player.Instance.AudioSource.volume = PlayerPrefs.GetFloat("VolumeSFX",1.0f);
        //Player.Instance.AudioSource.enabled = value;
        SoundsManager.Instance.AudioSource.mute = !value;
        SoundsManager.Instance.AudioSource.volume = PlayerPrefs.GetFloat("VolumeBGM",1.0f);
        //SoundsManager.Instance.AudioSource.enabled = value;
        AnnounceManager.Instance.AudioSource.mute = !value;
        AnnounceManager.Instance.AudioSource.volume = PlayerPrefs.GetFloat("VolumeSFX",1.0f);
        //AnnounceManager.Instance.AudioSource.enabled = value;
        PlayerPrefs.SetInt("IsSoundsOn", value ? 1 : 0);
    }

    private void OnClickButtonRestart()
    {
        SceneLoader.Instance.Restart(); 
    }

    IEnumerator WaitToRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneLoader.Instance.Restart();
    }

    private void OnClickButtonSettings()
    {
        //OnClickButtonPause();
        DialogManager.Instance.ShowDialog("DialogSettings");
    }

    private void OnClickButtonChangeSpeed()
    {
        switch (gameSpeed)
        {
            case 1.0f:
                gameSpeed = gameSpeeds[1];
                btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[1];
                break;
            case 1.5f:
                gameSpeed = gameSpeeds[2];
                btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[2];
                break;
            case 2.0f:
                gameSpeed = gameSpeeds[3];
                btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[3];
                break;
            case 3.0f:
                gameSpeed = gameSpeeds[4];
                btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[4];
                break;
            case 5.0f:
            default:
                gameSpeed = gameSpeeds[0];
                btChangeSpeed.GetComponentInChildren<Text>().text = gameSpeedsTxt[0];
                break;
        }
        PlayerPrefs.SetFloat("gameSpeed", gameSpeed);
    }

    private void OnClickButtonPause()
    {
        if (!isPausing)
        {
            Time.timeScale = 0f;
            btPause.GetComponentInChildren<Text>().text = "Unpause";
            isPausing = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            btPause.GetComponentInChildren<Text>().text = "Pause";
            isPausing = false;
        }
    }

    private string getMoveTypeName(MoveType type)
    {
        string result = "";
        switch (type)
        {
            case MoveType.Pawn:
                result = "Pawn";
                break;
            case MoveType.Bishop:
                result = "Bishop";
                break;
            case MoveType.Rook:
                result = "Rook";
                break;
            case MoveType.Knight:
                result = "Knight";
                break;
            case MoveType.King:
                result = "King";
                break;
            case MoveType.Queen:
                result = "Queen";
                break;
            default:
                break;
        }
        return result;
    }

    private MoveType getRandomMoveType(byte level)
    {
        MoveType result = MoveType.Pawn;
        if(level == 0)
        {
            result = gameConfig.chessPossibility.GetRandomItem().moveType;
        }
        else
        {
            result = (MoveType)Enum.GetValues(typeof(MoveType)).GetValue(level-1);
        }
        return result;
    }
}                               