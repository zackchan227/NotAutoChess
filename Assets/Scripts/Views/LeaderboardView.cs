using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using LootLocker.Requests;
using PooledScrollList.Data;
using PooledScrollList.Controller;

public class LeaderboardView : PooledDataProvider
{
    const int leaderboardID = 7528;
    int Count = 0;
    [SerializeField] ListView _listView;
    [SerializeField] GameObject _listViewItemPrefab;
    [SerializeField] Button btClose;
    [SerializeField] Button btAdd;
    [SerializeField] TMP_InputField ipCount;
    public PooledScrollRectBase ScrollRectController;
    public class LeaderboardData : ListViewItemData
    {
        // public int rank;
        // public string name;
        // public int score;
        // //long miliseconds;
        // public string date;
        // public LeaderboardData(int r, string n, int s, long m)
        // {
        //     this.rank = r;
        //     this.name = n;
        //     this.score = s;
        //     this.miliseconds = m;
        // }

        public LeaderboardData(int r, string n, int s, string d)
        {
            this._datas = new List<object>();
            this._datas.Add(r);
            this._datas.Add(n);
            this._datas.Add(s);
            this._datas.Add(d);
        }
    }

    private List<LeaderboardData> _listDatas = new List<LeaderboardData>();

    private void OnEnable()
    {
        StartCoroutine(GetLeaderboardData());
        btClose.onClick.AddListener(OnClickButtonClose);
        btAdd.onClick.AddListener(Apply);
    }

    private void OnDisable()
    {
        //_listView.Clear();
        btClose.onClick.RemoveAllListeners();
        btAdd.onClick.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        // get data leaderboard
        //generateListDataTest();
        ipCount.text = Count.ToString();
        btAdd.gameObject.SetActive(false);
        ipCount.gameObject.SetActive(false);
    }

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.T) && Input.GetKeyDown(KeyCode.E) || Input.touchCount > 4)
        {
            btAdd.gameObject.SetActive(true);
            ipCount.gameObject.SetActive(true);
        }
    }

    public override List<PooledData> GetData()
    {
        List<PooledData> data = new List<PooledData>();
        data.AddRange(_listDatas);

        // for(int i = 0; i < _listDatas.Count; i++)
        // {
        //     data.Add(_listDatas[i] as ListViewItemData);
        // }
        return data;
    }

    IEnumerator GetLeaderboardData()
    {
        _listView._content.gameObject.SetActive(false);
        _listView.PlayProgressWheel();
        bool isDone = false;
        _listDatas.Clear();
        int count = 0;
        if(!LootLockerSDKManager.CheckInitialized())
        {
            Toast.Instance.showToast("Loading Leaderboard Failed" + ":" + " " + "No Internet", 5.0f, Toast.ANIMATE.GO_UP_THEN_DISAPPEAR);
            isDone = true;
            Close();
        }
        else
        {
            LootLockerSDKManager.GetScoreList(leaderboardID, 1000, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    _listView.StopProgressWheel();
                    Debug.Log("Successful Receive Leaderboard Datas");
                    for (int i = 0; i < response.items.Length; i++)
                    {
                        string playerName = response.items[i].player.name;
                        if (string.IsNullOrEmpty(playerName))
                        {
                            count++;
                            playerName = "Anonymous " + count;
                        }
                        LeaderboardData data;
                        string[] splitDatas = response.items[i].metadata.Split(',');
                        if(splitDatas.Length == 0)
                        {
                            data = new LeaderboardData(response.items[i].rank, playerName, response.items[i].score, response.items[i].metadata);
                        }
                        else data = new LeaderboardData(response.items[i].rank, playerName, response.items[i].score, splitDatas[splitDatas.Length-1]);
                        _listDatas.Add(data);
                    }
                    isDone = true;
                    ScrollRectController.Initialize(GetData());
                    _listView._content.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                    Toast.Instance.showToast("Loading Leaderboard Failed: " + response.Error, 5.0f, Toast.ANIMATE.TRANSPARENT);
                    isDone = true;
                }
            });
        }
        yield return new WaitUntil(() => isDone);
    }

    IEnumerator WaitForSecond(float seconds, int index)
    {
        yield return new WaitForSeconds(seconds);
        //updateUI(OnGetLeaderboardItemView(index));
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    private void generateListDataTest()
    {
        for (int i = 0; i < Count; i++)
        {
            LeaderboardData data = new LeaderboardData(i+1,"Another " + (i + 1), UnityEngine.Random.Range(99999, 999999), DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss"));
            _listDatas.Add(data);
        }
    }

    public void Apply()
    {
        if (ipCount != null && !string.IsNullOrEmpty(ipCount.text))
        {
            if (int.TryParse(ipCount.text, out int result))
            {
                Count = result;
            }
        }

        generateListDataTest();

        var data = GetData();

        if (ScrollRectController != null)
        {
            ScrollRectController.Initialize(data);
        }
    }

    private void updateLeaderboard()
    {
        var data = GetData();
        if (ScrollRectController != null)
        {
            ScrollRectController.Initialize(data);
        }
    }

    // private void updateUI(ListViewItem item)
    // {
    //     if (_listDatas.Count > 0)
    //     {
    //         _listView.SetItem(item);
    //     }
    // }

    // private void OnGetLeaderboardItemView(int index)
    // {
    //     LeaderboardData data = _listDatas[index];
    //     ListViewItem item = _listView.GetPooledItem();
    //     item.GetReferenceData(0).GetComponent<TMP_Text>().text = data.rank.ToString();
    //     item.GetReferenceData(1).GetComponent<TMP_Text>().text = data.name;
    //     item.GetReferenceData(2).GetComponent<TMP_Text>().text = data.score.ToString();
    //     item.GetReferenceData(3).GetComponent<TMP_Text>().text = data.date;

    //     item.gameObject.SetActive(true);
    // }

    private void Close()
    {
        DialogManager.Instance.CloseDialog(this.gameObject);
    }

    private void OnClickButtonClose()
    {
       Close();
    }
}
