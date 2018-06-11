using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputRegEmailPasswordAgain : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        transform.GetComponent<InputField>().onEndEdit.AddListener(End_Value);	
	}

    public void End_Value(string inp)
    {
        DialogRegEmail.GetInstance().PasswordAgain = inp;
    }
}
