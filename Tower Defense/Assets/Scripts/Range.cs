using UnityEngine;
using System.Collections;

public class Range : MonoBehaviour {

    public static bool isOnTower;

    public static Vector3 localpos;

    public static float radius;
	// Use this for initialization
	void Start () {
        isOnTower = false;
        this.GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isOnTower)
        {
            this.GetComponent<Transform>().localScale = new Vector3(2 * radius, 2 * radius, 1.0f);
            this.GetComponent<Transform>().localPosition = localpos;         
            this.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            this.GetComponent<Renderer>().enabled = false;
        }
    }
}
