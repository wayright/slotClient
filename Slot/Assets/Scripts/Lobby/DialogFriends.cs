using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    };
    public static string[] DialogBtnStrings = { "BtnDFClose",
                            "BtnFriendsOn", "BtnFriendsOff",
                            "BtnSearchOn", "BtnSearchOff",
                            "BtnRequestOn", "BtnRequestOff",
                                              "BtnInvite"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

    // buttons obj
    GameObject friendsOnObj, friendsOffObj;
    GameObject searchOnObj, searchOffObj;
    GameObject requestOnObj, requestOffObj;
    GameObject friendsTabObj, searchTabObj, requestTabObj;
    private int m_activePage = 0;
    private string m_searchCode;

    public string SearchCode
    {
        get { return m_searchCode; }
        set { m_searchCode = value; }
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

    new public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogFriends dlg = obj.GetComponent<DialogFriends>();
        dlg.DoShow(obj, str);
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
                    m_activePage = 0;
                    UpdateUI();
                }
                break;
            case DialogBtn.SearchOn:
                {
                    m_activePage = 1;
                    UpdateUI();
                }
                break;
            case DialogBtn.RequestOn:
                {
                    m_activePage = 2;
                    UpdateUI();
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
        // 依据全局变量显示按钮
        UpdateUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
