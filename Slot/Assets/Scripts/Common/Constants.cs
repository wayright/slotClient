using System.Collections;
using System.Collections.Generic;

public class Constants{
    //// 常用变量
    // #Lobby
    public const int Client_QuickLoginInfo = 1000; // 快速登录
    public const int Client_LoginReq = 1001; // Dog登录
    public const int Client_RedirectReq = 1003; // Dog重定向
    public const int Server_UserInfo = 1000;// 用户信息（快速登录返回）
    public const int Server_LoginResp = 1001; // Dog登录
    public const int Server_RedirectResp = 1003; // Dog重定向

    // #Slot
    public const int Client_TigerReq = 10000;// spin请求
    public const int Server_TigerResp = 10000;// spin返回

    // #Common
    public const int Client_Reconnect = 99999; // 本地重连
    public const int Server_Error = -1;// 服务器错误
    
    //// 资源（包括按钮、音效等）
    // #Lobby

    // #Slot
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
        Btn_AutoSpin, // 自动摇
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
        "BtnWin",
        "BtnAutoSpin"
    };

    public enum Audio
    {
        Audio_Spin = 0,
        Audio_ReelRolling,
        Audio_3Cheer,
        Audio_4Cheer,
        Audio_5Cheer,
        Audio_AutoSpin,
        Audio_ReelStop,
        Audio_CoinFly,
        Audio_PlusMinus,
        Audio_Max,
    }

    public static string[] Audio_Strings = 
    {
        "Spin",
        "ReelRolling",
        "3Cheer",
        "4Cheer",
        "5Cheer",
        "AutoSpin",
        "ReelStop",
        "CoinFly",
        "PlusMinus"
    };
}
