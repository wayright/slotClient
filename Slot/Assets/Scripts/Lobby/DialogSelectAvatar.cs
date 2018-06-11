using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogSelectAvatar : DialogBase
{
    public const string DialogName = "DialogSelectAvatar";
    public enum DialogBtn
    {
        Close = 0,
        OK,
        Head1, Head2, Head3, Head4,
        Head5, Head6, Head7, Head8,
        Head9, Head10, Head11, Head12,
        Head13, Head14, Head15, Head16,
    };
    public static string[] DialogBtnStrings = { "BtnDSAClose",
                            "BtnDSAOK",
    "Head1", "Head2", "Head3", "Head4",
    "Head5", "Head6", "Head7", "Head8",
    "Head9", "Head10", "Head11", "Head12",
    "Head13", "Head14", "Head15", "Head16"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private int m_headIndex = 0; // based 1
    private WorkDone m_callBack = null;
    public int HeadIndex
    {
        get { return m_headIndex; }
    }
    public void InitBtn()
    {
        for (int i = 0; i < DialogBtnStrings.Length; ++i)
        {
            string btnName = DialogBtnStrings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
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

    public static void Show(WorkDone cb)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogSelectAvatar dlg = obj.GetComponent<DialogSelectAvatar>();
        dlg.DoShow(obj, cb);
    }
    public static DialogSelectAvatar GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogSelectAvatar dlg = obj.GetComponent<DialogSelectAvatar>();
        return dlg;
    }
    public void DoShow(GameObject obj, WorkDone cb = null)
    {
        m_dialog = obj;
        m_dialog.transform.localScale = new Vector3(0, 0, 0);
        m_dialog.SetActive(true);
        m_showing = true;
        m_show = s_InitScale;
        m_callBack = cb;

        string url = Lobby.getInstance().UserInfo.HeadImgUrl;
        int headIndex = Tools.StringToInt32(url);
        if (headIndex > 0)
        {
            m_headIndex = headIndex;
            GameObject goHead = GameObject.Find("Head" + headIndex.ToString());
            GameObject go = GameObject.Find("Review");
            go.GetComponent<Image>().sprite = goHead.GetComponent<Image>().sprite;
        }
    }
    public static GameObject GetHeadObject(string headImgUrl)
    {
        string headStr = "Head" + headImgUrl;
        GameObject goSrc = null;
        Transform tf =
            DialogSelectAvatar.GetInstance().transform.Find("main").
            transform.Find("Avatars").
            transform.Find("GridLayeout").
            transform.Find(headStr);

        if (tf != null)
            goSrc = tf.gameObject;

        return goSrc;
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
            case DialogBtn.OK:
                {                    
                    if (m_callBack != null)
                    {
                        m_callBack();
                        m_callBack = null;
                    }

                    GameObject btnObj = GameObject.Find(DialogName);
                    DoHide(btnObj);
                }
                break;            
            default:
                if (btnIndex >= (int)DialogBtn.Head1 && btnIndex <= (int)DialogBtn.Head16)
                {
                    m_headIndex = btnIndex - (int)DialogBtn.Head1 + 1;
                    GameObject goHead = GameObject.Find(sender.name);
                    GameObject go = GameObject.Find("Review");
                    go.GetComponent<Image>().sprite = goHead.GetComponent<Image>().sprite;
                }
                break;
        }       
    }
	// Use this for initialization
    new void Start()
    {
        InitBtn();        
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
