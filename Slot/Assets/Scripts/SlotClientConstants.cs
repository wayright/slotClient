using System.Collections;
using System.Collections.Generic;

public class SlotClientConstants{
    // 客户端
    public const int Client_QuickLoginInfo = 1002; // 快速登录
    public const int Client_TigerReq = 10000;// spin请求
    public const int Client_Reconnect = 99999; // 本地重连

    // 服务器
    public const int Server_UserInfo = 1002;// 用户信息（快速登录返回）
    public const int Server_TigerResp = 10000;// spin返回
    public const int Server_Error = -1;// 服务器错误

    // 按钮
    public enum Btn
    {
        Btn_UGold = 0, // 玩家金币总数
        Btn_UId, // 玩家ID
        Btn_Spin, // 摇
        Btn_LineMinus, // 减线
        Btn_LineAdd, // 加线
        Btn_BetMinus, // 减注
        Btn_BetAdd, // 加注
        Btn_MaxBet, // 最大下注
        Btn_Return, // 返回
        Btn_Deposit, // 充值
        Btn_Setting, // 设置
        Btn_Lines, // 线数
        Btn_Bet, // 注数
        Btn_Win, // 赢金币数
    }

    public static string[] Btn_Strings = 
    {
        "BtnUGold",
        "BtnUId",
        "BtnSpin",
        "BtnLineMinus",
        "BtnLineAdd",
        "BtnBetMinus",
        "BtnBetAdd",
        "BtnMaxBet",
        "BtnReturn",
        "BtnDeposit",
        "BtnSetting",
        "BtnLines",
        "BtnBet",
        "BtnWin"
    };
}
