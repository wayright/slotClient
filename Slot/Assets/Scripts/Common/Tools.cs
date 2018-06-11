using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Security.Cryptography;

// Tools
public class Tools
{
    /// <summary>  
    /// 金币转字符串
    /// </summary>  
    /// <param name="coin">金币数</param>
    public static string CoinToString(long coin)
    {
        string s = "";
        double b = coin;
        if (coin > 1000000000) // Billion
        {
            b /= 1000000000.0;
            s = System.String.Format("{0:N}", b);
            s += "B";
        }
        else if (coin > 10000000) // Million
        {
            b /= 1000000.0;
            s = System.String.Format("{0:N}", b);
            s += "M";
        }
        else if (coin == 0)
        {
            s = "0";
        }
        else
        {
            s = b.ToString("###,###");
        }

        return s;
    }
    /// <summary>  
    /// 字符串转Int32
    /// </summary>  
    /// <param name="str">字符串</param>
    public static int StringToInt32(string str)
    {
        int iVal = 0;
        try
        {
            iVal = System.Convert.ToInt32(str);
        }
        catch (System.Exception e)
        {
            DebugConsole.Log(e.Message);
        }
        return iVal;
    }
    /// <summary>  
    /// 字符串转Int64
    /// </summary>  
    /// <param name="str">字符串</param>
    public static long StringToInt64(string str)
    {
        long i64Val = 0;
        try
        {
            i64Val = System.Convert.ToInt64(str);
        }
        catch (System.Exception e)
        {
            DebugConsole.Log(e.Message);
        }
        return i64Val;
    }
    /// <summary>  
    /// Get string's md5 string
    /// </summary>  
    /// <param name="str">字符串</param>
    public static string GetMD5(string msg)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }
    /// <summary>  
    /// 加载图像，并设置到按钮
    /// </summary>  
    /// <param name="url">图像地址</param>
    /// <param name="btnName">按钮名称</param>
    public static IEnumerator LoadWWWImageToButton(string url, string btnName)
    {
        //请求WWW
        WWW www = new WWW(url);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //获取Texture
            Texture2D texture = www.texture;

            //创建Sprite 内存泄漏！！！ 暂时不用
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            GameObject.Find(btnName).GetComponent<Image>().sprite = sprite;
        }
    }
    public static IEnumerator LoadWWWImage(string url, Image img)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //获取Texture
            Texture2D texture = www.texture;

            //创建Sprite 内存泄漏！！！ 暂时不用
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            img.sprite = sprite;
        }
    }
    public static void PlayNotification(Constants.Audio aud)
    {
        if (!GlobalVars.instance.GetNotify())
            return;

        if (aud >= Constants.Audio.Audio_Max)
        {
            DebugConsole.Log("Ivalid audio enum");
            return;
        }

        string audStr = Constants.Audio_Strings[(int)aud];
        AudioSource aSource = GameObject.Find(audStr).GetComponent<AudioSource>();
        aSource.Play();
    }
    // button's audio
    public static void PlayAudio(Constants.Audio aud)
    {
        if (!GlobalVars.instance.GetSE())
            return;

        if (aud >= Constants.Audio.Audio_Max)
        {
            DebugConsole.Log("Ivalid audio enum");
            return;
        }

        string audStr = Constants.Audio_Strings[(int)aud];
        AudioSource aSource = GameObject.Find(audStr).GetComponent<AudioSource>();
        aSource.Play();
    }
    public static string MsecondToHHMMSS(long ms)
    {
        int s = (int)(ms / 1000);

        int h = (int)(s / 3600.0f);
        s -= h * 3600;
        int m = (int)(s / 60.0f);
        s -= m * 60;
        int ss = s;

        string ts = "";
        if (h < 10)
            ts += "0";
        ts += h.ToString() + ":";
        if (m < 10)
            ts += "0";
        ts += m.ToString() + ":";
        if (s < 10)
            ts += "0";
        ts += ss.ToString();

        return ts;
    }
}
