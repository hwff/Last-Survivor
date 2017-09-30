using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private GameObject spawnManager;
    private GameObject mapManager;
    //private EventBus eventManager;

    public string unitFilename;
    public string pathFilename;

    [HideInInspector]
    public List<Enemy> enemies;
    [HideInInspector]
    public List<Tower> towers;

    public static GameManager instance;

    public GameObject heroPrefab;
    public GameObject[] towerPrefabs;
    public Select[] selectPrefabs;
    public GameObject[] effectPrefabs;

    [HideInInspector]
    public GameObject hero;

    public Texture[] selectUI;
    public Material[] selectMat;

	// Use this for initialization
	void Start () {
        enemies = new List<Enemy>();
        towers = new List<Tower>();
        instance = this;
        //eventManager = new EventBus();
        spawnManager = GameObject.Find("EnemySpawn");
        mapManager = GameObject.Find("Map");
        mapManager.GetComponent<Map>().InitMap();
        spawnManager.GetComponent<EnemySpawn>().InitEnemySpawn();

        hero = Instantiate(heroPrefab) as GameObject;
        Transform mTransform = mapManager.GetComponent<Transform>();
        hero.GetComponent<Transform>().parent = mTransform;
        hero.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        hero.GetComponent<Hero>().id = 0;
        hero.GetComponent<Renderer>().material.renderQueue = 3003;
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void begin()
    {
        EnemySpawn.start = true;
        Button sb = GameObject.Find("StartButton").GetComponent<Button>();
        sb.interactable = false; 
    }
}
