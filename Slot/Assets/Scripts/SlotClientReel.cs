using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotClientReel : MonoBehaviour
{
    private static System.Random rdSpped = new System.Random();
    private static System.Random rdCheer = new System.Random();
    public float RollingTime = 0;// 滚动计时器
    private ScrollRect m_scrollRect;
    private Vector3 m_initPosition; // 原始位置
    private float m_curTimer = 2.0f; // 当前滚动计时器
    private float m_bufferTimer = 0.0f; // 缓冲时间
    private float m_bonusTimer = 0.0f; // bonus计时器，过时间对话框消失
    private float m_speed = 0; // 滚东速度
    private int m_item = 1; // 摇中的项索引    
    private float m_a1 = 10; // 加速度1
    private float m_a2 = 10; // 加速度2
    private float m_a3 = 10; // 加速度3
    private int m_bonusItem; // 中奖项
    public const int StartPositionY = -1143; // 第二个在中间
    public const int ItemHeight = 235;
    public static string[] ItemStringArray = { "None", "东", "南", "西", "北", "中" };
    private int[] m_posYArray = new int[6];
    private bool m_return = false;
    private List<Tiger.Proto.TigerBonus> m_bonus = null; // 奖励，最后一个才有
    private SlotClientUser m_user = null;
  
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

        m_user = GameObject.Find("SlotClientUser").GetComponent<SlotClientUser>();
	}
    public void Spin(int item, List<Tiger.Proto.TigerBonus> bonus)
    {
        m_speed = rdSpped.Next(2000, 3000);
        m_item = item > 5 ? item % 5 : item;

        //Debug.Log(ItemStringArray[m_item]);
        m_curTimer = RollingTime; // 滚动2秒

        if (m_bonus != null)
            m_bonus.Clear();
        m_bonusItem = -1;

        if (bonus != null)
        {
            m_bonus = bonus;
            GetBonusItem();
        }
    }

    void GetBonusItem()
    {
        Dictionary<int, int> dictPos = new Dictionary<int, int>();
        for (int i = 1; i <= 5; ++i)
        {
            string reelName = "reel" + i.ToString();
            GameObject gameObj = GameObject.Find(reelName);
            SlotClientReel curReel = gameObj.GetComponent<SlotClientReel>();
            if (dictPos.ContainsKey(curReel.m_item))
            {
                dictPos[curReel.m_item]++;
            }
            else
            {
                dictPos[curReel.m_item] = 1;
            }

        }

        int maxItem = -1;
        int maxTimes = -1;
        foreach (var item in dictPos)
        {
            if (item.Value > maxTimes)
            {
                maxTimes = item.Value;
                maxItem = item.Key;
            }
        }

        m_bonusItem = maxItem;
    }

    void RestoreBonusItems()
    {
        string imgName = "Image";
        // m_item在中间
        if (m_bonusItem >= 2)
        {
            imgName += " (";
            imgName += (m_bonusItem - 1).ToString();
            imgName += ")";
        }
        else if (m_bonusItem == 1)
        {
            imgName += " (5)";
        }

        for (int i = 1; i <= 5; ++i)
        {
            string reelName = "reel" + i.ToString();
            GameObject gameObj = GameObject.Find(reelName);
            SlotClientReel curReel = gameObj.GetComponent<SlotClientReel>();
            if (curReel.m_item == m_bonusItem)
            {
                Transform tfPanel = curReel.transform.Find("GridLayoutPanel");
                Transform tfImage = tfPanel.transform.Find(imgName);
                if (tfImage == null)
                {
                    Debug.Log("Cant find:" + imgName);
                }
                else
                {
                    Image img = tfImage.GetComponent<Image>();
                    img.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
        }
    }

    void RotateBonusItems()
    {
        string imgName = "Image";
        // m_item在中间
        if (m_bonusItem >= 2)
        {
            imgName += " (";
            imgName += (m_bonusItem - 1).ToString();
            imgName += ")";
        }
        else if (m_bonusItem == 1)
        {
            imgName += " (5)";
        }

        for (int i = 1; i <= 5; ++i)
        {
            string reelName = "reel" + i.ToString();
            GameObject gameObj = GameObject.Find(reelName);
            SlotClientReel curReel = gameObj.GetComponent<SlotClientReel>();
            if (curReel.m_item == m_bonusItem)
            {
                Transform tfPanel = curReel.transform.Find("GridLayoutPanel");
                Transform tfImage = tfPanel.transform.Find(imgName);
                if (tfImage == null)
                {
                    Debug.Log("Cant find:" + imgName);
                }
                else
                {
                    Image img = tfImage.GetComponent<Image>();
                    img.transform.Rotate(Vector3.up, Time.deltaTime * 360);
                }
            }
        }
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
        // 检查金币
        GameObject coin = GameObject.Find("Coin");
        Image coinImg = coin.GetComponent<Image>();
        Color col = coinImg.color;
        if (col.a > 0.0f)
        {
            if (coinImg.transform.localPosition.y >= 480)
            {
                // 几乎消失
                coin.transform.localPosition = new Vector3(385, -379, 0);
                coinImg.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
            }
            else
            {                
                // 移动吧！
                float step = 100 * Time.deltaTime;
                coin.transform.localPosition =
                    Vector3.MoveTowards(coin.transform.localPosition,
                    new Vector3(146, 486, 0), step); 
            }
        }

        // 检查缓冲
        if (m_bufferTimer > 0)
        {
            m_bufferTimer -= Time.deltaTime;
            if (m_bufferTimer < 0)
            {
                m_user.Displays.StopAudio(SlotClientConstants.Audio.Audio_ReelRolling);
                m_user.Displays.StopAudio(SlotClientConstants.Audio.Audio_ReelStop);

                if (m_bonus.Count == 0)
                {
                    // 没有奖励，可以继续摇了
                    m_user.Spinning = false;
                }
                else
                {
                    for (int i = 0; i < m_bonus.Count; ++i)
                    {
                        Tiger.Proto.TigerBonus bonus = m_bonus[i];
                        // 回调Display，更新金币数 
                        switch (bonus.type)
                        {
                            case 1:// 倍数
                                {
                                    m_user.Win = m_user.Bet * bonus.data1;
                                }
                                break;
                            case 2:// 金币
                                m_user.Win += bonus.data1;
                                break;
                            case 3:
                                break;
                            default:
                                break;
                        }
                    }
                    m_bonusTimer = 2.0f; // bonus展示
                    int audIdx = rdCheer.Next(2, 5);
                    m_user.Displays.PlayAudio((SlotClientConstants.Audio)audIdx);
                }
            }
        }

        // 检查奖励
        if (m_bonusTimer > 0)
        {
            m_bonusTimer -= Time.deltaTime;
            if (m_bonusTimer < 0)
            {
                // 确保都回0度
                RestoreBonusItems();
                
                // 显示跳跃数字
                m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_CoinFly);
                m_user.Displays.ShowJumpWin();
            }

            // 开始飞跃金币
            if (col.a == 0.0f)
            {
                //m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_CoinFly);
                coinImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            RotateBonusItems();
        }

        // 检查滚动
        if (m_speed > 0)
        {
            m_curTimer -= Time.deltaTime;
            if (m_curTimer <= 0)
            {
                // 滚动停止
                m_user.Displays.PlayAudio(SlotClientConstants.Audio.Audio_ReelStop);

                Vector3 pos = m_scrollRect.content.localPosition;
                pos.y = m_posYArray[m_item];

                m_scrollRect.content.localPosition = pos;
                m_speed = 0;

                if (m_bonus != null) // 判断为第五个Reel
                {
                    // 给出0.3秒的缓冲，让ReelStop执行完毕
                    m_bufferTimer = 0.3f;                    
                }
            }

            // 滚动
            m_scrollRect.content.Translate(Vector3.up * m_speed * Time.deltaTime, Space.Self);
            if (m_scrollRect.content.localPosition.y > 29)
            {
                m_scrollRect.content.localPosition = m_initPosition;
            }
        }
	}
}
