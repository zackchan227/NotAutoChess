using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event;
using Coffee.UIExtensions;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private Sprite[] _idles;
    [SerializeField] private Sprite[] _idles1;
    [SerializeField] private Sprite[] _runs;
    [SerializeField] private Sprite[] _attacks1;
    [SerializeField] private Sprite[] _attacks2;
    [SerializeField] private Sprite[] _attacks3;
    [SerializeField] private Sprite[] _attacks4;
    [SerializeField] private Sprite[] _attacks5;
    [SerializeField] private float _animTime = 0.5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private byte _moveSpeed = 5;
    [SerializeField] private ParticleSystem _runEffect, _onClickWrongParticleEffectPrefab, _onClickRightParticleEffectPrefab, _hitEffectPrefab, _bloodSplashEffectPrefab;
    [SerializeField] private UIParticle _onClickEffectPrefab;
    private AudioClip _attackSound, _hitSound;
    private SpriteRenderer _currentSprite;
    private Tile currentStanding;
    //private bool isAuto = false;
    private bool isAttacking = false;
    private bool isMoving = false;
    private byte _currentFrame = 0;
    private float _timer = 0f;
    public Transform _parentTransform;
    private Tile _hitTile;
    private List<Vector2> _moveableTile = new List<Vector2>();
    private byte _killStreak = 0;
    private List<Sprite> _currentAnim = new List<Sprite>();
    ParticleSystem _onClickWrongParticleEffect, _onClickRightParticleEffect, _hitEffect;
    UIParticle _onClickEffect;
    byte _countIdle = 0;
    byte _countIdleTarget = 0;

    private Transform _canvas, _gameScreen;
    public AudioSource AudioSource
    {
        get { return audioSource; }
        set { audioSource = value; }
    }

    bool hasFirstBlood = false;

    void Awake()
    {
        Instance = this;
        _currentSprite = this.GetComponent<SpriteRenderer>();
        _parentTransform = this.transform.parent;
        _countIdleTarget = (byte)UnityEngine.Random.Range(5, 10);
        checkIdle();
        _canvas = GameObject.Find("Canvas").transform;
        _gameScreen = GameObject.Find("GameScreen").transform;
        _onClickEffect = Instantiate(_onClickEffectPrefab, _canvas);
        _onClickEffect.gameObject.AddComponent<Canvas>();
        _onClickEffect.GetComponent<Canvas>().overrideSorting = true;
        _onClickEffect.GetComponent<Canvas>().sortingOrder = 2;
        _bloodSplashEffectPrefab.name = "BloodFX";
        _hitEffectPrefab.name = "HitFX";
        ParticleSystem.Instantiate(_bloodSplashEffectPrefab, _gameScreen);
        ParticleSystem.Instantiate(_hitEffectPrefab, _gameScreen);
        _moveSpeed = GameManager.Instance.gameConfig.playerMoveSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("randomMove", 1.0f);
    }

    // void OnDisable()
    // {
    //     this.RemoveAllListener();
    // }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPausing) return;
        _timer += Time.deltaTime;
        if (_timer > _animTime)
        {
            checkWhileAttacking();
            doAnimation();
            _timer = 0;
        }
        if (Input.touchCount < 2) onClickMove();
    }

    private void randomMove()
    {
        if (GameManager.Instance.isLost) return;
        List<Vector2> tiles = GridManager.Instance.GetAllTiles();
        Vector2 nextMovePos = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        Tile nextMoveTile = GridManager.Instance.GetTileAtPosition(nextMovePos);
        currentStanding = GridManager.Instance.GetTileAtPosition(this.transform.position);
        currentStanding.RemoveUnit();

        if (nextMoveTile.StandingUnit != null)
        {
            Destroy(nextMoveTile.StandingUnit);
            nextMoveTile.SetUnit(this.gameObject);
            this.PostEvent(EventID.OnEnemyDecrease);
            this.PostEvent(EventID.OnPlayerKill);
        }
        this.transform.position = nextMoveTile.transform.position;
        this.PostEvent(EventID.OnPlayerMove);
        UnitManager.Instance.SpawnDefenderRandomPos();
        Invoke("randomMove", 1.0f);
    }

    private bool checkMoveable(Vector2 targetV)
    {
        foreach (Vector2 v in _moveableTile)
        {
            if (v == targetV) return true;
        }
        return false;
    }

    IEnumerator moveToTargetAndReplace()
    {
        if (_parentTransform.position.x < _hitTile.transform.position.x)
        {
            _currentSprite.flipX = false;
        }
        else _currentSprite.flipX = true;
        isMoving = true;
        checkIsMoving();
        //Vector3 velocity = Vector3.zero;
        Vector3 tempHitPosition = new Vector3(_hitTile.transform.position.x, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f);
        while (_parentTransform.position != tempHitPosition)
        {
            //_parentTransform.position = Vector3.SmoothDamp(_parentTransform.position, _hitTile.transform.position, ref velocity, 0.1f);
            _parentTransform.position = Vector3.MoveTowards(_parentTransform.position, tempHitPosition, Time.deltaTime * _moveSpeed);
            
            yield return null;
        }
        _parentTransform.position = new Vector3(tempHitPosition.x, tempHitPosition.y, tempHitPosition.y);
        isMoving = false;
        checkIsMoving();
        _hitTile.SetUnit(this.gameObject);
        resetMoveableHighlight();
        GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
        GameManager.Instance.ChangeState(GameManager.GameState.DefenderTurn);
    }

    IEnumerator moveToTarget(Vector3 target)
    {
        checkFlipX(target);
        isMoving = true;
        checkIsMoving();
        //Vector3 velocity = Vector3.zero;
        while (_parentTransform.position != target)
        {
            //_parentTransform.position = Vector3.SmoothDamp(_parentTransform.position, _hitTile.transform.position, ref velocity, _moveSpeed * Time.deltaTime);
            _parentTransform.position = Vector3.MoveTowards(_parentTransform.position, target, Time.deltaTime * _moveSpeed);
            yield return null;
        }
        _parentTransform.position = new Vector3(target.x, target.y, target.y - 0.01f);
        isMoving = false;
        checkFlipX(_hitTile.transform.position);
        checkIsMoving();
        checkKillStreak();
        resetMoveableHighlight();
        isAttacking = true;
        _currentFrame = 0;
        GameManager.Instance.ChangeState(GameManager.GameState.DefenderTurn);
    }

    private void checkFlipX(Vector3 target)
    {
        if (_parentTransform.position.x < target.x)
        {
            _currentSprite.flipX = false;
        }
        else if (_parentTransform.position.x > target.x) 
        {
            _currentSprite.flipX = true;
        }
    }

    private void resetMoveableHighlight()
    {
        foreach (Vector2 v in _moveableTile)
        {
            if (v != null)
            {
                Tile _t = GridManager.Instance.GetTileAtPosition(v);
                if (_t != null) _t.Unmoveable();
            }
        }
    }

    private void displayMoveableTile(List<Vector2> moves)
    {
        _moveableTile = new List<Vector2>();
        _moveableTile.AddRange(moves);
        foreach (Vector2 v in _moveableTile)
        {
            if (v != null)
            {
                Tile _t = GridManager.Instance.GetTileAtPosition(v);
                if (_t != null) _t.Moveable();
            }
        }
    }

    public void Move(byte type, List<Vector2> moves)
    {
        displayMoveableTile(moves);
    }

    private void onClickMove()
    {
        if (!GameManager.Instance.isLost && !isAttacking && !isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {


                //Converting Mouse Pos to 2D (vector2) World Pos
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit)
                {
                    //_onClickEffect.gameObject.SetActive(true);
                    //StartCoroutine(disableClickEffect());

                    _hitTile = null;
                    _hitTile = hit.transform.gameObject.GetComponent<Tile>();

                    if (_hitTile != null)
                    {
                        if (_hitTile.transform.position == _parentTransform.position)
                        {
                            _onClickEffect.transform.position = Input.mousePosition;
                            _onClickEffect.Play();
                            return;
                        }

                        if (checkMoveable(hit.transform.position))
                        {
                            currentStanding = GridManager.Instance.GetTileAtPosition(_parentTransform.position);
                            if (currentStanding != null)
                            {
                                currentStanding.RemoveUnit();
                                if (_hitTile.StandingUnit != null)
                                {
                                    _killStreak++;
                                    if (_parentTransform.position.x < _hitTile.transform.position.x)
                                    {
                                        StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x - 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
                                    }
                                    else if (_parentTransform.position.x > _hitTile.transform.position.x)
                                    {
                                        StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x + 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
                                    }
                                    else
                                    {
                                        if(_parentTransform.position.x < 0)
                                        {
                                            StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x + 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
                                        }
                                        else
                                        {
                                            StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x - 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
                                        }
                                    }
                                }
                                else
                                {
                                    _killStreak = 0;
                                    //_parentTransform.position = _hitTile.transform.position;
                                    StartCoroutine(moveToTargetAndReplace());
                                    _hitTile.SetUnit(this.gameObject);
                                    GameManager.Instance.ChangeState(GameManager.GameState.SpawnDefender);
                                    resetMoveableHighlight();
                                    //GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
                                }
                            }
                            if (_onClickRightParticleEffect == null)
                            {
                                _onClickRightParticleEffect = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particle/ClickCorrectEffect"), hit.transform.position, Quaternion.identity);
                            }
                            else
                            {
                                _onClickRightParticleEffect.transform.position = hit.transform.position;
                            }
                            _onClickRightParticleEffect.Play();
                            this.PostEvent(EventID.OnPlayerMove);
                        }
                        else
                        {
                            if (_onClickWrongParticleEffect == null)
                            {
                                _onClickWrongParticleEffect = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particle/ClickXEffect"), hit.transform.position, Quaternion.identity);
                            }
                            else
                            {
                                _onClickWrongParticleEffect.transform.position = hit.transform.position;
                            }
                            _onClickWrongParticleEffect.Play();
                        }
                    }
                }
                else
                {
                    _onClickEffect.transform.position = Input.mousePosition;
                    _onClickEffect.Play();
                }
            }
        }
    }

    // IEnumerator disableClickEffect()
    // {
    //     while(!_onClickEffect.isStopped) 
    //     {
    //         yield return null;
    //     }
    //     _onClickEffect.gameObject.SetActive(false);    
    // }

    private void checkKillStreak()
    {
        _currentAnim.Clear();
        _hitSound = _clips[0];
        _hitEffect = _gameScreen.Find("BloodFX(Clone)").GetComponent<ParticleSystem>();
        if (_attackSound == null) _attackSound = _clips[1];
        switch (_killStreak)
        {
            case 1:
                _currentAnim.AddRange(_attacks1);
                break;
            case 2:
                _currentAnim.AddRange(_attacks2);
                break;
            case 3:
                _hitSound = _clips[2];
                _attackSound = null;
                _hitEffect = _gameScreen.Find("HitFX(Clone)").GetComponent<ParticleSystem>();
                _currentAnim.AddRange(_attacks3);
                break;
            case 4:
                _currentAnim.AddRange(_attacks4);
                break;
            case 5:
                _currentAnim.AddRange(_attacks5);
                break;
            default:
                _currentAnim.AddRange(_attacks5);
                break;
        }
    }

    private void checkIsMoving()
    {
        _currentAnim.Clear();
        if (isMoving)
        {
            _currentAnim.AddRange(_runs);
            _runEffect.Play();
        }
        else
        {
            _runEffect.Stop();
            checkIdle();
        }
    }

    private void checkWhileAttacking()
    {
        if (isAttacking)
        {
            if (_killStreak < 5)
            {
                if(_killStreak != 3)
                {
                    if (_currentFrame == 2)
                    {
                        if (_hitEffect != null)
                        {
                            _hitEffect.transform.position = _hitTile.transform.position;
                            _hitEffect.Play();
                        }
                    }
                }
                else
                {
                    if (_currentFrame == 0)
                    {
                        if(_attackSound != null) audioSource.PlayOneShot(_attackSound);
                    }
                }
            }
            else
            {
                // sound FX and blood splash FX when _killStreak = 5
                switch (_currentFrame)
                {
                    case 1:
                    case 6:
                    case 12:
                    case 20:
                        audioSource.PlayOneShot(_attackSound);
                        break;
                    case 2:
                    case 7:
                    case 13:
                    case 21:
                        _hitEffect.transform.position = _hitTile.transform.position;
                        _hitEffect.Play();
                        break;
                    default:
                        break;
                }
                // if (_currentFrame == 1)
                // {
                //     audioSource.PlayOneShot(_attackSound);
                // }
                // if (_currentFrame == 2)
                // {
                //     _hitEffect.transform.position = _hitTile.transform.position;
                //     _hitEffect.Play();
                // }
                // if (_currentFrame == 6) audioSource.PlayOneShot(_attackSound);

                // if (_currentFrame == 7)
                // {
                //     _hitEffect.transform.position = _hitTile.transform.position;
                //     _hitEffect.Play();
                // }
                // if (_currentFrame == 12) audioSource.PlayOneShot(_attackSound);
                // if (_currentFrame == 13)
                // {
                //     _hitEffect.transform.position = _hitTile.transform.position;
                //     _hitEffect.Play();
                // }
                // if (_currentFrame == 20) audioSource.PlayOneShot(_attackSound);
                // if (_currentFrame == 21)
                // {
                //     _hitEffect.transform.position = _hitTile.transform.position;
                //     _hitEffect.Play();
                // }
            }

            // last hit sound FX
            if (_hitSound != null)
            {
                if (_currentFrame == _currentAnim.Count - 3)
                {
                    audioSource.PlayOneShot(_hitSound);
                    if(_killStreak == 3)
                    {
                        _hitEffect.transform.position = _hitTile.transform.position;
                        _hitEffect.Play();
                    }
                }
            }


            // kill target
            if (_currentFrame >= _currentAnim.Count - 1)
            {
                isAttacking = false;
                UnitManager.Instance.RemoveEnemy(_hitTile.StandingUnit);
                this.PostEvent(EventID.OnPlayerKill);
                this.PostEvent(EventID.OnScoreCount, _killStreak);
                if(!hasFirstBlood) 
                {
                    hasFirstBlood = true;
                    this.PostEvent(EventID.OnFirstBlood);
                }
                if(_killStreak > 1) this.PostEvent(EventID.OnKillStreak,_killStreak);
                StartCoroutine(moveToTargetAndReplace());
            }
        }
    }

    private void doAnimation()
    {
        if (_currentFrame >= _currentAnim.Count - 1)
        {
            if(!isMoving && !isAttacking)
            {
                checkIdle();
            }
            _currentFrame = 0;
        }
        else
        {
            _currentFrame++;
        }
        if (_currentSprite != null)
        {
            _currentSprite.sprite = _currentAnim[_currentFrame];
        }
    }

    private void checkIdle()
    {
        _countIdle++;
        //Debug.Log(_countIdle);
        _currentAnim.Clear();
        if(_countIdle == _countIdleTarget)
        {
            _countIdle = 0;
            _countIdleTarget = (byte)UnityEngine.Random.Range(5, 10);
            _currentAnim.AddRange(_idles1);
        }
        else
        {
            _currentAnim.AddRange(_idles);
        }
    }

}
