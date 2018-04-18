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
    };
    public static string[] DialogBtnStrings = { "BtnDPClose",
                            "BtnProfileOn", "BtnProfileOff",
                            "BtnRecordsOn", "BtnRecordsOff",
                            "BtnUpAvatar", "BtnUploadAvatar"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private string m_headUrl;
    private Sprite m_headSprite;

    // buttons obj
    GameObject profileOnObj, profileOffObj;
    GameObject recordsOnObj, recordsOffObj;
    GameObject profileTabObj, recordsTabObj;
    private bool m_profileOn = true;

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

    new public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogPersonalInfo dlg = obj.GetComponent<DialogPersonalInfo>();
        dlg.DoShow(obj, str);
    }
    public static DialogPersonalInfo instance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogPersonalInfo dlg = obj.GetComponent<DialogPersonalInfo>();

        return dlg;
    }
    IEnumerator LoadHeadImage()
    {
        //请求WWW
        WWW www = new WWW(m_headUrl);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //获取Texture
            Texture2D texture = www.texture;

            //创建Sprite
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f));
            GameObject.Find("BtnUpAvatar").GetComponent<Image>().sprite = sprite;
        }
    }
    public void UpdateUserInfo()
    {
        LionUserInfo ui = Lobby.getInstance().UserInfo;
        
        // NickName
        GameObject go = GameObject.Find("InputNickName");
        if (go == null)
            Debug.Log("InputNickName is null");
        else
        {
            InputField field = go.GetComponent<InputField>();
            field.text = ui.Name;
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
        m_headUrl = ui.HeadImgUrl;
        if (m_headUrl != "")
        {
            StartCoroutine(Tools.LoadWWWImageToButton(ui.HeadImgUrl, "BtnUpAvatar"));
        }
    }
    
    void UpdateUI()
    {
        profileOnObj.SetActive(!m_profileOn);
        profileOffObj.SetActive(m_profileOn);
        recordsOnObj.SetActive(m_profileOn);
        recordsOffObj.SetActive(!m_profileOn);

        profileTabObj.SetActive(m_profileOn);
        recordsTabObj.SetActive(!m_profileOn);
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
                    UpdateUI();
                }
                break;
            case DialogBtn.UpAvatar:
            case DialogBtn.UploadAvatar:
                {
                    Debug.Log("Upload...");
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
