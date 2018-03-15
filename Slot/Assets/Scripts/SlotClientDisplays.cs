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
using slotClient;

public class SlotClientDisplays
{
    public SlotClientUser User { get; set; }

    public void Execute(ProtoPacket packet)
    {
        SlotClientNet.WriteLog("handle cmd from server:" + packet.cmdId);
        switch (packet.cmdId)
        {
            case SlotConstants.Server_UserInfo:// QuickLoginInfo返回
                {
                    Tiger.Info.UserInfo usrInfo = (Tiger.Info.UserInfo)packet.proto;
                    SlotClientNet.WriteLog("Recv proto packet[UserInfo]:\nid=" +
                        usrInfo.user_id + "\ngold=" + usrInfo.gold);

                    UpdateUserInfo(usrInfo);
                }
                break;
            case SlotConstants.Server_TigerResp: // TigerReq返回
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
            case SlotConstants.Client_Reconnect:
                {
                    SlotClientNet.WriteLog("Reconnecting...");
                }
                break;
            case SlotConstants.Server_Error:
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
        // 刷新我的金币
        GameObject gameObj = GameObject.Find("BtnUId");
        Button btnUId = gameObj.GetComponent<Button>();
        Text text = btnUId.transform.Find("Text").GetComponent<Text>();
        text.text = usrInfo.user_id.ToString();

        gameObj = GameObject.Find("BtnUGold");
        Button btnUGold = gameObj.GetComponent<Button>();
        text = btnUGold.transform.Find("Text").GetComponent<Text>();
        text.text = usrInfo.gold.ToString();

        gameObj = GameObject.Find("BtnLines");
        Button btnLines = gameObj.GetComponent<Button>();
        text = btnLines.transform.Find("Text").GetComponent<Text>();
        text.text = User.Lines.ToString();

        gameObj = GameObject.Find("BtnBet");
        Button btnBet = gameObj.GetComponent<Button>();
        text = btnBet.transform.Find("Text").GetComponent<Text>();
        text.text = User.Bet.ToString();

        // 刷新数据
        User.UId = (int)usrInfo.user_id;
        User.Gold = (int)usrInfo.gold;
        User.Login = true;
    }

    void UpdateTigerResp(TigerResp tigerResp)
    {
        // 本地减金币先
        // ...
        User.Gold -= User.Bet;
        {
            GameObject gameObj = GameObject.Find("BtnUGold");
            Button btnUGold = gameObj.GetComponent<Button>();
            Text text = btnUGold.transform.Find("Text").GetComponent<Text>();
            text.text = User.Gold.ToString();
        }

        // 滚动开始
        for (int i = 0; i < tigerResp.pos.Count; ++i)
        {
            int pos = tigerResp.pos[i];
            string name = "reel" + (i + 1).ToString();
            SlotClientReel reel = GameObject.Find(name).GetComponent<SlotClientReel>();
            Debug.Log("pos" + i.ToString() + ":" + pos.ToString());

            if (i == tigerResp.pos.Count - 1)
                reel.Spin(pos + 2, tigerResp.bonus);
            else
                reel.Spin(pos + 2, null);
        }

        // 中奖效果在Reel中滞后实现
        if (tigerResp.bonus.Count > 0)
        {
            User.Gold += tigerResp.bonus[0].data1;
            {
                GameObject gameObj = GameObject.Find("BtnUGold");
                Button btnUGold = gameObj.GetComponent<Button>();
                Text text = btnUGold.transform.Find("Text").GetComponent<Text>();
                text.text = User.Gold.ToString();
            }
        }
    }
}
