using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogReconnect : MonoBehaviour {
    private bool m_bActived = true;
    private const float ELAPSE = 0.3f;// 每0.3秒增加一个点
    private float m_elapse = ELAPSE;
    private const int DOTCOUNT = 6;
    private int m_dotCount = 0; // 0~6
    private string m_reconnecting;
    private string m_dot;
    private Text m_text;
    public static void Show()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogReconnect").gameObject;
        DialogReconnect dlg = obj.GetComponent<DialogReconnect>();
        dlg.DoShow(obj);        
    }
    public static void Hide()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find("DialogReconnect").gameObject;
        DialogReconnect dlg = obj.GetComponent<DialogReconnect>();
        dlg.DoHide(obj);   
    }
    public void DoShow(GameObject obj)
    {
        m_bActived = true;
        obj.SetActive(true);

        GameObject textObj = obj.transform.Find("Content").gameObject;
        if (textObj == null)
            Debug.Log("textObj == null");
        m_text = textObj.GetComponent<Text>();
        if (m_text == null)
            Debug.Log("m_text == null");
    }
    public void DoHide(GameObject obj)
    {
        m_bActived = false;
        obj.SetActive(false);
    }
	// Use this for initialization
	void Start () {
        LoadStrings();
	}
    void LoadStrings()
    {
        m_reconnecting = LocalizationManager.instance.GetValue("Reconnecting");
        m_dot = LocalizationManager.instance.GetValue("Dot");

        GameObject textObj = transform.Find("Content").gameObject;
        m_text = textObj.GetComponent<Text>();
        m_text.text = m_reconnecting;

        GameObject togObj = transform.Find("Toggle").gameObject;
        Toggle tog = togObj.GetComponent<Toggle>();
        tog.isOn = false;
    }
	
	// Update is called once per frame
	void Update () {
		//GetComponent<DialogReconnect>().
        if (m_bActived)
        {
            if (m_elapse > 0)
            {
                m_elapse -= Time.deltaTime;
            }
            else
            {
                string curText = m_reconnecting;
                for (int i = 0; i < m_dotCount; ++i)
                {
                    curText += m_dot;
                }

                m_text.text = curText;
                m_elapse = ELAPSE;
                m_dotCount++;
                if (m_dotCount == DOTCOUNT)
                    m_dotCount = 0;
            }
        }
	}
}
