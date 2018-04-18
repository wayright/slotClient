using System.Collections;
using System.Collections.Generic;

public class Constants{
    //// 常用变量   
    // Dog
    public const int Dog_Login = 1100; // Dog普通登录
    public const int Dog_Redirect = 301; // 服务跳转，目前Dog和Lion支持

    // Lobby-Lion    
    public const int Lion_Redirect = 301; // 服务跳转，目前Dog和Lion支持
    public const int Lion_QuickLoginInfo = 4000; // 大厅快速登录
    public const int Lion_GetProfile = 4001; // 获取某个玩家的个人资料
    public const int Lion_UpdateProfile = 4002; // 更新个人资料某一项
    public const int Lion_Get_Tiger_Stat = 4300; // 获取某个玩家的老虎机统计信息
    
    // Slot-Tiger
    public const int Tiger_QuickLoginInfo = 10000; // Tiger快速登录
    public const int Tiger_Spin = 10050;// spin请求

    // #Common
    public const int Reconnect = 99999; // 本地重连
    public const int Error = -1;// 服务器错误
    
    //// 资源（包括按钮、音效等）
    // #Lobby
    public enum LobbyBtn
    {
        Btn_Credits = 0, // Lobby金币数
        Btn_Gems, // 钻石数
        Btn_Message, // 消息
        Btn_Option, // 选项
        Btn_Avatar, // 个人信息
        Btn_Head, // 头
        Btn_Friends, // 朋友
        Btn_MainReward, // 奖励
        Btn_FreeBonus, // 免费送奖励
        Btn_Slot, // slot
        Btn_Bingo, // bingo
        Btn_Poker, // poker
        Btn_Sj, // sj
    }

    public static string[] LobbyBtn_Strings = 
    {
        "BtnCredits",
        "BtnGems",
        "BtnMessage",
        "BtnOption",
        "BtnAvatar",
        "BtnHead",
        "BtnFriends",
        "BtnMainReward",
        "BtnFreeBonus",
        "BtnSlot",
        "BtnBingo",
        "BtnPoker",
        "BtnSj",
    };

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

    public const int Svc_Tiger = 100;
    public const int Svc_Lion = 101;

    // 语言
    public const string Lang_Eng = "eng"; // 英文
    public const string Lang_CN = "cn"; // 简体中文
}
