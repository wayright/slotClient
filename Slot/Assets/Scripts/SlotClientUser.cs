// 基本用户信息
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotClientUser : MonoBehaviour {

    private int m_gold = 0; // 总金额
    private int m_bet = 20; // 下注数
    private int m_lines = 0; // 下注线
    private int m_spinCount = 0; // 摇次数
    private int m_id = 0; // UID
    private int m_key = 0; // Key
    private int m_tigerNo; // 机器编号
    private int m_seqNo; // 前端序号
    private bool m_login; // 是否登录
    private int m_win; // 中奖金币
    private SlotClientNet m_client; // 网络客户端
    private SlotClientDisplays m_displays; // 展示器
    private SlotClientRequests m_requests; // 请求器
    private bool m_spinning = false; // 是否正在摇

	// Use this for initialization
	void Start () {
        m_spinCount = 0;
        m_id = 123456;
        m_key = 123456;
        m_tigerNo = 888;
        m_seqNo = 666;
        m_gold = 0;
        m_client = new SlotClientNet();
        m_login = false;
        m_lines = 1;
        m_win = 0;

        // 启动登录
        if (false == m_client.Init("182.92.74.240", 7878))
        {
            Debug.Log("Client init failed!");
        }

        // 初始化Displays
        //m_displays = new SlotClientDisplays(); // MonoBehaviour不可以new
        m_displays = GameObject.Find("SlotClientDisplays").GetComponent<SlotClientDisplays>();
        m_displays.User = this;

        // 初始化Requests
        m_requests = new SlotClientRequests();
        m_requests.Client = m_client;
        m_requests.User = this;

        // 发送快速登录
        m_requests.ReqQuickLogin();
	}
    void OnApplicationQuit()
    {
        m_client.Close();
    }
	
	// Update is called once per frame
	void Update () {        
        ProtoPacket packet = new ProtoPacket();
        if (m_client.RecvTryDequeue(ref packet))
        {
            m_displays.Execute(packet);
        }
	}

    void OnBtnChanged(SlotClientConstants.Btn btnIndex, int value)
    {
        int bi = (int)btnIndex;
        if (bi <0 || bi >= SlotClientConstants.Btn_Strings.Length)
        {
            Debug.Log("Invalid bi:" + bi.ToString());
            return;
        }

        string btnName = SlotClientConstants.Btn_Strings[bi];
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
        set { m_lines = value; OnBtnChanged(SlotClientConstants.Btn.Btn_Lines, m_lines); }
    }
    public int Bet 
    {
        get { return m_bet; }
        set { m_bet = value; OnBtnChanged(SlotClientConstants.Btn.Btn_Bet, m_bet); }
    }
    public int UId
    {
        get { return m_id; }
        set { m_id = value; OnBtnChanged(SlotClientConstants.Btn.Btn_UId, m_id); }
    }
    public int SpinCount
    {
        get { return m_spinCount; }
        set { m_spinCount = value; }
    }
    public int Key
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
    public int Gold 
    {
        get { return m_gold; }
        set { m_gold = value; OnBtnChanged(SlotClientConstants.Btn.Btn_UGold, m_gold); }
    }
    public int Win
    {
        get { return m_win; }
        set { m_win = value; OnBtnChanged(SlotClientConstants.Btn.Btn_Win, m_win); }
    }
    public SlotClientNet Client
    {
        get { return m_client; }
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
    public SlotClientRequests Requests { get { return m_requests; } }
    public SlotClientDisplays Displays { get { return m_displays; } }
}
