// 封装客户端的请求函数
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using ProtoBuf;
using Login;
using User;
using Common;
using Tiger.Info;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SlotClientRequests{
    private SlotClientUser m_user;
    private SlotClientNet m_client;

    public SlotClientUser User
    {
        get { return m_user; }
        set { m_user = value; }
    }
    public SlotClientNet Client
    {
        get { return m_client; }
        set { m_client = value; }
    }
   
    // 公共函数-以Req开头
    // 快速登录
    public void ReqQuickLogin()
    {
        QuickLoginInfo quickLoginInfo = new QuickLoginInfo();
        if (m_user == null)
            Debug.Log("user is null");
        quickLoginInfo.user_id = m_user.UId;
        quickLoginInfo.key = m_user.Key;

        m_client.SendEnqueue(SlotClientConstants.Client_QuickLoginInfo, 0, quickLoginInfo);
    }

    public void ReqSpin()
    {
        if (!m_user.Login)
        {
            Debug.Log("!Login, cant reqspin");
            return;
        }

        TigerReq tigerReq = new TigerReq();
        tigerReq.bet_gold = m_user.Bet; // only 10, 20, 30
        tigerReq.seq_no = m_user.SeqNo;
        tigerReq.tiger_no = m_user.TigerNo;
        for (int i = 0; i < m_user.Lines; ++i )
            tigerReq.lines.Add(i);

        m_client.SendEnqueue(SlotClientConstants.Client_TigerReq, 0, tigerReq);        
    }
}
