// 封装客户端的请求函数
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Login.Proto;
using Common;
using Tiger.Proto;
using Dog.Proto;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SlotRequests{
    private SlotClerk m_clerk;
    private ProtoNet m_net;

    public SlotClerk Clerk
    {
        get { return m_clerk; }
        set { m_clerk = value; }
    }
    public ProtoNet Net
    {
        get { return m_net; }
        set { m_net = value; }
    }
   
    // 公共函数-以Req开头
    // 快速登录
    public void QuickLogin()
    {
        if (m_clerk == null)
            DebugConsole.Log("Clerk is null");

        QuickLoginInfo quickLoginInfo = new QuickLoginInfo();
        quickLoginInfo.UserId = m_clerk.UId;
        quickLoginInfo.Key = m_clerk.Key;

        m_net.SendEnqueue(Constants.Tiger_QuickLoginInfo, 0, quickLoginInfo);
    }
    public void ReqSpin()
    {
        if (!m_clerk.Login)
        {
            DebugConsole.Log("!Login, cant reqspin");
            // Show dialog here
            DialogBase.Show("MESSAGE", "!Login, cant reqspin");
            return;
        }

        TigerReq tigerReq = new TigerReq();
        tigerReq.BetGold = m_clerk.Bet; // only 10, 20, 30
        // SeqNo 当前用来对应消息ID
        m_clerk.SeqNo = m_clerk.SpinCount;
        tigerReq.SeqNo = m_clerk.SpinCount;
        m_clerk.Begin(); // 开始计时

        tigerReq.TigerNo = m_clerk.TigerNo;
        for (int i = 0; i < m_clerk.Lines; ++i)
            tigerReq.Lines.Add(i);

        m_net.SendEnqueue(Constants.Tiger_Spin, 0, tigerReq);        
    }
}
