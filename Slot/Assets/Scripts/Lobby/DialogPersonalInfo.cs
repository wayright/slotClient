﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lion.Proto;
public class DialogPersonalInfo : DialogBase
{
    public const string DialogName = "DialogPersonalInfo";
    public enum DialogBtn
    {
        Close = 0,
        UpAvatar, 
        ModifyPassword,
        RegEmail,
        AddFriend, RemoveFriend,
    };
    public static string[] DialogBtnStrings = { "BtnDPClose",
                            "BtnUpAvatar", "BtnModifyPassword",
                            "BtnRegEmail",
                            "BtnAddFriend", "BtnRemoveFriend"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();    
    public LionUserInfo UserInfo
    {
        get { return m_userInfo; }
        set { m_userInfo = value; }
    }

    // buttons obj
    GameObject profileTabObj, recordsTabObj;

    // 区分自身还是其他人，区分朋友还是非朋友
    GameObject selfObj, otherObj;
    GameObject inputNickNameObj, valNickNameObj;

    // privates
    private bool m_profileOn = true;
    private LionUserInfo m_userInfo;

    public void InitBtn()
    {
        for (int i = 0; i < DialogBtnStrings.Length; ++i)
        {
            string btnName = DialogBtnStrings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
            if (null == btnObj)
                DebugConsole.Log(btnName + " is null!");
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }
    }
    public int GetBtn(string btnName)
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

    public static void Show(LionUserInfo usrInfo, string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogPersonalInfo dlg = obj.GetComponent<DialogPersonalInfo>();
        // 以下几句注意顺序
        // 先设置用户信息
        dlg.UserInfo = usrInfo;
        dlg.DoShow(obj, str);
        dlg.UpdateUserInfo(); // 默认第一个页面
    }
    public static DialogPersonalInfo GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogPersonalInfo dlg = obj.GetComponent<DialogPersonalInfo>();

        return dlg;
    }
    public void UpdateUserInfo()
    {
        if (null == inputNickNameObj)
            return;

        LionUserInfo ui = m_userInfo;

        // 区分自己和他人
        bool isSelf = m_userInfo.UserId == Lobby.getInstance().UId;

        // NickName
        if (isSelf)
        {
            valNickNameObj.SetActive(false);
            inputNickNameObj.SetActive(true);
            InputField field = inputNickNameObj.GetComponent<InputField>();
            field.text = ui.Name;
        }
        else
        {
            inputNickNameObj.SetActive(false);
            valNickNameObj.SetActive(true);
            valNickNameObj.GetComponent<Text>().text = ui.Name;
        }

        // Level
        GameObject.Find("valLevel").GetComponent<Text>().text = ui.Level.ToString();

        // Coins
        GameObject.Find("valCoins").GetComponent<Text>().text = Tools.CoinToString(ui.Gold);

        // Location
        GameObject.Find("valLocation").GetComponent<Text>().text = ui.Location;

        // Like
        GameObject.Find("valLike").GetComponent<Text>().text = ui.Praise.ToString();

        // UUID
        GameObject.Find("valUId").GetComponent<Text>().text = ui.UserId.ToString();

        // 头像
        //if (ui.HeadImgUrl != "")
        {
            GameObject goSrc = DialogSelectAvatar.GetHeadObject(ui.HeadImgUrl); ;

            if (goSrc != null)
            {
                GameObject goDest =
                   DialogPersonalInfo.GetInstance().transform.Find("main").
                   transform.Find("BtnUpAvatar").gameObject;
                goDest.GetComponent<Image>().sprite = goSrc.GetComponent<Image>().sprite;
            }
            else
            {
                GameObject goHeadDefault = GameObject.Find("BtnAvatar").transform.Find("BtnHeadDefault").gameObject;
                GameObject goDest =
                   DialogPersonalInfo.GetInstance().transform.Find("main").
                   transform.Find("BtnUpAvatar").gameObject;
                goDest.GetComponent<Image>().sprite = goHeadDefault.GetComponent<Image>().sprite;
            }
        }

        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.GetTigerStatInfo(m_userInfo.UserId, UpdateTigerStatUI);
    }
    
    void UpdateUI()
    {
        if (profileTabObj == null)
            return;

        //profileTabObj.SetActive(m_profileOn);
        //recordsTabObj.SetActive(!m_profileOn);

        // 区分自己和他人
        bool isSelf = m_userInfo.UserId == Lobby.getInstance().UId;
        //otherObj.SetActive(!isSelf);
        //selfObj.SetActive(isSelf);

        // 区分朋友
        bool isFriend = IsFriend();
        GameObject.Find("BtnAddFriend").GetComponent<Button>().interactable = !isSelf && !isFriend;
        GameObject.Find("BtnRemoveFriend").GetComponent<Button>().interactable = !isSelf && isFriend;

        GameObject.Find("BtnModifyPassword").GetComponent<Button>().interactable =
            isSelf && (GlobalVars.instance.LoginType == Constants.Login_Email);
        GameObject.Find("BtnRegEmail").GetComponent<Button>().interactable =
            isSelf && (GlobalVars.instance.LoginType == Constants.Login_UUID);

        GameObject.Find("BtnGiveGifts").GetComponent<Button>().interactable = false;
        GameObject.Find("BtnGiveChips").GetComponent<Button>().interactable = false;
        GameObject.Find("BtnLike").GetComponent<Button>().interactable = false;        
    }
    bool IsFriend()
    {
        if (null == Lobby.getInstance().FriendIDArray)
            return false;

        for (int i = 0;i < Lobby.getInstance().FriendIDArray.Data.Count; ++i)
        {
            if (Lobby.getInstance().FriendIDArray.Data[i] == m_userInfo.UserId)
                return true;
        }
        return false;
    }
    void UpdateTigerStatUI()
    {
        UpdateUI();

        TigerStat stat = Lobby.getInstance().TigerStatInfo;

        GameObject.Find("valTotalWinnings").GetComponent<Text>().text = stat.TotalWin.ToString();
        GameObject.Find("valBiggestWin").GetComponent<Text>().text = stat.BiggestWin.ToString();
        GameObject.Find("valSpinsWon").GetComponent<Text>().text = stat.WinSpins.ToString();
        GameObject.Find("valTotalSpins").GetComponent<Text>().text = stat.TotalSpins.ToString();        
    }
    void UpdateRelateUI()
    {
        if (DialogFriends.instance().ActivePage == 0)
        {
            DialogFriends.instance().UpdateFriendsUI();
        }
        UpdateUI();
    }
    void AfterModifyFriend()
    {
        // 增加删除好友
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.GetFriends(UpdateRelateUI);
    }
    void AddFriend()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.AddFriend(m_userInfo.UserId, AfterModifyFriend);
    }
    void RemoveFriend()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.RemoveFriend(m_userInfo.UserId, AfterModifyFriend);
    }
    void UpdateAvatar()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.UpdateProfileHeadImgUrl(DialogSelectAvatar.GetInstance().HeadIndex.ToString());
    }
    void OnClick(GameObject sender)
    {
        Tools.PlayAudio(Constants.Audio.Audio_LobbyClickButton);

        DebugConsole.Log(sender.name);
        int btnIndex = GetBtn(sender.name);
        if (btnIndex < 0)
        {
            DebugConsole.Log("Cant find button:" + sender.name);
            return;
        }
        switch ((DialogBtn)btnIndex)
        {
            case DialogBtn.Close:
                {
                    GameObject btnObj = GameObject.Find(DialogName);
                    if (null == btnObj)
                    {
                        DebugConsole.Log("null");
                    }
                    else
                    {
                        DebugConsole.Log("DoHide");
                        DoHide(btnObj);
                    }
                }
                break;
                /*
            case DialogBtn.ProfileOn:
            case DialogBtn.RecordsOn:
                {
                    m_profileOn = !m_profileOn;
                    // 若是老虎机统计信息，这里需要手动获取一次
                    if (!m_profileOn)
                    {
                        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                        recp.GetTigerStatInfo(m_userInfo.UserId, UpdateTigerStatUI);
                    }
                    else
                    {
                        UpdateUI();
                    }
                }
                break;*/
            case DialogBtn.UpAvatar:
                {
                    //DebugConsole.Log("Upload...");
                    DialogSelectAvatar.Show(UpdateAvatar);
                }
                break;
            case DialogBtn.AddFriend:
                {                
                    DialogBase.Show("FRIEND REQUEST", "Request to add Friend?", AddFriend);
                }
                break;
            case DialogBtn.RemoveFriend:
                {
                    DialogBase.Show("FRIEND REMOVE", "Are you sure to remove?", RemoveFriend);
                }
                break;
            case DialogBtn.RegEmail:
                {
                    DialogRegEmail.Show();
                }
                break;
            case DialogBtn.ModifyPassword:
                {
                    DialogModifyPass.Show();
                }
                break;
            default:
                break;
        }       
    }
	// Use this for initialization
    new void Start()
    {
        InitBtn();

        //profileOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.ProfileOn]);
        //profileOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.ProfileOff]);

        //recordsOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RecordsOn]);
        //recordsOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RecordsOff]);

        profileTabObj = GameObject.Find("ProfileTable");
        recordsTabObj = GameObject.Find("RecordsTable");

        otherObj = GameObject.Find("ProfileOther");
        selfObj = GameObject.Find("ProfileSelf");

        inputNickNameObj = GameObject.Find("InputNickName");
        valNickNameObj = GameObject.Find("valNickName");

        // 依据全局变量显示按钮
        UpdateUI();
        UpdateUserInfo();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
