using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour {
	
	public static int hp;

	// Use this for initialization
	void Start () {
		hp = 10;
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<Text>().text = string.Format("Health: {0}", hp);
	}
}
