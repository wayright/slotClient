using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dog.Proto;
using Login.Proto;
using Lion.Proto;
using Common.Proto;
using System.Threading;
using UnityEngine.SceneManagement;

// 前台-目前只负责一个客户
// 负责大大厅里传送消息
// 转达客户请求到不同的柜台
public class Reception : MonoBehaviour
{
    // 需要客户的基本信息
    //private int m_gold = 0; // 总金额
    //private int m_key = 0; // Key
    static private bool m_login = false; // 是否登录
    static private ProtoNet m_net = null; // 网络-Dog&Lion
    private Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private string m_nick; // 临时昵称名
    private string m_headImgUrl; // 临时头像url，如果数字为inddex
    private string m_broadcastMsg = ""; // 当前广播内容
    static public ProtoNet Net()
    {
        return m_net;
    }
    
    // Use this for initialization
	void Start () {
        // 初始化ProtoNet
        if (m_net == null)
        {
            m_login = false;
            m_net = new ProtoNet();

            // 增加前台支持的网络包类型
            m_net.Add(Constants.Lion_QuickLoginInfo, LionUserInfo.Parser);
            m_net.Add(Constants.Lion_Redirect, RedirectResp.Parser);
            m_net.Add(Constants.Lion_GetProfile, LionUserInfo.Parser);
            m_net.Add(Constants.Lion_UpdateProfile, Status.Parser);
            m_net.Add(Constants.Lion_GetTigerStat, TigerStat.Parser);
            m_net.Add(Constants.Lion_GetFriends, LongArray.Parser);
            m_net.Add(Constants.Lion_GetFriendRequests, LongArray.Parser);
            m_net.Add(Constants.Lion_AddFriend, Status.Parser);
            m_net.Add(Constants.Lion_DeleteFriend, Status.Parser);
            m_net.Add(Constants.Lion_AcceptFriend, Status.Parser);
            m_net.Add(Constants.Lion_IgnoreFriend, Status.Parser);
            m_net.Add(Constants.Lion_GetFriendSummary, FriendSummaryList.Parser);
            m_net.Add(Constants.Lion_NotifyWeeklyLogin, IntValue.Parser);
            m_net.Add(Constants.Lion_TakeLoginBonus, LongArray.Parser);
            m_net.Add(Constants.Lion_NotifyFreeBonus, LongValue.Parser);
            m_net.Add(Constants.Lion_TakeFreeBonus, LongArray.Parser);
            m_net.Add(Constants.Lion_BuyItem, Status.Parser);
            m_net.Add(Constants.Lion_GetItems, UserItemList.Parser);
            m_net.Add(Constants.Lion_Register, Status.Parser);
            m_net.Add(Constants.Lion_ModPass, Status.Parser);
            m_net.Add(Constants.Lion_RefreshGold, LongValue.Parser);
            m_net.Add(Constants.Lion_GetShopItems, ShopList.Parser);
            m_net.Add(Constants.Lion_BroadcastSystemMessage, StringValue.Parser);

            m_net.Add(-2, null);
            m_net.Add(Constants.Reconnect, null);
            m_net.Add(Constants.Error, Status.Parser);
            m_net.Name = "Reception";  
        }

        if (!m_login)
        {
            // 启动登录
            Lobby lobby = Lobby.getInstance();
            DebugConsole.Log("Loading start:" + lobby.Domain);
            if (lobby.Domain == "")
            {
                SceneManager.LoadSceneAsync("sloading");
            }
            else
            {
                DebugConsole.Log("Loading start2:" + lobby.Domain);
                if (false == m_net.Init(lobby.Domain, lobby.Port))
                {
                    // 这里不重连，在发送请求失败后再重连
                    DebugConsole.Log("Reception:Client init failed!");
                }

                QuickLogin();
            }
        }
        else
        {
            UpdateUserInfoUI();
        }

        // 刷新倒计时
        UpdateCountDown();

        for (int i = 0; i < Constants.LobbyBtn_Strings.Length; ++i)
        {
            string btnName = Constants.LobbyBtn_Strings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }
	}
    int GetBtnIndexFromName(string btnName)
    {
        if (m_btnIndexDict.ContainsKey(btnName))
        {
            return m_btnIndexDict[btnName];
        }
        else
        {
            return -1;
        }
    }
    void QuitGame()
    {
        Application.Quit();
    }
    public void SwitchUser()
    {
        m_net.Close();
        m_net = null;

        GlobalVars.instance.SwitchUser = true;
        DebugConsole.Log("Reception enter start loading scene");
        Global.NextSceneName = "sloading";
        SceneManager.LoadScene("loading");
    }
    void OnClick(GameObject sender)
    {        
        Tools.PlayAudio(Constants.Audio.Audio_LobbyClickButton);

        int btnIndex = GetBtnIndexFromName(sender.name);
        if (btnIndex < 0)
        {
            DebugConsole.Log("Cant find button:" + sender.name);
            return;
        }

        switch ((Constants.LobbyBtn)btnIndex)
        {
            case Constants.LobbyBtn.Btn_Slot:
                {
                    // 检查是否登录
                    if (m_login)
                    {
                        Redirect();
                    }
                    else
                    {
                        WorkDone callBack = new WorkDone(Redirect);
                        QuickLogin(callBack);
                    }
                }
                break;
            case Constants.LobbyBtn.Btn_Poker:
                {
                    //DialogBase.Show("POKER", "Exit game?", QuitGame);
                }
                break;
            case Constants.LobbyBtn.Btn_Option:
                {
                    DialogOption.Show();
                }
                break;
            case Constants.LobbyBtn.Btn_Avatar:
            case Constants.LobbyBtn.Btn_Head:
                {
                    GetProfile(Lobby.getInstance().UId, ShowPersonalInfoDlg);
                }
                break;
            case Constants.LobbyBtn.Btn_Message:
                {
                    DialogMessage.Show();
                }
                break;
            case Constants.LobbyBtn.Btn_Credits:
                {
                    DialogStore.Show(0);
                }
                break;
            case Constants.LobbyBtn.Btn_Gems:
                {
                    DialogStore.Show(1);
                }
                break;
            case Constants.LobbyBtn.Btn_Friends:
                {
                    // 根据ActivePage获取数据
                    GetFriends();                    
                }
                break;
            case Constants.LobbyBtn.Btn_FreeBonus:
                {
                    TakeFreeBonus();
                }
                break;
            case Constants.LobbyBtn.Btn_Bag: // Bag
                {
                    GetItems();
                }
                break;
            case Constants.LobbyBtn.Btn_Bingo:
                {
                    //DoBuy("jb_1");
                }
                break;
            case Constants.LobbyBtn.Btn_Sj:
                {
                    //string uuid = GetUUID();
                    //DialogBase.Show("UUID", uuid);
                }
                break;
            default:
                DialogBase.Show("Button clicked", sender.name);
                break;
        }
    }
    static public void GetShopItems(WorkDone cb)
    {
        StringValue sv = new StringValue();
        string shopName = "GoldShop";
        sv.Value = shopName;
        m_net.SendEnqueue(Constants.Lion_GetShopItems,
            0,
            sv,
            cb);
    }
    static public void DoBuy(string buykey)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (jc == null)
            {
                DialogBase.Show("ANDROID", "js is null");
                return;
            }

            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if (jo == null)
            {
                DialogBase.Show("ANDROID", "jo is null");
                return;
            }

            try
            {
                //string uuid = jo.CallStatic<string>("GetUUID");
                jo.Call("Pay", buykey);
            }
            catch (System.Exception e)
            {
                DialogBase.Show("ANDROID", e.Message);
            }
        }
    }
    private string GetUUID()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (jc == null)
            {
                DialogBase.Show("ANDROID", "js is null");
                return "";
            }

            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if (jo == null)
            {
                DialogBase.Show("ANDROID", "jo is null");
                return "";
            }

            try
            {
                string uuid = jo.Call<string>("GetUID");
                return uuid;
            }
            catch (System.Exception e)
            {
                DialogBase.Show("ANDROID", e.Message);
            }
        }
        return "";
    }
    public void RegisterByEmail(string email, string pwdMd5, WorkDone cb = null)
    {
        RegisterReq regReq = new RegisterReq();
        regReq.Version = 2;
        regReq.Args.Add(email);
        regReq.Args.Add(pwdMd5);

        m_net.SendEnqueue(Constants.Lion_Register,
            0,
            regReq,
            cb);        
    }
    public void ModifyPassword(string pwdMd5, string npwdMd5, WorkDone cb = null)
    {
        RegisterReq regReq = new RegisterReq();
        regReq.Version = 2;
        regReq.Args.Add(pwdMd5);
        regReq.Args.Add(npwdMd5);

        m_net.SendEnqueue(Constants.Lion_ModPass,
            0,
            regReq,
            cb);
    }
    void ShowPersonalInfoDlg()
    {
        DialogPersonalInfo.Show(Lobby.getInstance().UserInfo);
    }
    public void GetProfile(long uId, WorkDone callBack = null)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;

        m_net.SendEnqueue(Constants.Lion_GetProfile,
            0,
            lv,
            callBack);
    }
    public void TakeLoginBonus(WorkDone callBack = null)
    {
        Empty empty = new Empty();

        m_net.SendEnqueue(Constants.Lion_TakeLoginBonus,
            0,
            empty,
            callBack);
    }
    public void TakeFreeBonus(WorkDone callBack = null)
    {
        Empty empty = new Empty();

        m_net.SendEnqueue(Constants.Lion_TakeFreeBonus,
            0,
            empty,
            callBack);
    }
    public void GetItems(WorkDone callBack = null)
    {
        Empty empty = new Empty();

        m_net.SendEnqueue(Constants.Lion_GetItems,
            0,
            empty,
            callBack);
    }
    void ShowFriendsDlg()
    {
        DialogFriends.Show();
    }
    public void GetFriendSummaryByUId(long uId, WorkDone callBack = null)
    {
        LongArray laArray = new LongArray();
        laArray.Data.Add(uId);

        m_net.SendEnqueue(Constants.Lion_GetFriendSummary,
                    0,
                    laArray,
                    callBack);
    }
    public void IgnoreFriend(long uId, WorkDone callBack)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;

        m_net.SendEnqueue(Constants.Lion_IgnoreFriend,
            0,
            lv,
            callBack);
    }
    public void AcceptFriend(long uId, WorkDone callBack)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;

        m_net.SendEnqueue(Constants.Lion_AcceptFriend,
            0,
            lv,
            callBack);
    }
    public void AddFriend(long uId, WorkDone callBack)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;

        m_net.SendEnqueue(Constants.Lion_AddFriend,
            0,
            lv,
            callBack);
    }
    public void RemoveFriend(long uId, WorkDone callBack)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;

        m_net.SendEnqueue(Constants.Lion_DeleteFriend,
            0,
            lv,
            callBack);
    }
    public void GetFriendSummary(WorkDone callBack = null)
    {
        // 初始显示第一页
        LongArray fIdArray = Lobby.getInstance().GetCurrentFriendPageArray();
        DebugConsole.Log("Friend summary count:" + fIdArray.Data.Count);

        m_net.SendEnqueue(Constants.Lion_GetFriendSummary,
            0,
            fIdArray,
            callBack);
    }
    public void HearBeat(WorkDone callBack = null)
    {
        Empty empty = new Empty();
        m_net.SendEnqueue(-2,
            0,
            empty,
            callBack);
    }
    public void GetFriends(WorkDone callBack = null)
    {
        Empty empty = new Empty();
        
        m_net.SendEnqueue(Constants.Lion_GetFriends,
            0,
            empty,
            callBack);
    }
    public void GetFriendRequests(WorkDone callBack = null)
    {
        Empty empty = new Empty();

        m_net.SendEnqueue(Constants.Lion_GetFriendRequests,
            0,
            empty,
            callBack);
    }
    void QuickLogin(WorkDone callBack = null)
    {
        QuickLoginInfo quickLoginInfo = new QuickLoginInfo();
        Lobby lobby = Lobby.getInstance();
        quickLoginInfo.UserId = lobby.UId;
        quickLoginInfo.Key = lobby.Key;

        m_net.SendEnqueue(Constants.Lion_QuickLoginInfo, 
            0,
            quickLoginInfo,
            callBack);
    }
    // 更新名字回调函数
    void AfterUpdateProfileName()
    {
        // 个人信息对话框
        GameObject go = GameObject.Find("InputNickName");
        if (go == null)
            DebugConsole.Log("InputNickName is null");
        else
        {
            InputField field = go.GetComponent<InputField>();
            field.text = m_nick;
        }

        // 主界面更新
        GameObject goAvatar = GameObject.Find("BtnAvatar");
        GameObject goText = goAvatar.transform.Find("Text").gameObject;
        Text text = goText.GetComponent<Text>();
        text.text = m_nick;

        LionUserInfo ui = Lobby.getInstance().UserInfo;
        ui.Name = m_nick;
    }
    public void AfterUpdateProfileHeadImgUrl()
    {        
        GameObject goSrc = DialogSelectAvatar.GetHeadObject(m_headImgUrl);

        if (goSrc != null)
        {
            Lobby.getInstance().UserInfo.HeadImgUrl = m_headImgUrl;
            GameObject goDest =
               DialogPersonalInfo.GetInstance().transform.Find("main").
               transform.Find("BtnUpAvatar").gameObject;
            goDest.GetComponent<Image>().sprite = goSrc.GetComponent<Image>().sprite;
            GameObject.Find("BtnAvatar").transform.Find("BtnHead").GetComponent<Image>().sprite = goSrc.GetComponent<Image>().sprite;
        }
        else
        {
            GameObject goHeadDefault = GameObject.Find("BtnAvatar").transform.Find("BtnHeadDefault").gameObject;
            GameObject goDest =
               DialogPersonalInfo.GetInstance().transform.Find("main").
               transform.Find("BtnUpAvatar").gameObject;
            goDest.GetComponent<Image>().sprite = goHeadDefault.GetComponent<Image>().sprite;
            GameObject.Find("BtnAvatar").transform.Find("BtnHead").GetComponent<Image>().sprite = goHeadDefault.GetComponent<Image>().sprite;
        }
    }
    public void BuyItem(string packageName, 
        string sku, 
        string token, 
        string orderId)
    {
        // Android buyType = "GooglePlay"
        string androidBuyType = "GooglePlay";
        StringArray sa = new StringArray();
        sa.Data.Add(androidBuyType);
        sa.Data.Add(packageName);
        sa.Data.Add(sku);
        sa.Data.Add(token);
        sa.Data.Add(orderId);

        m_net.SendEnqueue(Constants.Lion_BuyItem,
            0,
            sa,
            null);
    }
    public void UpdateProfileName(string nickName)
    {
        LionUserInfo ui = Lobby.getInstance().UserInfo;
        if (nickName == "" || nickName == ui.Name)
            return;

        StringArray sa = new StringArray();
        sa.Data.Add("Name");
        sa.Data.Add(nickName);
        m_nick = nickName;

        m_net.SendEnqueue(Constants.Lion_UpdateProfile,
            0,
            sa,
            AfterUpdateProfileName);
    }
    public void UpdateProfileHeadImgUrl(string headImgUrl)
    {
        StringArray sa = new StringArray();
        sa.Data.Add("HeadImgUrl");
        sa.Data.Add(headImgUrl);
        m_headImgUrl = headImgUrl;

        m_net.SendEnqueue(Constants.Lion_UpdateProfile,
            0,
            sa,
            AfterUpdateProfileHeadImgUrl);
    }
    public void GetTigerStatInfo(long uId, WorkDone cb)
    {
        LongValue lv = new LongValue();
        lv.Value = uId;
        m_net.SendEnqueue(Constants.Lion_GetTigerStat,
            0,
            lv,
            cb);        
    }
    void CheckLogin()
    {
        if (!m_login)
        {
            QuickLogin();
        }
    }

    void Redirect()
    {        
        RedirectReq rdReq = new RedirectReq();
        rdReq.UserId = Lobby.getInstance().UId;
        rdReq.Svc = Constants.Svc_Tiger; // 重定向到老虎机
        rdReq.Version = 1;
        rdReq.SubSvc = 0;

        m_net.SendEnqueue(Constants.Lion_Redirect, 0, rdReq);
    }
    void OnApplicationQuit()
    {
        m_net.Close();
    }
    void UpdateUserInfoUI()
    {
        // 大厅主窗口只刷新金币、名称、等级和头像
        LionUserInfo ui = Lobby.getInstance().UserInfo;
        
        // NickName
        GameObject goAvatar = GameObject.Find("BtnAvatar");
        GameObject goText = goAvatar.transform.Find("Text").gameObject;
        Text text = goText.GetComponent<Text>();
        text.text = ui.Name;

        // Coins
        GameObject goCredits = GameObject.Find("BtnCredits");
        goText = goCredits.transform.Find("Text").gameObject;
        text = goText.GetComponent<Text>();
        text.text = Tools.CoinToString(ui.Gold);
        
        // Level
        GameObject goStar = GameObject.Find("xp_star");
        goText = goStar.transform.Find("txtLevel").gameObject;
        text = goText.GetComponent<Text>();
        text.text = ui.Level.ToString();

        //  Head
        if (ui.HeadImgUrl != "")
        {
            m_headImgUrl = ui.HeadImgUrl;
            //int headIndex = Tools.StringToInt32(m_headImgUrl);            
            //StartCoroutine(Tools.LoadWWWImageToButton(ui.HeadImgUrl, "BtnHead"));
        }
        AfterUpdateProfileHeadImgUrl();
    }
    public void StartCountDown(long val)
    {
        string btnStr = Constants.LobbyBtn_Strings[(int)Constants.LobbyBtn.Btn_FreeBonus];
        GameObject go = GameObject.Find(btnStr);
        CountDown cd = go.transform.Find("Text").gameObject.GetComponent<CountDown>();
        cd.LeftTime = val;

        go.GetComponent<Button>().interactable = false;
        go.transform.Find("Coin").gameObject.SetActive(false);
        go.transform.Find("Text").gameObject.SetActive(true);
    }
    public void UpdateCountDown()
    {
        long epoch = Lobby.getInstance().FreeBonusEpoch;
        if (epoch < 0)
        {
            DebugConsole.Log("epoch < 0");
            return;
        }

        // add one second
        epoch += 1000;

        long curEpoch = (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        if (epoch == 0 || curEpoch >= epoch)
        {
            EndCountDown();
        }
        else
        {

            long msDiff = epoch - curEpoch;
            double temp = (double)msDiff / 1000.0;
            temp += 0.5; // 向上取整，保证时间到了
            StartCountDown((long)temp);
        }
    }
    public void EndCountDown()
    {
        string btnStr = Constants.LobbyBtn_Strings[(int)Constants.LobbyBtn.Btn_FreeBonus];
        GameObject go = GameObject.Find(btnStr);
        CountDown cd = go.transform.Find("Text").gameObject.GetComponent<CountDown>();
        if (cd.LeftTime > 0)
            cd.LeftTime = 0;
        
        go.GetComponent<Button>().interactable = true;
        go.transform.Find("Text").gameObject.SetActive(false);
        go.transform.Find("Coin").gameObject.SetActive(true);

        ExplodeCoin.Show(Constants.Bonus_Free);
    }
    public void FlyCoin(int type)
    {
        GameObject go = GameObject.Find("BtnCredits");
        ExplodeCoin.MoveTo(type, go.transform.position);        
    }
    void JumpAndMoveCoins(int cc, WorkDone cb)
    {
        if (cc > 20)
            cc = 20;

        JumpCoin.Init(cb);
        // 动态金币个数
        JumpCoin.sTotalCoinCount = cc; // max 20
        for (int i = 0; i < JumpCoin.sTotalCoinCount; ++i)
        {
            string jcName = "JumpCoin" + (i + 1).ToString();
            GameObject goJc = GameObject.Find("JumpCoins").transform.Find("Group").transform.Find(jcName).gameObject;
            goJc.SetActive(true);

            JumpCoin jc = goJc.GetComponent<JumpCoin>();
            jc.JumpAndMove();
        }
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (DialogBase.Actived())
            {
                DebugConsole.Log("Hide");
                DialogBase.Hide();
            }
            else
            {
                DebugConsole.Log("Show");
                DialogBase.Show("ESC", "Are you sure to exit game?", QuitGame);
            }
        }
        
        if (m_broadcastMsg != "")
        { 
            // 有系统消息，平移吧            
            GameObject goBroadcast = GameObject.Find("BroadcastText");
            Vector3 pos = goBroadcast.transform.localPosition;
            pos.x -= 50 * Time.deltaTime;
            goBroadcast.transform.localPosition = pos;

            // 从600～-600
            if (goBroadcast.transform.localPosition.x < -600)
                m_broadcastMsg = "";
        }
        else
        {
            m_broadcastMsg = Lobby.getInstance().GetBroadcast();
            if (m_broadcastMsg != "")
            {
                GameObject goBroadcast = GameObject.Find("BroadcastText");
                goBroadcast.GetComponent<Text>().text = m_broadcastMsg;
                goBroadcast.transform.localPosition = new Vector3(600, 0, 0);
            }
        }

        if (m_net == null || !m_net.IsRunning())
        {
            // 主动结束了
            return;
        }

        if (m_net.CheckReconnect())
        {
            CheckLogin();

            DialogReconnect.Hide();
        }

        ProtoPacket packet = new ProtoPacket();
        if (m_net.RecvTryDequeue(ref packet))
        {
            DebugConsole.Log("Reception handle cmdId:" + packet.cmdId);
            switch (packet.cmdId)
            {
                case Constants.Lion_QuickLoginInfo:
                    {
                        Lobby.getInstance().UserInfo = (LionUserInfo)packet.proto;// 更新LionUser
                        m_login = true;
                        UpdateUserInfoUI(); // 更新大厅主界面中信息
                        if (packet.callback != null)
                        {
                            packet.callback();
                        }
                    }
                    break;
                case Constants.Lion_GetProfile:
                    {
                        LionUserInfo usrInfo = (LionUserInfo)packet.proto;// 更新LionUser
                        if (usrInfo.UserId == Lobby.getInstance().UId)
                        {
                            Lobby.getInstance().UserInfo = usrInfo;
                        }
                        Lobby.getInstance().QueryUserInfo = usrInfo;

                        if (packet.callback != null)
                        {
                            // 通常这里显示个人信息对话框
                            packet.callback();
                        }
                    }
                    break;
                case Constants.Lion_GetTigerStat:
                    {
                        Lobby.getInstance().TigerStatInfo = (TigerStat)packet.proto;
                        if (packet.callback != null)
                        {
                            packet.callback();
                        }
                    }
                    break;
                case Constants.Lion_GetFriendRequests:                   
                case Constants.Lion_GetFriends:
                    {
                        Lobby.getInstance().FriendIDArray = (LongArray)packet.proto;
                        if (packet.callback != null)
                        {
                            GetFriendSummary(packet.callback);
                        }
                        else
                        {
                            GetFriendSummary(ShowFriendsDlg);
                        }
                    }
                    break;
                case Constants.Lion_GetFriendSummary:
                    {
                        Lobby.getInstance().CurrentSummaryList = (FriendSummaryList)packet.proto;
                        DebugConsole.Log("Summary count:" + Lobby.getInstance().CurrentSummaryList.Data.Count);
                        if (packet.callback != null)
                        {
                           packet.callback();
                        }
                    }
                    break;
                case Constants.Lion_IgnoreFriend:
                case Constants.Lion_AcceptFriend:
                case Constants.Lion_AddFriend:
                case Constants.Lion_DeleteFriend:
                    {
                        Status stat = (Status)packet.proto;
                        if (stat.Code == 0)// successful
                        {
                            if (packet.callback != null)
                            {
                                packet.callback();
                            }
                        }
                        else
                        {
                            DebugConsole.Log(stat.Desc);
                        }
                    }
                    break;
                case Constants.Lion_Redirect:
                    {
                        Lobby.getInstance().RedirectInfo = (RedirectResp)packet.proto;
                        // 切换到游戏场景中
                        //m_net.Close();
                        DebugConsole.Log("Reception enter slot scene");
                        Global.NextSceneName = "slot";
                        SceneManager.LoadScene("loading");
                    }
                    break;
                case Constants.Lion_UpdateProfile:
                    {
                        DebugConsole.Log("Lion_UpdateProfile");
                        Status stat = (Status)packet.proto;
                        if (stat.Code == 0)// successful
                        {
                            if (packet.callback != null)
                            {
                                packet.callback();
                            }
                        }
                        else
                        {
                            DebugConsole.Log(stat.Desc);
                        }
                    }
                    break;
                case Constants.Lion_NotifyWeeklyLogin:
                    {
                        // 连续登录奖励
                        // NotifyWeeklyLogin 返回的intvalue是0-6，0表示今天登陆了（昨天没登录）
                        IntValue iv = (IntValue)packet.proto;
                        DialogDailyBonus.Show(iv.Value);

                        //ExplodeCoin.Show();
                    }
                    break;
                case Constants.Lion_TakeLoginBonus:
                    {
                        LongArray la = (LongArray)packet.proto;
                        // la[0] 奖励金币数
                        // la[1] 最终总数
                        if (la.Data.Count >= 2)
                        {
                            Lobby.getInstance().UserInfo.Gold = la.Data[1];
                            // 若有动画，在此添加
                            JumpAndMoveCoins(6, UpdateUserInfoUI);
                        }
                    }
                    break;
                case Constants.Lion_NotifyFreeBonus:
                    {
                        // 第一次登陆的时候，数据库里面没有数据，所以返回0
                        // 后端推送，倒计时剩余时间长度（毫秒），如果小于等于0，直接显示奖励
                        // 免费奖励
                        LongValue lv = (LongValue)packet.proto;
                        long curEpoch = (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                        Lobby.getInstance().FreeBonusEpoch = curEpoch + lv.Value;
                        UpdateCountDown();
                    }
                    break;
                case Constants.Lion_TakeFreeBonus:
                    {
                        LongArray la = (LongArray)packet.proto;
                        if (la.Data.Count >= 2)
                        {
                            Lobby.getInstance().UserInfo.Gold = la.Data[1];
                            // 若有动画，在此添加
                            FlyCoin(Constants.Bonus_Free);
                            UpdateUserInfoUI();
                        }
                    }
                    break;
                case Constants.Lion_BuyItem:
                    {
                        Status stat = (Status)packet.proto;
                        // 更新购买相关的：金币 or 背包
                        DebugConsole.Log("Buy item return:" + stat.Code.ToString());
                        if (stat.Code == 0)// successful
                        {
                            if (packet.callback != null)
                            {
                                packet.callback();
                            }
                        }
                        else
                        {
                            DebugConsole.Log(stat.Desc);
                        }                        
                    }
                    break;
                case Constants.Lion_Register:
                    {
                        Status stat = (Status)packet.proto;
                        DebugConsole.Log("Register by email return:" + stat.Code.ToString());
                        if (stat.Code == 0)// successful
                        {
                            if (packet.callback != null)
                            {
                                packet.callback();
                            }
                        }
                        else
                        {
                            DebugConsole.Log(stat.Desc);
                            DialogBase.Show("Register by email", "Error:" + stat.Desc);
                        }                
                    }
                    break;
                case Constants.Lion_ModPass:
                    {
                        Status stat = (Status)packet.proto;
                        DebugConsole.Log("Modify password return:" + stat.Code.ToString());
                        if (stat.Code == 0)// successful
                        {
                            if (packet.callback != null)
                            {
                                packet.callback();
                            }
                        }
                        else
                        {
                            DebugConsole.Log(stat.Desc);
                            DialogBase.Show("Modify password", "Error:" + stat.Desc);
                        }
                    }
                    break;
                case Constants.Lion_RefreshGold:
                    {
                        LongValue lv = (LongValue)packet.proto;
                        DebugConsole.Log("Refresh gold:" + lv.ToString());
                        Lobby.getInstance().UserInfo.Gold = lv.Value;
                        UpdateUserInfoUI();
                    }
                    break;
                case Constants.Lion_GetShopItems:
                    {
                        Lobby.getInstance().ShopList = (ShopList)packet.proto;
                        DebugConsole.Log("ShopName:" + Lobby.getInstance().ShopList.ShopName);
                        if (packet.callback != null)
                        {
                            packet.callback();
                        }
                    }
                    break;
                case Constants.Lion_BroadcastSystemMessage:
                    {
                        Tools.PlayNotification(Constants.Audio.Audio_Notification);
                        StringValue sv = (StringValue)packet.proto;
                        Lobby.getInstance().AddBroadcast(sv.Value);
                    }
                    break;
                case Constants.Lion_GetItems:
                    {
                        //
                        Lobby.getInstance().UserItemList = (UserItemList)packet.proto;                       

                        if (packet.callback != null)
                        {
                            packet.callback();
                        }
                        else
                        {
                            DialogBag.Show(null);
                        }
                    }
                    break;
                case Constants.Reconnect:
                    {
                        // 展示重连对话框，直到重连成功                        
                        if (packet.msgId == 1)
                        {
                            ProtoNet.WriteLog("Reconnecting...");
                            // 3s后Display中重连
                            m_net.CheckReconnect(3);
                            DialogReconnect.Show();
                        }
                    }
                    break;
                case Constants.Error:
                    {
                        // 展示错误
                        Status stat = (Status)packet.proto;
                        string err = "Error:" + stat.Code.ToString() + "-" + stat.Desc;
                        DialogBase.Show("ERROR", err);
                    }
                    break;
                default:
                    {
                        DebugConsole.Log("Reception invalid cmdId:" + packet.cmdId);
                    }
                    break;
            }
        }
	}
}
