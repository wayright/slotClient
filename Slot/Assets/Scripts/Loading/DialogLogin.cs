using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogLogin : MonoBehaviour {
    protected GameObject m_dialog = null;
    protected const float s_InitScale = 0.1f;
    protected float m_disappear = 0;
    protected float m_show = 0;
    protected bool m_disappearing = false;
    protected bool m_showing = false;
    private WorkDone m_cbGuest = null;
    private WorkDone m_cbFS = null;
    private WorkDone m_cbWechat = null;
    private static string DIALOG_NAME = "DialogLogin";
    public static void Show(WorkDone cbGuest,
        WorkDone cbFS,
        WorkDone cbWechat)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DIALOG_NAME).gameObject;
        DialogLogin dlg = obj.GetComponent<DialogLogin>();
        dlg.DoShow(obj, cbGuest, cbFS, cbWechat);
    }

    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DIALOG_NAME).gameObject;
        DialogLogin dlg = obj.GetComponent<DialogLogin>();
        dlg.DoHide(obj);
    }
    public static bool Actived()
    {
        GameObject dialog = GameObject.Find(DIALOG_NAME);
        return dialog != null;
    }
	// Use this for initialization
	public void Start () {
        string[] btns = { "LoginAsGuest", "LoginWithFSID", "LoginWithWechat" };
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

    void OnClick(GameObject sender)
    {
        Tools.PlayAudio(Constants.Audio.Audio_LobbyClickButton);

        if (sender.name == "LoginAsGuest")
        {
            if (m_cbGuest != null)
                m_cbGuest();
        }
        else if (sender.name == "LoginWithFSID")
        {
            DialogLoginByEmail.Show(m_cbGuest, m_cbFS, m_cbWechat);
        }
        else if (sender.name == "LoginWithWechat")
        {
            // 尚未实现 do nothing
            if (m_cbWechat != null)
                m_cbWechat();
        }

        string btnName = DIALOG_NAME;
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
