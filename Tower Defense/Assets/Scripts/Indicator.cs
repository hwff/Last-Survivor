using UnityEngine;
using System.Collections;

public class Indicator : MonoBehaviour {

    public static bool isOn;

    public static Vector3 localpos;

	// Use this for initialization
	void Start () {
        isOn = false;
        this.GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isOn)
        {
            this.GetComponent<Transform>().localPosition = localpos;
            this.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            this.GetComponent<Renderer>().enabled = false;
        }	
	}
}
