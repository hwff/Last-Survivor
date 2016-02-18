using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class UnitData
{
    public int state;               //the state of a unit, “Empty(0)” means tower can be built, "Used(1)" means tower has been built on this unit, "NoTower(2)" means unit is not for tower. 
    public int type;                //the type of a unit, "Path(0)" means it can be walked through, whereas "Obstacle(1)" means cannot.
}

public class PathData
{
    public float x;
    public float z;
}

public class Map : MonoBehaviour
{
    public int numX;                //the number of horizontal units
    public int numZ;                //the number of vertical units

    public float unitSizeX;         //the width of a unit
    public float unitSizeZ;         //the heigth of a unit

    public Unit towerUnitPrefab;        //tower unit
    public Unit pathUnitPrefab;        //other unit
    public Unit otherUnitPrefab;        //other unit
    private Unit[,] mapUnits;                //all units on the map  

    private int pathNum;                     //the number of nodes on the path
    public static PathNode startNode;

    public GameObject indicatorPrefab;       //a square that indicates a tower unit
    private GameObject indicator;

    public GameObject rangePrefab;           //a square that show the range of a tower
    private GameObject range;

    private GameObject _BG;                  //the map background

    public TextAsset UnitFile;
    public TextAsset PathFile;

    public static int towerNum;

    public void InitMap()
    {
        indicator = Instantiate(indicatorPrefab) as GameObject;
        indicator.transform.parent = transform;
        indicator.transform.localScale = new Vector3(unitSizeX, unitSizeZ, 1.0f);

        range = Instantiate(rangePrefab) as GameObject;
        range.transform.parent = transform;

        _BG = GameObject.Find("BG");
        _BG.GetComponent<Transform>().localScale = new Vector3(numX * unitSizeX, numZ * unitSizeZ, 1.0f);    //set the sizes of as the sizes of the combination of all units

        Unit.unitSizeX = unitSizeX;
        Unit.unitSizeZ = unitSizeZ;

        GenerateUnits();
        GeneratePath();
    }

    public void GenerateUnits()
    {
        List<UnitData> unitDatas = new List<UnitData>();
        ReadUnitsXml(unitDatas);

        mapUnits = new Unit[numX, numZ];
        for (int z = 0, i = 0; z < numZ; z++)
        {
            for (int x = 0; x < numX; x++)
            {
                CreateUnit(x, z, unitDatas[i].state, unitDatas[i++].type);
            }
        }
    }

    private void ReadUnitsXml(List<UnitData> ud)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(UnitFile.text);
        XmlElement root = document.DocumentElement;

        XmlNodeList nodes = root.SelectNodes("/Units/Unit");
        foreach (XmlNode node in nodes)
        {
            UnitData data = new UnitData();
            data.state = int.Parse(node.Attributes[0].Value);
            data.type = int.Parse(node.Attributes[1].Value);

            ud.Add(data);
        }
    }

    private void CreateUnit(int x, int z, int state, int type)
    {
        Unit newUnit;
        if (state == 0)
        {
            newUnit = Instantiate(towerUnitPrefab) as Unit;
            towerNum++;
        }
        else if (type == 0)
        {
            newUnit = Instantiate(pathUnitPrefab) as Unit;
        }
        else
        {
            newUnit = Instantiate(otherUnitPrefab) as Unit;
        }

        mapUnits[x, z] = newUnit;
        newUnit.name = "Map Unit " + x + ", " + z;
        newUnit.transform.parent = transform;  //set the parent of transform of each unit as the tranform of the map
        newUnit.transform.localScale = new Vector3(unitSizeX, unitSizeZ, 1.0f);
        newUnit.transform.localPosition = new Vector3((x - numX / 2.0f) * unitSizeX + unitSizeX / 2, 0.0f, (numZ / 2.0f - z) * unitSizeZ - unitSizeZ / 2);

        newUnit.localpos = new Vector3((x - numX / 2.0f) * unitSizeX + unitSizeX / 2, 0.0f, (numZ / 2.0f - z) * unitSizeZ - unitSizeZ / 2);
        newUnit.unitState = (UnitState)state;
        newUnit.unitType = (UnitType)type;
    }

    public void GeneratePath()
    {
        List<PathData> pathDatas = new List<PathData>();
        ReadPathXml(pathDatas);

        pathNum = pathDatas.Count;
        startNode = new PathNode(new Vector3(pathDatas[0].x, 0, pathDatas[0].z), null);
        PathNode p = startNode;
        for (int i = 1; i < pathNum; i++)
        {
            p.next = new PathNode(new Vector3(pathDatas[i].x, 0, pathDatas[i].z), null);
            p = p.next;
        }
    }

    private void ReadPathXml(List<PathData> pd)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(PathFile.text);
        XmlElement root = document.DocumentElement;

        XmlNodeList nodes = root.SelectNodes("/Path/Node");
        foreach (XmlNode node in nodes)
        {
            PathData data = new PathData();
            data.x = float.Parse(node.Attributes[0].Value);
            data.z = float.Parse(node.Attributes[1].Value);

            pd.Add(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
