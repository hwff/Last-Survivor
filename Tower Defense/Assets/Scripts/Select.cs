using UnityEngine;
using System.Collections;

public class Select : MonoBehaviour
{

    public int towerType;
    public int price;
    public int towerDamage;
    public int towerRange;
    public int towerAttackSpeed;

    private SelectState selectState;
    private Unit unit;
    private Transform parent;

    public Material onMat;
    [HideInInspector]
    public Material offMat;

    void Start()
    {
        parent = GetComponent<Transform>().parent;
        unit = parent.GetComponent<Unit>();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Renderer>().material.renderQueue = 3005;
        gameObject.layer = 2;
        offMat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (Gold.gold < price)
        {
            if (selectState == SelectState.Available)
            {
                selectState = SelectState.UnAvailable;
                GetComponent<Renderer>().material.SetColor("_Color",Color.grey);
            }
        }
        else
        {
            if (selectState == SelectState.UnAvailable)
            {
                selectState = SelectState.Available;
                GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
        }
    }

    void OnMouseEnter()
    {
        if (selectState == SelectState.Available)
        {
            GetComponent<Renderer>().material = onMat;
        }

        Introduction.isOnSelect = true;
        Transform st = GetComponent<Transform>();
        Vector3 sPos = st.localPosition;
        sPos.x *= Unit.unitSizeX;
        float temp = -sPos.z;
        sPos.z = sPos.y * Unit.unitSizeZ;
        sPos.y = temp;
        Introduction.localpos = st.parent.localPosition + sPos + new Vector3(4.2f, 0.0f, 3.2f);
        IntroText.isOnSelect = true;
        IntroText.localpos = st.parent.localPosition + sPos + new Vector3(5.9f, 0.0f, 3.3f);
        AssignText();

        Range.localpos = unit.localpos;
        Range.radius = towerRange * 1.1f;
        Range.isOnTower = true;
    }

    private void AssignText()
    {
        IntroText.towerType = towerType;
        IntroText.towerPrice = price;
        IntroText.towerDamage = towerDamage;
        IntroText.towerRange = towerRange;
        IntroText.towerAttackSpeed = towerAttackSpeed;
    }

    void OnMouseExit()
    {
        if (selectState == SelectState.Available)
        {
            GetComponent<Renderer>().material = offMat;
        }
        Introduction.isOnSelect = false;
        IntroText.isOnSelect = false;
        Range.isOnTower = false;
    }

    void OnMouseDown()
    {
        if (selectState == SelectState.Available)
        {
            int id = 0;
            if (unit.unitState == UnitState.Used)
            {
                id = unit.tower.GetComponent<Tower>().id;
                Destroy(unit.tower);
            }

            unit.tower = Instantiate(GameManager.instance.towerPrefabs[towerType]) as GameObject;
            Transform tTransform = unit.tower.GetComponent<Transform>();
            tTransform.parent = parent;
            tTransform.localScale = new Vector3(1.1f, 1.38f, 1.0f);   //should be tuned
            tTransform.localPosition = new Vector3(0.0f, 0.3f, 0.0f);
            unit.tower.GetComponent<Tower>().type = towerType;

            if (unit.unitState == UnitState.Empty)
            {
                GameManager.instance.towers.Add(unit.tower.GetComponent<Tower>());
                unit.tower.GetComponent<Tower>().id = GameManager.instance.towers.Count;
            }
            else
            {
                Tower t = unit.tower.GetComponent<Tower>();
                t.id = id;
                GameManager.instance.towers[id - 1] = t;
            }

            if (unit.towerType == towerType && unit.unitState == UnitState.Used)
            {
                Tower t = unit.tower.GetComponent<Tower>();
                t.powerUp = true;
                Gold.gold -= price;
                unit.toggle();
                Destroy(this.gameObject);
                Introduction.isOnSelect = false;
                IntroText.isOnSelect = false;
                Range.isOnTower = false;
                return;
            }
            else if (towerType > 19)
            {
                unit.towerType = towerType;
                Gold.gold -= price;
                unit.toggle();
                Destroy(this.gameObject);
                Introduction.isOnSelect = false;
                IntroText.isOnSelect = false;
                Range.isOnTower = false;
                return;
            }
            else
            {
                unit.towerType = towerType;
            }

            unit.unitState = UnitState.Used;
            Gold.gold -= price;
            unit.ChangeSelectUI(towerType);
            unit.toggle();
            //Unit.unitOnSelect = null;
        }
    }

}
