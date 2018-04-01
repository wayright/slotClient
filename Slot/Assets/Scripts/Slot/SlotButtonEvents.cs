using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlotButtonEvents : MonoBehaviour {
    public System.Random rd = new System.Random(); // Test
    private Dictionary<string, int> m_btnIndexDict = new Dictionary<string,int>();
    private SlotClerk m_clerk = null;
    private int m_spinCheck = 0; // 检查金币是否有误
	// Use this for initialization
	void Start () {
        // 为所有按钮添加点击事件
        // 部分没有点击事件
        for (int i = 0; i < Constants.Btn_Strings.Length;++i )
        {
            string btnName = Constants.Btn_Strings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }

        m_clerk = GameObject.Find("SlotClerk").GetComponent<SlotClerk>();
	}

    public void OnClick(GameObject sender)
    {
        int btnIndex = GetBtnIndexFromName(sender.name);
        if (btnIndex < 0)
        {
            Debug.Log("Cant find button:" + sender.name);
            return;
        }

        Debug.Log("You click button:" + sender.name);
        
        switch ((Constants.Btn)btnIndex)
        {
            case Constants.Btn.Btn_Spin:
                OnButtonSpin();                
                break;
            case Constants.Btn.Btn_LineMinus:
                break;
            case Constants.Btn.Btn_LineAdd:
                break;
            case Constants.Btn.Btn_BetAdd:
                OnButtonBetAdd();
                break;
            case Constants.Btn.Btn_BetMinus:
                OnButtonBetMinus();
                break;
            case Constants.Btn.Btn_AutoSpin:
                m_clerk.AutoSpin = !m_clerk.AutoSpin;
                m_clerk.Displays.PlayAudio(Constants.Audio.Audio_Spin);
                break;
            case Constants.Btn.Btn_Return:
                m_clerk.Displays.PlayAudio(Constants.Audio.Audio_Spin);
                m_clerk.Net.Close();
                Debug.Log("Slot enter lobby scene");
                SceneManager.LoadScene("lobby");
                break;
            default:
                break;
        }
    }

    int GetBtnIndexFromName(string btnName)
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
    
	// Update is called once per frame
	void Update () {
        if (m_clerk.AutoSpin && !m_clerk.Spinning && m_clerk.Login)
        {
            if (m_spinCheck > 0 && m_spinCheck % 10 == 0)
            {
                // 重新登录检查金币是否准确
                m_spinCheck = 0;
                //m_clerk.Login = false;
                //m_clerk.Requests.ReqQuickLogin();
            }
            else
            {
                OnButtonSpin();
            }
        }
	}
    void OnButtonSpin()
    {
        m_clerk.Displays.PlayAudio(Constants.Audio.Audio_Spin);
        
        if (m_clerk.Spinning)
        {
            Debug.Log("I'm spinning!");
            return;
        }
        else
        {
            m_clerk.SpinCount++;
            m_spinCheck = m_clerk.SpinCount;
            m_clerk.Spinning = true;
        }

        if (m_clerk.Win > 0) // 有奖励没有领取
        {
            Debug.Log("Error!"); // 当前是自动领取
            m_clerk.Displays.ShowJumpWin(); // 点击领取
        }
        else
        {
            m_clerk.Requests.ReqSpin();
        }
    }
    void OnButtonBetAdd()
    {
        m_clerk.Displays.PlayAudio(Constants.Audio.Audio_PlusMinus);
        int bet = m_clerk.Bet;
        if (bet == 30)
        {
            bet = 10;
        }
        else
        {
            bet += 10;
        }
        m_clerk.Bet = bet;
    }
    void OnButtonBetMinus()
    {
        m_clerk.Displays.PlayAudio(Constants.Audio.Audio_PlusMinus);
        int bet = m_clerk.Bet;
        if (bet == 10)
        {
            bet = 30;
        }
        else
        {
            bet -= 10;
        }
        m_clerk.Bet = bet;
    }
}
