using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SpawnData
{
    public int wave;
    public string enemyName;
    public int enemyType;
    public int level;
    public float wait;
}

public class EnemySpawn : MonoBehaviour {

    private List<SpawnData> EnemyDatas;
    public TextAsset ConfigFile;   

    private int idx;
    private float wait;

    public static int wave;

    public GameObject[] enemyPrefabs;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (idx < EnemyDatas.Count)
        {
            SpawnEnemy();
        }
        else
        {
            
        }
	}

    public void InitEnemySpawn()
    {
        ReadXml();
        wave = 1;
        Wave.wave = wave;
        wait = 5.0f;
    }

    private void SpawnEnemy()
    {
        SpawnData data = EnemyDatas[idx];

        wait -= Time.deltaTime;
        if (wait <= 0)
        {
            if (wave == data.wave)
            {
                wait = data.wait;
                CreateEnemy(data);
                idx++;
            }
            else if (wave < data.wave && GameManager.instance.enemies.Count == 0)
            {
                wait = data.wait;
                wave = data.wave;
                Wave.wave = wave;
                CreateEnemy(data);
                idx++;
            }
        }     
    }

    private void CreateEnemy(SpawnData data)
    {
        GameObject enemy = Instantiate(enemyPrefabs[data.enemyType]) as GameObject;
        GameManager.instance.enemies.Add(enemy.GetComponent<Enemy>());
        GameObject map = GameObject.Find("Map");
        Transform eTransform = enemy.GetComponent<Transform>();
        eTransform.parent = map.GetComponent<Transform>();
        eTransform.localPosition = Map.startNode.position;
        enemy.GetComponent<Enemy>().DataInitialization(data.level, data.enemyType, idx);
    } 

    private void ReadXml()
    {
        EnemyDatas = new List<SpawnData>();

        XmlDocument document = new XmlDocument();
        document.LoadXml(ConfigFile.text);
        XmlElement root = document.DocumentElement;

        XmlNodeList nodes = root.SelectNodes("/Enemies/Enemy");
        foreach (XmlNode node in nodes)
        {
            SpawnData data = new SpawnData();
            data.wave = int.Parse(node.Attributes[0].Value);
            data.enemyName = node.Attributes[1].Value;
            data.enemyType = int.Parse(node.Attributes[2].Value);
            data.level = int.Parse(node.Attributes[3].Value);
            data.wait = float.Parse(node.Attributes[4].Value);

            EnemyDatas.Add(data);
        }
    }
}
