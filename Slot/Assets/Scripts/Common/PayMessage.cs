using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class PayMessage : MonoBehaviour {
    private string m_token = "";
    private string m_sku = "";
    private string m_packageName = "";
    private string m_orderId = "";
    private string m_others = "";
    private int m_idx = -1;
	// Use this for initialization
	void Start () {
        m_others = "";
	}
	// Update is called once per frame
	void Update () {
	}

    
    void Message(string msg)
    {        
        if (msg == "beginConsumeComplete")
        {
            m_idx = 1;
            return;
        }
        else if (msg == "endConsumeComplete")
        {
            Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
            recp.BuyItem(m_packageName, m_sku, m_token, m_orderId);

            m_idx = -1;
            return;
        }

        if (m_idx >= 0)
        {
            switch (m_idx)
            {
                case 1:
                    m_packageName = msg;
                    break;
                case 2:
                    m_sku = msg;
                    break;
                case 3:
                    m_token = msg;
                    break;
                case 4:
                    m_orderId = msg;
                    break;
                case 5:
                    m_others = msg;
                    break;
            }

            m_idx++;
        }        
    }
}
