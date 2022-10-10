using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform _player;
    private Transform _trans;
    private SpriteRenderer _sprite;

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

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.isPausing) return;
        if(_player.position.x > _trans.position.x)
        {
            _sprite.flipX = true;
        }
        else _sprite.flipX = false;
    }
}
