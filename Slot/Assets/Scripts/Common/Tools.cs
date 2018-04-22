using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

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
        else
        {
            s = b.ToString("###,###");
        }

        return s;
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
}
