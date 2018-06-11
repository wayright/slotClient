using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lion.Proto;
using Common.Proto;
public class DialogStore : DialogBase
{
    public const string DialogName = "DialogStore";
    public enum DialogBtn
    {
        Close = 0,
        CoinOn, CoinOff,
        GemOn, GemOff,
        OnSaleOn, OnSaleOff,
        CRecharge1, CRecharge2, CRecharge3, CRecharge4, CRecharge5, CRecharge6,
        GRecharge1, GRecharge2, GRecharge3, GRecharge4, GRecharge5, GRecharge6,
        SRecharge1, SRecharge2, SRecharge3, SRecharge4, SRecharge5, SRecharge6,
    };
    public static string[] DialogBtnStrings = { "BtnDSClose",
                                              "BtnCoinOn", "BtnCoinOff",
                                              "BtnGemOn", "BtnGemOff",
                                              "BtnOnSaleOn", "BtnOnSaleOff",
        "BtnCRecharge1","BtnCRecharge2","BtnCRecharge3","BtnCRecharge4","BtnCRecharge5","BtnCRecharge6",
        "BtnGRecharge1","BtnGRecharge2","BtnGRecharge3","BtnGRecharge4","BtnGRecharge5","BtnGRecharge6",
        "BtnSRecharge1","BtnSRecharge2","BtnSRecharge3","BtnSRecharge4","BtnSRecharge5","BtnSRecharge6",};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    GameObject coinOnObj, coinOffObj;
    GameObject gemOnObj, gemOffObj;
    GameObject onSaleOnObj, onSaleOffObj;
    GameObject coinTabObj, gemTabObj, onSaleTabObj;
    private int m_activePage = 0; // 0-coin, 1-gem, 2-onsale
    private int m_subPage = 0; // 子项页码
    private bool m_hasCountDown = false;
    public void InitBtn()
    {
        for (int i = 0; i < DialogBtnStrings.Length; ++i)
        {
            string btnName = DialogBtnStrings[i];            
            GameObject btnObj = GameObject.Find(btnName);
            if (btnObj != null)
            {
                m_btnIndexDict.Add(btnName, i);
                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(delegate()
                {
                    this.OnClick(btnObj);
                });
            }
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
        dlg.m_subPage = 0;
        dlg.DoShow(obj, str);
        dlg.GetShopData();
    }

    void GetShopData()
    {
        Reception.GetShopItems(UpdateUI);
    }
    void UpdateShop()
    { 
        // 显示商店项目
        // 隐藏所有项
        ShopList sl = Lobby.getInstance().ShopList;
        if (m_activePage != 0 || sl == null)
            return;

        Transform tfCoinTable = transform.Find("main").Find("CoinTable");
        for (int i = 0; i < 6; ++i)
        {
            string record = "CoinRecord" + (i + 1).ToString();
            tfCoinTable.Find(record).gameObject.SetActive(false);
        }

        // 计算索引
        int startIndex = m_subPage * 6;
        int endIndex = (m_subPage + 1) * 6;
        if (endIndex > sl.Data.Count)
        {
            endIndex = sl.Data.Count;
        }

        for (int i = startIndex; i < endIndex; ++i)
        {
            string record = "CoinRecord" + (i - startIndex + 1).ToString();
            Transform tfRecord = tfCoinTable.Find(record);
            tfRecord.gameObject.SetActive(true);

            ShopItem si = sl.Data[i];
            tfRecord.Find("ItemID").GetComponent<Text>().text = si.ItemId.ToString();
            tfRecord.Find("ItemNameWrapper").Find("ItemName").GetComponent<Text>().text = si.Name;
            tfRecord.Find("ItemNameWrapper").Find("ItemDesc").GetComponent<Text>().text = si.Desc;

            if (si.Expire > 0)
            {
                // if expired time > 1day, show days, or show time as HH:MM::SS
                double ms = si.Expire; // mill seconds
                double s = ms / 1000;
                const int OneDaySeconds = 24 * 60 * 60;

                if (s > OneDaySeconds)
                {
                    int d = (int)(s / OneDaySeconds);
                    if (d < 2)
                        tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = d.ToString() + "Day";
                    else
                        tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = d.ToString() + "Days";
                }
                else
                {
                    tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = Tools.MsecondToHHMMSS(si.Expire);
                    m_hasCountDown = true;
                }
            }
            else if (si.Expire == 0)
            {
                tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = "";
            }

            string priceStr = "$" + si.Price.ToString();
            priceStr += "\n(";
            priceStr += "$";
            priceStr += si.OldPrice.ToString();
            priceStr += ")";
            string charge = "BtnCRecharge" + (i - startIndex + 1).ToString();
            tfRecord.Find("ItemRechargeWrapper").Find(charge).GetComponent<Button>().interactable = true;
            tfRecord.Find("ItemRechargeWrapper").Find(charge).Find("Price").GetComponent<Text>().text = priceStr;
            Tools.LoadWWWImage(si.Url, tfRecord.Find("ItemImg").GetComponent<Image>());
        }
    }
    void UpdateShopCountDown(float deltaTime)
    {
        ShopList sl = Lobby.getInstance().ShopList;
        if (m_activePage != 0 || sl == null)
            return;

        Transform tfCoinTable = transform.Find("main").Find("CoinTable");

        // 计算索引
        int startIndex = m_subPage * 6;
        int endIndex = (m_subPage + 1) * 6;
        if (endIndex > sl.Data.Count)
        {
            endIndex = sl.Data.Count;
        }

        for (int i = startIndex; i < endIndex; ++i)
        {
            string record = "CoinRecord" + (i - startIndex + 1).ToString();
            Transform tfRecord = tfCoinTable.Find(record);

            ShopItem si = sl.Data[i];
            si.Expire -= (long)(deltaTime * 1000);

            if (si.Expire <= 0)
            {
                // count down is over set button interactive
                string charge = "BtnCRecharge" + (i - startIndex + 1).ToString();
                tfRecord.Find("ItemRechargeWrapper").Find(charge).GetComponent<Button>().interactable = false;
            }
            else
            {
                // if expired time > 1day, show days, or show time as HH:MM::SS
                double ms = si.Expire; // mill seconds
                double s = ms / 1000;
                const int OneDaySeconds = 24 * 60 * 60;

                if (s > OneDaySeconds)
                {
                    int d = (int)(s / OneDaySeconds);
                    tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = d.ToString() + "Day";
                }
                else
                {
                    tfRecord.Find("ItemExpireDate").GetComponent<Text>().text = Tools.MsecondToHHMMSS(si.Expire);
                }
            }
        }
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

        LionUserInfo ui = Lobby.getInstance().UserInfo;
        transform.Find("main").Find("top").Find("BtnCredits").Find("Text").GetComponent<Text>().text = Tools.CoinToString(ui.Gold);

        m_hasCountDown = false;
        UpdateShop();
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
            case DialogBtn.CoinOff:
            case DialogBtn.GemOff:
            case DialogBtn.OnSaleOff:
                break;
            case DialogBtn.CoinOn:
                {
                    m_activePage = 0;
                    m_subPage = 0;
                    UpdateUI();
                }
                break;
            case DialogBtn.GemOn:
                {
                    m_activePage = 1;
                    m_subPage = 0;
                    UpdateUI();
                }
                break;
            case DialogBtn.OnSaleOn:
                {
                    m_activePage = 2;
                    m_subPage = 0;
                    UpdateUI();
                }
                break;
            case DialogBtn.CRecharge1:
            case DialogBtn.CRecharge2:
            case DialogBtn.CRecharge3:
            case DialogBtn.CRecharge4:
            case DialogBtn.CRecharge5:
            case DialogBtn.CRecharge6:
                {
                    Reception.DoBuy("jb_" + (btnIndex - DialogBtn.CRecharge1 + 1).ToString());
                }
                break;
            case DialogBtn.GRecharge1:
            case DialogBtn.GRecharge2:
            case DialogBtn.GRecharge3:
            case DialogBtn.GRecharge4:
            case DialogBtn.GRecharge5:
            case DialogBtn.GRecharge6:
                {
                    Reception.DoBuy("gem_" + (btnIndex - DialogBtn.CRecharge1 + 1).ToString());
                }
                break;
            case DialogBtn.SRecharge1:
            case DialogBtn.SRecharge2:
            case DialogBtn.SRecharge3:
            case DialogBtn.SRecharge4:
            case DialogBtn.SRecharge5:
            case DialogBtn.SRecharge6:
                {
                    Reception.DoBuy("sale_" + (btnIndex - DialogBtn.CRecharge1 + 1).ToString());
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
        // updateUI在其中
        UpdateUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
   
        if (m_hasCountDown)
        {
            UpdateShopCountDown(Time.deltaTime);
        }
	}
}
