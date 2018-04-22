using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lion.Proto;
public class DialogFriends : DialogBase
{
    public const string DialogName = "DialogFriends";
    public enum DialogBtn
    {
        Close = 0,
        FriendsOn, FriendseOff,
        SearchOn, SearchOff,
        RequestOn, RequestOff,
        Invite,
        FriendNextPage,
        FriendLastPage,
        SearchFriend,
        ViewFriend1, ViewFriend2, ViewFriend3, ViewFriend4, ViewFriend5, ViewFriend6,
        ViewSearchResult,
        RequestNextPage,
        RequestLastPage,
        IngorFriend1, IngorFriend2, IngorFriend3, IngorFriend4, IngorFriend5, IngorFriend6,
        AcceptFriend1, AcceptFriend2, AcceptFriend3, AcceptFriend4, AcceptFriend5, AcceptFriend6,
    };
    public static string[] DialogBtnStrings = { "BtnDFClose",
                            "BtnFriendsOn", "BtnFriendsOff",
                            "BtnSearchOn", "BtnSearchOff",
                            "BtnRequestOn", "BtnRequestOff",
                                              "BtnInvite",
                            "BtnFriendNextPage", "BtnFriendLastPage",
                                              "BtnSearchFriend",
    "BtnViewFriend1","BtnViewFriend2","BtnViewFriend3","BtnViewFriend4","BtnViewFriend5","BtnViewFriend6",
                                              "BtnViewSearchResult",
                            "BtnRequestNextPage", "BtnRequestLastPage",
    "BtnIngoreFriend1","BtnIngoreFriend2", "BtnIngoreFriend3","BtnIngoreFriend4", "BtnIngoreFriend5","BtnIngoreFriend6",
    "BtnAcceptFriend1","BtnAcceptFriend2", "BtnAcceptFriend3","BtnAcceptFriend4", "BtnAcceptFriend5","BtnAcceptFriend6",};

    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

    // buttons obj
    GameObject friendsOnObj, friendsOffObj;
    GameObject searchOnObj, searchOffObj;
    GameObject requestOnObj, requestOffObj;
    GameObject friendsTabObj, searchTabObj, requestTabObj;
    GameObject searchRecordObj;
    private int m_activePage = 0;
    private string m_searchCode;
    private long m_opId = 0;

    public string SearchCode
    {
        get { return m_searchCode; }
        set { m_searchCode = value; }
    }
    public int ActivePage
    {
        get
        {
            return m_activePage;
        }

        set
        {
            m_activePage = value;
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

    public static DialogFriends instance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogFriends dlg = obj.GetComponent<DialogFriends>();

        return dlg;
    }    
    public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogFriends dlg = obj.GetComponent<DialogFriends>();
        dlg.ActivePage = 0;
        dlg.DoShow(obj, str);
    }
    public void UpdateFriendsUI()
    {
        GameObject goLayout = friendsTabObj.transform.Find("FriendList").transform.Find("FriendGridLayout").gameObject;
        FriendSummaryList fslist = Lobby.getInstance().CurrentSummaryList;
        List<GameObject> friendRecords = new List<GameObject>();
        for (int i = 0; i < Constants.PageItemCount; ++i)
        {
            string fr = "FriendRecord" + (i + 1).ToString();
            GameObject go = goLayout.transform.Find(fr).gameObject;
            friendRecords.Add(go);
            go.SetActive(false);
        }

        // 保证list的个数小于PageItemCount
        int showCount = fslist.Data.Count;
        if (fslist.Data.Count > Constants.PageItemCount)
        {
            showCount = Constants.PageItemCount;
            Debug.Log("list.Data.Count > Constants.PageItemCount");
        }

        for (int i = 0; i < showCount; ++i)
        {
            GameObject go = friendRecords[i];
            go.SetActive(true);
            go.transform.Find("txtFriendName").GetComponent<Text>().text = fslist.Data[i].Name;
            go.transform.Find("txtFriendLevel").GetComponent<Text>().text = "Lv." + fslist.Data[i].Level.ToString();
        }

        GameObject goLastPage = friendsTabObj.transform.Find(DialogBtnStrings[(int)DialogBtn.FriendLastPage]).gameObject;
        GameObject goNextPage = friendsTabObj.transform.Find(DialogBtnStrings[(int)DialogBtn.FriendNextPage]).gameObject; 
        if (0 == Lobby.getInstance().CurrentFriendPage)
        {            
            goLastPage.GetComponent<Button>().interactable = false;
        }
        else
        {
            goLastPage.GetComponent<Button>().interactable = true;
        }

        if (Lobby.getInstance().CurrentFriendPage == Lobby.getInstance().FriendPageCount - 1)
        {
            goNextPage.GetComponent<Button>().interactable = false;
        }
        else
        {
            goNextPage.GetComponent<Button>().interactable = true;
        }

        // 准备工作完毕，可以显示界面了
        UpdateUI();
    }
    public void UpdateRequestUI()
    {
        GameObject goLayout = requestTabObj.transform.Find("RequestList").transform.Find("RequestGridLayeout").gameObject;
        //GameObject goLayout = GameObject.Find("RequestGridLayeout");
        FriendSummaryList fslist = Lobby.getInstance().CurrentSummaryList;
        List<GameObject> records = new List<GameObject>();
        for (int i = 0; i < Constants.PageItemCount; ++i)
        {
            string fr = "RequestRecord" + (i + 1).ToString();
            GameObject go = goLayout.transform.Find(fr).gameObject;
            records.Add(go);
            go.SetActive(false);
        }

        // 保证list的个数小于PageItemCount
        int showCount = fslist.Data.Count;
        if (fslist.Data.Count > Constants.PageItemCount)
        {
            showCount = Constants.PageItemCount;
            Debug.Log("list.Data.Count > Constants.PageItemCount");
        }

        for (int i = 0; i < showCount; ++i)
        {
            GameObject go = records[i];
            go.SetActive(true);
            go.transform.Find("txtRequestName").GetComponent<Text>().text = fslist.Data[i].Name;
            go.transform.Find("txtRequestLevel").GetComponent<Text>().text = "Lv." + fslist.Data[i].Level.ToString();
        }

        GameObject goLastPage = requestTabObj.transform.Find(DialogBtnStrings[(int)DialogBtn.RequestLastPage]).gameObject;
        GameObject goNextPage = requestTabObj.transform.Find(DialogBtnStrings[(int)DialogBtn.RequestNextPage]).gameObject; 
        if (0 == Lobby.getInstance().CurrentFriendPage)
        {
            goLastPage.GetComponent<Button>().interactable = false;
        }
        else
        {
            goLastPage.GetComponent<Button>().interactable = true;
        }

        if (Lobby.getInstance().CurrentFriendPage == Lobby.getInstance().FriendPageCount - 1)
        {
            goNextPage.GetComponent<Button>().interactable = false;
        }
        else
        {
            goNextPage.GetComponent<Button>().interactable = true;
        }

        GameObject goNoResults = requestTabObj.transform.Find("RequestNoRecords").gameObject; 
        if (fslist.Data.Count > 0)
        {
            if (goNoResults != null)
                goNoResults.GetComponent<Text>().text = "";
        }
        else
        {
            if (goNoResults != null)
                goNoResults.GetComponent<Text>().text = "YOU HAVE NO REQUESTS";
        }

        // 准备工作完毕，可以显示界面了
        UpdateUI();
    }
    void UpdateSearchUI()
    {
        if (searchRecordObj)
            searchRecordObj.SetActive(false);

        FriendSummaryList fslist = Lobby.getInstance().CurrentSummaryList;
        GameObject goNoResults = GameObject.Find("SearchNoResults");
        if (fslist.Data.Count > 0)
        {
            searchRecordObj.SetActive(true);
            GameObject go = GameObject.Find("SearchResult");
            go.SetActive(true);
            go.transform.Find("txtSearchResultName").GetComponent<Text>().text = fslist.Data[0].Name;
            go.transform.Find("txtSearchResultLevel").GetComponent<Text>().text = fslist.Data[0].Level.ToString();

            if (goNoResults != null)
                goNoResults.GetComponent<Text>().text = "";
        }
        else
        {
            searchRecordObj.SetActive(false);
            if (goNoResults != null)
                goNoResults.GetComponent<Text>().text = "NO RESULTS";
        }
    }
    
    void UpdateUI()
    {
        if (null == friendsOnObj)
            return;

        friendsOnObj.SetActive(m_activePage != 0);
        friendsOffObj.SetActive(m_activePage == 0);

        searchOnObj.SetActive(m_activePage != 1);
        searchOffObj.SetActive(m_activePage == 1);

        requestOnObj.SetActive(m_activePage != 2);
        requestOffObj.SetActive(m_activePage == 2);

        friendsTabObj.SetActive(m_activePage == 0);
        searchTabObj.SetActive(m_activePage == 1);
        requestTabObj.SetActive(m_activePage == 2);
    }
    void ShowPersonalInfoDlg()
    {
        DialogPersonalInfo.Show(Lobby.getInstance().QueryUserInfo);
    }
    void AfterHandleRequest()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.GetFriendRequests(UpdateRequestUI);
    }
    void IngoreFriend()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.IgnoreFriend(m_opId, AfterHandleRequest);
    }
    void AcceptFriend()
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.AcceptFriend(m_opId, AfterHandleRequest);
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
            case DialogBtn.SearchOff:
            case DialogBtn.FriendseOff:
            case DialogBtn.RequestOff:
                break;
            case DialogBtn.FriendsOn:
                {
                    Lobby.getInstance().CurrentSummaryList.Data.Clear();
                    m_activePage = 0;
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriends(UpdateFriendsUI);
                }
                break;
            case DialogBtn.SearchOn:
                {
                    Lobby.getInstance().CurrentSummaryList.Data.Clear();

                    m_activePage = 1;
                    UpdateSearchUI();
                    UpdateUI();
                }
                break;
            case DialogBtn.RequestOn:
                {
                    Lobby.getInstance().CurrentSummaryList.Data.Clear();

                    m_activePage = 2;
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriendRequests(UpdateRequestUI);
                }
                break;
            case DialogBtn.SearchFriend:
                {                   
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    GameObject goInput = GameObject.Find("InputSearchFriendCode");
                    string uIdStr = goInput.transform.Find("Text").GetComponent<Text>().text;
                    Debug.Log("Search:" + uIdStr);
                    long uId = 0;
                    try
                    {
                        uId = System.Convert.ToInt64(uIdStr);
                    }
                    catch(System.Exception e)
                    {
                        Debug.Log(e.Message);
                    }

                    // 先清理结果
                    Lobby.getInstance().CurrentSummaryList.Data.Clear();
                    if (uId > 0)
                    {
                        recp.GetFriendSummaryByUId(uId, UpdateSearchUI);
                    }
                    else
                    {
                        UpdateSearchUI();
                    }
                }
                break;
            case DialogBtn.FriendLastPage:
                {
                    if (Lobby.getInstance().CurrentFriendPage == 0)
                    {
                        Lobby.getInstance().CurrentFriendPage = Lobby.getInstance().FriendPageCount - 1;
                    }
                    else
                    {
                        Lobby.getInstance().CurrentFriendPage--;
                    }
                    
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriendSummary(UpdateFriendsUI);
                }
                break;
            case DialogBtn.RequestLastPage:
                {
                    if (Lobby.getInstance().CurrentFriendPage == 0)
                    {
                        Lobby.getInstance().CurrentFriendPage = Lobby.getInstance().FriendPageCount - 1;
                    }
                    else
                    {
                        Lobby.getInstance().CurrentFriendPage--;
                    }

                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriendSummary(UpdateRequestUI);
                    recp.HearBeat();
                }
                break;
            case DialogBtn.FriendNextPage:            
                {
                    Lobby.getInstance().CurrentFriendPage++;
                    if (Lobby.getInstance().CurrentFriendPage == Lobby.getInstance().FriendPageCount)
                    {
                        Lobby.getInstance().CurrentFriendPage = 0;
                    }

                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriendSummary(UpdateFriendsUI);
                }
                break;
            case DialogBtn.RequestNextPage:
                {
                    Lobby.getInstance().CurrentFriendPage++;
                    if (Lobby.getInstance().CurrentFriendPage == Lobby.getInstance().FriendPageCount)
                    {
                        Lobby.getInstance().CurrentFriendPage = 0;
                    }

                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetFriendSummary(UpdateRequestUI);
                }
                break;
            case DialogBtn.ViewFriend1:
            case DialogBtn.ViewFriend2:
            case DialogBtn.ViewFriend3:
            case DialogBtn.ViewFriend4:
            case DialogBtn.ViewFriend5:
            case DialogBtn.ViewFriend6:
                {
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    int idx = Lobby.getInstance().CurrentFriendPage * Constants.PageItemCount + btnIndex - (int)DialogBtn.ViewFriend1;
                    long uId = Lobby.getInstance().FriendIDArray.Data[idx];
                    recp.GetProfile(uId, ShowPersonalInfoDlg);//ShowPersonalInfoDlg
                }
                break;
            case DialogBtn.IngorFriend1:
            case DialogBtn.IngorFriend2:
            case DialogBtn.IngorFriend3:
            case DialogBtn.IngorFriend4:
            case DialogBtn.IngorFriend5:
            case DialogBtn.IngorFriend6:
                {
                    int idx = Lobby.getInstance().CurrentFriendPage * Constants.PageItemCount + btnIndex - (int)DialogBtn.IngorFriend1;
                    m_opId = Lobby.getInstance().FriendIDArray.Data[idx];

                    DialogBase.Show("REQUEST IGNORE", "Are you sure to ingore the request?", IngoreFriend);
                }
                break;
            case DialogBtn.AcceptFriend1:
            case DialogBtn.AcceptFriend2:
            case DialogBtn.AcceptFriend3:
            case DialogBtn.AcceptFriend4:
            case DialogBtn.AcceptFriend5:
            case DialogBtn.AcceptFriend6:
                {
                    int idx = Lobby.getInstance().CurrentFriendPage * Constants.PageItemCount + btnIndex - (int)DialogBtn.AcceptFriend1;
                    m_opId = Lobby.getInstance().FriendIDArray.Data[idx];

                    DialogBase.Show("REQUEST ACCEPT", "Are you sure to accept the request?", AcceptFriend);
                }
                break;
            case DialogBtn.ViewSearchResult:
                {
                    Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    recp.GetProfile(Lobby.getInstance().CurrentSummaryList.Data[0].UserId, ShowPersonalInfoDlg);
                }
                break;
            default:
                break;
        }       
    }
	// Use this for initialization
	new void Start () {
        InitBtn();

        friendsOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.FriendsOn]);
        friendsOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.FriendseOff]);

        searchOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SearchOn]);
        searchOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SearchOff]);

        requestOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RequestOn]);
        requestOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.RequestOff]);

        friendsTabObj = GameObject.Find("FriendsTable");
        searchTabObj = GameObject.Find("SearchTable");
        requestTabObj = GameObject.Find("RequestTable");

        searchRecordObj = GameObject.Find("SearchResult");
        
        // 初始化显示好友界面
        UpdateFriendsUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
