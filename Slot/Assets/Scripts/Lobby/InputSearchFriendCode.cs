using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNickName : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.GetComponent<InputField>().onEndEdit.AddListener(End_Value);	
	}
    public void End_Value(string inp)
    {
        Reception recp = GameObject.Find("Reception").GetComponent<Reception>();
        recp.UpdateProfileName(inp);
    }
}
