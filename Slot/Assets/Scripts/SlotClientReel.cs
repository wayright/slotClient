using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotClientReel : MonoBehaviour
{
    private static System.Random rdSpped = new System.Random();
    public void Spin(int item, List<Tiger.Info.TigerBonus> bonus)
    {
        m_speed = rdSpped.Next(2000, 3000);
        m_item = item > 5 ? item % 5 : item;

        //Debug.Log(ItemStringArray[m_item]);
        m_timer = 2.0f;
        m_bonus.Clear();
        if (bonus != null)
        {
            m_bonus = bonus;
        }
    }

    private ScrollRect m_scrollRect;
    private Vector3 m_initPosition; // 原始位置
    private float m_timer = 2.0f; // 滚动计时器
    private float m_bonusTimer = 0.0f; // bonus计时器，过时间对话框消失
    private float m_speed = 0; // 股东速度
    public int m_item = 1; // 摇中的项索引    
    private float m_a1 = 10; // 加速度1
    private float m_a2 = 10; // 加速度2
    private float m_a3 = 10; // 加速度3
    public const int StartPositionY = -1143; // 第二个在中间
    public const int ItemHeight = 235;
    public static string[] ItemStringArray = { "None", "东", "南", "西", "北", "中" };
    public int[] m_posYArray = new int[6];
    private bool m_return = false;
    private List<Tiger.Info.TigerBonus> m_bonus = new List<Tiger.Info.TigerBonus>(); // 奖励，最后一个才有
  
	// Use this for initialization
	void Start () {
        m_scrollRect = gameObject.GetComponent<ScrollRect>();
        m_initPosition = m_scrollRect.content.localPosition;
        m_a1 = 1.1f / 0.6f;
        m_a2 = 1.1f / 0.8f;
        m_a3 = 1.1f / 1.2f;

        m_posYArray[0] = StartPositionY; // 2的替代值
        m_posYArray[1] = StartPositionY + ItemHeight * 4;
        m_posYArray[2] = StartPositionY + ItemHeight * 0;
        m_posYArray[3] = StartPositionY + ItemHeight * 1;
        m_posYArray[4] = StartPositionY + ItemHeight * 2;
        m_posYArray[5] = StartPositionY + ItemHeight * 3;

        //Debug.Log("PosY Init=" + m_scrollRect.content.localPosition.y);
	}

    void SpeedDown()
    {
        float a = m_a1;
        if (m_speed > 20)
        {
            a = m_a1;
        }
        else if (m_speed > 10)
        {
            a = m_a2;
        }
        else
        {
            a = m_a3;
        }

        if (m_speed > 60)
        {
            m_speed -= a;
        }
        else
        {
            m_return = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
               
        // 降速过程
        /*float a = m_a1;
        if (m_speed > 20)
        {
            a = m_a1;
        }
        else if (m_speed > 10)
        {
            a = m_a2;
        }
        else
        {
            a = m_a3;
        }

        if (m_speed > 60)
        {
            m_speed -= a;
        }
        else
        {
            m_return = true;
        }
        */
        if (m_bonusTimer > 0)
        {
            m_bonusTimer -= Time.deltaTime;
            if (m_bonusTimer < 0)
            {
                GameObject canvas = GameObject.Find("Canvas");
                GameObject bonusDialog = canvas.transform.Find("BonusDialog").gameObject;
                bonusDialog.SetActive(false);
            }
        }

        if (m_speed > 0)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                Vector3 pos = m_scrollRect.content.localPosition;
                pos.y = m_posYArray[m_item];

                m_scrollRect.content.localPosition = pos;
                m_speed = 0;

                if (m_bonus.Count > 0)
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    GameObject bonusDialog = canvas.transform.Find("BonusDialog").gameObject;
                    Image img = bonusDialog.GetComponent<Image>();
                    Text text = img.transform.Find("Text").GetComponent<Text>();
                    text.text = "Bonus:\n";
                    for (int i = 0; i < m_bonus.Count; ++i )
                    {
                        Tiger.Info.TigerBonus bonus = m_bonus[i];
                        text.text += "payline=" + bonus.line.ToString();
                        text.text += "\npattern=" + bonus.pattern.ToString();
                        text.text += "\ntype=" + bonus.type.ToString();
                        text.text += "\ndata1=" + bonus.data1.ToString();
                        text.text += "\ndata2=" + bonus.data2.ToString();

                        if (i < m_bonus.Count - 1)
                        {
                            text.text += "\n";
                        }
                    }
                    bonusDialog.SetActive(true);
                    m_bonusTimer = 3.0f;
                }
            }

            if (m_return && m_speed < 60)
            {
                float dis = m_scrollRect.content.localPosition.y - (float)m_posYArray[m_item];
                if (dis < 0)
                    dis = -dis;
                if (dis < 5)
                {
                    Vector3 pos = m_scrollRect.content.localPosition;
                    pos.y = m_posYArray[m_item];

                    m_scrollRect.content.localPosition = pos;
                    m_speed = 0;
                    //Debug.Log("结束慢速行动,dis=" + dis);
                    m_return = false;
                }                
            }
            //Debug.Log("Speed:" + m_speed.ToString());
            //Debug.Log("Before:" + m_scrollRect.content.localPosition.y.ToString());
            m_scrollRect.content.Translate(Vector3.up * m_speed * Time.deltaTime, Space.Self);
            //Debug.Log("After:" + m_scrollRect.content.localPosition.y.ToString());
            if (m_scrollRect.content.localPosition.y > 29)
            {
                //Debug.Log("CurPosition:" + m_scrollRect.content.localPosition.y.ToString());
                m_scrollRect.content.localPosition = m_initPosition;
            }
        }
	}
}
