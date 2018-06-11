using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogRegEmail : DialogBase
{
    public const string DialogName = "DialogRegEmail";
    public enum DialogBtn
    {
        Close = 0,
        Reset,
        Register,
    };
    public static string[] DialogBtnStrings = { "BtnDREClose",
                            "BtnReset", "BtnRegister"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

    private string m_email = "";
    private string m_pwd = "";
    private string m_pwd2 = "";
    private string m_pwdMD5 = "";
    public string Email
    {
        get { return m_email; }
        set { m_email = value; }
    }
    public string Password
    {
        get { return m_pwd; }
        set { m_pwd = value; }
    }
    public string PasswordAgain
    {
        get { return m_pwd2; }
        set { m_pwd2 = value; }
    }
    public static DialogRegEmail GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogRegEmail dlg = obj.GetComponent<DialogRegEmail>();
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
        DialogRegEmail dlg = obj.GetComponent<DialogRegEmail>();
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
            case DialogBtn.Reset:
                {
                    Reset();
                }
                break;
            case DialogBtn.Register:
                {
                    Register();
                }
                break;
            default:
                break;
        }       
    }    
    void Register()
    {
        if (m_email == "")
        {
            DialogBase.Show("REGISTER EMAIL", "INVALID EMAIL(NULL).");
            return;
        }
       
        string expression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)" + @"|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(m_email, expression))
        {
            DialogBase.Show("REGISTER EMAIL", "INVALID EMAIL ADDRESS.");
            return;
        }  

        if (m_pwd != m_pwd2)
        {
            DialogBase.Show("REGISTER EMAIL", "YOUR NEW AND CONFIRM PASSWORDS\n ARE DIFFERENT, PLEASE RETRY.");
            return;
        }

        if (m_pwd == "")
        {
            DialogBase.Show("REGISTER EMAIL", "INVALID PASSWORD(NULL).");
            return;
        }

        m_pwdMD5 = Tools.GetMD5(m_pwd);
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.RegisterByEmail(m_email, m_pwdMD5, AfterRegisterByEmail);
    }
    public void AfterRegisterByEmail()
    {
        DebugConsole.Log("AfterRegisterByEmail");
        GlobalVars.instance.LoginType = Constants.Login_Email;
        GlobalVars.instance.LoginEmail = m_email;
        GlobalVars.instance.LoginPwd = m_pwdMD5;
        // 清空旧的GUID
        DebugConsole.Log("Clear old guid:" + GlobalVars.instance.LoginGuid);
        GlobalVars.instance.LoginGuid = "";

        DialogBase.Show("Register by email", "Register successfully!");
    }
    void Reset()
    {
        m_pwd = m_pwd2 = "";
        GameObject.Find("KvpPassword").transform.Find("InputValue").GetComponent<InputField>().text = "";
        GameObject.Find("KvpPassword2").transform.Find("InputValue").GetComponent<InputField>().text = "";
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
