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

    private Text txtMessage;
    private TMP_Text tmpMessage;
    private Image imgBG;
    bool runAnim = false;
    Color tempColor;
    float timer = 0f;
    float animTime;
    ANIMATE currentAnim;
    Vector2 originPos;
    float countdown = 0f;

    void Awake()
    {
        if(toast == null)
        {
           toast = this;
        }
        //DontDestroyOnLoad(this);
    }

    void Start()
    {
        txtMessage = GetComponentInChildren<Text>();
        tmpMessage = GetComponent<TMP_Text>();
        imgBG = GetComponent<Image>();
        originPos = gameObject.GetComponent<RectTransform>().localPosition;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(!runAnim) return;
        timer += Time.unscaledDeltaTime;
        switch(currentAnim)
        {
            case ANIMATE.TRANSPARENT:
                if (timer < animTime) runTransparentAnim();
                else resetAnim();
                break;
            case ANIMATE.GO_UP_THEN_DISAPPEAR:
                if (timer < animTime) runGoUpThenDisappearAnim();
                else resetAnim();
                break;
            case ANIMATE.CHANGE_TEXT:
                if (timer < animTime) runChangeTextAnim();
                else resetAnim();
                break;
            default:
                if (timer < animTime) runTransparentAnim();
                else resetAnim();
                break;
        }
    }

    public void showToast(short returnCode, string message, float timeSecond, ANIMATE anim)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        if(returnCode  == 0) txtMessage.text = message;
        else txtMessage.text = "[" + returnCode + "] : " + message;
        switch(currentAnim)
        {
            case ANIMATE.TRANSPARENT:
                currentAnim = anim;
                break;
            case ANIMATE.GO_UP_THEN_DISAPPEAR:
                currentAnim = anim;
                break;
            case ANIMATE.CHANGE_TEXT:
                currentAnim = anim;
                break;
            default:
                currentAnim = anim;
                break;
        }
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showToast(string message, float timeSecond, ANIMATE anim)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        txtMessage.text = message;
        switch(currentAnim)
        {
            case ANIMATE.TRANSPARENT:
                currentAnim = anim;
                break;
            case ANIMATE.GO_UP_THEN_DISAPPEAR:
                currentAnim = anim;
                break;
            case ANIMATE.CHANGE_TEXT:
                currentAnim = anim;
                break;
            default:
                currentAnim = anim;
                break;
        }
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showToast(short returnCode, string message, float timeSecond)
    {
        resetAnim();
        animTime = timeSecond;
        if(returnCode  == 0) txtMessage.text = message;
        else txtMessage.text = "[" + returnCode + "] : " + message;
        currentAnim = ANIMATE.TRANSPARENT;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    private void runTransparentAnim()
    {
        tempColor = txtMessage.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime);
        txtMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime);
        imgBG.color = tempColor;   
    }

    private void stopTransparentAnim()
    {
        tempColor = txtMessage.color;
        tempColor.a = 1;
        txtMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = 1;
        imgBG.color = tempColor;
        timer = 0f;
        this.gameObject.SetActive(false);
        runAnim = false;
    }

    private void runGoUpThenDisappearAnim()
    {
        tempColor = txtMessage.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime);
        txtMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime);
        imgBG.color = tempColor;  
        this.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(originPos.x,Mathf.Lerp(this.gameObject.GetComponent<RectTransform>().localPosition.y, this.gameObject.GetComponent<RectTransform>().localPosition.y + 50, Time.unscaledDeltaTime / animTime));
    }

    private void stopGoUpThenDisappearAnim()
    {
        tempColor = txtMessage.color;
        tempColor.a = 1;
        txtMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = 1;
        imgBG.color = tempColor;
        this.gameObject.GetComponent<RectTransform>().localPosition = originPos;
        timer = 0f;
        this.gameObject.SetActive(false);
        runAnim = false;
    }

    private void runChangeTextAnim()
    {
        txtMessage.text = ((int)(countdown - timer)).ToString();
    }

    private void resetAnim()
    {
        tempColor = txtMessage.color;
        tempColor.a = 1;
        txtMessage.color = tempColor;
        tempColor = imgBG.color;
        tempColor.a = 1;
        imgBG.color = tempColor;
        this.gameObject.GetComponent<RectTransform>().localPosition = originPos;
        timer = 0f;
        this.gameObject.SetActive(false);
        runAnim = false;
    }
}
