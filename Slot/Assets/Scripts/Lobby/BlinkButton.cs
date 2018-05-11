using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlinkButton : MonoBehaviour {
    Color colorStart = Color.white;
    Color colorEnd = Color.red;
    float duration = 1.0f;

    //float minimum = 0.0f;
    //float maximum = 200.0f;
    //CanvasGroup cg;
	// Use this for initialization
	void Start () {
        //cg = this.transform.GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        transform.GetComponent<Image>().color = Color.Lerp(colorStart, colorEnd, lerp);
        //transform.position.x = Mathf.Lerp(minimum, maximum, lerp);	
	}
}
