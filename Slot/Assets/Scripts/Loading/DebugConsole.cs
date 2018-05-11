using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
public class DebugConsole : MonoBehaviour {
    static List<string> mLines = new List<string>();
    static List<string> mWriteTxt = new List<string>();

    private Text m_text = null;
    private static bool mDirty = false;
    private static AndroidJavaObject m_ajo = null;
   
	// Use this for initialization
	void Start () {
        m_text = GameObject.Find("DebugConsole").GetComponent<Text>();        
	}
	
	// Update is called once per frame
	void Update () {
		//因为写入文件的操作必须在主线程中完成，所以在Update中哦给你写入文件。  
        if (mWriteTxt.Count > 0)  
        {  
            string[] temp = mWriteTxt.ToArray();  
            foreach (string t in temp)  
            {                
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (m_ajo == null)
                    {
                        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        if (jc == null)
                        {
                            DialogBase.Show("ANDROID", "js is null");
                            return;
                        }

                        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                        if (jo == null)
                        {
                            DialogBase.Show("ANDROID", "jo is null");
                            return;
                        }
                        m_ajo = jo;
                    }

                    try
                    {
                        const string TAG = "FirstStudio.sloter";
                        m_ajo.Call("DebugLog", TAG, t);
                    }
                    catch (System.Exception e)
                    {
                        DialogBase.Show("ANDROID", e.Message);
                    }
                }
                mWriteTxt.Remove(t);  
            }  
        }

        if (mDirty)
        {
            mDirty = false;
            string[] temp = mLines.ToArray();
            string s = "";
            foreach (string item in temp)
            {
                s += item;
                s += "\n";
            }
            m_text.text = s;
        }
	}   
    //这里我把错误的信息保存起来，用来输出在手机屏幕上  
    public static void Log(string text)
    {
        if (mLines.Count > 45)
        {
            mLines.RemoveAt(0);
        }

        mLines.Add(text);
        mWriteTxt.Add(text);

        mDirty = true;

        Debug.Log(text);        
    }
}
