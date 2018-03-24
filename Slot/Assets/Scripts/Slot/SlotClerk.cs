// 老虎机柜员类
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Login.Proto;
using Tiger.Proto;

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
    public Dictionary<int, int> CallbackDict = new Dictionary<int,int>();

	// Use this for initialization
	void Start () {
        m_spinCount = 0;
        m_tigerNo = 888;
        m_seqNo = 666;
        m_gold = 0;
        
        m_net = new ProtoNet();

        // 增加前台支持的网络包类型
        m_net.Add(Constants.Server_UserInfo, TigerUserInfo.Parser);
        m_net.Add(Constants.Server_TigerResp, TigerResp.Parser);
        m_net.Add(Constants.Client_Reconnect, null);
        m_net.Add(Constants.Server_Error, null);
        m_net.Name = "SlotClerk";

        m_login = false;
        m_lines = 1;
        m_win = 0;
        
        m_id = Lobby.getInstance().UId;
        m_key = Lobby.getInstance().Key;
        // 启动登录
        if (false == m_net.Init(Lobby.getInstance().Domain, Lobby.getInstance().Port))
        {
            Debug.Log("Client init failed!");
        }
        /* 
        m_id = 123456;
        m_key = 123456;
        // 启动登录
        if (false == m_net.Init("127.0.0.1", 1234))
        {
            Debug.Log("Client init failed!");
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
	
	// Update is called once per frame
	void Update () {
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
	}

    void OnBtnChanged(Constants.Btn btnIndex, long value)
    {
        int bi = (int)btnIndex;
        if (bi < 0 || bi >= Constants.Btn_Strings.Length)
        {
            Debug.Log("Invalid bi:" + bi.ToString());
            return;
        }

        string btnName = Constants.Btn_Strings[bi];
        GameObject gameObj = GameObject.Find(btnName);
        if (null == gameObj)
        {
            Debug.Log("Cant find gameObj:" + btnName);
            return;
        }

        Button btnUId = gameObj.GetComponent<Button>();
        if (null == btnUId)
        {
            Debug.Log("Cant find btn:" + btnName);
            return;
        }

        Text text = btnUId.transform.Find("Text").GetComponent<Text>();
        if (null == text)
        {
            Debug.Log("Cant find text:" + btnName);
            return;
        }

        text.text = value.ToString();
    }

    public int Lines 
    {
        get { return m_lines; }
        set { m_lines = value; OnBtnChanged(Constants.Btn.Btn_Lines, m_lines); }
    }
    public int Bet 
    {
        get { return m_bet; }
        set { m_bet = value; OnBtnChanged(Constants.Btn.Btn_Bet, m_bet); }
    }
    public long UId
    {
        get { return m_id; }
        set { m_id = value; OnBtnChanged(Constants.Btn.Btn_UId, m_id); }
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
        set { m_gold = value; OnBtnChanged(Constants.Btn.Btn_UGold, m_gold); }
    }
    public long Win
    {
        get { return m_win; }
        set { m_win = value; OnBtnChanged(Constants.Btn.Btn_Win, m_win); }
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
