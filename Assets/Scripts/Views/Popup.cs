using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class Popup : MonoBehaviour
{
    public enum ANIMATE
    {
        TRANSPARENT,
        GO_UP_THEN_DISAPPEAR,
        CHANGE_TEXT,
        TEXT_WRITING,
        WOBBLE
    }
    private static Popup popup;
    public static Popup Instance => popup;

    [SerializeField] private TMP_Text tmpMessage;
    [SerializeField] private Image imgBG;
    bool runAnim = false;
    //bool needEraseText = false;
    Color tempColor;
    float animTime, timer = 0f, countdown = 0f; //timerEraseText = 0f, timeErase;
    ANIMATE currentAnim;
    Vector2 originPos;

    void Awake()
    {
        if (popup == null)
        {
            popup = this;
        }
    }

    void Start()
    {
        tmpMessage = GetComponentInChildren<TMP_Text>();
        originPos = gameObject.GetComponent<RectTransform>().localPosition;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (runAnim)
        {
            timer += Time.unscaledDeltaTime;
            if (timer < animTime)
            {
               checkAndRunAnim();
            }
            else resetAnim();
        }
       
        // if(needEraseText)
        // {
        //     timerEraseText += Time.unscaledDeltaTime;
        //     Debug.Log(timerEraseText);
        //     if (timerEraseText >= timeErase)
        //     {
        //         ErasedText();
        //         timerEraseText = 0f;
        //     }
        // }
    }

    public void showPopup(short returnCode, string message, float timeSecond, ANIMATE anim)
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

    public void showPopup(string message, float timeSecond, ANIMATE anim)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        tmpMessage.text = message;
        currentAnim = anim;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showPopupWithTextWriting(string message, float timeSecond, ANIMATE anim, float timeBtwChars)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        currentAnim = anim;
        //timeErase = timeBtwChars / 5f;
        show();
        runAnim = true;
        showPopupTextWriting(message, timeBtwChars);
    }

    public void showPopupChangeText(string message, float timeSecond)
    {
        resetAnim();
        animTime = timeSecond;
        countdown = animTime;
        tmpMessage.text = message;
        currentAnim = ANIMATE.CHANGE_TEXT;
        this.gameObject.SetActive(true);
        runAnim = true;
    }

    public void showPopupTextWriting(string message, float timeBtwChars)
    {
        StartCoroutine(TypeWriterTMP(message, timeBtwChars));
    }

    private void show()
    {
        if(!this.gameObject.activeSelf) this.gameObject.SetActive(true);
    }

    private void close()
    {
        if(this.gameObject.activeSelf) this.gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
        ResetPosition();
    }

    private void Transparent()
    {
        tempColor = tmpMessage.color;
        tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime/animTime);
        tmpMessage.color = tempColor;
        // tempColor = imgBG.color;
        // tempColor.a = Mathf.Lerp(tempColor.a, 0, Time.unscaledDeltaTime/animTime);
        // imgBG.color = tempColor;
    }

    private void Opaque()
    {
        tempColor = tmpMessage.color;
        tempColor.a = 1.0f;
        tmpMessage.color = tempColor;
        // tempColor = imgBG.color;
        // tempColor.a = 0.7f;
        // imgBG.color = tempColor;
    }

    private void MoveUp()
    {
        this.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(originPos.x,
                    Mathf.Lerp(this.gameObject.GetComponent<RectTransform>().localPosition.y,
                                this.gameObject.GetComponent<RectTransform>().localPosition.y + 100,
                                Time.unscaledDeltaTime / 2));
    }

    IEnumerator TypeWriterTMP(string message, float timeBtwChars)
    {

		foreach (char c in message)
		{
			if (tmpMessage.text.Length > 0)
			{
				tmpMessage.text = tmpMessage.text.Substring(0, tmpMessage.text.Length);
			}
			tmpMessage.text += c;
			yield return new WaitForSecondsRealtime(timeBtwChars);
		}
        //needEraseText = true;
	}

    IEnumerator EraseText(float time)
    {
        foreach (char c in tmpMessage.text)
		{
            if(c != ' ')
            {
                tmpMessage.text = ReplaceAtIndex(tmpMessage.text, tmpMessage.text.IndexOf(c), ' ');
            }
            yield return new WaitForSecondsRealtime(0.001f);
		}
    }

    private string ReplaceAtIndex(string text, int index, char c)
	{
		var stringBuilder = new StringBuilder(text);
		stringBuilder[index] = c;
		return stringBuilder.ToString();
	}

    // private void ErasedText()
    // {
    //     foreach (char c in tmpMessage.text)
	// 	{
    //         if(c != ' ')
    //         {
    //             tmpMessage.text = Utils.ReplaceAtIndex(tmpMessage.text, tmpMessage.text.IndexOf(c), ' ');
    //             break;
    //         }

    //         if(tmpMessage.text.IndexOf(c) == tmpMessage.text.Length - 1) needEraseText = false;
	// 	}
    // }

    public void ResetPosition()
    {
        this.gameObject.GetComponent<RectTransform>().localPosition = originPos;
    }

    private void StopAnim()
    {
        timer = 0f;
        close();
        tmpMessage.text = "";
        runAnim = false;
    }

    private void runTransparentAnim()
    {
        Transparent();
    }

    private void runGoUpThenDisappearAnim()
    {
        //Transparent();
        MoveUp();
    }

    private void runChangeTextAnim()
    {
        tmpMessage.text = ((int)(countdown - timer)).ToString();
    }

    private void runWobbleAnim()
    {
        //tmpMessage.gameObject.AddComponent<WordWobble>();
        tmpMessage.GetComponent<WordWobble>().enabled = true;
    }

    private void resetAnim()
    {
        Opaque();
        //ResetPosition();
        tmpMessage.GetComponent<WordWobble>().enabled = false;
        //tmpMessage.gameObject.SetActive(false);
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
            case ANIMATE.WOBBLE:
                runWobbleAnim();
                break;
            default:
                runTransparentAnim();
                break;
        }
    }
}
