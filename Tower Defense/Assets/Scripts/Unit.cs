using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{

    public static float unitSizeX;
    public static float unitSizeZ;

    [HideInInspector]
    public Vector3 localpos;

    [HideInInspector]
    public UnitState unitState;
    [HideInInspector]
    public UnitType unitType;

    [HideInInspector]
    public GameObject tower;

    [HideInInspector]
    public int towerType;

    public static UnitType onType;

    private Select[] selects;

    //private Skill[] skills;

    public static Unit unitOnSelect;

    // Use this for initialization
    void Start()
    {
        if (unitState != UnitState.NoTower)
        {
            towerType = 0;
            selects = new Select[4];
            Transform sTransform;
            GameManager inst = GameManager.instance;
            selects[0] = Instantiate(inst.selectPrefabs[0]) as Select;
            sTransform = selects[0].GetComponent<Transform>();
            sTransform.parent = transform;
            sTransform.localPosition = new Vector3(0, unitSizeZ / 2, -0.05f);
            selects[1] = Instantiate(inst.selectPrefabs[1]) as Select;
            sTransform = selects[1].GetComponent<Transform>();
            sTransform.parent = transform;
            sTransform.localPosition = new Vector3(unitSizeX / 2 - 0.2f, 0.2f, -0.05f);
            selects[2] = Instantiate(inst.selectPrefabs[2]) as Select;
            sTransform = selects[2].GetComponent<Transform>();
            sTransform.parent = transform;
            sTransform.localPosition = new Vector3(0, -unitSizeZ / 2 + 0.4f, -0.05f);
            selects[3] = Instantiate(inst.selectPrefabs[3]) as Select;
            sTransform = selects[3].GetComponent<Transform>();
            sTransform.parent = transform;
            sTransform.localPosition = new Vector3(-unitSizeX / 2 + 0.2f, 0.2f, -0.05f);
        }
    }


    // Update is called once per frame
    void Update()
    {

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
        onType = UnitType.Obstacle;
    }

    void OnMouseDown()
    {
        if (unitState != UnitState.NoTower)
        {
            if (unitOnSelect != null && unitOnSelect != this)
            {
                unitOnSelect.toggle();
            }
            toggle();
        }
        else
        {
            if (unitOnSelect != null)
            {
                unitOnSelect.toggle();
            }
        }
    }

    public void toggle()
    {
        for (int i = 0; i < 4; i++)
        {
            if (selects[i] != null)
            {
                selects[i].GetComponent<Renderer>().enabled ^= true;
                selects[i].gameObject.layer = (selects[i].gameObject.layer == 0) ? 2 : 0;
            }          
        }
        unitOnSelect = (unitOnSelect == null) ? this : null;
    }

    public void ChangeSelectUI(int type)  //Change all towerType, price, and materials in selects array
    {
        if (type < 4)
        {
            for (int i = 0; i < 4; i++)
            {
                selects[i].towerType = i + 4 + type * 4;
                selects[i].price = GetPrice(selects[i].towerType);
                SetSelectProperty(selects[i], selects[i].towerType);
                selects[i].offMat.mainTexture = GameManager.instance.selectUI[selects[i].towerType];
                selects[i].GetComponent<Renderer>().material = selects[i].offMat;
                selects[i].onMat = GameManager.instance.selectMat[selects[i].towerType];
            }
        }
        else
        {
            if (type % 4 != 0)
            {
                selects[0].towerType = selects[type % 4].towerType;
                selects[0].towerRange = selects[type % 4].towerRange + 1;
                selects[0].towerDamage = selects[type % 4].towerDamage + 1;
                selects[0].towerAttackSpeed = selects[type % 4].towerAttackSpeed + 1;
            }
            
            for (int i = 1; i < 4; i++)
            {
                Destroy(selects[i]);
            }
            if (type % 4 == 0)
            {
                selects[0].towerType = type / 4 + 19;
                SetSelectProperty(selects[0], selects[0].towerType);
            }
            selects[0].price = GetPrice(type) + 80;
            Introduction.isOnSelect = false;
            IntroText.isOnSelect = false;
            Range.isOnTower = false;
        }
    }

    public static int GetPrice(int type)
    {
        switch (type)
        {
            case 0:
                return 50;
                break;
            case 1:
                return 80;
                break;
            case 2:
                return 100;
                break;
            case 3:
                return 60;
                break;
            case 4:
                return 90;
                break;
            case 5:
                return 200;
                break;
            case 6:
                return 140;
                break;
            case 7:
                return 150;
                break;
            case 8:
                return 130;
                break;
            case 9:
                return 170;
                break;
            case 10:
                return 150;
                break;
            case 11:
                return 160;
                break;
            case 12:
                return 180;
                break;
            case 13:
                return 240;
                break;
            case 14:
                return 180;
                break;
            case 15:
                return 240;
                break;
            case 16:
                return 100;
                break;
            case 17:
                return 110;
                break;
            case 18:
                return 200;
                break;
            case 19:
                return 165;
                break;
            case 20:
                return 150;
                break;
            case 21:
                return 190;
                break;
            case 22:
                return 260;
                break;
            case 23:
                return 140;
                break;
            default:
                return -1;
                break;
        }
    }

    public static void SetSelectProperty(Select s, int type)
    {
        switch (type)
        {
            case 0:
                s.towerRange = 5;
                s.towerDamage = 3;
                s.towerAttackSpeed = 6;
                break;
            case 1:
                s.towerRange = 5;
                s.towerDamage = 4;
                s.towerAttackSpeed = 5;
                break;
            case 2:
                s.towerRange = 6;
                s.towerDamage = 7;
                s.towerAttackSpeed = 3;
                break;
            case 3:
                s.towerRange = 6;
                s.towerDamage = 3;
                s.towerAttackSpeed = 5;
                break;
            case 4:
                s.towerRange = 6;
                s.towerDamage = 4;
                s.towerAttackSpeed = 7;
                break;
            case 5:
                s.towerRange = 5;
                s.towerDamage = 7;
                s.towerAttackSpeed = 6;
                break;
            case 6:
                s.towerRange = 5;
                s.towerDamage = 4;
                s.towerAttackSpeed = 5;
                break;
            case 7:
                s.towerRange = 6;
                s.towerDamage = 4;
                s.towerAttackSpeed = 6;
                break;
            case 8:
                s.towerRange = 6;
                s.towerDamage = 5;
                s.towerAttackSpeed = 6;
                break;
            case 9:
                s.towerRange = 5;
                s.towerDamage = 7;
                s.towerAttackSpeed = 6;
                break;
            case 10:
                s.towerRange = 6;
                s.towerDamage = 5;
                s.towerAttackSpeed = 5;
                break;
            case 11:
                s.towerRange = 7;
                s.towerDamage = 6;
                s.towerAttackSpeed = 6;
                break;
            case 12:
                s.towerRange = 7;
                s.towerDamage = 8;
                s.towerAttackSpeed = 4;
                break;
            case 13:
                s.towerRange = 9;
                s.towerDamage = 9;
                s.towerAttackSpeed = 3;
                break;
            case 14:
                s.towerRange = 6;
                s.towerDamage = 6;
                s.towerAttackSpeed = 6;
                break;
            case 15:
                s.towerRange = 6;
                s.towerDamage = 7;
                s.towerAttackSpeed = 7;
                break;
            case 16:
                s.towerRange = 7;
                s.towerDamage = 3;
                s.towerAttackSpeed = 6;
                break;
            case 17:
                s.towerRange = 8;
                s.towerDamage = 2;
                s.towerAttackSpeed = 5;
                break;
            case 18:
                s.towerRange = 7;
                s.towerDamage = 4;
                s.towerAttackSpeed = 4;
                break;
            case 19:
                s.towerRange = 6;
                s.towerDamage = 6;
                s.towerAttackSpeed = 6;
                break;
            case 20:
                s.towerRange = 7;
                s.towerDamage = 5;
                s.towerAttackSpeed = 8;
                break;
            case 21:
                s.towerRange = 7;
                s.towerDamage = 6;
                s.towerAttackSpeed = 7;
                break;
            case 22:
                s.towerRange = 8;
                s.towerDamage = 9;
                s.towerAttackSpeed = 5;
                break;
            case 23:
                s.towerRange = 8;
                s.towerDamage = 4;
                s.towerAttackSpeed = 7;
                break;
        }
    }

    //Unit and Select emission: done
    //When the tower cannot be upgraded anymore, the selectState should be set to UnAvailable.
    //The materials of select should be store in GameManage corresponding to towerPrefabs. 
    //Tower Changing and Upgrade.
}
