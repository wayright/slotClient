// 客户端登录、展示等处理服务端反馈
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

public class SlotClientDisplays
{
    private SlotClientUser m_user = null;
    public SlotClientUser User
    {
        get { return m_user; }
        set { m_user = value; }
    }

    public void Execute(ProtoPacket packet)
    {
        SlotClientNet.WriteLog("handle cmd from server:" + packet.cmdId);
        switch (packet.cmdId)
        {
            case SlotClientConstants.Server_UserInfo:// QuickLoginInfo返回
                {
                    Tiger.Info.UserInfo usrInfo = (Tiger.Info.UserInfo)packet.proto;
                    SlotClientNet.WriteLog("Recv proto packet[UserInfo]:\nid=" +
                        usrInfo.user_id + "\ngold=" + usrInfo.gold);

                    UpdateUserInfo(usrInfo);
                }
                break;
            case SlotClientConstants.Server_TigerResp: // TigerReq返回
                {
                    TigerResp tigerResp = (TigerResp)packet.proto;
                    SlotClientNet.WriteLog("Recv proto packet[TigerResp]:\ntiger_no=" +
                        tigerResp.tiger_no +
                        "\nseq_no=" + tigerResp.seq_no +
                        "\npos=" + tigerResp.pos.ToString() +
                        "\nbonus=" + tigerResp.bonus +
                        "\npos=" + tigerResp.pos);

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
    void UpdateUserInfo(Tiger.Info.UserInfo usrInfo)
    {
        // 刷新数据
        m_user.UId = (int)usrInfo.user_id;
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
        if (tigerResp.bonus.Count > 0)
        {
            switch (tigerResp.bonus[0].type)
            {
                case 1:// 倍数
                    m_user.Gold += m_user.Bet * tigerResp.bonus[0].data1;
                    break;
                case 2:// 金币
                    m_user.Gold += tigerResp.bonus[0].data1;
                    break;
                case 3:// 免费局
                    break;
                default:
                    break;
            }
        }
    }
}
