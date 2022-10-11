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

    public enum MoveType
    {
        Pawn = 0,
        Knight,
        Bishop,
        Rook,
        Queen,
        King = 5
    }

    public Button btChangeSpeed;
    public Button btPause;
    public float gameSpeed = 1.0f;
    public bool _isPlaying = false;
    //[SerializeField] Transform backgroundTransform;
    [SerializeField] Toggle tgSwitchDimension;
    [SerializeField] Button btRestart;
    [SerializeField] Button btSettings;
    [SerializeField] TMP_Text tmpFPS;

    public bool _isReadyToSwitchIsometric = false;

    float[] gameSpeeds = { 1.0f, 1.5f, 2.0f, 3.0f, 5.0f };
    string[] gameSpeedsTxt = { "x1", "x1.5", "x2", "x3", "x5" };
    public bool isPausing = false;
    public bool isLost = false;
    public float _maxZoom = 9.0f;
    float time;
    int frameCount;
    float pollingTime = 1.0f;
    public byte _enemyCount = 0;
    public int _score = 0;

    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        tgSwitchDimension.onValueChanged.AddListener(OnValueChangedToggleSwitchDimension);
        btRestart.onClick.AddListener(OnClickButtonRestart);
        // btChangeSpeed.onClick.AddListener(OnClickButtonChangeSpeed);
        btSettings.onClick.AddListener(OnClickButtonSettings);
    }

    private void OnDisable()
    {
        tgSwitchDimension.onValueChanged.RemoveAllListeners();
        btRestart.onClick.RemoveAllListeners();
        // btChangeSpeed.onClick.RemoveAllListeners();
        btSettings.onClick.RemoveAllListeners();
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
        _maxZoom = 9.0f;
        ChangeState(GameState.GenerateGrid);
        _isReadyToSwitchIsometric = true;
        ChangeState(GameState.SpawnAttacker);
        ChangeState(GameState.SpawnDefender);
        ChangeState(GameState.AttackerTurn);
        Application.targetFrameRate = 20;
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
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPlaying = true;
            SceneManager.LoadScene(0);
        }

        if(isLost) return;

        //tmpTimer.text = Utils.formatTimer(currentTimeFromStart);

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            switchDimension(_isReadyToSwitchIsometric);
        }

        

        if (_enemyCount == 10)
        {
            Toast.Instance.showToast(0, "YOU LOST", 5.0f, Toast.ANIMATE.TRANSPARENT);
            isLost = true;
            btRestart.gameObject.SetActive(true);
            //btRestart.GetComponentInChildren<TMP_Text>().gameObject.SetActive(Time.unscaledTime % .5 < .2);
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
        string playerName = SetPlayerName();
        LootLockerSDKManager.SubmitScore(playerName, _score, 7528, DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss"), (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Player " + playerName + " success submits score: " + _score);
                isDone = true;
            }
            else
            {
                Debug.Log("Player " + playerName + " submits score failed: " + response.Error);
                isDone = true;
            }
        });
        yield return new WaitUntil(() => isDone);
    }    

    private string SetPlayerName()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");
        LootLockerSDKManager.SetPlayerName(playerName, (response) => 
        {
            if(response.success)
            {
                
            }
            else
            {
                playerName = "Another " + UnityEngine.Random.Range(1, 6969); 
            }
        });
        return playerName;
    }

    public void ChangeState(GameState newState)
    {
        //gameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateNormalGrid();
                //GridManager.Instance.GenerateIsometricBlockGrid();
                break;
            case GameState.SpawnAttacker:
                UnitManager.Instance.SpawnAttackerRandomPos();
                break;
            case GameState.SpawnDefender:
                UnitManager.Instance.SpawnDefenderRandomPos();
                break;
            case GameState.AttackerTurn:
                // MoveType randomMoveType = (MoveType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(MoveType)).Length);
                // Player.Instance.Move(0, GridManager.Instance.GetPlayerMoveableTiles(!_isReadyToSwitchIsometric, Player.Instance._parentTransform.position, randomMoveType));
                Player.Instance.Move(0, GridManager.Instance.GetPlayerMoveableTiles(!_isReadyToSwitchIsometric, Player.Instance._parentTransform.position, MoveType.Knight));
                break;
            case GameState.DefenderTurn:
                break;
            default:
                Debug.Log(nameof(newState));
                break;
        }
    }

    private void switchDimension(bool value)
    {
        if (value)
        {
            StartCoroutine(GridManager.Instance.ConvertToIsometricTileWithDestroy());
            //backgroundTransform.position = new Vector2(0, 0.5f);
            Camera.main.transform.position = new Vector3(0, 0.5f, -10);
            _isReadyToSwitchIsometric = false;
            _maxZoom = 16.0f;
        }
        else
        {
            StartCoroutine(GridManager.Instance.ConvertToNormalTileWithDestroy());
            //backgroundTransform.position = new Vector2(-0.5f, 0.5f);
            Camera.main.transform.position = new Vector3(-0.5f, 0.5f, -10);
            _isReadyToSwitchIsometric = true;
            _maxZoom = 9.0f;
        }
    }

    private void OnValueChangedToggleSwitchDimension(bool value)
    {
        string tmpText = value ? "Isometric" :  "2D";
        tgSwitchDimension.GetComponentInChildren<TMP_Text>().text = tmpText;
        switchDimension(value);
    }

    private void OnClickButtonRestart()
    {
        StartCoroutine(SubmitScore());
        _isPlaying = true;
        Toast.Instance.showToast(0, "6", 6.0f, Toast.ANIMATE.CHANGE_TEXT);
        StartCoroutine(WaitToRestart(5.0f));
        //SceneManager.LoadScene(0);
    }

    IEnumerator WaitToRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(0);
    }

    private void OnClickButtonSettings()
    {
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
}