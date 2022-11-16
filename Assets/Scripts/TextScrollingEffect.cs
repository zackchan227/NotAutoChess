using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextScrollingEffect : MonoBehaviour
{

    public float scrollSpeed;
    private RectTransform textRectTransform;

    // Use this for initialization
    void Awake()
    {
        textRectTransform = GetComponent<RectTransform>();
        // cloneText = Instantiate(text) as TMP_Text;
        // RectTransform cloneRectTransform = cloneText.GetComponent<RectTransform>();
        // cloneRectTransform.SetParent(mask);
        // cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
        // cloneRectTransform.localPosition = new Vector3(text.preferredWidth, 0, cloneRectTransform.position.z);
        // cloneRectTransform.localScale = new Vector3(1, 1, 1);
        // cloneText.text = text.text;
    }

    void Start()
    {
        StartCoroutine(MoveLeft());
    }

    // private IEnumerator Start()
    // {
    //     float width = text.preferredWidth;
    //     Vector3 startPosition = textRectTransform.localPosition;

    //     float scrollPosition = 0;

    //     while (true)
    //     {
    //         textRectTransform.localPosition = new Vector3(-scrollPosition % width, startPosition.y, startPosition.z);
    //         scrollPosition += scrollSpeed * 20 * Time.deltaTime;
    //         //if(textRectTransform.off)
    //         yield return null;
    //     }
    // }

    private void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    private void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(right, rt.offsetMax.y);
    }

    IEnumerator MoveLeft()
    {
        while(textRectTransform.offsetMin.x >= -315)
        {
            textRectTransform.offsetMin += new Vector2(-scrollSpeed,0);
            textRectTransform.offsetMax -= new Vector2(scrollSpeed,0);
            yield return null;
        }

        if(textRectTransform.offsetMin.x <= -315)
        {
            SetLeft(textRectTransform,250);
            SetRight(textRectTransform,-250);
            StartCoroutine(MoveLeft());
        }
    }
}