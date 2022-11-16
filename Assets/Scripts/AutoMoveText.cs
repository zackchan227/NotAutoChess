using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AutoMoveText : MonoBehaviour
{
    private TMP_Text _tmpText;
    private RectTransform _rect;
    private float speed = 1.0f;
    //private Text
    void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        _rect = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    void OnEnable()
    {
        AutoMove();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private void AutoMove()
    {
       StartCoroutine(MoveRight());
    }

    IEnumerator MoveLeft()
    {
        while(_rect.offsetMin.x >= -50)
        {
            _rect.offsetMin += new Vector2(-speed,0);
            _rect.offsetMax -= new Vector2(speed,0);
            yield return null;
        }

        if(_rect.offsetMin.x <= - 50)
        {
            StartCoroutine(MoveRight());
        }
    }

    IEnumerator MoveRight()
    {
        while(_rect.offsetMin.x <= 50)
        {
            _rect.offsetMin += new Vector2(speed,0);
            _rect.offsetMax -= new Vector2(-speed,0);
            yield return null;
        }

        if(_rect.offsetMin.x >= 50)
        {
            StartCoroutine(MoveLeft());
        }
    }

    private void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    private void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
}
