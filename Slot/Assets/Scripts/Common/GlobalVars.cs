using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 背景音乐，全程播放
public class GlobalVars : MonoBehaviour
{
    private bool m_musicOn = true;
    private bool m_seOn = true;
    private bool m_notifyOn = true;
    static GlobalVars m_instance;
    // Use this for initialization  
    void Start()
    {

    }
    public static GlobalVars instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GlobalVars>();
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
    }
    public bool GetMusic()
    {
        return m_musicOn;
    }
    public void SetMusic(bool bOn = true)
    {
        if (m_musicOn == bOn)
            return;

        m_musicOn = bOn;
        AudioSource aSource = this.gameObject.GetComponent<AudioSource>();
        if (m_musicOn)
        {            
            aSource.Play();
        }
        else
        {
            aSource.Pause();
        }
    }
    public bool GetSE()
    {
        return m_seOn;
    }
    public void SetSE(bool bOn = true)
    {
        if (m_seOn == bOn)
            return;

        m_seOn = bOn;
    }
    public bool GetNotify()
    {
        return m_notifyOn;
    }
    public void SetNotify(bool bOn = true)
    {
        if (m_notifyOn == bOn)
            return;

        m_notifyOn = bOn;
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
