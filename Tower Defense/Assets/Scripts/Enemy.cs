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

    private bool fliped;
    // Use this for initialization
    void Start()
    {
        float temp;
        Vector3 ePos = this.GetComponent<Transform>().localPosition;    //z should have been adjusted
        temp = ePos.y;
        ePos.y = ePos.z + 3.2f;
        ePos.z = -temp;
        HPBar = Instantiate(HPBarPrefab) as GameObject;
        Transform UITransform = GameObject.Find("UIManager").transform;
        Transform HPBTransform = HPBar.GetComponent<Transform>();
        HPBTransform.parent = UITransform;
        HPBTransform.localScale = new Vector3(0.02f, 0.04f, 1.0f);
        HPBTransform.localPosition = ePos;
        HPBarSlider = HPBTransform.Find("HPBarSlider").GetComponent<Slider>();
        curNode = Map.startNode;
        //Debug.Log("start "+GetComponent<Transform>().localScale);

        //this.GetComponent<Renderer>().material.renderQueue = 3000;
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
        if (dist < 0.3f)
        {
            if (curNode.next.next == null)
            {
                GameManager.instance.enemies.Remove(this);
                Destroy(this.gameObject);    //this can be changed to an reusable source like the runner tutorial
                Destroy(HPBar.gameObject);   //also resuable 
                GameManager.instance.enemies.Remove(this.GetComponent<Enemy>());

                Health.hp -= 1;
            }
            else
            {
                curNode = curNode.next;
                Vector3 scale = GetComponent<Transform>().localScale;
                Vector3 d = curNode.next.position - GetComponent<Transform>().localPosition;
                if (d.x < 0 && scale.x < 0)
                {
                    GetComponent<Transform>().Rotate(0, 180, 0);
                }
                else if (d.x > 0 && scale.x > 0)
                {
                    GetComponent<Transform>().Rotate(0, 180, 0);
                }
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
        ePos.y = ePos.z + 3.2f;
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

            Gold.gold += 10;
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
                if (!targeted[i])
                {
                    tower.InRange(this);
                }              
            }
            else
            {
                if (tower.type != 11)
                {
                    int bulletidx = 0;
                    for(int j = 0; j < tower.bulletNum; j++)
                    {
                        if (tower.type == 7)
                        {
                            Debug.Log("idx: " + j + " bulletNum " + tower.bulletNum);
                        }
                        if (tower.bulletToEnemy[j] == id)
                        {
                            bulletidx = j;
                            break;
                        }
                    }

                    //Debug.Log(Map.towerNum + " " + i + "BulletNum: " + tower.bulletNum + " idx: " + bulletidx);
                    if (targeted[i] && !tower.bulletFlying[bulletidx])
                    {
                        tower.targeting[bulletidx] = false;
                        targeted[i] = false;
                    }
                }
                else
                {
                    if (targeted[i])
                    {
                        tower.targeting[0] = false;
                        targeted[i] = false;
                        tower.DestroyBullet(0);
                    }
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
                hero.A.SetBool("shoot", false);
            }
        }
    }

}
