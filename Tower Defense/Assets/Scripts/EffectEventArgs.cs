using UnityEngine;
using System.Collections;

public class EffectEventArgs {

    public Enemy target;

    public int towerType;

    public float damage;

    public EffectEventArgs(Enemy e, int t, float dm)
    {
        target = e;
        towerType = t;
        damage = dm;
    }
}
