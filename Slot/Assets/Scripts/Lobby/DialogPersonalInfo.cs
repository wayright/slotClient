using System.Collections;
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
        ProfileOn, ProfileOff,
        RecordsOn, RecordsOff,
        UpAvatar, UploadAvatar, // the same 
        AddFriend, RemoveFriend,
    };
    public static string[] DialogBtnStrings = { "BtnDPClose",
                            "BtnProfileOn", "BtnProfileOff",
                            "BtnRecordsOn", "BtnRecordsOff",
                            "BtnUpAvatar", "BtnUploadAvatar",
                            "BtnAddFriend", "BtnRemoveFriend"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();    
    public LionUserInfo UserInfo
    {
        get { return m_userInfo; }
        set { m_userInfo = value; }
    }

    // buttons obj
    GameObject profileOnObj, profileOffObj;
    GameObject recordsOnObj, recordsOffObj;
    GameObject profileTabObj, recordsTabObj;

    // 区分自身还是其他人，区分朋友还是非朋友
    GameObject selfObj, otherObj;
    GameObject inputNickNameObj, valNickNameObj;

    // privates
    private bool m_profileOn = true;
    private LionUserInfo m_userInfo;
    public int ActivePage
    {
        get 
        {
            if (m_profileOn)
                return 0;
            else
                return 1;
        }

        set 
        {
            m_profileOn = (value == 0);
            UpdateUI();
        }
    }

    public void InitBtn()
    {
        for (int i = 0; i < DialogBtnStrings.Length; ++i)
        {
            string btnName = DialogBtnStrings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
            if (null == btnObj)
                Debug.Log(btnName + " is null!");
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
        dlg.ActivePage = 0;
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

        // 头像
        if (ui.HeadImgUrl != "")
        {
            //StartCoroutine(Tools.LoadWWWImageToButton(ui.HeadImgUrl, "BtnUpAvatar"));
        }
    }
    
    void UpdateUI()
    {
        if (profileOnObj == null)
            return;

        profileOnObj.SetActive(!m_profileOn);
        profileOffObj.SetActive(m_profileOn);
        recordsOnObj.SetActive(m_profileOn);
        recordsOffObj.SetActive(!m_profileOn);

        profileTabObj.SetActive(m_profileOn);
        recordsTabObj.SetActive(!m_profileOn);

        // 区分自己和他人
        bool isSelf = m_userInfo.UserId == Lobby.getInstance().UId;
        otherObj.SetActive(!isSelf);
        selfObj.SetActive(isSelf);

        // 区分朋友
        bool isFriend = IsFriend();
        otherObj.transform.Find("BtnAddFriend").gameObject.SetActive(!isFriend);
        otherObj.transform.Find("BtnRemoveFriend").gameObject.SetActive(isFriend);

        otherObj.transform.Find("BtnGiveGifts").GetComponent<Button>().interactable = false;
        otherObj.transform.Find("BtnGiveChips").GetComponent<Button>().interactable = false;
        otherObj.transform.Find("BtnLike").GetComponent<Button>().interactable = false;
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
        Debug.Log(sender.name);
        int btnIndex = GetBtn(sender.name);
        if (btnIndex < 0)
        {
            Debug.Log("Cant find button:" + sender.name);
            return;
        }
        switch ((DialogBtn)btnIndex)
        {
            case DialogBtn.Close:
                {
                    GameObject btnObj = GameObject.Find(DialogName);
                    if (null == btnObj)
                    {
                        Debug.Log("null");
                    }
                    else
                    {
                        Debug.Log("DoHide");
                        DoHide(btnObj);
                    }
                }
                break;
            case DialogBtn.ProfileOff:
            case DialogBtn.RecordsOff:
                {
                    // Do nothing
                }
                break;
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
                break;
            case DialogBtn.UpAvatar:
            case DialogBtn.UploadAvatar:
                {
                    //Debug.Log("Upload...");
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
            default:
                break;
        }       
    }
	// Use this for initialization
    new void Start()
    {
        InitBtn();

        profileOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.ProfileOn]);
        profileOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.ProfileOff]);

        recordsOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RecordsOn]);
        recordsOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RecordsOff]);

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
