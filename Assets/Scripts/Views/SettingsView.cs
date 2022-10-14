using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;

public class SettingsView : MonoBehaviour
{
    [SerializeField] Button btClose;
    [SerializeField] Button btRank;
    [SerializeField] Button btChangePlayerName;
    [SerializeField] Button btSave;
    [SerializeField] Button btSaveChangePlayerName;
    [SerializeField] TMP_Text tmpPlayerName;
    [SerializeField] TMP_Text tmpFPS;
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderSound;
    [SerializeField] Slider sliderFPS;
    [SerializeField] GameObject dialogChangePlayerName;
    [SerializeField] TMP_InputField ipChangePlayerName;
    [SerializeField] Button btCloseDialogChangePlayerName;

    string playerName;
    const string comingSoon = "Coming Soon !!!";
    // Start is called before the first frame update
    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        if(string.IsNullOrEmpty(playerName)) playerName = SystemInfo.deviceName;
        if(playerName.Length > 12) playerName = playerName.Substring(0,12);
        tmpPlayerName.text = playerName;
        sliderFPS.minValue = 5;
        sliderFPS.maxValue = 120;
        sliderFPS.value = Application.targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        btRank.onClick.AddListener(OnClickButtonRank);
        btClose.onClick.AddListener(OnClickButtonClose);
        btChangePlayerName.onClick.AddListener(OnClickButtonChangePlayerName);
        btSave.onClick.AddListener(OnClickButtonSave);
        sliderMusic.onValueChanged.AddListener(delegate {OnMusicValueChange();});
        sliderSound.onValueChanged.AddListener(delegate {OnSoundValueChange();});
        sliderFPS.onValueChanged.AddListener(delegate {OnFPSValueChange();});
    }

    private void OnDisable()
    {
        btRank.onClick.RemoveAllListeners();
        btClose.onClick.RemoveAllListeners();
        btChangePlayerName.onClick.RemoveAllListeners();
        btSave.onClick.RemoveAllListeners();
        sliderSound.onValueChanged.RemoveAllListeners();
    }

    private void OnClickButtonClose()
    {
        DialogManager.Instance.CloseDialog(this.gameObject);
    }

    private void OnClickButtonRank()
    {
        DialogManager.Instance.ShowDialog("DialogLeaderboard");
    }

    private void OnClickButtonChangePlayerName()
    {
        btCloseDialogChangePlayerName.onClick.AddListener(OnClickButtonCloseDialogChangePlayerName);
        btSaveChangePlayerName.onClick.AddListener(OnClickButtonSaveChangePlayerName);
        ipChangePlayerName.text = "";
        dialogChangePlayerName.SetActive(true);
    }

    private void OnClickButtonSaveChangePlayerName()
    {
        if(Utils.checkLengthString(1, 14, ipChangePlayerName.text))
        {
            StartCoroutine(SetPlayerName());
        }
        else
        {
            Toast.Instance.showToast("Player name must be from 1 to 14 characters", 5.0f, Toast.ANIMATE.GO_UP_THEN_DISAPPEAR);
            ipChangePlayerName.text = "";
        }
    }

    IEnumerator SetPlayerName()
    {
        bool isDone = false;
        LootLockerSDKManager.SetPlayerName(playerName, (response) =>
        {
            if (response.success)
            {
                PlayerPrefs.SetString("PlayerName", ipChangePlayerName.text);
                tmpPlayerName.text = ipChangePlayerName.text;
                Toast.Instance.showToast("Success Change Name", 5.0f, Toast.ANIMATE.TRANSPARENT);
                CloseDialogChangePlayerName();
                isDone = true;
            }
            else
            {
                Toast.Instance.showToast(response.Error, 5.0f, Toast.ANIMATE.GO_UP_THEN_DISAPPEAR);
                isDone = true;
            }
        });
        yield return new WaitUntil(() => isDone);
    }

    private void CloseDialogChangePlayerName()
    {
        btSaveChangePlayerName.onClick.RemoveAllListeners();
        dialogChangePlayerName.SetActive(false);
    }

    private void OnClickButtonCloseDialogChangePlayerName()
    {
        CloseDialogChangePlayerName();
    }

    private void OnClickButtonSave()
    {
       #if !UNITY_WEBGL
       Toast.Instance.showToast(0, comingSoon, 2.0f, Toast.ANIMATE.GO_UP_THEN_DISAPPEAR);
       #endif
    }

    private void OnMusicValueChange()
    {
        SoundsManager.Instance.AudioSource.volume = sliderMusic.value;
    }

    private void OnSoundValueChange()
    {
        Player.Instance.AudioSource.volume = sliderSound.value;
    }

    private void OnFPSValueChange()
    {
        Application.targetFrameRate = (int)sliderFPS.value;
        tmpFPS.text = "FPS " + Application.targetFrameRate.ToString();
    }
}
