// 客户端登录、展示等处理服务端反馈
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoBuf;
using Login;
using User;
using Common;
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
    public int From
    {
        get { return m_from; }
        set { m_from = value; }
    }

    public int To
    {
        get { return m_to; }
        set { m_to = value; }
    }
    public int Result
    {
        get { return m_result; }
        set { m_result = value; }
    }
    public int JumpTimes
    {
        get { return m_jumpTimes; }
        set { m_jumpTimes = value; }
    }
    public int Type
    {
        get { return m_type; }
        set { m_type = value; }
    }

    private int m_from = 0;
    private int m_to = 0;
    private int m_jumpTimes = 1;
    private int m_result = 0;
    private int m_type = 0; // 0-win, 1-gold
}

public class SlotClientDisplays : MonoBehaviour
{
    private SlotClientUser m_user = null;
    private JumpNumberData m_jndWin = null, m_jndGold = null;
    public SlotClientUser User
    {
        get { return m_user; }
        set { m_user = value; }
    }
    void Start() 
    {
        // 隐藏金币
        GameObject gameObj = GameObject.Find("Coin");
        Image img = gameObj.GetComponent<Image>();
        img.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void Execute(ProtoPacket packet)
    {
        //SlotClientNet.WriteLog("handle cmd from server:" + packet.cmdId);
        switch (packet.cmdId)
        {
            case SlotClientConstants.Server_UserInfo:// QuickLoginInfo返回
                {
                    UserInfo usrInfo = (UserInfo)packet.proto;
                    /*
                    SlotClientNet.WriteLog("Recv proto packet[UserInfo]:\nid=" +
                        usrInfo.user_id + "\ngold=" + usrInfo.gold);
                    */
                    UpdateUserInfo(usrInfo);
                }
                break;
            case SlotClientConstants.Server_TigerResp: // TigerReq返回
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
            case SlotClientConstants.Client_Reconnect:
                {
                    SlotClientNet.WriteLog("Reconnecting...");
                }
                break;
            case SlotClientConstants.Server_Error:
                {
                    SlotClientNet.WriteLog("Reconnecting...");
                }
                break;
            default:
                SlotClientNet.WriteLog("Unknown send cmd");
                break;
        }
    }

    // 更新本地界面和数据，以Update开头
    void UpdateUserInfo(UserInfo usrInfo)
    {
        // 刷新数据
        m_user.UId = (int)usrInfo.user_id;
        if (m_user.Gold != 0 && m_user.Gold != (int)usrInfo.gold)
        {
            Debug.Log("Gold cant match!!!");
        }
        else
        {
            Debug.Log("Gold can match!!!");
        }
        m_user.Gold = (int)usrInfo.gold;
        m_user.Login = true;

        // 以下代码用于刷新界面
        m_user.Bet = m_user.Bet;
        m_user.Lines = m_user.Lines;
    }

    void UpdateTigerResp(TigerResp tigerResp)
    {
        // 本地减金币先
        m_user.Gold -= m_user.Bet * m_user.Lines;

        // 滚动开始
        PlayAudio(SlotClientConstants.Audio.Audio_ReelRolling);
        for (int i = 0; i < tigerResp.pos.Count; ++i)
        {
            int pos = tigerResp.pos[i];
            string name = "reel" + (i + 1).ToString();
            SlotClientReel reel = GameObject.Find(name).GetComponent<SlotClientReel>();
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

        m_jndWin.From = m_user.Win;
        m_jndWin.To = 0;
        m_jndWin.JumpTimes = 60;
        m_jndWin.Type = JumpNumberData.JumpTypeWin;

        if (null == m_jndGold)
            m_jndGold = new JumpNumberData();
        m_jndGold.From = m_user.Gold;
        m_jndGold.To = m_user.Gold + m_user.Win;
        m_jndGold.JumpTimes = 60;
        m_jndGold.Type = JumpNumberData.JumpTypeGold;

        StartCoroutine(JumpWinNumber(m_jndWin));
        StartCoroutine(JumpWinNumber(m_jndGold));
    }
    public IEnumerator JumpWinNumber(JumpNumberData data)
    {
        int step = 1;
        bool increase = true;
        int total = 0;
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
                m_user.Win = data.Result;
            else if (data.Type == JumpNumberData.JumpTypeGold)
                m_user.Gold = data.Result;

            yield return 1;
        }
        
        data.Result = data.To;
        if (data.Type == JumpNumberData.JumpTypeWin)
        {
            m_user.Win = data.Result;
        }
        else if (data.Type == JumpNumberData.JumpTypeGold)
        {
            m_user.Gold = data.Result;
            m_user.Spinning = false;

            // 隐藏金币
            GameObject coin = GameObject.Find("Coin");
            Image coinImg = coin.GetComponent<Image>();
            coinImg.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            coin.transform.localPosition = new Vector3(385, -379, 0);
        }

        StopCoroutine(JumpWinNumber(data));
    }

    public void PlayAudio(SlotClientConstants.Audio aud)
    {
        if (aud >= SlotClientConstants.Audio.Audio_Max)
        {
            Debug.Log("Ivalid audio enum");
            return;
        }

        string audStr = SlotClientConstants.Audio_Strings[(int)aud];
        AudioSource aSource = transform.Find(audStr).GetComponent<AudioSource>();
        aSource.Play();
    }
    public void StopAudio(SlotClientConstants.Audio aud)
    {
        if (aud >= SlotClientConstants.Audio.Audio_Max)
        {
            Debug.Log("Ivalid audio enum");
            return;
        }

        string audStr = SlotClientConstants.Audio_Strings[(int)aud];
        AudioSource aSource = transform.Find(audStr).GetComponent<AudioSource>();
        aSource.Stop();
    }
}
