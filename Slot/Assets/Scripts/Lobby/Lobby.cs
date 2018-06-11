using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lion.Proto;
using Common.Proto;
using Login.Proto;
// 大厅
// 用singleTon模式来维持数据
// 不需要挂载到任何场景
public class Lobby{
    private static Lobby s_Instance = null;
    private long m_uId = 0;
    private long m_key = 0; // 临时
    private string m_domain = ""; // 重定向地址
    private int m_port; // 重定向端口
    private LionUserInfo m_userInfo; // 本机用户信息
    private LionUserInfo m_queryUserInfo; // 其他用户信息
    private RedirectResp m_redirectInfo;
    private TigerStat m_tigerStat;
    private LongArray m_friendArray;
    private int m_curFriendPage = 0; // base 0
    private FriendSummaryList m_curSummaryList;
    private long m_freeBonusEpoch = -1;
    private UserItemList m_userItemList;
    private ShopList m_shopList;
    private Queue<string> m_broadcastSysMsg = new Queue<string>();
    public UserItemList UserItemList
    {
        get { return m_userItemList; }
        set { m_userItemList = value; }
    }
    public ShopList ShopList
    {
        get { return m_shopList; }
        set { m_shopList = value; }
    }
    public Queue<string> SystemMessage
    {
        get { return m_broadcastSysMsg; }
    }
    public void AddBroadcast(string str)
    {
        m_broadcastSysMsg.Enqueue(str);
    }
    public string GetBroadcast()
    {
        if (m_broadcastSysMsg.Count == 0)
            return "";
        else
            return m_broadcastSysMsg.Dequeue();
    }
    public long FreeBonusEpoch
    {
        get { return m_freeBonusEpoch; }
        set { m_freeBonusEpoch = value; }
    }
    public FriendSummaryList CurrentSummaryList
    {
        get { return m_curSummaryList; }
        set { m_curSummaryList = value; }
    }
    public LongArray FriendIDArray
    {
        get { return m_friendArray; }
        set { m_friendArray = value; }
    }
    public int CurrentFriendPage
    {
        get { return m_curFriendPage; }
        set { m_curFriendPage = value; }
    }
    public int FriendPageCount
    {
        get 
        {
            int pc = 1;
            int ic = m_friendArray.Data.Count;
            while (pc * Constants.PageItemCount < ic)
            {
                ++pc;
            }
            return pc;
        }
    }
    public LongArray GetCurrentFriendPageArray()
    {
        int totalPage = FriendPageCount;
        if (m_curFriendPage < 0 || m_curFriendPage >= totalPage)
        {
            DebugConsole.Log("Invalid friend page index." + m_curFriendPage);
            m_curFriendPage = 0;
        }

        LongArray la = new LongArray();
        for (int i = 0; i < Constants.PageItemCount;++i )
        {
            int idx = m_curFriendPage * Constants.PageItemCount + i;
            if (idx >= m_friendArray.Data.Count)
                break;
            la.Data.Add(m_friendArray.Data[idx]);
        }
        return la;
    }
    public LionUserInfo UserInfo
    {
        get { return m_userInfo; }
        set { m_userInfo = value; }
    }
    public LionUserInfo QueryUserInfo
    {
        get { return m_queryUserInfo; }
        set { m_queryUserInfo = value; }
    }
    public TigerStat TigerStatInfo
    {
        get { return m_tigerStat; }
        set { m_tigerStat = value; }
    }
    public RedirectResp RedirectInfo
    {
        get { return m_redirectInfo; }
        set { m_redirectInfo = value; }
    }
    public static Lobby getInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = new Lobby();

        }
        return s_Instance;
    } 
	
    public long UId
    {
        get { return m_uId; }
        set { m_uId = value; }
    }
    public long Key
    {
        get { return m_key; }
        set { m_key = value; }
    }
    public string Domain
    {
        get { return m_domain; }
        set { m_domain = value; }
    }
    public int Port
    {
        get { return m_port; }
        set { m_port = value; }
    }
}
