using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogOption : DialogBase
{
    public const string DialogName = "DialogOption";
    public enum DialogBtn
    {
        Close = 0,
        SEOn, SEOff,
        MusicOn, MusicOff,
        NotifyOn, NotifyOff
    };
    public static string[] DialogBtnStrings = { "BtnDOClose",
                            "BtnSEOn", "BtnSEOff",
                            "BtnMusicOn", "BtnMusicOff",
                            "BtnNotifyOn", "BtnNotifyOff"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();

    GameObject musicOnObj, musicOffObj;
    GameObject seOnObj, seOffObj;
    GameObject notifyOnObj, notifyOffObj;

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

    new public static void Show(string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogOption dlg = obj.GetComponent<DialogOption>();
        dlg.DoShow(obj, str);
    }
    
    void UpdateMusicUI()
    {
        if (GlobalVars.instance.GetMusic())
        {            
            musicOnObj.SetActive(false);            
            musicOffObj.SetActive(true);
        }
        else
        {
            musicOnObj.SetActive(true);
            musicOffObj.SetActive(false);
        }
    }
    void OnMusicOn()
    {
        GlobalVars.instance.SetMusic();
        UpdateMusicUI();
    }
    void OnMusicOff()
    {
        GlobalVars.instance.SetMusic(false);
        UpdateMusicUI();
    }
    void UpdateSEUI()
    {
        if (GlobalVars.instance.GetSE())
        {
            seOnObj.SetActive(false);
            seOffObj.SetActive(true);
        }
        else
        {
            seOnObj.SetActive(true);
            seOffObj.SetActive(false);
        }
    }
    void OnSEOn()
    {
        GlobalVars.instance.SetSE();
        UpdateSEUI();
    }
    void OnSEOff()
    {
        GlobalVars.instance.SetSE(false);
        UpdateSEUI();
    }
    void UpdateNotifyUI()
    {
        if (GlobalVars.instance.GetNotify())
        {
            notifyOnObj.SetActive(false);
            notifyOffObj.SetActive(true);
        }
        else
        {
            notifyOnObj.SetActive(true);
            notifyOffObj.SetActive(false);
        }
    }
    void OnNotifyOn()
    {
        GlobalVars.instance.SetNotify();
        UpdateNotifyUI();
    }
    void OnNotifyOff()
    {
        GlobalVars.instance.SetNotify(false);
        UpdateNotifyUI();
    }
    void OnClick(GameObject sender)
    {
        Debug.Log(sender.name);
        int btnIndex = GetBtn(sender.name);
        if (btnIndex < 0)
        {
            Debug.Log("Cant find button:" + sender.name);
            return;
        }
        switch ((DialogBtn)btnIndex)
        {
            case DialogBtn.Close:
                {
                    GameObject btnObj = GameObject.Find(DialogName);
                    if (null == btnObj)
                    {
                        Debug.Log("null");
                    }
                    else
                    {
                        Debug.Log("DoHide");
                        DoHide(btnObj);
                    }
                }
                break;
            case DialogBtn.MusicOn:
                {
                    OnMusicOn();
                }
                break;
            case DialogBtn.MusicOff:
                {
                    OnMusicOff();
                }
                break;
            case DialogBtn.NotifyOn:
                {
                    OnNotifyOn();
                }
                break;
            case DialogBtn.NotifyOff:
                {
                    OnNotifyOff();
                }
                break;
            case DialogBtn.SEOn:
                {
                    OnSEOn();
                }
                break;
            case DialogBtn.SEOff:
                {
                    OnSEOff();
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

        musicOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.MusicOn]);
        musicOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.MusicOff]);

        seOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SEOn]);
        seOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.SEOff]);

        notifyOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.NotifyOn]);
        notifyOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.NotifyOff]);

        // 依据全局变量显示按钮
        UpdateMusicUI();
        UpdateSEUI();
        UpdateNotifyUI();

        //Canvas canvas = GetComponent<Canvas>();
        //if (canvas == null)
        //{
        //    canvas = gameObject.AddComponent<Canvas>();
        //}
        //canvas.overrideSorting = true;
        //canvas.sortingOrder = 2;
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
