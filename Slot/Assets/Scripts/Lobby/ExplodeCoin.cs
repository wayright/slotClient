using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeCoin : MonoBehaviour {
    private float m_vHori = 0;
    //private float m_airResistance = 0;
    private float m_g = 4.9f;
    //private float m_vDown = 0;
    private float m_startTime = 0f;
    private float m_startY = 0f;
    //private static GameObject sExplodeCoin;
    private bool m_moving = false;
    private Vector3 m_tar = new Vector3();
    private Vector3 m_ori = new Vector3();
    public static void Hide(int type = Constants.Bonus_Daily)
    {
        GameObject goBA = GameObject.Find("BonusAnimation");
        string objStr = "DailyBonusCoin";
        if (type == Constants.Bonus_Free)
            objStr = "FreeBonusCoin";
        GameObject goCoin = goBA.transform.Find(objStr).gameObject;
        goCoin.SetActive(false);
    }
    public static void Show(int type = Constants.Bonus_Daily)
    {
        GameObject goBA = GameObject.Find("BonusAnimation");
        string objStr = "DailyBonusCoin";
        if (type == Constants.Bonus_Free)
            objStr = "FreeBonusCoin";
        GameObject goCoin = goBA.transform.Find(objStr).gameObject;
        goCoin.SetActive(true);   
    }

    public static void MoveTo(int type, Vector3 tar)
    {
        GameObject goBA = GameObject.Find("BonusAnimation");
        string objStr = "DailyBonusCoin";
        if (type == Constants.Bonus_Free)
            objStr = "FreeBonusCoin";
        GameObject goCoin = goBA.transform.Find(objStr).gameObject;
        ExplodeCoin ec = goCoin.GetComponent<ExplodeCoin>();
        ec.DoMoveTo(tar);
    }
    public void DoMoveTo(Vector3 tar)
    {
        m_tar = tar;
        m_moving = true;
    }
    //private void MoveTo(Vector3 tar)
    //{
    //    if (m_moving)
    //    {
    //        const float speed = 0.5f;
    //        Vector3 offSet = tar - transform.position;
    //        transform.position += offSet.normalized * speed * Time.deltaTime;
    //        if (Vector3.Distance(tar, transform.position) < 0.5f)
    //        {
    //            m_moving = false;
    //            transform.position = tar;
    //        }
    //    }
    //}
    public float VHori
    {
        get { return m_vHori; }
        set { m_vHori = value; }
    }
	// Use this for initialization
	void Start () {
        m_ori = transform.position;
        m_startY = transform.position.y;
        //System.Threading.Thread.Sleep(OffsetTime);
        m_startTime = 666;
        //DebugConsole.Log("StartY:" + m_startY);
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_moving)
        {
            float t = (System.DateTime.Now.Millisecond - m_startTime) / 1000f;
            float h = 0.5f * m_g * t * t;
            transform.position = new Vector3(transform.position.x, m_startY - h, transform.position.z);
        }

        //DebugConsole.Log("CurY:" + (m_startY - h).ToString() + ",t=" + t.ToString());
        //transform.Translate(Vector3.right * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up, Time.deltaTime * 720);

        if (m_moving)
        {
            const float speed = 50f;
            Vector3 offSet = m_tar - transform.position;
            transform.position += offSet.normalized * speed * Time.deltaTime;
            if (Vector3.Distance(m_tar, transform.position) < 0.5f)
            {
                m_moving = false;
                transform.position = m_tar;
                gameObject.SetActive(false);
                transform.position = m_ori;
            }
        }
	}
}
