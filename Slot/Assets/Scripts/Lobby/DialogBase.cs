using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBase : MonoBehaviour {
    protected GameObject m_dialog = null;
    protected const float s_InitScale = 0.6f;
    protected float m_disappear = 0;
    protected float m_show = 0;
    protected bool m_disappearing = false;
    protected bool m_showing = false;
    public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogBase").gameObject;
        DialogBase dlg = obj.GetComponent<DialogBase>();
        dlg.DoShow(obj, str);
    }

    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogBase").gameObject;
        DialogBase dlg = obj.GetComponent<DialogBase>();
        dlg.DoHide(obj);
    }
    public static bool Actived()
    {
        GameObject dialog = GameObject.Find("DialogBase");
        return dialog != null;
    }
	// Use this for initialization
	public void Start () {
        string[] btns = { "Cancel", "OK", "Close" };
        for (int i = 0; i < btns.Length; ++i)
        {
            GameObject btnObj = GameObject.Find(btns[i]);
            if (btnObj == null)
            {
                Debug.Log("null");
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
        if (sender.name == "Cancel" || sender.name == "Close")
        {
            string btnName = "DialogBase";
            GameObject btnObj = GameObject.Find(btnName);
            if (null == btnObj)
            {
                Debug.Log("null");
            }
            else
            {
                DoHide(btnObj);
            }
        }
        else if (sender.name == "OK")
        {
            Debug.Log("OK");
            Application.Quit();
        }
    }
    public void DoHide(GameObject obj)
    {
        m_dialog = obj;
        m_disappearing = true;
        m_disappear = s_InitScale;
    }
    public void DoShow(GameObject obj, string str = "")
    {
        m_dialog = obj;
        //Debug.Log(m_dialog.transform.position);
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
                Text content = contentObj.GetComponent<Text>();
                content.text = str;
            }
        }
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
