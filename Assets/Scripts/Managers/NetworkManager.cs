using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    WebSocket ws;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ws = new WebSocket("ws://localhost:3000");
        ws.ConnectAsync();
        ws.OnMessage += (sender, e) =>
        {
            //Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
            Debug.Log("Message Received Data : " + e.Data);
            if(e.Data.Equals("test"))
            {
                //GameManager.Instance.ChangeState(GameManager.GameState.SpawnDefender);
                
            }
        };
        ws.OnError += (sender, e) =>
        {
            Debug.Log("Network error : " + "(" + e.Message + ")");
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Network closed : " + e.Reason);
            ws.ConnectAsync();
        };
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Network open : connected");
        };
    }

    public void SaveScore(string metadata)
    {
        if(ws != null) 
        {
            ws.SendAsync(metadata, (completed) => 
            {
                if(completed)
                {
                    Debug.Log("Save score successfully");
                }
                else
                {
                    Debug.Log("Failed to save score");
                }
            });
        }
    }

    public void ReZero()
    {
        if(ws != null) ws.Send("RE ZERO");
    }
}
