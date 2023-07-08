using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Event;

public class TextManager : MonoBehaviour
{
    [SerializeField] TMP_Text tmpTimer;
    [SerializeField] TMP_Text tmpEnemyCount;
    [SerializeField] TMP_Text tmpMoveCount;
    [SerializeField] TMP_Text tmpScoreCount;
    [SerializeField] TMP_Text tmpKillStreakCount;
   

    // Start is called before the first frame update
    void Start()
    {
        // Add to receive events
        updateUI();
		this.AddListener(EventID.OnTimeCount, (param) => UpdateTimer());
        this.AddListener(EventID.OnEnemyIncrease, (param) => UpdateEnemyIncrease());
        this.AddListener(EventID.OnEnemyDecrease, (param) => UpdateEnemyDecrease());
        this.AddListener(EventID.OnPlayerMove, (param) => UpdatePlayerMoveCount());
        this.AddListener(EventID.OnPlayerKill, (param) => UpdatePlayerKillCount());
        this.AddListener(EventID.OnScoreCount, (param) => UpdateScore((byte)param));
        InvokeRepeating("CountTimer", 1.0f, 1.0f);
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        this.RemoveListener(EventID.OnTimeCount);
        this.RemoveListener(EventID.OnEnemyIncrease);
        this.RemoveListener(EventID.OnEnemyDecrease);
        this.RemoveListener(EventID.OnPlayerMove);
        this.RemoveListener(EventID.OnPlayerKill);
        this.RemoveListener(EventID.OnScoreCount);
    }

    void OnDestroy()
    {
        this.RemoveAllListener();
    }

    #region Event callback

    private byte _enemyCount = 0;
    public byte EnemyCount
    {
        get => _enemyCount;
        set => _enemyCount = value;
    }
    private ulong _killCount = 0;
    public ulong KillCount
    {
        get => _killCount;
        set => _killCount = value;
    }
    private ulong _moveCount = 0;
    public ulong MoveCount
    {
        get => _moveCount;
        set => _moveCount = value;
    }
    private ulong _scoreCount = 0;
    public ulong ScoreCount
    {
        get => _scoreCount;
        set => _scoreCount = value;
    }
    private double _currentTimeFromStart = 0;
    public double CurrentTimeFromStart
    {
        get => _currentTimeFromStart;
        set => _currentTimeFromStart = value;
    }

	const string ENEMY_TEXT_PREFIX = "Enemy ";
	const string MOVE_TEXT_PREFIX = "Move ";
	const string KILL_TEXT_PREFIX = "Kill ";
	const string SCORE_TEXT_PREFIX = "Score ";

	private void UpdateTimer()
	{
		tmpTimer.text = Utils.formatTimer(_currentTimeFromStart);
        GameManager.Instance._playTime = Utils.formatTimer(_currentTimeFromStart);
	}

	private void UpdateEnemyIncrease()
	{
        _enemyCount++;
        GameManager.Instance._enemyCount = _enemyCount;
		tmpEnemyCount.text = ENEMY_TEXT_PREFIX + _enemyCount;
	}

    private void UpdateEnemyDecrease()
	{
        _enemyCount--;
        GameManager.Instance._enemyCount = _enemyCount;
		tmpEnemyCount.text = ENEMY_TEXT_PREFIX + _enemyCount;
	}

	private void UpdatePlayerMoveCount()
	{
		_moveCount++;
        GameManager.Instance._moveCount = _moveCount;
		tmpMoveCount.text = MOVE_TEXT_PREFIX + _moveCount;
	}

    private void UpdatePlayerKillCount()
	{
		_killCount++;
        SwitchAnnounce(_killCount);
        GameManager.Instance._killCount = _killCount;
		//tmpKillCount.text = MOVE_TEXT_PREFIX + _killCount;
	}

	private void UpdateScore(byte streak)
	{
		_scoreCount += streak;
        GameManager.Instance._score = _scoreCount;
		tmpScoreCount.text = SCORE_TEXT_PREFIX + _scoreCount;
	}

    private void updateUI()
    {
        tmpEnemyCount.text = ENEMY_TEXT_PREFIX + _enemyCount.ToString();
        tmpMoveCount.text = MOVE_TEXT_PREFIX + _moveCount.ToString();
        tmpScoreCount.text = SCORE_TEXT_PREFIX + _scoreCount.ToString();
    }

    private void CountTimer()
    {
        if(GameManager.Instance.isLost) return;
        _currentTimeFromStart++;
        if(GameManager.Instance.isPausing) WaitUntilUnpause();
        this.PostEvent(EventID.OnTimeCount);
        //Invoke("CountTimer", 1.0f);
    }

    IEnumerator WaitUntilUnpause()
    {
        yield return new WaitUntil(() => Time.timeScale == 1.0f);
    }

    private void SwitchAnnounce(ulong kill)
    {   
        switch(kill)
        {
            case 10:
                this.PostEvent(EventID.OnOneHundredPoint);
                break;
            case 30:
                this.PostEvent(EventID.OnThreeHundredPoint);
                break;
            case 50:
                this.PostEvent(EventID.OnFiveHundredPoint);
                break;
            case 100:
                this.PostEvent(EventID.OnOneThousandPoint);
                break;
            case 200:
                this.PostEvent(EventID.OnTwoThousandPoint);
                break;
            case 300:
                this.PostEvent(EventID.OnThreeThousandPoint);
                break;
            case 350:
            case 400:
            case 450:
            case 500:
            case 550:
            case 600:
            case 650:
            case 700:
            case 750:
            case 800:
            case 850:
            case 900:
            case 950:
            case 1000:
                this.PostEvent(EventID.OnShowMeMore);
                break;
            default:
                break;
        }
    }

	#endregion
}
