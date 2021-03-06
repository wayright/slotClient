﻿// 老虎机柜员类
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Login.Proto;
using Tiger.Proto;
using Lion.Proto;
using Common.Proto;

public class SlotClerk : MonoBehaviour {

    private long m_gold = 0; // 总金额
    private int m_bet = 20; // 下注数
    private int m_lines = 0; // 下注线
    private int m_spinCount = 0; // 摇次数
    private long m_id = 0; // UID
    private long m_key = 0; // Key
    private int m_tigerNo; // 机器编号
    private int m_seqNo; // 前端序号
    private bool m_login; // 是否登录
    private long m_win; // 中奖金币
    private ProtoNet m_net; // 网络
    private SlotDisplays m_displays; // 展示器
    private SlotRequests m_requests; // 请求器
    private bool m_spinning = false; // 是否正在摇
    private bool m_autoSpin = false; // 自动摇奖
    //private int m_escapeTimes = 1; // 退出
    private string m_broadcastMsg = "";
    public Dictionary<int, int> CallbackDict = new Dictionary<int,int>();
    private System.Diagnostics.Stopwatch m_stopWatch = new System.Diagnostics.Stopwatch();

	// Use this for initialization
	void Start () {
        m_spinCount = 0;
        m_tigerNo = 888;
        m_seqNo = 666;
        m_gold = 0;
        
        m_net = new ProtoNet();

        // 增加前台支持的网络包类型
        m_net.Add(Constants.Tiger_QuickLoginInfo, TigerUserInfo.Parser);
        m_net.Add(Constants.Tiger_Spin, TigerResp.Parser);
        m_net.Add(Constants.Reconnect, null);
        m_net.Add(Constants.Error, null);
        m_net.Name = "SlotClerk";

        m_login = false;
        m_lines = 1;
        m_win = 0;
        
        // 启动登录
        RedirectResp rr = Lobby.getInstance().RedirectInfo;
        m_id = rr.UserId;
        m_key = rr.Key;
        if (false == m_net.Init(rr.Domain, rr.Port))
        {
            DebugConsole.Log("Client init failed!");
        }
        /* 
        m_id = 123456;
        m_key = 123456;
        // 启动登录
        if (false == m_net.Init("127.0.0.1", 1234))
        {
            DebugConsole.Log("Client init failed!");
        }
        */
        // 初始化Displays
        //m_displays = new SlotClientDisplays(); // MonoBehaviour不可以new
        m_displays = GameObject.Find("SlotDisplays").GetComponent<SlotDisplays>();
        m_displays.User = this;

        // 初始化Requests
        m_requests = new SlotRequests();
        m_requests.Net = m_net;
        m_requests.Clerk = this;

        // 发送快速登录
        m_requests.QuickLogin();
	}
    void OnApplicationQuit()
    {
        m_net.Close();
    }

    void CheckLogin()
    {
        if (!m_login)
            m_requests.QuickLogin();
    }
    //IEnumerator resetTimes()
    //{
    //    yield return new WaitForSeconds(1);
    //    m_escapeTimes = 1;
    //}
    void QuitGame()
    {
        Application.Quit();
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //这个地方可以写“再按一次退出”的提示
            if (DialogBase.Actived())
            {
                DebugConsole.Log("Hide in Clerk");
                DialogBase.Hide();
            }
            else
            {
                DebugConsole.Log("Show in Clerk");
                DialogBase.Show("ESC", "Exit game?", QuitGame);
            }
            //m_escapeTimes++;
            //StartCoroutine("resetTimes");
            //if (m_escapeTimes > 1)
            //{
            //    Application.Quit();
            //}
        }

        if (!m_net.IsRunning())
            return;

        if (m_net.CheckReconnect())
        {
            CheckLogin();
        }

        ProtoPacket packet = new ProtoPacket();
        if (m_net.RecvTryDequeue(ref packet))
        {
           m_displays.Execute(packet);
        }

        GameObject recp = GameObject.Find("Reception");
        if (recp == null)
        {
            if (m_broadcastMsg != "")
            {
                // 有系统消息，平移吧            
                GameObject goBroadcast = GameObject.Find("BroadcastText");
                Vector3 pos = goBroadcast.transform.localPosition;
                pos.x -= 50 * Time.deltaTime;
                goBroadcast.transform.localPosition = pos;

                // 从600～-600
                if (goBroadcast.transform.localPosition.x < -600)
                    m_broadcastMsg = "";
            }
            else
            {
                m_broadcastMsg = Lobby.getInstance().GetBroadcast();
                if (m_broadcastMsg != "")
                {
                    GameObject goBroadcast = GameObject.Find("BroadcastText");
                    goBroadcast.GetComponent<Text>().text = m_broadcastMsg;
                    goBroadcast.transform.localPosition = new Vector3(600, 0, 0);
                }
            }

            // 不是Lobby，需要处理Reception消息
            // 如广播，以及store相关的
            ProtoNet net = Reception.Net();
            if (net != null)
            {
                packet = null;
                packet = new ProtoPacket();
                if (net.RecvTryDequeue(ref packet))
                {
                    switch (packet.cmdId)
                    {
                        case Constants.Lion_GetShopItems:
                            {
                                Lobby.getInstance().ShopList = (ShopList)packet.proto;
                                DebugConsole.Log("ShopName:" + Lobby.getInstance().ShopList.ShopName);
                                if (packet.callback != null)
                                {
                                    packet.callback();
                                }
                            }
                            break;
                        case Constants.Lion_BroadcastSystemMessage:
                            {
                                Tools.PlayNotification(Constants.Audio.Audio_Notification);
                                StringValue sv = (StringValue)packet.proto;
                                Lobby.getInstance().AddBroadcast(sv.Value);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
	}
    void OnTextChanged(string txtName, long value)
    {
        GameObject.Find(txtName).GetComponent<Text>().text = Tools.CoinToString(value);
    }
    void OnBtnChanged(Constants.Btn btnIndex, long value)
    {
        int bi = (int)btnIndex;
        if (bi < 0 || bi >= Constants.Btn_Strings.Length)
        {
            DebugConsole.Log("Invalid bi:" + bi.ToString());
            return;
        }

        string btnName = Constants.Btn_Strings[bi];
        GameObject gameObj = GameObject.Find(btnName);
        if (null == gameObj)
        {
            DebugConsole.Log("Cant find gameObj:" + btnName);
            return;
        }

        Button btnUId = gameObj.GetComponent<Button>();
        if (null == btnUId)
        {
            DebugConsole.Log("Cant find btn:" + btnName);
            return;
        }

        Text text = btnUId.transform.Find("Text").GetComponent<Text>();
        if (null == text)
        {
            DebugConsole.Log("Cant find text:" + btnName);
            return;
        }

        text.text = value.ToString();
    }

    public void Begin()
    {
        m_stopWatch.Start();
    }
    public long End()
    {
        m_stopWatch.Stop();
        long elapse = m_stopWatch.ElapsedMilliseconds;
        m_stopWatch.Reset();

        return elapse;
    }

    public int Lines 
    {
        get { return m_lines; }
        set { m_lines = value; /*OnBtnChanged(Constants.Btn.Btn_Lines, m_lines);*/ }
    }
    public int Bet 
    {
        get { return m_bet; }
        set { m_bet = value; OnTextChanged("totalBetText", m_bet); }
    }
    public long UId
    {
        get { return m_id; }
        set { m_id = value; /*OnBtnChanged(Constants.Btn.Btn_UId, m_id);*/ }
    }
    public int SpinCount
    {
        get { return m_spinCount; }
        set { m_spinCount = value; }
    }
    public long Key
    {
        get { return m_key; }
        set { m_key = value; }
    }
    public int TigerNo
    {
        get { return m_tigerNo; }
        set { m_tigerNo = value; }
    }
    public int SeqNo
    {
        get { return m_seqNo; }
        set { m_seqNo = value; }
    }
    public long Gold 
    {
        get { return m_gold; }
        set { m_gold = value; OnTextChanged("uGoldText", m_gold); }
    }
    public long Win
    {
        get { return m_win; }
        set { m_win = value; OnTextChanged("winText", m_win); }
    }
    public ProtoNet Net
    {
        get { return m_net; }
    }
    public bool Login
    {
        get { return m_login; }
        set { m_login = value; }
    }
    public bool Spinning
    {
        get { return m_spinning; }
        set { m_spinning = value; }
    }
    public bool AutoSpin
    {
        get { return m_autoSpin; }
        set { m_autoSpin = value; }
    }
    public SlotRequests Requests { get { return m_requests; } }
    public SlotDisplays Displays { get { return m_displays; } }
}
