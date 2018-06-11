using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCoin : MonoBehaviour
{
    private float m_a = 5000f; // acceleration
    private float m_vY = 0; // velocity for Y
    private float m_vX = 0;
    private float m_bv = 1000; // bounce velocity
    private float m_horizontalY = -176; // plane
    private float m_delay = 3; // 3 seconds
    private Vector3 m_startPosition = new Vector3();
    private float m_elapse = 0;
    private bool m_touchPlane = false;
    static private System.Random m_rand = new System.Random();
    static private int sJumpedCount = 0;
    static private int sMovedCount = 0;
    private bool m_jumpped = false;
    private bool m_end = true;
    public static int sTotalCoinCount = 20; // max-20
    public static WorkDone sCallback = null;
    private Vector3 m_tar = new Vector3();
    private bool m_moving = false;
    private float m_moveSpeed = 1;
    public void JumpAndMove(float height = 0)
    {
        m_end = false;
        m_touchPlane = false;
        m_delay = 1;
        transform.localPosition = new Vector3(0, height, 0);
        m_startPosition = transform.localPosition;
    }
    public static void Init(WorkDone cb)
    {
        sMovedCount = 0;
        sJumpedCount = 0;
        sCallback = cb;
    }
    // Use this for initialization
    void Start()
    {
        //JumpAndMove(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_jumpped)
        {
            if (sJumpedCount > 0)
            {
                if (sJumpedCount == sTotalCoinCount)
                {
                    // 所有金币跳跃完成
                    sJumpedCount = 0;
                }
                return;
            }
            else
            {
                m_jumpped = false;

                Transform tfPic = GameObject.Find("BtnCredits").transform.Find("ImgBagPic");
                m_tar = tfPic.position;

                m_moveSpeed = Vector3.Distance(m_tar, transform.position) / 1.5f;// m_rand.Next(500, 2000);
                int factor = m_rand.Next(1, 10);
                m_moveSpeed *= ((float)factor * 1.0f);
                m_moving = true;
                //m_moveSpeed = 25;
              
                //Init(0);
                //m_jumpped = false;         
                return;
            }
        }

        if (m_moving)
        {
            Vector3 offSet = m_tar - transform.position;
            transform.position += offSet.normalized * m_moveSpeed * Time.deltaTime;

            if (Vector3.Distance(m_tar, transform.position) < 0.5f || m_tar.y > transform.position.y)
            {
                m_moving = false;
                transform.position = m_tar;
                gameObject.SetActive(false);
                transform.position = m_startPosition;
                m_end = true;

                sMovedCount++;
                if (sMovedCount == sTotalCoinCount)
                {
                    if (sCallback != null)
                    {
                        sCallback();
                    }
                }
            }           

            return;
        }

        if (m_end)
            return;

        if (m_delay > 0)
        {
            m_delay -= Time.deltaTime;
            if (m_delay < 0)
            {
                // begin jump
                m_vY = m_rand.Next(600, 1800);
                m_vX = m_rand.Next(-800, 800);

                // 2
                /*
                transform.localPosition = new Vector3(0, 500, 0);
                m_startPosition = transform.localPosition;
                m_vY = 0;
                m_vX = 0;
                */
                return;
            }
        }
        else
        {
            // jumping
            m_elapse += Time.deltaTime;
            float shiftY = m_vY * m_elapse - 0.5f * m_a * m_elapse * m_elapse; // vt - (gt^2) / 2;
            float shiftX = m_vX * m_elapse;

            if (shiftY < m_horizontalY - m_startPosition.y)
            {
                m_vX = 0;
                m_elapse = 0;

                if (!m_touchPlane)
                {
                    m_vY = m_bv * m_vY / 2000;
                    m_touchPlane = true;
                }
                else
                {
                    m_vY /= 1.5f;
                }

                Vector3 curPosition = new Vector3(shiftX, 0, 0);
                curPosition = m_startPosition + curPosition;
                curPosition.y = m_horizontalY;
                m_startPosition = curPosition;

                if (m_vY < 100)
                {
                    m_jumpped = true;
                    sJumpedCount++;
                    //Init();
                }

                return;
            }
            else
            {
                Vector3 curPosition = new Vector3(shiftX, shiftY, 0);
                transform.localPosition = m_startPosition + curPosition;
            }
        }
    }
}
