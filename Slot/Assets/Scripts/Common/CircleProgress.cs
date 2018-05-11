using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleProgress : MonoBehaviour {
    private float m_currentAmout = 0;
    private float m_speed = 1.0f;
    private const float s_TargetAmout = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_currentAmout < s_TargetAmout)
        {
            //DebugConsole.Log("currentAmount:" + currentAmout.ToString());

            m_currentAmout += m_speed;
            if (m_currentAmout > s_TargetAmout)
                m_currentAmout = s_TargetAmout;
            //indicator.GetComponent<Text>().text = ((int)currentAmout).ToString() + "%";
            GetComponent<Image>().fillAmount = m_currentAmout / 100.0f;
        }
        else
        {
            m_currentAmout = 0;
        }
	}
}
