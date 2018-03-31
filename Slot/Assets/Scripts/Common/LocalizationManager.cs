using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
    static LocalizationManager m_instance;
    public static LocalizationManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LocalizationManager();
            }
            return m_instance;
        }
    }

    public const string m_language = Constants.Lang_CN;
    private Dictionary<string, string> m_dic = new Dictionary<string, string>();

    /// <summary>    
    /// 读取配置文件，将文件信息保存到字典里    
    /// </summary>    
    public LocalizationManager()
    {
        TextAsset ta = Resources.Load<TextAsset>(m_language);
        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null)
            {
                continue;
            }
            string[] keyAndValue = line.Split('=');
            m_dic.Add(keyAndValue[0], keyAndValue[1]);
        }
    }

    /// <summary>    
    /// 获取value    
    /// </summary>    
    /// <param name="key"></param>    
    /// <returns></returns>    
    public string GetValue(string key)
    {
        if (m_dic.ContainsKey(key) == false)
        {
            return null;
        }
        string value = null;
        m_dic.TryGetValue(key, out value);
        if (value == null)
        {
            Debug.Log("Cant find value!");
        }
        return value;
    }  
}
