using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{

    [HideInInspector]
    public PathNode curNode;

    //Common Enemy Attributes
    [HideInInspector]
    public int type;
    [HideInInspector]
    public int id;

    public float HP_type;
    public float Speed_type;

    private float maxHP;
    [HideInInspector]
    public float HP;
    [HideInInspector]
    public float speed;

    [HideInInspector]
    public bool[] targeted;         

    public GameObject HPBarPrefab;
    private GameObject HPBar;
    private Slider HPBarSlider;  //a component attaching to the HPBarPrefab 

    // Use this for initialization
    void Start()
    {
        float temp;
        Vector3 ePos = this.GetComponent<Transform>().localPosition;    //z should have been adjusted
        temp = ePos.y;
        ePos.y = ePos.z + 0.8f;
        ePos.z = -temp;
        Debug.Log(ePos);
        HPBar = Instantiate(HPBarPrefab) as GameObject;
        Transform UITransform = GameObject.Find("UIManager").transform;
        Transform HPBTransform = HPBar.GetComponent<Transform>();
        HPBTransform.parent = UITransform;
        HPBTransform.localScale = new Vector3(0.02f, 0.04f, 1.0f);
        HPBTransform.localPosition = ePos;
        //Debug.Log("HPB " + HPBTransform.position);
        HPBarSlider = HPBTransform.Find("HPBarSlider").GetComponent<Slider>();
        curNode = Map.startNode;

        this.GetComponent<Renderer>().material.renderQueue = 3000;
        targeted = new bool[Map.towerNum + 1];
    }

    public void DataInitialization(int level, int t, int i)
    {
        maxHP = level * HP_type * 10;
        HP = maxHP;
        speed = level * Speed_type * 0.5f;
        type = t;
        id = i;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateHPBar();
        DetectTower();
        DetectHero();
        //Debug.Log("Enemy " + id + " HP " + HP + " HPBarValue " + HPBarSlider.value);
    }

    private void Move()
    {
        Vector3 curPos = this.GetComponent<Transform>().localPosition;
        Vector3 nextPos = curNode.next.position;

        float dist = Vector3.Distance(curPos, nextPos);
        if (dist < 0.1f)
        {
            if (curNode.next.next == null)
            {
                GameManager.instance.enemies.Remove(this);
                Destroy(this.gameObject);    //this can be changed to an reusable source like the runner tutorial
                Destroy(HPBar.gameObject);   //also resuable 
                GameManager.instance.enemies.Remove(this.GetComponent<Enemy>());
            }
            else
            {
                curNode = curNode.next;
            }
        }

        Vector3 dir = (nextPos - curPos).normalized;
        Transform eTransform = this.GetComponent<Transform>();
        eTransform.Translate(dir * speed * Time.deltaTime, eTransform.parent);
    }

    private void UpdateHPBar()
    {
        HPBarSlider.value = HP / maxHP;
        float temp;
        Vector3 ePos = this.GetComponent<Transform>().localPosition;    //z should have been adjusted
        temp = ePos.y;
        ePos.y = ePos.z + 0.8f;
        ePos.z = -temp;
        HPBar.GetComponent<Transform>().localPosition = ePos;
    }

    public void SetDamage(float dmValue)
    {
        //Debug.Log(HP);
        HP -= dmValue;
        if (HP <= 0)
        {
            Destroy(this.gameObject);
            Destroy(HPBar.gameObject);
            GameManager.instance.enemies.Remove(this.GetComponent<Enemy>());
        }
    }

    public void DetectTower()
    {
        int i = 1;
        foreach (Tower tower in GameManager.instance.towers)
        {
            Vector3 CurPos = this.GetComponent<Transform>().position;
            Vector3 tPos = tower.GetComponent<Transform>().parent.position;
            tPos.y = 0.1f;      
            if (Vector3.Distance(CurPos, tPos) <= tower.range)
            {
                //Debug.Log("Enemy " + CurPos + " tower " + tPos);
                tower.InRange(this);
            }
            else
            {
                if (targeted[i] && !tower.bulletFlying)
                {
                    tower.targeting = false;
                    targeted[i] = false;
                }
            }
            i++;
        }
    }

    public void DetectHero()
    {
        Hero hero = GameManager.instance.hero.GetComponent<Hero>();
        Vector3 CurPos = this.GetComponent<Transform>().position;
        Vector3 hPos = hero.GetComponent<Transform>().position;

        if (Vector3.Distance(CurPos, hPos) <= hero.range)
        {
            hero.InRange(this);
        }
        else
        {
            if (targeted[hero.id] && !hero.bulletFlying)
            {
                hero.targeting = false;
                targeted[hero.id] = false;
            }
        }
    }

}
