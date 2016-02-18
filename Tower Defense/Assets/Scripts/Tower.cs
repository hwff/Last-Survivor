using UnityEngine;
using System;
using System.Collections;

public class Tower : MonoBehaviour
{
    [HideInInspector]
    public int type;
    [HideInInspector]
    public int id;

    //Common Tower Attributes
    public float attackTime_type;
    public float attackRange_type;
    public float attackDamage_type;
    public float bulletSpeed_type;

    private int level;
    private float damage;
    private float time;
    [HideInInspector]
    public float range;
    private float bulletSpeed;

    private event Action<Enemy> InAttackRange;

    [HideInInspector]
    public bool targeting;
    private Enemy target;
    [HideInInspector]
    public bool bulletFlying;

    public GameObject bulletPrefab;

    private GameObject bullet;

    private float wait;  //may be deleted

    // Use this for initialization
    void Start()
    {
        level = 1;
        damage = level * attackDamage_type * 4;   // this 4 may have to be tuned.
        time = level * (1 / attackTime_type);     // may be tuned
        range = level * attackRange_type * 0.7f;  // may be tuned
        wait = 0.0f;
        bulletSpeed = level * bulletSpeed_type * 0.8f;
        targeting = false;
        bulletFlying = false;

        InAttackRange += OnInAttackRange;

        this.GetComponent<Renderer>().material.renderQueue = 3001;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void InRange(Enemy e)                    // activate
    {
        Action<Enemy> local = OnInAttackRange;
        if (local != null)
        {
            local(e);
        }
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

    private void Attack()
    {
        if (targeting || bulletFlying)
        {
            if (wait<=0 && bullet == null)
            {
                bullet = Instantiate(bulletPrefab) as GameObject;
                Transform bTransform = bullet.GetComponent<Transform>();
                bTransform.parent = transform;
                bTransform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);  //this number should be replace with variables, 1.8*1.5/2
                bTransform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
                bullet.GetComponent<Renderer>().material.renderQueue = 3002;

                bulletFlying = true;
            }
            else if(bullet!=null)
            {
                if (target!=null)
                {
                    Vector3 eCurPos = target.GetComponent<Transform>().position;   //parent: map
                    Vector3 tPos = bullet.GetComponent<Transform>().position;        //parent: unit
                  
                    if (Vector3.Distance(eCurPos, tPos) > 0.2f)
                    {
                        Vector3 dir = eCurPos - tPos;
                        dir.y = dir.z;
                        dir.z = 0;
                        dir = dir.normalized;                       
                        bullet.GetComponent<Transform>().Translate(dir * (bulletSpeed + target.speed) * Time.deltaTime, transform);
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
            if (target.HP <= 0 || target == null)
            {
                targeting = false;
                target.targeted[id] = false;
                Destroy(bullet);
            }
        }
    }
}
