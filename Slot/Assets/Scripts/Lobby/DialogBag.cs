using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Proto;
public class DialogBag : DialogBase
{
    public const string DialogName = "DialogBag";
    public enum DialogBtn
    {
        Close = 0,
        Sell, Use, Drop, Join,
        Item1,
    };
    public static string[] DialogBtnStrings = { "BtnDBagClose",
    "BtnDBagSell","BtnDBagUse","BtnDBagDrop","BtnDBagJoin",
    "Item1"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private int m_headIndex = 0; // based 1
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
        DialogBag dlg = obj.GetComponent<DialogBag>();
        dlg.DoShow(obj, cb);
    }
    public static DialogBag GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogBag dlg = obj.GetComponent<DialogBag>();
        return dlg;
    }
    public void DoShow(GameObject obj, WorkDone cb = null)
    {
        m_dialog = obj;
        m_dialog.transform.localScale = new Vector3(0, 0, 0);
        m_dialog.SetActive(true);
        m_showing = true;
        m_show = s_InitScale;

        // UpdateUI
        UpdateUI();
    }
    void UpdateUI()
    {
        UserItemList uil = Lobby.getInstance().UserItemList;

        DebugConsole.Log("UserItemList kind:" + uil.Kind);
        if (uil.Data.Count > 0)
        {
            DebugConsole.Log("Attr:" + uil.Data[0].Attr);
            DebugConsole.Log("ItemId:" + uil.Data[0].ItemId);
            DebugConsole.Log("ItemCount:" + uil.Data[0].ItemCount);
            DebugConsole.Log("ExpireTime:" + uil.Data[0].ExpireTime);

            GameObject goItem = GameObject.Find("Item1");
            goItem.transform.Find("data").transform.Find("Count").GetComponent<Text>().text = uil.Data[0].ItemCount.ToString();
        }
    }
    void OnClick(GameObject sender)
    {
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
                break;/*
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
                break;  */          
            default:
                if (btnIndex >= (int)DialogBtn.Item1 && btnIndex <= (int)DialogBtn.Item1)
                {
                    m_headIndex = btnIndex - (int)DialogBtn.Item1 + 1;
                    //GameObject goHead = GameObject.Find(sender.name);
                    //GameObject go = GameObject.Find("Review");
                    //go.GetComponent<Image>().sprite = goHead.GetComponent<Image>().sprite;
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
