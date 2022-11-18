using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private Transform _player;
    private Transform _trans;
    private SpriteRenderer _sprite;
    private IObjectPool<Enemy> _pool;

    void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();
        _trans = this.GetComponent<Transform>();
        _sprite = this.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(_player.position.x > _trans.position.x)
        {
            _sprite.flipX = true;
        }
    }

    public void FlipX()
    {
        if(_player.position.x > _trans.position.x)
        {
            _sprite.flipX = true;
        }
        else _sprite.flipX = false;
    }

    public void SetPool(IObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    public void ReturnPool()
    {
        _pool?.Release(this);
    }
}
