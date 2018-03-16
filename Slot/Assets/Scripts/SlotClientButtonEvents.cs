using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; 

public class SlotClientButtonEvents : MonoBehaviour {
    public System.Random rd = new System.Random(); // Test
    private Dictionary<string, int> m_btnIndexDict = new Dictionary<string,int>();
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
                
                //SlotClientRequests slotClientReq = GameObject.Find("BtnSpin").GetComponent<SlotClientRequests>();
                SlotClientUser slotClientUser = GameObject.Find("SlotClientUser").GetComponent<SlotClientUser>();
                slotClientUser.Requests.ReqSpin();
                break;
            case SlotClientConstants.Btn.Btn_LineMinus:
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
		
	}
}
