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
using slotClient;

public class SlotClientRequests{
    public SlotClientUser User { get; set; }
    public SlotClientNet Net { get; set; }
   
    // 公共函数-以Req开头
    // 快速登录
    public void ReqQuickLogin()
    {
        QuickLoginInfo quickLoginInfo = new QuickLoginInfo();
        quickLoginInfo.user_id = User.UId;
        quickLoginInfo.key = User.Key;

        Net.SendEnqueue(SlotConstants.Client_QuickLoginInfo, 0, quickLoginInfo);
    }

    public void ReqSpin()
    {
        if (!User.Login)
        {
            Debug.Log("!Login, cant reqspin");
            return;
        }

        TigerReq tigerReq = new TigerReq();
        tigerReq.bet_gold = User.Bet; // only 10, 20, 30
        tigerReq.seq_no = User.SeqNo;
        tigerReq.tiger_no = User.TigerNo;
        tigerReq.lines.Add(User.Lines);

        Net.SendEnqueue(SlotConstants.Client_TigerReq, 0, tigerReq);        
    }
}
