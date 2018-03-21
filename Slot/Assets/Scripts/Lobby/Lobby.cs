using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 大厅
// 用singleTon模式来维持数据
// 不需要挂载到任何场景
public class Lobby{
    private static Lobby s_Instance = null;
    private long m_uId = 0;
    private long m_key = 0; // 临时
    private string m_domain; // 重定向地址
    private int m_port; // 重定向端口
    public static Lobby getInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = new Lobby();

        }
        return s_Instance;
    } 
	
    public long UId
    {
        get { return m_uId; }
        set { m_uId = value; }
    }
    public long Key
    {
        get { return m_key; }
        set { m_key = value; }
    }
    public string Domain
    {
        get { return m_domain; }
        set { m_domain = value; }
    }
    public int Port
    {
        get { return m_port; }
        set { m_port = value; }
    }
}
