using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogStore : DialogBase
{
    public const string DialogName = "DialogStore";
    public enum DialogBtn
    {
        Close = 0,
        CoinOn, CoinOff,
        GemOn, GemOff,
        OnSaleOn, OnSaleOff,
    };
    public static string[] DialogBtnStrings = { "BtnDSClose",
                                              "BtnCoinOn", "BtnCoinOff",
                                              "BtnGemOn", "BtnGemOff",
                                              "BtnOnSaleOn", "BtnOnSaleOff",};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    GameObject coinOnObj, coinOffObj;
    GameObject gemOnObj, gemOffObj;
    GameObject onSaleOnObj, onSaleOffObj;
    GameObject coinTabObj, gemTabObj, onSaleTabObj;
    private int m_activePage = 0; // 0-coin, 1-gem, 2-onsale
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

    public static void Show(int ap, string str = "")
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogStore dlg = obj.GetComponent<DialogStore>();
        dlg.m_activePage = ap;
        dlg.DoShow(obj, str);
        dlg.UpdateUI();
    }
    void UpdateUI()
    {
        if (null == coinOnObj)
            return;

        coinOnObj.SetActive(m_activePage != 0);
        coinOffObj.SetActive(m_activePage == 0);

        gemOnObj.SetActive(m_activePage != 1);
        gemOffObj.SetActive(m_activePage == 1);

        onSaleOnObj.SetActive(m_activePage != 2);
        onSaleOffObj.SetActive(m_activePage == 2);

        coinTabObj.SetActive(m_activePage == 0);
        gemTabObj.SetActive(m_activePage == 1);
        onSaleTabObj.SetActive(m_activePage == 2);
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
            case DialogBtn.CoinOff:
            case DialogBtn.GemOff:
            case DialogBtn.OnSaleOff:
                break;
            case DialogBtn.CoinOn:
                {
                    m_activePage = 0;
                    UpdateUI();
                }
                break;
            case DialogBtn.GemOn:
                {
                    m_activePage = 1;
                    UpdateUI();
                }
                break;
            case DialogBtn.OnSaleOn:
                {
                    m_activePage = 2;
                    UpdateUI();
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
        coinOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.CoinOn]);
        coinOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.CoinOff]);

        gemOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.GemOn]);
        gemOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.GemOff]);

        onSaleOnObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.OnSaleOn]);
        onSaleOffObj = GameObject.Find(DialogBtnStrings[(int)DialogBtn.OnSaleOff]);

        coinTabObj = GameObject.Find("CoinTable");
        gemTabObj = GameObject.Find("GemTable");
        onSaleTabObj = GameObject.Find("OnSaleTable");

        // 依据全局变量显示按钮
        UpdateUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
