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
    public bool[] targeting;
    private Enemy[] targets;
    //private Enemy target;
    [HideInInspector]
    public bool[] bulletFlying;

    public GameObject bulletPrefab;

    [HideInInspector]
    public int bulletNum;

    private GameObject[] bullets;
    //private GameObject bullet;
    [HideInInspector]
    public int[] bulletToEnemy; 

    private float[] wait;  //may be deleted

    private Effect eff;

    private float originalSpeed;

    [HideInInspector]
    public bool powerUp;

    private float waitforgold;

    // Use this for initialization
    void Start()
    {
        
        if (powerUp)
        {
            attackTime_type += 1;
            attackDamage_type += 1;
            attackRange_type += 1;
            bulletSpeed_type += 1;
        }

        level = 1;
        damage = level * attackDamage_type * 4;   // this 4 may have to be tuned.
        time = level * (1 / attackTime_type) * 1.3f;     // may be tuned
        range = level * attackRange_type * 1.1f;  // may be tuned
        bulletSpeed = level * bulletSpeed_type * 0.8f;

        InAttackRange += OnInAttackRange;

        this.GetComponent<Renderer>().material.renderQueue = 3001;

        eff = GetComponent<Effect>();

        bullets = new GameObject[3];
        targets = new Enemy[3];
        targeting = new bool[3];
        bulletFlying = new bool[3];
        wait = new float[3];
        bulletToEnemy = new int[3];

        if (type != 7)
        {
            bulletNum = 1;
        }
        else
        {
            if (!powerUp)
            {
                bulletNum = 2;
            }
            else
            {
                bulletNum = 3;
            }
            
        }

        if (powerUp && type == 11)
        {
            bulletNum = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < bulletNum; i++)
        {
            Attack(i);
        }

        GenerateGold();
    }

    public void InRange(Enemy e)                    // activate
    {
        Action<Enemy> local = InAttackRange;
        if (local != null)
        {
            local(e);
        }
    }

    private void OnInAttackRange(Enemy e)           // reactive function
    {
        for(int i = 0; i < bulletNum; i++)
        {
            if (!targeting[i])
            {
                targeting[i] = true;
                //Debug.Log(this.GetComponent<Transform>().position + " tower attack activated!");
                targets[i] = e;
                e.targeted[id] = true;
                originalSpeed = e.speed;
                bulletToEnemy[i] = e.id;
                break;
            }
        }
    }

    private void Attack(int idx)
    {
        if (targeting[idx] || bulletFlying[idx])
        {
            if (wait[idx] <= 0 && bullets[idx] == null)
            {
                bullets[idx] = Instantiate(bulletPrefab) as GameObject;
                Transform bTransform = bullets[idx].GetComponent<Transform>();
                bTransform.parent = transform;
                bTransform.localPosition = new Vector3(0.0f, 0.3f, 0.0f);  //this number should be replace with variables, 1.8*1.5/2
                if (type != 11 && type != 15)
                {
                    bTransform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
                    bulletFlying[idx] = true;
                }
                else if(type == 11)
                {
                    bTransform.localScale = new Vector3(1.0f / 2.75f, 0.0f, 1.0f);
                }
                else
                {
                    bTransform.localScale = new Vector3(1.0f / 1.2f, 0.0f, 1.0f);
                }
                bullets[idx].GetComponent<Renderer>().material.renderQueue = 3002;

            }
            else if(bullets[idx]!=null)
            {
                if (targets[idx]!=null)
                {
                    Vector3 eCurPos = targets[idx].GetComponent<Transform>().position;   //parent: map
                    Vector3 tPos = bullets[idx].GetComponent<Transform>().position;        //parent: unit

                    if (type != 11 && type != 15)
                    {

                        if (Vector3.Distance(eCurPos, tPos) > 0.2f)
                        {
                            Vector3 dir = eCurPos - tPos;
                            dir.y = dir.z;
                            dir.z = 0;
                            dir = dir.normalized;
                            Transform bTransform = bullets[idx].GetComponent<Transform>();
                            bTransform.localRotation = Quaternion.FromToRotation(Vector3.right, dir);
                            bTransform.Translate(dir * (bulletSpeed + originalSpeed) * Time.deltaTime, transform);
                        }
                        else
                        {
                            Destroy(bullets[idx]);
                            wait[idx] = time;
                            if (type != 5)
                            {
                                targets[idx].SetDamage(damage);
                                eff.ActivateEffect(new EffectEventArgs(targets[idx], type, damage));
                            }
                            else
                            {
                                eff.ActivateEffect(new EffectEventArgs(targets[idx], type, damage));
                            }

                            bulletFlying[idx] = false;
                        }
                    }
                    else if (type == 11)
                    {
                        Transform bTransform = bullets[idx].GetComponent<Transform>();
                        Vector3 ls = bTransform.lossyScale;
                        Vector3 dir = eCurPos - tPos;
                        dir.y = dir.z;
                        dir.z = 0;
                        if (dir.magnitude - ls.y > 0.2f)
                        {
                            dir = dir.normalized;

                            Vector3 locals = bTransform.localScale;
                            locals.y += bulletSpeed / 30;
                            bTransform.localScale = locals;
                            bTransform.localRotation = Quaternion.FromToRotation(Vector3.down, dir);
                        }
                        else
                        {
                            bTransform.localScale = new Vector3(1.0f / 1.0f, dir.magnitude / 3.4f, 1.0f);
                            bTransform.localRotation = Quaternion.FromToRotation(Vector3.down, dir);
                            targets[idx].SetDamage(damage / 30);
                            eff.ActivateEffect(new EffectEventArgs(targets[idx], type, damage));
                        }
                    }
                    else
                    {
                        Transform bTransform = bullets[idx].GetComponent<Transform>();
                        Vector3 ls = bTransform.lossyScale;
                        Vector3 dir = eCurPos - tPos;
                        dir.y = dir.z;
                        dir.z = 0;
                        if (dir.magnitude - ls.y > 0.2f)
                        {
                            dir = dir.normalized;

                            Vector3 locals = bTransform.localScale;
                            locals.y += bulletSpeed / 20;
                            bTransform.localScale = locals;
                            bTransform.localRotation = Quaternion.FromToRotation(Vector3.down, dir);
                        }
                        else
                        {
                            Destroy(bullets[idx]);
                            wait[idx] = time;
                            targets[idx].SetDamage(damage);
                            Debug.Log(attackDamage_type);
                            bulletFlying[idx] = false;
                        }
                    }
                }
            }
            else
            {
                wait[idx] -= Time.deltaTime;
            }
            if (targets[idx].HP <= 0 || targets[idx] == null)
            {
                targeting[idx] = false;
                targets[idx].targeted[id] = false;
                Destroy(bullets[idx]);
            }
        }
    }

    public void DestroyBullet(int idx)
    {
        if (bullets[idx] != null)
        {
            Destroy(bullets[idx]);
        }
    }

    private void GenerateGold()
    {
        if (type == 14)
        {
            if (waitforgold <= 0)
            {
                if (powerUp)
                {
                    Gold.gold += 5;
                    waitforgold = attackTime_type / 1.5f;
                }
                else
                {
                    Gold.gold += 3;
                    waitforgold = attackTime_type;
                }
            }
            else
            {
                waitforgold -= Time.deltaTime;
            }
        }
    }
}
