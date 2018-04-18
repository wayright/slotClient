using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSearchFriendCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //transform.GetComponent<InputField>().onValueChanged.AddListener(Changed_Value);
        transform.GetComponent<InputField>().onEndEdit.AddListener(End_Value);	
	}
    //public void Changed_Value(string inp)
    //{
    //   //print("正在输入:" + inp);
    //}
    public void End_Value(string inp)
    {
        DialogFriends.instance().SearchCode = inp;
    }
	// Update is called once per frame
    //void Update () {
		
    //}
}
