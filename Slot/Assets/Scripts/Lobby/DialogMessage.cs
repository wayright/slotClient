using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogMessage : DialogBase
{
    public const string DialogName = "DialogMessage";
    public enum DialogBtn
    {
        Close = 0,
    };
    public static string[] DialogBtnStrings = { "BtnDMClose"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

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

    public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogMessage dlg = obj.GetComponent<DialogMessage>();
        dlg.DoShow(obj, str);
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
                break;            
            default:
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
