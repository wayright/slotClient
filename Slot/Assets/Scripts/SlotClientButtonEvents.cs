using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; 

public class SlotClientButtonEvents : MonoBehaviour {
    public System.Random rd = new System.Random(); // Test
	// Use this for initialization
	void Start () {
        List<string> btnsName = new List<string>();
        btnsName.Add("BtnSpin");
        btnsName.Add("BtnLineMinus");
        btnsName.Add("BtnLineAdd");
        btnsName.Add("BtnBetMinus");
        btnsName.Add("BtnBetAdd");
        btnsName.Add("BtnMaxBet");
        btnsName.Add("BtnReturn"); // 返回
        btnsName.Add("BtnDeposit"); // 购买充值

        foreach (string btnName in btnsName)
        {
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
        switch (sender.name)
        {
            case "BtnSpin":
                Debug.Log("BtnSpin");
                //SlotClientRequests slotClientReq = GameObject.Find("BtnSpin").GetComponent<SlotClientRequests>();
                SlotClientUser slotClientUser = GameObject.Find("SlotClientUser").GetComponent<SlotClientUser>();
                slotClientUser.Requests.ReqSpin();
                break;
            case "BtnShop":
                Debug.Log("BtnShop");
                break;
            case "BtnLineMinus":
                Debug.Log("BtnLineMinus");
                break;
            default:
                Debug.Log("none");
                break;
        }
    }  
    
	// Update is called once per frame
	void Update () {
		
	}
}
