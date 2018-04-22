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
    private bool m_login; // 是否登录
    private ProtoNet m_net; // 网络-Dog&Lion
    private Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private string m_nick; // 临时昵称名

    // Use this for initialization
	void Start () {
        m_login = false;

        // 初始化ProtoNet
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
        m_net.Add(-2, null);
        m_net.Add(Constants.Reconnect, null);
        m_net.Add(Constants.Error, null);
        m_net.Name = "Reception";

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

        // 启动登录
        Lobby lobby = Lobby.getInstance();
        Debug.Log("Loading start:" + lobby.Domain);
        if (lobby.Domain == "")
        {            
            SceneManager.LoadSceneAsync("sloading");
        }
        else
        {
            Debug.Log("Loading start2:" + lobby.Domain);
            if (false == m_net.Init(lobby.Domain, lobby.Port))
            {
                // 这里不重连，在发送请求失败后再重连
                Debug.Log("Reception:Client init failed!");
            }

            QuickLogin();
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
    void OnClick(GameObject sender)
    {
        int btnIndex = GetBtnIndexFromName(sender.name);
        if (btnIndex < 0)
        {
            Debug.Log("Cant find button:" + sender.name);
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
                    DialogBase.Show("POKER", "Exit game?", Application.Quit());
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
            default:
                DialogBase.Show("Button clicked", sender.name);
                break;
        }
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
        Debug.Log("Friend summary count:" + fIdArray.Data.Count);

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
            Debug.Log("InputNickName is null");
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
            StartCoroutine(Tools.LoadWWWImageToButton(ui.HeadImgUrl, "BtnHead"));
        }
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
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (DialogBase.Actived())
            {
                Debug.Log("Hide");
                DialogBase.Hide();
            }
            else
            {
                Debug.Log("Show");
                DialogBase.Show("ESC", "Are you sure to exit game?");
            }
        }

        if (!m_net.IsRunning())
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
            Debug.Log("Reception handle cmdId:" + packet.cmdId);
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
                        Debug.Log("Summary count:" + Lobby.getInstance().CurrentSummaryList.Data.Count);
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
                            Debug.Log(stat.Desc);
                        }
                    }
                    break;
                case Constants.Lion_Redirect:
                    {
                        Lobby.getInstance().RedirectInfo = (RedirectResp)packet.proto;
                        // 切换到游戏场景中
                        m_net.Close();
                        Debug.Log("Reception enter slot scene");
                        Global.NextSceneName = "slot";
                        SceneManager.LoadScene("loading");
                    }
                    break;
                case Constants.Lion_UpdateProfile:
                    {
                        Debug.Log("Lion_UpdateProfile");
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
                            Debug.Log(stat.Desc);
                        }
                    }
                    break;
                case Constants.Lion_NotifyWeeklyLogin:
                    {
                        // 连续登录奖励
                        IntValue iv = (IntValue)packet.proto;
                        DialogDailyBonus.Show(iv.Value);
                    }
                    break;
                case Constants.Lion_TakeLoginBonus:
                    {
                        LongArray la = (LongArray)packet.proto;
                        Debug.Log("Take login bonus!");
                    }
                    break;
                case Constants.Lion_NotifyFreeBonus:
                    {
                        // 免费奖励
                        LongValue lv = (LongValue)packet.proto;                              
                        if (lv.Value == 0)
                        {
                            EndCountDown();
                        }
                        else
                        {
                            Debug.Log("Lion_NotifyFreeBonus:" + lv.Value);
                            StartCountDown(5);
                        }
                    }
                    break;
                case Constants.Lion_TakeFreeBonus:
                    {
                        LongArray la = (LongArray)packet.proto;
                        Debug.Log("Take free bonus!");
                    }
                    break;
                case Constants.Reconnect:
                    {
                        // 展示重连对话框，直到重连成功
                        ProtoNet.WriteLog("Reconnecting...");
                        if (packet.msgId > 0)
                        {
                            // 3s后Display中重连
                            m_net.CheckReconnect(3);
                            DialogReconnect.Show();
                        }
                    }
                    break;
                case Constants.Error:
                    {
                        // 展示错误
                    }
                    break;
                default:
                    {
                        Debug.Log("Reception invalid cmdId:" + packet.cmdId);
                    }
                    break;
            }
        }
	}
}
