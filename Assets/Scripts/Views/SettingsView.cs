using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsView : MonoBehaviour
{
    [SerializeField] Button btClose;
    [SerializeField] Button btRank;
    [SerializeField] Button btChangePlayerName;
    [SerializeField] Button btSave;
    [SerializeField] TMP_Text tmpPlayerName;
    string playerName;
    // Start is called before the first frame update
    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        if(string.IsNullOrEmpty(playerName)) playerName = SystemInfo.deviceName;
        if(playerName.Length > 12) playerName = playerName.Substring(0,12);
        tmpPlayerName.text = playerName;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        btRank.onClick.AddListener(OnClickButtonRank);
        btClose.onClick.AddListener(OnClickButtonClose);
        // btChangePlayerName.onClick.AddListener(OnClickButtonChangePlayerName);
        // btSave.onClick.AddListener(OnClickButtonSave);
    }

    private void OnDisable()
    {
        btRank.onClick.RemoveAllListeners();
        btClose.onClick.RemoveAllListeners();
        // btChangePlayerName.onClick.RemoveAllListeners();
        // btSave.onClick.RemoveAllListeners();
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
       
    }

    private void OnClickButtonSave()
    {
       
    }

}
