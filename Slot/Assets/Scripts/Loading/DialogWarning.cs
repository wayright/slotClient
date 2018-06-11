using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWarning : MonoBehaviour {
    protected GameObject m_dialog = null;
    protected const float s_InitScale = 0.1f;
    protected float m_disappear = 0;
    protected float m_show = 0;
    protected bool m_disappearing = false;
    protected bool m_showing = false;
    private WorkDone m_cbOK = null;
    private WorkDone m_cbCancel = null;
    private WorkDone m_cbClose = null;
    public static void Show(string title = "Warning", 
        string content = "content",
        string ok = "OK",
        string cancel = "Cancel",
        WorkDone cbOK = null,
        WorkDone cbCancel = null,
        WorkDone cbClose = null)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogWarning").gameObject;
        DialogWarning dlg = obj.GetComponent<DialogWarning>();
        dlg.DoShow(obj,
            title,
            content,
            ok,
            cancel,
            cbOK,
            cbCancel,
            cbClose);
    }

    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogWarning").gameObject;
        DialogWarning dlg = obj.GetComponent<DialogWarning>();
        dlg.DoHide(obj);
    }
    public static bool Actived()
    {
        GameObject dialog = GameObject.Find("DialogWarning");
        return dialog != null;
    }
	// Use this for initialization
	public void Start () {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject dialog = canvas.transform.Find("DialogWarning").gameObject;
        string[] btns = { "Cancel", "OK", "Close" };
        for (int i = 0; i < btns.Length; ++i)
        {
            GameObject btnObj = dialog.transform.Find(btns[i]).gameObject;
            if (btnObj == null)
            {
                DebugConsole.Log(btns[i] + "is null");
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

        if (sender.name == "Cancel" || sender.name == "Close")
        {
            string btnName = "DialogWarning";
            GameObject btnObj = GameObject.Find(btnName);
            if (null == btnObj)
            {
                DebugConsole.Log("null");
            }
            else
            {
                if (sender.name == "Cancel" && m_cbCancel != null)
                {
                    m_cbCancel();
                    m_cbCancel = null;
                }

                if (sender.name == "Close" && m_cbClose != null)
                {
                    m_cbClose();
                    m_cbClose = null;
                }

                DoHide(btnObj);
            }
        }
        else if (sender.name == "OK")
        {
            DebugConsole.Log("OK");
            if (m_cbOK != null)
            {
                m_cbOK();
                m_cbOK = null;
            }

            string btnName = "DialogWarning";
            GameObject btnObj = GameObject.Find(btnName);
            DoHide(btnObj);
        }
    }
    public void DoHide(GameObject obj)
    {
        m_dialog = obj;
        m_disappearing = true;
        m_disappear = s_InitScale;
    }
    public void DoShow(GameObject obj,
        string title = "TIP", 
        string content = "Are you sure to exit game?",
        string ok = "OK",
        string cancel = "Cancel",
        WorkDone cbOK = null,
        WorkDone cbCancel = null,
        WorkDone cbClose = null)
    {
        m_dialog = obj;
        //DebugConsole.Log(m_dialog.transform.position);
        //m_dialog.transform.position = new Vector3(0, 0, 0);
        m_dialog.transform.localScale = new Vector3(0, 0, 0);
        m_dialog.SetActive(true);
        m_showing = true;
        m_show = s_InitScale;

        Transform tf = m_dialog.transform.Find("Content");
        if (null != tf)
        {
            GameObject contentObj = tf.gameObject;
            if (contentObj != null)
            {
                Text contentText = contentObj.GetComponent<Text>();
                contentText.text = content;
            }
        }

        tf = m_dialog.transform.Find("Title");
        if (null != tf)
        {
            GameObject contentObj = tf.gameObject;
            if (contentObj != null)
            {
                Text titleText = contentObj.GetComponent<Text>();
                titleText.text = title;
            }
        }

        m_cbOK = cbOK;
        m_cbCancel = cbCancel;
        m_cbClose = cbClose;

        m_dialog.transform.Find("OK").transform.Find("Text").GetComponent<Text>().text = ok;
        m_dialog.transform.Find("Cancel").transform.Find("Text").GetComponent<Text>().text = cancel;
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
