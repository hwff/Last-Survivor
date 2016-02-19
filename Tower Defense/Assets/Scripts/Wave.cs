using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Wave : MonoBehaviour {

	public static int wave;

	// Use this for initialization
	void Start () {
		wave = 0;
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<Text>().text = string.Format("Wave: {0}", wave);
	}
}
