using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogDailyBonus : DialogBase
{
    public const string DialogName = "DialogDailyBonus";
    public enum DialogBtn
    {
        Close = 0,
        Collect
    };
    public static string[] DialogBtnStrings = { "BtnDDBClose",
                            "BtnDDBCollect"};
    public Dictionary<string, int> m_btnIndexDict = new Dictionary<string, int>();
    private int m_day = 0;
    GameObject dailyEdgesObj;
    public int Day
    {
        get { return m_day; }
        set { m_day = value; }
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

    public static void Show(int day)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject obj = canvas.transform.Find(DialogName).gameObject;
        DialogDailyBonus dlg = obj.GetComponent<DialogDailyBonus>();
        dlg.Day = day;
        dlg.DoShow(obj);
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
            case DialogBtn.Collect:
                {
                    //Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
                    //recp.TakeLoginBonus();
                }
                break;
            default:
                break;
        }       
    }
    void UpdateUI()
    {
        if (null == dailyEdgesObj)
            return;

        // 先隐藏
        for (int i = 1;i <=7; ++i)
        {
            string goName = "Day" + i.ToString();
            dailyEdgesObj.transform.Find(goName).gameObject.SetActive(false);
        }

        // 再显示
        if (m_day > 0 && m_day < 8)
        {
            string goName = "Day" + m_day.ToString();
            dailyEdgesObj.transform.Find(goName).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("invalid day:" + m_day);
        }
    }
	// Use this for initialization
    new void Start()
    {
        InitBtn();

        dailyEdgesObj = GameObject.Find("DailyImageEdges");
        UpdateUI();
	}
	
	// Update is called once per frame
    new void Update()
    {
        base.Update();
	}
}
