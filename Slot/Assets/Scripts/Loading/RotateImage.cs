using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(-Vector3.forward * 300 * Time.deltaTime);
	}
}
