using UnityEngine;
using System;
using System.Collections;

public class Effect : MonoBehaviour
{

    [HideInInspector]
    public float originalSpeed;

    [HideInInspector]
    public Enemy target;
    [HideInInspector]
    public float duration;
    [HideInInspector]
    public float slowedRatio;             //frozen, poisoned
    [HideInInspector]
    public float lostHPRatio;             //poisoned, burnt

    [HideInInspector]
    public Vector3 toPos;                 //teleport
    [HideInInspector]
    public Vector3 translation;           //repel

    [HideInInspector]
    public float critiRatio;              //critical strike
    [HideInInspector]
    public float critiProb;
    [HideInInspector]
    public float damage;

    [HideInInspector]
    public bool onehitKill;

    [HideInInspector]
    public int effectID;
    [HideInInspector]
    public GameObject effectPic;

    private bool slowed;

    private event Action<EffectEventArgs> EffectActivated;

    // Use this for initialization
    void Start()
    {
        EffectActivated += OnEffectActivated;
        duration = -20.0f;
        slowedRatio = 1;
        toPos.y = -1;
        effectID = -1;
        effectPic = null;
        critiRatio = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (duration > 0)
            {
                target.SetDamage(lostHPRatio * damage);
                duration -= Time.deltaTime;
                if (effectPic != null)
                {
                    effectPic.GetComponent<Transform>().localPosition = target.GetComponent<Transform>().localPosition + new Vector3(0, 0.5f, 1.5f);
                }
            }
            else
            {
                if (duration != -20.0f)
                {
                    target.speed = originalSpeed;
                    if (onehitKill)
                    {
                        target.SetDamage(10000);
                    }
                    target = null;
                    if (effectPic != null)
                    {
                        effectPic.GetComponent<Renderer>().enabled = false;
                    }
                    duration = -20.0f;
                    slowed = false;
                }
            }
            Debug.Log("1 " + (effectPic == null));
        }
        else
        {
            if (effectPic != null)
            {
                effectPic.GetComponent<Renderer>().enabled = false;         //紧跟着的怪可能会没有图片出现，而且立即切换target时会重置duration，从而使得前面的怪的speed没有设回原值
                slowed = false;
            }                                                               
        }
    }

    public void ActivateEffect(EffectEventArgs e)
    {
        Action<EffectEventArgs> local = EffectActivated;
        if (local != null)
        {
            local(e);
        }
    }

    private void OnEffectActivated(EffectEventArgs e)
    {
        target = e.target;
        SetEffectParameters(e.towerType);
        if (target != null)
        {
            if (effectID >= 0 && effectID != 4)
            {
                if (effectPic == null)
                {
                    effectPic = Instantiate(GameManager.instance.effectPrefabs[effectID]) as GameObject;
                    effectPic.GetComponent<Transform>().parent = target.GetComponent<Transform>().parent;
                }

                Transform eT = effectPic.GetComponent<Transform>();
                eT.localPosition = target.GetComponent<Transform>().localPosition + new Vector3(0, 0.5f, 1.5f);
                effectPic.GetComponent<Renderer>().material.renderQueue = 3010;
                effectPic.GetComponent<Renderer>().enabled = true;
            }

            if (!onehitKill)
            {
                if (!slowed)
                {
                    originalSpeed = target.speed;
                    target.speed *= slowedRatio;
                    slowed = true;
                }          

                damage = e.damage;

                Transform tT = target.GetComponent<Transform>();
                if (toPos.y != -1)
                {
                    tT.localPosition = toPos;
                }
                else
                {
                    tT.localPosition = tT.localPosition + translation;
                }

                if (UnityEngine.Random.value < critiProb)
                {
                    target.SetDamage(e.damage * critiRatio);

                    if (effectPic == null)
                    {
                        effectPic = Instantiate(GameManager.instance.effectPrefabs[effectID]) as GameObject;
                        effectPic.GetComponent<Transform>().parent = target.GetComponent<Transform>().parent;
                    }

                    Transform eT = effectPic.GetComponent<Transform>();
                    eT.localPosition = target.GetComponent<Transform>().localPosition + new Vector3(0, 0.5f, 1.5f);
                    effectPic.GetComponent<Renderer>().material.renderQueue = 3010;
                    effectPic.GetComponent<Renderer>().enabled = true;
                }
                else if (critiRatio > 0)
                {
                    target.SetDamage(e.damage);
                    if (effectPic != null)
                    {
                        effectPic.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            else
            {
                
            }
        }
    }

    private void SetEffectParameters(int type)
    {
        switch (type)
        {
            case 5:
                critiProb = 0.4f;
                critiRatio = 2.5f;
                effectID = 4;
                duration = 0.15f;
                break;
            case 9:
                duration = 0.12f;
                lostHPRatio = 0.1f;
                effectID = 1;
                break;
            case 10:
                slowedRatio = 0;
                duration = 1.5f;
                effectID = 0;
                break;
            case 11:
                duration = 1.0f;
                slowedRatio = 0.3f;
                effectID = 3;
                break;
            case 18:
                duration = 0.4f;
                effectID = 5;
                onehitKill = true;
                break;
            case 19:
                duration = 0.8f;
                slowedRatio = 0.6f;
                lostHPRatio = 0.0001f;
                effectID = 2;
                break;
            default:
                break;
        }
    }

}
