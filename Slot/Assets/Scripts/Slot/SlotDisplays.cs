﻿// 客户端登录、展示等处理服务端反馈
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoBuf;
using Login.Proto;
using User;
using Common;
//using Dog.Proto;
using Tiger.Proto;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Events;

// 跳动数字效果展示
public class JumpNumberData
{
    public const int JumpTypeWin = 0;
    public const int JumpTypeGold = 1;
    public long From
    {
        get { return m_from; }
        set { m_from = value; }
    }

    public long To
    {
        get { return m_to; }
        set { m_to = value; }
    }
    public long Result
    {
        get { return m_result; }
        set { m_result = value; }
    }
    public long JumpTimes
    {
        get { return m_jumpTimes; }
        set { m_jumpTimes = value; }
    }
    public int Type
    {
        get { return m_type; }
        set { m_type = value; }
    }

    private long m_from = 0;
    private long m_to = 0;
    private long m_jumpTimes = 1;
    private long m_result = 0;
    private int m_type = 0; // 0-win, 1-gold
}
public class DelayWork
{
    private float m_elapse;

    public float Elapse
    {
        get { return m_elapse; }
        set { m_elapse = value; }
    }

    public bool Active()
    {
        return m_elapse > 0;
    }

    public bool Step()
    {
        m_elapse -= Time.deltaTime;
        return Active();
    }
}

public class SlotDisplays : MonoBehaviour
{
    private SlotClerk m_clerk = null;
    private JumpNumberData m_jndWin = null, m_jndGold = null;
    private DelayWork m_dw = new DelayWork();
    public SlotClerk User
    {
        get { return m_clerk; }
        set { m_clerk = value; }
    }
    void Start() 
    {
        // 隐藏金币
        GameObject gameObj = GameObject.Find("Coin");
        Image img = gameObj.GetComponent<Image>();
        img.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        m_dw.Elapse = 0;
    }

    void Update()
    {
        if (m_dw.Active())
        {
            if (!m_dw.Step())
            {
                m_clerk.Net.Init("127.0.0.1", 7879);
            }
        }
    }

    public void Execute(ProtoPacket packet)
    {
        ProtoNet.WriteLog("handle cmd from server:" + packet.cmdId);
        switch (packet.cmdId)
        {
            case Constants.Server_UserInfo:// QuickLoginInfo返回
                {
                    UserInfo usrInfo = (UserInfo)packet.proto;
                    /*
                    SlotClientNet.WriteLog("Recv proto packet[UserInfo]:\nid=" +
                        usrInfo.user_id + "\ngold=" + usrInfo.gold);
                    */
                    UpdateUserInfo(usrInfo);
                }
                break;
            case Constants.Server_TigerResp: // TigerReq返回
                {
                    TigerResp tigerResp = (TigerResp)packet.proto;
                    /*
                    SlotClientNet.WriteLog("Recv proto packet[TigerResp]:\ntiger_no=" +
                        tigerResp.tiger_no +
                        "\nseq_no=" + tigerResp.seq_no +
                        "\npos=" + tigerResp.pos.ToString() +
                        "\nbonus=" + tigerResp.bonus +
                        "\npos=" + tigerResp.pos);*/

                    UpdateTigerResp(tigerResp);                        
                }
                break;           
            case Constants.Client_Reconnect:
                {
                    ProtoNet.WriteLog("Reconnecting...");
                    if (packet.msgId > 0)
                    {
                        // 3s后Display中重连
                        m_dw.Elapse = 3;
                    }
                }
                break;
            case Constants.Server_Error:
                {
                    ProtoNet.WriteLog("Reconnecting...");
                }
                break;
            default:
                ProtoNet.WriteLog("Unknown send cmd");
                break;
        }

        if (m_clerk.CallbackDict.ContainsKey(packet.cmdId))
        {
            ProtoNet.WriteLog("Callback:" + packet.cmdId);
            m_clerk.CallBack(m_clerk.CallbackDict[packet.cmdId], packet.proto);
        }
    }
    // 更新本地界面和数据，以Update开头
    void UpdateUserInfo(UserInfo usrInfo)
    {
        // 刷新数据
        m_clerk.UId = usrInfo.user_id;
        if (m_clerk.Gold != 0 && m_clerk.Gold != usrInfo.gold)
        {
            Debug.Log("Gold cant match!!!");
        }
        else
        {
            Debug.Log("Gold can match!!!");
        }
        m_clerk.Gold = usrInfo.gold;
        m_clerk.Login = true;

        // 以下代码用于刷新界面
        m_clerk.Bet = m_clerk.Bet;
        m_clerk.Lines = m_clerk.Lines;
    }

    void UpdateTigerResp(TigerResp tigerResp)
    {
        // 本地减金币先
        m_clerk.Gold -= m_clerk.Bet * m_clerk.Lines;

        // 滚动开始
        PlayAudio(Constants.Audio.Audio_ReelRolling);
        for (int i = 0; i < tigerResp.pos.Count; ++i)
        {
            int pos = tigerResp.pos[i];
            string name = "reel" + (i + 1).ToString();
            SlotReel reel = GameObject.Find(name).GetComponent<SlotReel>();
            //Debug.Log("pos" + i.ToString() + ":" + pos.ToString());

            if (i == tigerResp.pos.Count - 1)
                reel.Spin(pos + 2, tigerResp.bonus);
            else
                reel.Spin(pos + 2, null);
        }

        // 中奖效果在Reel中滞后实现
    }

    public void ShowJumpWin()
    {
        if (null == m_jndWin)
            m_jndWin = new JumpNumberData();

        m_jndWin.From = m_clerk.Win;
        m_jndWin.To = 0;
        m_jndWin.JumpTimes = 60;
        m_jndWin.Type = JumpNumberData.JumpTypeWin;

        if (null == m_jndGold)
            m_jndGold = new JumpNumberData();
        m_jndGold.From = m_clerk.Gold;
        m_jndGold.To = m_clerk.Gold + m_clerk.Win;
        m_jndGold.JumpTimes = 60;
        m_jndGold.Type = JumpNumberData.JumpTypeGold;

        StartCoroutine(JumpWinNumber(m_jndWin));
        StartCoroutine(JumpWinNumber(m_jndGold));
    }
    public IEnumerator JumpWinNumber(JumpNumberData data)
    {
        long step = 1;
        bool increase = true;
        long total = 0;
        if (data.From > data.To)
        {
            increase = false;
            total = data.From - data.To;
        }
        else
        {
            total = data.To - data.From;
        }

        if (data.JumpTimes <= 0)
            data.JumpTimes = 1;

        if (total > data.JumpTimes)
        {
            step = total / data.JumpTimes;
        }
        else
        {
            data.JumpTimes = total;
        }

        data.Result = data.From;

        for (int i = 0; i < data.JumpTimes; i++)
        {
            if (increase)
                data.Result += step;
            else
                data.Result -= step;

            if (data.Type == JumpNumberData.JumpTypeWin)
                m_clerk.Win = data.Result;
            else if (data.Type == JumpNumberData.JumpTypeGold)
                m_clerk.Gold = data.Result;

            yield return 1;
        }
        
        data.Result = data.To;
        if (data.Type == JumpNumberData.JumpTypeWin)
        {
            m_clerk.Win = data.Result;
        }
        else if (data.Type == JumpNumberData.JumpTypeGold)
        {
            m_clerk.Gold = data.Result;
            m_clerk.Spinning = false;

            // 隐藏金币
            GameObject coin = GameObject.Find("Coin");
            Image coinImg = coin.GetComponent<Image>();
            coinImg.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            coin.transform.localPosition = new Vector3(385, -379, 0);
        }

        StopCoroutine(JumpWinNumber(data));
    }

    public void PlayAudio(Constants.Audio aud)
    {
        if (aud >= Constants.Audio.Audio_Max)
        {
            Debug.Log("Ivalid audio enum");
            return;
        }

        string audStr = Constants.Audio_Strings[(int)aud];
        AudioSource aSource = transform.Find(audStr).GetComponent<AudioSource>();
        aSource.Play();
    }
    public void StopAudio(Constants.Audio aud)
    {
        if (aud >= Constants.Audio.Audio_Max)
        {
            Debug.Log("Ivalid audio enum");
            return;
        }

        string audStr = Constants.Audio_Strings[(int)aud];
        AudioSource aSource = transform.Find(audStr).GetComponent<AudioSource>();
        aSource.Stop();
    }
}