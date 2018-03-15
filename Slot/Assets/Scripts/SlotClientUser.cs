// 基本用户信息
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using slotClient;

public class SlotClientUser : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Lines = 0;
        Bet = 20;
        SpinCount = 0;
        UId = 123456;
        Key = 123456;
        TigerNo = 888;
        SeqNo = 666;
        Gold = 0;
        Net = new SlotClientNet();
        Login = false;

        // 启动登录
        if (false == Net.Init("182.92.74.240", 7878))
        {
            Debug.Log("Client init failed!");
        }

        // 初始化Displays
        Displays = new SlotClientDisplays();
        Displays.User = this;

        // 初始化Requests
        Requests = new SlotClientRequests();
        Requests.Net = Net;
        Requests.User = this;

        // 发送快速登录
        Requests.ReqQuickLogin();
	}
    void OnApplicationQuit()
    {
        Net.Close();
    }
	
	// Update is called once per frame
	void Update () {        
        ProtoPacket packet = new ProtoPacket();
        if (Net.RecvTryDequeue(ref packet))
        {
            Displays.Execute(packet);
        }
	}

    public int Lines { get; set; } // payline
    public int Bet { get; set; } // bet gold
    public int UId { get; set; }// user id
    public int SpinCount { get; set; } // spin count
    public int Key { get; set; } // key
    public int TigerNo { get; set; } // tiger
    public int SeqNo { get; set; } // seq
    public int Gold { get; set; } // gold
    public SlotClientNet Net { get; set; } // network
    public bool Login { get; set; } // login status
    public SlotClientRequests Requests { get; set; } // Requests
    public SlotClientDisplays Displays { get; set; } // Displayes
}
