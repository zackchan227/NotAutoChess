using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toast : MonoBehaviour
{
    public enum ANIMATE
    {
        TRANSPARENT,
        GO_UP_THEN_DISAPPEAR,
        CHANGE_TEXT
    }
    private static Toast toast;
    public static Toast Instance => toast;

    [SerializeField] private TMP_Text tmpMessage;
    [SerializeField] private Image imgBG, imgFrame;
    bool runAnim = false;
    Color tempColor;
    float animTime, timer = 0f, countdown = 0f;
    ANIMATE currentAnim;
    Vector2 originPos;

    void Awake()
    {
        if (toast == null)
        {
            toast = this;
        }
    }

    void Start()
    {
        originPos = gameObject.GetComponent<RectTransform>().localPosition;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!runAnim) return;
        timer += Time.unscaledDeltaTime;
        if (timer < animTime)
        {
            checkAndRunAnim();
        }
        else resetAnim();
    }

    public void showToast(short returnCode, string message, float timeSecond, ANIMATE anim)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        if (returnCode == 0) tmpMessage.text = message;
        else tmpMessage.text = "[" + returnCode + "] : " + message;
        currentAnim = anim;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showToast(string message, float timeSecond, ANIMATE anim)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        tmpMessage.text = message;
        currentAnim = anim;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showToastChangeText(string message, float timeSecond)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        tmpMessage.text = message;
        currentAnim = ANIMATE.CHANGE_TEXT;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    private void Transparent()
    {
        tempColor = tmpMessage.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime/animTime);
        tmpMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime/animTime);
        imgBG.color = tempColor;
        tempColor = imgFrame.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime/animTime);
        imgFrame.color = tempColor;
    }

    private void Opaque()
    {
        tempColor = tmpMessage.color;
        tempColor.a = 1;
        tmpMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = 1;
        imgBG.color = tempColor;
        tempColor = imgFrame.color;
        tempColor.a = 1;
        imgFrame.color = tempColor;
    }

    private void MoveUp()
    {
        this.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(originPos.x,
                    Mathf.Lerp(this.gameObject.GetComponent<RectTransform>().localPosition.y,
                                this.gameObject.GetComponent<RectTransform>().localPosition.y + 50,
                                Time.unscaledDeltaTime / animTime));
    }

    private void ResetPosition()
    {
        this.gameObject.GetComponent<RectTransform>().localPosition = originPos;
    }

    private void StopAnim()
    {
        timer = 0f;
        this.gameObject.SetActive(false);
        runAnim = false;
    }

    private void runTransparentAnim()
    {
        Transparent();
    }

    private void runGoUpThenDisappearAnim()
    {
        Transparent();
        MoveUp();
    }

    private void runChangeTextAnim()
    {
        tmpMessage.text = ((int)(countdown - timer)).ToString();
    }

    private void resetAnim()
    {
        Opaque();
        ResetPosition();
        StopAnim();
    }

    private void checkAndRunAnim()
    {
        switch (currentAnim)
        {
            case ANIMATE.TRANSPARENT:
                runTransparentAnim();
                break;
            case ANIMATE.GO_UP_THEN_DISAPPEAR:
                runGoUpThenDisappearAnim();
                break;
            case ANIMATE.CHANGE_TEXT:
                runChangeTextAnim();
                break;
            default:
                runTransparentAnim();
                break;
        }
    }
}
