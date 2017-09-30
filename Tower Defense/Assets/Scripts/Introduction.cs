using UnityEngine;
using System.Collections;

public class Introduction : MonoBehaviour {

    public static bool isOnSelect;

    public static Vector3 localpos;

    public static GameObject intro;

    // Use this for initialization
    void Start () {
        isOnSelect = false;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Renderer>().material.renderQueue = 3009;
    }
	
	// Update is called once per frame
	void Update () {
        if (isOnSelect)
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
