using UnityEngine;
using UnityEditor;
using System.Collections;

public class Unit : MonoBehaviour {

    public static float unitSizeX;
    public static float unitSizeZ;

    [HideInInspector]
    public Vector3 localpos;

    [HideInInspector]
    public UnitState unitState;
    [HideInInspector]
    public UnitType unitType;

    public GameObject[] towerPrefab;

    private GameObject tower;

    private int towerType;

    public static UnitType onType;

	// Use this for initialization
	void Start () {
        towerType = 0;
	}
	

	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        onType = unitType;
        if (unitState != UnitState.NoTower)
        {
            Indicator.localpos = localpos;
            Indicator.isOn = true;
            if (unitState == UnitState.Used)
            {
                //Debug.Log("isOnTower");
                Range.localpos = localpos;
                Range.radius = tower.GetComponent<Tower>().range;
                Range.isOnTower = true;
            }
        }
    }

    void OnMouseExit()
    {
        if (unitState != UnitState.NoTower)
        {
            Indicator.isOn = false;
            Range.isOnTower = false;
        }
    }

    void OnMouseDown()
    {
        if (unitState == UnitState.Empty)
        {
            unitState = UnitState.Used;
            tower = Instantiate(towerPrefab[towerType]) as GameObject;
            Transform tTransform = tower.GetComponent<Transform>();
            tTransform.parent = transform;
            tTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            tTransform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
            GameManager.instance.towers.Add(tower.GetComponent<Tower>());
            tower.GetComponent<Tower>().id = GameManager.instance.towers.Count;
        }
    }
}
