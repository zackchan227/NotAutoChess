using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private Sprite[] _idles;
    [SerializeField] private Sprite[] _runs;
    [SerializeField] private Sprite[] _attacks1;
    [SerializeField] private Sprite[] _attacks2;
    [SerializeField] private Sprite[] _attacks3;
    [SerializeField] private Sprite[] _attacks4;
    [SerializeField] float _animTime = 0.5f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] _clips;
    [SerializeField] private byte _moveSpeed = 0;
    private AudioClip _attackEffect, _hitEffect;
    private SpriteRenderer _currentSprite;
    private Tile currentStanding;
    private bool isAuto = false;
    private bool isAttacking = false;
    private bool isMoving = false;
    private byte _currentFrame = 0;
    private float _timer = 0f;
    public Transform _parentTransform;
    private Tile _hitTile;
    private List<Vector2> _moveableTile = new List<Vector2>();
    private byte _killStreak = 0;
    private List<Sprite> _currentAnim = new List<Sprite>();
    private ParticleSystem _onClickParticleEffect, _bloodParticleEffect;

    void Awake()
    {
        Instance = this;
        _currentSprite = this.GetComponent<SpriteRenderer>();
        _parentTransform = this.transform.parent;
        _moveSpeed = 5;
        _currentAnim.AddRange(_idles);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("randomMove", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.isPausing) return;
        _timer += Time.deltaTime;
        if (_timer > _animTime)
        {
            checkWhileAttacking();
            doAnimation();
            _timer = 0;
        }
        if(Input.touchCount < 2) onClickMove();
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
    }

    IEnumerator moveToTarget(Vector3 target)
    {
        if (_parentTransform.position.x <= target.x)
        {
            _currentSprite.flipX = false;
        }
        else _currentSprite.flipX = true;
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
        checkIsMoving();
        isAttacking = true;
        checkKillStreak();
        _currentFrame = 0;
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
                    if(_onClickParticleEffect == null)
                    {
                        _onClickParticleEffect = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particle/ClickParticleEffect"), hit.transform.position, Quaternion.identity);
                    }

                    if(_onClickParticleEffect != null) 
                    {
                        _onClickParticleEffect.transform.position = hit.transform.position;
                        _onClickParticleEffect.Play();
                    }
                    
                    //_onClickParticleEffect.gameObject.SetActive(true);
                    //StartCoroutine(disableClickEffect());

                    _hitTile = hit.transform.gameObject.GetComponent<Tile>();
                    if (_hitTile.transform.position == _parentTransform.position) return;
                    if (_hitTile != null)
                    {
                        if (checkMoveable(hit.transform.position))
                        {
                            currentStanding = GridManager.Instance.GetTileAtPosition(_parentTransform.position);
                            if (currentStanding != null)
                            {
                                currentStanding.RemoveUnit();
                                if (_hitTile.StandingUnit != null)
                                {
                                    _killStreak++;
                                    if (_parentTransform.position.x <= _hitTile.transform.position.x + 0.75f)
                                    {
                                        StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x - 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
                                    }
                                    else
                                    {
                                        StartCoroutine(moveToTarget(new Vector3(_hitTile.transform.position.x + 0.75f, _hitTile.transform.position.y, _hitTile.transform.position.y - 0.01f)));
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
                                    GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
                                }
                            }
                            this.PostEvent(EventID.OnPlayerMove);
                        }
                    }
                }
            }
        }
    }

    IEnumerator disableClickEffect()
    {
        while(!_onClickParticleEffect.isStopped) 
        {
            yield return null;
        }
        _onClickParticleEffect.gameObject.SetActive(false);    
    }

    private void checkKillStreak()
    {
        _currentAnim.Clear();
        _hitEffect = _clips[0];
        _attackEffect = _clips[1];
        switch (_killStreak)
        {
            case 1:
                _currentAnim.AddRange(_attacks1);
                break;
            case 2:
                _currentAnim.AddRange(_attacks2);
                break;
            case 3:
                _hitEffect = _clips[2];
                _attackEffect = null;
                _currentAnim.AddRange(_attacks3);
                break;
            case 4:
                _currentAnim.AddRange(_attacks4);
                break;
            case 5:
                _currentAnim.AddRange(_attacks4);
                break;
            default:
                _currentAnim.AddRange(_attacks3);
                break;
        }
    }

    private void checkIsMoving()
    {
        _currentAnim.Clear();
        if (isMoving) _currentAnim.AddRange(_runs);
        else _currentAnim.AddRange(_idles);
    }

    private void checkWhileAttacking()
    {
        if (isAttacking)
        {
            if(_attackEffect != null)
            {
                if (_currentFrame == 0) 
                {
                    audioSource.PlayOneShot(_attackEffect);
                }
            }

            if (_currentFrame == 2)
            {
                if (_bloodParticleEffect == null)
                {
                    _bloodParticleEffect = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particle/BloodSplash"), _hitTile.transform.position, Quaternion.identity);
                }

                if (_bloodParticleEffect != null)
                {
                    _bloodParticleEffect.transform.position = _hitTile.transform.position;
                    _bloodParticleEffect.Play();
                }
            }

            if(_hitEffect != null)
            {
                if (_currentFrame == _currentAnim.Count - 3) 
                {
                    audioSource.PlayOneShot(_hitEffect);
                }
            }

            
            if (_currentFrame >= _currentAnim.Count - 1)
            {
                isAttacking = false;
                //_currentFrame = 0;
                //_parentTransform.position = _hitTile.transform.position;
                UnitManager.Instance.RemoveEnemy(_hitTile.StandingUnit);
                Destroy(_hitTile.StandingUnit);
                this.PostEvent(EventID.OnPlayerKill);
                this.PostEvent(EventID.OnScoreCount, _killStreak);
                StartCoroutine(moveToTargetAndReplace());
            }
        }
    }

    private void doAnimation()
    {
        if (_currentFrame >= _currentAnim.Count - 1)
        {
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

}
