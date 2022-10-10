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
        Invoke("CountTimer", 1.0f);
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
    private int _killCount = 0;
    public int KillCount
    {
        get => _killCount;
        set => _killCount = value;
    }
    private int _moveCount = 0;
    public int MoveCount
    {
        get => _moveCount;
        set => _moveCount = value;
    }
    private int _scoreCount = 0;
    public int ScoreCount
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
		tmpMoveCount.text = MOVE_TEXT_PREFIX + _moveCount;
	}

    private void UpdatePlayerKillCount()
	{
		_killCount++;
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
        Invoke("CountTimer", 1.0f);
    }

    IEnumerator WaitUntilUnpause()
    {
        yield return new WaitUntil(() => Time.timeScale == 1.0f);
    }

	#endregion
}
