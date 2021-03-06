﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour {
    private float m_cdLeft = 4 * 60 * 60;
    public long LeftTime
    {
        get { return (int)m_cdLeft; }
        set { m_cdLeft = value; }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_cdLeft <= 0)
        {
            return;
        }

        int before = (int)m_cdLeft;
        m_cdLeft -= Time.deltaTime;
        int after = (int)m_cdLeft;

        if (after != before)
        {
            int h = (int)(after / 3600.0f);
            after -= h * 3600;
            int m = (int)(after / 60.0f);
            after -= m * 60;
            int s = after;

            string ts = "";
            if (h < 10)
                ts += "0";
            ts += h.ToString() + ":";
            if (m < 10)
                ts += "0";
            ts += m.ToString() + ":";
            if (s < 10)
                ts += "0";
            ts += s.ToString();

            GetComponent<Text>().text = ts;
        }

        if (after == 0)
        {
            Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
            recp.EndCountDown();
        }
	}
}
