using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; 

public class SlotClientButtonEvents : MonoBehaviour {
    public System.Random rd = new System.Random(); // Test
    private Dictionary<string, int> m_btnIndexDict = new Dictionary<string,int>();
    private SlotClientUser m_user = null;
    private int m_spinCheck = 0; // 检查金币是否有误
	// Use this for initialization
	void Start () {
        // 为所有按钮添加点击事件
        // 部分没有点击事件
        for (int i = 0; i < SlotClientConstants.Btn_Strings.Length;++i )
        {
            string btnName = SlotClientConstants.Btn_Strings[i];
            m_btnIndexDict.Add(btnName, i);
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }

        m_user = GameObject.Find("SlotClientUser").GetComponent<SlotClientUser>();
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
        
        switch ((SlotClientConstants.Btn)btnIndex)
        {
            case SlotClientConstants.Btn.Btn_Spin:
                OnButtonSpin();                
                break;
            case SlotClientConstants.Btn.Btn_LineMinus:
                break;
            case SlotClientConstants.Btn.Btn_LineAdd:
                break;
            case SlotClientConstants.Btn.Btn_BetAdd:
                OnButtonBetAdd();
                break;
            case SlotClientConstants.Btn.Btn_BetMinus:
                OnButtonBetMinus();
                break;
            case SlotClientConstants.Btn.Btn_AutoSpin:
                m_user.AutoSpin = !m_user.AutoSpin;
                m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_Spin);
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
        if (m_user.AutoSpin && !m_user.Spinning && m_user.Login)
        {
            if (m_spinCheck > 0 && m_spinCheck % 10 == 0)
            {
                // 重新登录检查金币是否准确
                m_spinCheck = 0;
                m_user.Login = false;
                m_user.Requests.ReqQuickLogin();
            }
            else
            {
                OnButtonSpin();
            }
        }
	}
    void OnButtonSpin()
    {
        m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_Spin);
        
        if (m_user.Spinning)
        {
            Debug.Log("I'm spinning!");
            return;
        }
        else
        {
            m_user.SpinCount++;
            m_spinCheck = m_user.SpinCount;
            m_user.Spinning = true;
        }

        if (m_user.Win > 0) // 有奖励没有领取
        {
            Debug.Log("Error!"); // 当前是自动领取
            m_user.Displays.ShowJumpWin(); // 点击领取
        }
        else
        {
            m_user.Requests.ReqSpin();
        }
    }
    void OnButtonBetAdd()
    {
        m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_PlusMinus);
        int bet = m_user.Bet;
        if (bet == 30)
        {
            bet = 10;
        }
        else
        {
            bet += 10;
        }
        m_user.Bet = bet;
    }
    void OnButtonBetMinus()
    {
        m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_PlusMinus);
        int bet = m_user.Bet;
        if (bet == 10)
        {
            bet = 30;
        }
        else
        {
            bet -= 10;
        }
        m_user.Bet = bet;
    }
}
