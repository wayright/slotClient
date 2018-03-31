using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 背景音乐，全程播放
public class DontDestroyOnLoad : MonoBehaviour
{
    static DontDestroyOnLoad m_instance;
    // Use this for initialization  
    void Start()
    {

    }
    public static DontDestroyOnLoad instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<DontDestroyOnLoad>();
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
    }

    void Awake()
    {
        //此脚本永不消毁，并且每次进入初始场景时进行判断，若存在重复的则销毁  
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != m_instance)
        {
            Destroy(gameObject);
        }
        AudioSource aSource = this.gameObject.GetComponent<AudioSource>();
        //aSource.volume = 0;
    }
    // Update is called once per frame  
    void Update()
    {        
        //if (aSource.isPlaying)
        //{
        //    aSource.Pause();
        //}
    }  
}
