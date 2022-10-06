using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{

    [SerializeField] Sprite[] _sprites;
    [SerializeField] float _animTime = 0.5f;
    private SpriteRenderer _currentSprite;
    private Image _currentImage;
    private byte _currentFrame = 0;
    private float _timer = 0f;
    

    void Awake()
    {
        _currentSprite = this.GetComponent<SpriteRenderer>();
        _currentImage = this.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _animTime)
        {
            if(_currentFrame >= _sprites.Length - 1)
            {
                _currentFrame = 0;
            }
            else
            {
                _currentFrame++;
            }
            if(_currentSprite != null)
            {
                 _currentSprite.sprite = _sprites[_currentFrame];
            }
            else _currentImage.sprite = _sprites[_currentFrame];
            _timer = 0;
        }
    }
}
