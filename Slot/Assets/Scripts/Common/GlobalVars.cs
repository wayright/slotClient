using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 背景音乐，全程播放
public class GlobalVars : MonoBehaviour
{
    private bool m_musicOn = true;
    private bool m_seOn = true;
    private bool m_notifyOn = true;
    private int m_loginType = Constants.Login_UUID;
    private string m_loginGuid = "";
    private string m_loginEmail = "";
    private string m_pwd = "";
    private bool m_testing = false;
    private bool m_switchUser = false;

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
    public bool SwitchUser
    {
        get { return m_switchUser; }
        set { m_switchUser = value; }
    }
    public int LoginType
    {
        get { return m_loginType; }
        set { m_loginType = value; WriteOption("loginType", m_loginType); }
    }
    public string LoginGuid
    {
        get { return m_loginGuid; }
        set { m_loginGuid = value; WriteOption("loginGuid", m_loginGuid); }
    }
    public string LoginEmail
    {
        get { return m_loginEmail; }
        set { m_loginEmail = value; WriteOption("loginEmail", m_loginEmail); }
    }
    public string LoginPwd // MD5
    {
        get { return m_pwd; }
        set { m_pwd = value; WriteOption("loginPwd", m_pwd); }
    }
    public bool Testing
    {
        get { return m_testing; }
        set { m_testing = value; WriteOption("testing", m_testing); }
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
        WriteOption("musicOn", bOn);
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
        WriteOption("seOn", bOn);
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
        WriteOption("notifyOn", bOn);
    }

    public bool ReadOption(string key, bool def)
    {
        int iDefault = 0;
        if (def)
            iDefault = 1;

        int iVal = PlayerPrefs.GetInt(key, iDefault);
        if (iVal == 0)
            return false;
        else
            return true;
    }
    public int ReadOption(string key, int def)
    {
        int iVal = PlayerPrefs.GetInt(key, def);
        return iVal;
    }
    public string ReadOption(string key, string def)
    {
        string ret = PlayerPrefs.GetString(key, def);
        return ret;
    }
    public void WriteOption(string key, bool val)
    {
        int iVal = 0;
        if (val)
            iVal = 1;

        PlayerPrefs.SetInt(key, iVal);
    }
    public void WriteOption(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
    }
    public void WriteOption(string key, string val)
    {
        PlayerPrefs.SetString(key, val);
    }
    void Awake()
    {
        //此脚本永不消毁，并且每次进入初始场景时进行判断，若存在重复的则销毁  
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this);

            // read config file
            m_musicOn = ReadOption("musicOn", true);
            m_notifyOn = ReadOption("notifyOn", true);
            m_seOn = ReadOption("seOn", true);
            m_loginType = ReadOption("loginType", Constants.Login_UUID);
            m_loginEmail = ReadOption("loginEmail", "");
            m_loginGuid = ReadOption("loginGuid", "");
            m_pwd = ReadOption("loginPwd", "");
            m_testing = ReadOption("testing", false);

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
        else if (this != m_instance)
        {
            Destroy(gameObject);
        }    
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
