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

    private NavMeshAgent heroAgent;
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

        heroAgent = GetComponent<NavMeshAgent>();
        heroAgent.speed = speed;
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
        }
    }

    public void InRange(Enemy e)                    // activate
    {
        Action<Enemy> local = OnInAttackRange;
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
                heroAgent.destination = hit.point;
            }
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
                bTransform.parent = GetComponent<Transform>().parent;
                bTransform.localPosition = GetComponent<Transform>().localPosition;  //this number should be replace with variables, 1.8*1.5/2
                bTransform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
                bullet.GetComponent<Renderer>().material.renderQueue = 3002;

                bulletFlying = true;
            }
            else if (bullet != null)
            {
                if (target != null)
                {
                    Vector3 eCurPos = target.GetComponent<Transform>().position;   //parent: map
                    Vector3 tPos = bullet.GetComponent<Transform>().position;        //parent: hero

                    if (Vector3.Distance(eCurPos, tPos) > 0.2f)
                    {
                        Vector3 dir = eCurPos - tPos;
                        dir.y = dir.z;
                        dir.z = 0;
                        dir = dir.normalized;
                        bullet.GetComponent<Transform>().Translate(dir * (bulletSpeed + target.speed) * Time.deltaTime);
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
                Destroy(bullet);
            }
        }
    }
}
