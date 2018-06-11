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
    public const int Lion_Register = 4003; // 注册，当前只能是email
    public const int Lion_ModPass = 4004;  // 修改密码
    public const int Lion_RefreshGold = 4010;  // 刷新金币
    public const int Lion_GetTigerStat = 4300; // 获取某个玩家的老虎机统计信息
    public const int Lion_GetFriends = 4021; // 获取好友ID列表
    public const int Lion_GetFriendRequests = 4022; // 获取请求加好友ID列表
    public const int Lion_AddFriend = 4023; // 申请加好友
    public const int Lion_DeleteFriend = 4024; // 删除好友
    public const int Lion_AcceptFriend = 4025; // 同意加好友
    public const int Lion_IgnoreFriend = 4026; // 忽略加好友
    public const int Lion_GetFriendSummary = 4027; // 获取部分好友的简介
    public const int Lion_NotifyWeeklyLogin = 4201; // 后端推送，前端弹出连续登录面板，根据后端的值设置第几天可领取
    public const int Lion_TakeLoginBonus = 4202; // 领取连续登录奖励
    public const int Lion_NotifyFreeBonus = 4203; // 后端推送，倒计时的时间点，如果当前时间已经超过，直接显示奖励
    public const int Lion_TakeFreeBonus = 4204; // 领取免费奖励
    public const int Lion_GetItems = 4110; // 获取背包数据
    public const int Lion_SellItem = 4112; // 出售道具
    public const int Lion_GiveItem = 4113; // 赠送道具
    public const int Lion_DropItem = 4114; // 丢弃道具
    public const int Lion_UseItem = 4115; // 使用道具
    public const int Lion_JoinItem = 4116; // 合成道具
    public const int Lion_GetShopItems = 4130; // 获取商店物品列表
    public const int Lion_BuyItem = 4131; // 购买物品
    public const int Lion_BroadcastSystemMessage = 4400; // 系统消息
    
    // Slot-Tiger
    public const int Tiger_QuickLoginInfo = 10000; // Tiger快速登录
    public const int Tiger_Spin = 10050;// spin请求

    // #Common
    public const int Reconnect = 99999; // 本地重连
    public const int Error = -1;// 服务器错误
    public const int PageItemCount = 6; // 一页容纳几项
    
    //// 资源（包括按钮、音效等）
    // #Lobby

    public const int Login_UUID = 0;
    public const int Login_Email = 1;

    public enum LobbyBtn
    {
        Btn_Credits = 0, // Lobby金币数
        Btn_Gems, // 钻石数
        Btn_Message, // 消息
        Btn_Option, // 选项
        Btn_Avatar, // 个人信息
        Btn_Head, // 头
        Btn_Friends, // 朋友
        Btn_Bag, // Bag
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
        "BtnBag",
        "BtnFreeBonus",
        "BtnSlot",
        "BtnBingo",
        "BtnPoker",
        "BtnSj",
    };

    // #Slot
    public enum Btn
    {
        Btn_Spin, // 摇
        Btn_LineMinus, // 减线
        Btn_LineAdd, // 加线
        Btn_BetMinus, // 减注
        Btn_BetAdd, // 加注
        Btn_MaxBet, // 最大下注
        Btn_Return, // 返回
        Btn_Deposit, // 充值
        Btn_AutoSpin, // 自动摇
    }

    public static string[] Btn_Strings = 
    {
        "BtnSpin",
        "BtnLineMinus",
        "BtnLineAdd",
        "BtnBetMinus",
        "BtnBetAdd",
        "BtnMaxBet",
        "BtnReturn",
        "BtnDeposit",
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
        Audio_LobbyClickButton,
        Audio_Notification,
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
        "PlusMinus",
        "LobbyClickButton",
        "Notification"
    };

    public const int Svc_Tiger = 100;
    public const int Svc_Lion = 101;

    // 语言
    public const string Lang_Eng = "eng"; // 英文
    public const string Lang_CN = "cn"; // 简体中文

    // 奖励类型
    public const int Bonus_Daily = 0;
    public const int Bonus_Free = 1;
}
