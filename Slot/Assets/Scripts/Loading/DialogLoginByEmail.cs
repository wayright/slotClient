using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogLoginByEmail : MonoBehaviour
{
    protected GameObject m_dialog = null;
    protected const float s_InitScale = 0.1f;
    protected float m_disappear = 0;
    protected float m_show = 0;
    protected bool m_disappearing = false;
    protected bool m_showing = false;
    private WorkDone m_cbGuest = null;
    private string m_email = "";
    private string m_pwd = "";
    private WorkDone m_cbFS = null;
    private WorkDone m_cbWechat = null;
    private static string DialogName = "DialogLoginByEmail";
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
    public static void Show(WorkDone cbGuest,
        WorkDone cbFS,
        WorkDone cbWechat)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogLoginByEmail dlg = obj.GetComponent<DialogLoginByEmail>();
        dlg.DoShow(obj, cbGuest, cbFS, cbWechat);
    }

    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogLoginByEmail dlg = obj.GetComponent<DialogLoginByEmail>();
        dlg.DoHide(obj);
    }
    public static bool Actived()
    {
        GameObject dialog = GameObject.Find(DialogName);
        return dialog != null;
    }
	// Use this for initialization
	public void Start () {
        string[] btns = { "BtnLogin", "BtnClose" };
        for (int i = 0; i < btns.Length; ++i)
        {
            GameObject btnObj = GameObject.Find(btns[i]);
            if (btnObj == null)
            {
                DebugConsole.Log("null");
            }
            else
            {
                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(delegate()
                {
                    this.OnClick(btnObj);
                });
            }
        }
	}
    public static DialogLoginByEmail GetInstance()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogLoginByEmail dlg = obj.GetComponent<DialogLoginByEmail>();
        return dlg;
    }
    void OnClick(GameObject sender)
    {
        Tools.PlayAudio(Constants.Audio.Audio_LobbyClickButton);

        if (sender.name == "BtnLogin")
        {
            if (m_cbFS != null)
                m_cbFS();
        }
        else if (sender.name == "BtnClose")
        {
           // show login dialog
            DialogLogin.Show(m_cbGuest, m_cbFS, m_cbWechat);
        } 

        string btnName = DialogName;
        GameObject btnObj = GameObject.Find(btnName);
        DoHide(btnObj);
    }
    public void DoHide(GameObject obj)
    {
        m_dialog = obj;
        m_disappearing = true;
        m_disappear = s_InitScale;
    }
    public void DoShow(GameObject obj, WorkDone cbGuest,
        WorkDone cbFS,
        WorkDone cbWechat)
    {
        m_dialog = obj;
        m_dialog.transform.localScale = new Vector3(0, 0, 0);
        m_dialog.SetActive(true);
        m_showing = true;
        m_show = s_InitScale;

        m_cbGuest = cbGuest;
        m_cbFS = cbFS;
        m_cbWechat = cbWechat;
    }
	
	// Update is called once per frame
	public void Update () {
        if (m_dialog != null)
        {
            if (m_disappearing)
            {
                if (m_disappear > 0)
                {
                    m_disappear -= Time.deltaTime;
                    if (m_disappear < 0)
                        m_disappear = 0;
                    float factor = m_disappear / s_InitScale;
                    m_dialog.transform.localScale = new Vector3(factor, factor, factor);
                }
                else
                {
                    m_disappear = 0;
                    m_dialog.transform.localScale = new Vector3(0, 0, 0);
                    m_dialog.SetActive(false);
                    m_dialog = null;
                    m_disappearing = false;
                }

                return;
            }

            if (m_showing)
            {
                if (m_show > 0)
                {
                    m_show -= Time.deltaTime;
                    if (m_show < 0)
                        m_show = 0;
                    float factor = (s_InitScale - m_show) / s_InitScale;
                    m_dialog.transform.localScale = new Vector3(factor, factor, factor);
                }
                else
                {
                    m_show = 0;
                    m_dialog.transform.localScale = new Vector3(1, 1, 1);
                    m_dialog = null;
                    m_showing = false;
                }
            }
        }
	}
}
