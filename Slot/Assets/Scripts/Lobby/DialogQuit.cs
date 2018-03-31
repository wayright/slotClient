using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogQuit : MonoBehaviour {
    private GameObject m_dialog = null;
    private const float s_InitScale = 0.6f;
    private float m_disappear = 0;
    private float m_show = 0;
    private bool m_disappearing = false;
    private bool m_showing = false;
    public static void Show()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogQuit").gameObject;
        DialogQuit dlg = obj.GetComponent<DialogQuit>();
        dlg.DoShow(obj);
    }

    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogQuit").gameObject;
        DialogQuit dlg = obj.GetComponent<DialogQuit>();
        dlg.DoHide(obj);
    }
    public static bool Actived()
    {
        GameObject dialog = GameObject.Find("DialogQuit");
        return dialog != null;
    }
	// Use this for initialization
	void Start () {
        string[] btns = { "Cancel", "OK" };
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
        if (sender.name == "Cancel")
        {
            string btnName = "DialogQuit";
            GameObject btnObj = GameObject.Find(btnName);
            if (null == btnObj)
            {
                Debug.Log("null");
            }
            else
            {
                //btnObj.SetActive(false);
            }

            m_dialog = btnObj;
            m_disappearing = true;
            m_disappear = s_InitScale;
        }
        else if (sender.name == "OK")
        {
            Debug.Log("OK");
            Application.Quit();
        }
    }
    void DoHide(GameObject obj)
    {
        m_dialog = obj;
        m_disappearing = true;
        m_disappear = s_InitScale;
    }
    void DoShow(GameObject obj)
    {
        m_dialog = obj;
        m_dialog.transform.localScale = new Vector3(0, 0, 0);
        m_dialog.SetActive(true);
        m_showing = true;
        m_show = s_InitScale;
    }
	
	// Update is called once per frame
	void Update () {
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
