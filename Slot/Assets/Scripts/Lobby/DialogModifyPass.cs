using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogModifyPass : DialogBase
{
    public const string DialogName = "DialogModifyPass";
    public enum DialogBtn
    {
        Close = 0,
        Modify,
    };
    public static string[] DialogBtnStrings = { "BtnDMPClose",
                            "BtnModifyPass"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

    private string m_pwd = "";
    private string m_npwd = "";
    private string m_npwd2 = "";
    private string m_pwdMD5 = "";
    private string m_npwdMD5 = "";
    public string Password
    {
        get { return m_pwd; }
        set { m_pwd = value; }
    }
    public string NewPassword
    {
        get { return m_npwd; }
        set { m_npwd = value; }
    }
    public string NewPasswordAgain
    {
        get { return m_npwd2; }
        set { m_npwd2 = value; }
    }
    public static DialogModifyPass GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogModifyPass dlg = obj.GetComponent<DialogModifyPass>();
        return dlg;
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

    public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogModifyPass dlg = obj.GetComponent<DialogModifyPass>();
        dlg.DoShow(obj, str);
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
            case DialogBtn.Modify:
                {
                    Modify();
                }
                break;
            default:
                break;
        }       
    }
    void Modify()
    {
        if (m_pwd == "")
        {
            DialogBase.Show("Modify Password", "Invalid password(NULL).");
            return;
        }

        if (m_npwd != m_npwd2)
        {
            DialogBase.Show("Modify Password", "Your new and confirm passwords\n are different, Please retry.");
            return;
        }

        if (m_npwd == "")
        {
            DialogBase.Show("Modify Password", "Invalid new password(NULL).");
            return;
        }

        m_pwdMD5 = Tools.GetMD5(m_pwd);
        m_npwdMD5 = Tools.GetMD5(m_npwd);
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.ModifyPassword(m_pwdMD5, m_npwdMD5, AfterModifyPassword);
    }
    public void AfterModifyPassword()
    {
        DebugConsole.Log("AfterModifyPassword");
        GlobalVars.instance.LoginPwd = m_npwdMD5;
        DialogBase.Show("Modify Password", "Modify successfully!");
    }
	// Use this for initialization
    new void Start()
    {
        InitBtn();
        /*
        musicOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.MusicOn]);
        musicOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.MusicOff]);

        seOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SEOn]);
        seOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SEOff]);

        notifyOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.NotifyOn]);
        notifyOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.NotifyOff]);
        */
        // 依据全局变量显示按钮
        //UpdateMusicUI();
        //UpdateSEUI();
        //UpdateNotifyUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
