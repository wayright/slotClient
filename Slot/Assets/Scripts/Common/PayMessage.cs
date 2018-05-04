using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayMessage : MonoBehaviour {

    private string m_messageFromAndroid;//从安卓端接受的消息

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    void Messgae(string message)
    {
        m_messageFromAndroid = message;
        DialogBase.Show(m_messageFromAndroid, "Message");
    }
}
