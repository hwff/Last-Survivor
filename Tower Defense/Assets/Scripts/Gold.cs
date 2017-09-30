using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gold : MonoBehaviour {

	public static int gold;

	// Use this for initialization
	void Start () {
		gold = 300;
	}

	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<Text>().text = string.Format("Gold: {0}", gold);
	}
}
