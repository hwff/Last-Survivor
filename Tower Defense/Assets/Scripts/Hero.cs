using UnityEngine;
using System;
using System.Collections;

public class Hero : MonoBehaviour
{

    //common hero data
    [HideInInspector]
    public int id;
    [HideInInspector]
    public int type;

    public float attackDamage_type;
    public float attackTime_type;
    public float attackRange_type;
    public float Speed_type;
    public float HP_type;
    public float bulletSpeed_type;

    private float damage;
    private float time;
    [HideInInspector]
    public float range;
    private float speed;
    private float bulletSpeed;
    private float HP;
    private int level;

    private float wait;
    private event Action<Enemy> InAttackRange;

    [HideInInspector]
    public bool targeting;
    private Enemy target;
    [HideInInspector]
    public bool bulletFlying;

    public GameObject bulletPrefab;

    private GameObject bullet;

    private Vector3 destination;

    [HideInInspector]
    public Animator A;

    private int prevdir;
    private int preattackdir;

    // Use this for initialization
    void Start()
    {
        level = 2;
        speed = level * Speed_type * 0.6f;
        damage = level * attackDamage_type * 4;   // this 4 may have to be tuned.
        time = level * (1 / attackTime_type);     // may be tuned
        range = level * attackRange_type * 0.45f;  // may be tuned
        wait = 0.0f;
        bulletSpeed = level * bulletSpeed_type * 0.8f;
        targeting = false;
        bulletFlying = false;
        A = GetComponent<Animator>();
        prevdir = -1;
        preattackdir = -1;

        InAttackRange += OnInAttackRange;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }

    private void OnInAttackRange(Enemy e)           // reactive function
    {
        if (!targeting)
        {
            targeting = true;
            //Debug.Log(this.GetComponent<Transform>().position + " tower attack activated!");
            target = e;
            e.targeted[id] = true;
            A.SetBool("shoot", true);
        }
    }

    public void InRange(Enemy e)                    // activate
    {
        Action<Enemy> local = InAttackRange;
        if (local != null)
        {
            local(e);
        }
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(0) && Unit.onType == UnitType.Path)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                destination = hit.point;
            }
        }

        Vector3 curPos, dir;
        curPos = GetComponent<Transform>().position;
        dir = (destination - curPos).normalized;
        dir.y = dir.z;
        dir.z = 0;

        if (Vector3.Distance(destination, curPos) < 0.55f)
        {
            A.SetBool("walk", false);
        }
        else
        {
            A.SetBool("walk", true);
        }

        if (dir.y > 0 && dir.x > 0)
        {
            if (dir.y > dir.x)
            {
                setFalse(prevdir);
                A.SetBool("back", true);
                prevdir = 0;
            }
            else
            {
                setFalse(prevdir);
                A.SetBool("right", true);
                prevdir = 1;              
            }
        }
        else if (dir.y > 0)
        {
            if (dir.y > -dir.x)
            {
                setFalse(prevdir);
                A.SetBool("back", true);
                prevdir = 0;               
            }
            else
            {
                setFalse(prevdir);
                A.SetBool("left", true);
                prevdir = 3;               
            }
        }
        else if (dir.x > 0)
        {
            if (-dir.y > dir.x)
            {
                setFalse(prevdir);
                A.SetBool("front", true);
                prevdir = 2;
            }
            else
            {
                setFalse(prevdir);
                A.SetBool("right", true);
                prevdir = 1;
            }
        }
        else if (dir.x < 0 && dir.y < 0)
        {
            if (dir.y < dir.x)
            {
                setFalse(prevdir);
                A.SetBool("front", true);
                prevdir = 2;
            }
            else
            {
                setFalse(prevdir);
                A.SetBool("left", true);
                prevdir = 3;
            }
        }

        GetComponent<Transform>().Translate(dir * speed * Time.deltaTime);
    }

    private void setFalse(int dir)
    {
        switch (dir)
        {
            case 0:
                A.SetBool("back", false);
                break;
            case 1:
                A.SetBool("right", false);
                break;
            case 2:
                A.SetBool("front", false);
                break;
            case 3:
                A.SetBool("left", false);
                break;
            default:
                break;
        }
    }

    private void Attack()
    {
        if (targeting || bulletFlying)
        {
            if (wait <= 0 && bullet == null)
            {
                bullet = Instantiate(bulletPrefab) as GameObject;
                Transform bTransform = bullet.GetComponent<Transform>();
                bTransform.parent = transform;
                bTransform.localPosition = new Vector3(0, 1.5f, 0);  //this number should be replace with variables, 1.8*1.5/2
                bTransform.localScale = new Vector3(1.2f, 1.5f, 1.0f);
                bullet.GetComponent<Renderer>().material.renderQueue = 3002;

                bulletFlying = true;
            }
            else if (bullet != null)
            {
                if (target != null)
                {
                    Vector3 eCurPos = target.GetComponent<Transform>().position;   //parent: map
                    Vector3 tPos = bullet.GetComponent<Transform>().position;        //parent: hero
                    Vector3 hPos = GetComponent<Transform>().position;

                    SetAttackDir(eCurPos - hPos);

                    if (Vector3.Distance(eCurPos, tPos) > 0.2f)
                    {
                        Vector3 dir = eCurPos - tPos;
                        dir.y = dir.z;
                        dir.z = 0;
                        dir = dir.normalized;
                        
                        Transform bTransform = bullet.GetComponent<Transform>();
                        bTransform.localRotation = Quaternion.FromToRotation(Vector3.right, dir);
                        bTransform.Translate(dir * (bulletSpeed + target.speed) * Time.deltaTime, transform);
                    }
                    else
                    {
                        Destroy(bullet);
                        wait = time;
                        target.SetDamage(damage);

                        bulletFlying = false;
                    }               
                }
            }
            else
            {
                wait -= Time.deltaTime;
            }
            if (target == null)
            {
                targeting = false;
                target.targeted[id] = false;
                A.SetBool("shoot", false);
                Destroy(bullet);
            }
        }
    }

    private void SetAttackDir(Vector3 dir)
    {
        if (dir.z > 0 && dir.x > 0)
        {
            if (dir.z > dir.x)
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackback", true);
                preattackdir = 0;
            }
            else
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackright", true);
                preattackdir = 1;
            }
        }
        else if (dir.z > 0)
        {
            if (dir.z > -dir.x)
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackback", true);
                preattackdir = 0;
            }
            else
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackleft", true);
                preattackdir = 3;
            }
        }
        else if (dir.x > 0)
        {
            if (-dir.z > dir.x)
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackfront", true);
                preattackdir = 2;
            }
            else
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackright", true);
                preattackdir = 1;
            }
        }
        else if (dir.x < 0 && dir.z < 0)
        {
            if (dir.z < dir.x)
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackfront", true);
                preattackdir = 2;
            }
            else
            {
                setFalse_Attack(preattackdir);
                A.SetBool("attackleft", true);
                preattackdir = 3;
            }
        }
    }

    private void setFalse_Attack(int dir)
    {
        switch (dir)
        {
            case 0:
                A.SetBool("attackback", false);
                break;
            case 1:
                A.SetBool("attackright", false);
                break;
            case 2:
                A.SetBool("attackfront", false);
                break;
            case 3:
                A.SetBool("attackleft", false);
                break;
            default:
                break;
        }
    }
}
