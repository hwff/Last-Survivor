using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroText : MonoBehaviour {

    public static int towerType;
    public static float towerDamage;
    public static float towerRange;
    public static float towerAttackSpeed;
    public static int towerPrice;
    private string comment;

    public static bool isOnSelect;
    public static Vector3 localpos;
    private bool isSet;

	// Use this for initialization
	void Start () {
        isOnSelect = false;
        isSet = false;
        GetComponent<CanvasRenderer>().SetAlpha(0);
    }
	
	// Update is called once per frame
	void Update () {
        if (isOnSelect)
        {
            if (!isSet)
            {
                SetComment();
                GetComponent<Text>().text = string.Format("{4}\nPrice: {0}\nDamage: {1}\nAttackSpeed: {2}\nRange: {3}\n☆ {5}",
                                                          towerPrice, towerDamage, towerAttackSpeed, towerRange, TowerType(towerType), comment);
                float temp = localpos.y;
                localpos.y = localpos.z;
                localpos.z = -temp;
                GetComponent<Transform>().localPosition = localpos;
                GetComponent<CanvasRenderer>().SetAlpha(0.8f);
                isSet = true;
            }
        }
        else
        {
            if (isSet)
            {
                GetComponent<CanvasRenderer>().SetAlpha(0);
                isSet = false;
            }           
        }
    }

    private string TowerType(int type)
    {
        switch (type)
        {
            case 0:
                return "Arrow Tower";
                break;
            case 1:
                return "Magic Tower";
                break;
            case 2:
                return "Bomb Tower";
                break;
            case 3:
                return "Fairy Tower";
                break;
            case 4:
                return "Upgrade";
                break;
            case 5:
                return "Heavy Arrow Tower";
                break;
            case 6:
                return "Magic Arrow Tower";
                break;
            case 7:
                return "Multi Arrow Tower";
                break;
            case 8:
                return "Upgrade";
                break;
            case 9:
                return "Fire Magic Tower";
                break;
            case 10:
                return "Ice Magic Tower";
                break;
            case 11:
                return "Thunder Magic Tower";
                break;
            case 12:
                return "Upgrade";
                break;
            case 13:
                return "Missile Tower";
                break;
            case 14:
                return "Gold Tower";
                break;
            case 15:
                return "Laser Tower";
                break;
            case 16:
                return "Upgrade";
                break;
            case 17:
                return "Heal Fairy Tower";
                break;
            case 18:
                return "Time Wizard Tower";
                break;
            case 19:
                return "Plant Wizard Tower";
                break;
            case 20:
                return "Upgrade";
                break;
            case 21:
                return "Upgrade";
                break;
            case 22:
                return "Upgrade";
                break;
            case 23:
                return "Upgrade";
                break;
            default:
                return "";
                break;
        }
    }

    private void SetComment()
    {
        switch (towerType)
        {
            case 0:
                comment = "Basic Arrow Tower";
                break;
            case 1:
                comment = "Basic Magic Tower";
                break;
            case 2:
                comment = "Basic Bomb Tower";
                break;
            case 3:
                comment = "Basic Fairy Tower";
                break;
            case 4:
                comment = "Upgrade to Medium\nArrow Tower";
                break;
            case 5:
                comment = "Deal critical\nstrike sometimes";
                break;
            case 6:
                comment = "";
                break;
            case 7:
                comment = "Shoot up to 2\narrows";
                break;
            case 8:
                comment = "Upgrade to Medium\nMagic Tower";
                break;
            case 9:
                comment = "Burn the enemies\nand deal much damage";
                break;
            case 10:
                comment = "Freeze and root\nthe enemies";
                break;
            case 11:
                comment = "Paralyze and slow\nthe enemies";
                break;
            case 12:
                comment = "Upgrade to Medium\nBomb Tower";
                break;
            case 13:
                comment = "Launch missiles and\ndeal high damage";
                break;
            case 14:
                comment = "Generate extra\ngold per second";
                break;
            case 15:
                comment = "Emit laser and\ndeal high damage";
                break;
            case 16:
                comment = "Upgrade to Medium\nFairy Tower";
                break;
            case 17:
                comment = "";
                break;
            case 18:
                comment = "Generate black hole\nand swallow a enemy";
                break;
            case 19:
                comment = "Poison and slow\nthe enemies";
                break;
            case 20:
                comment = "Upgrade to Advanced\nAttack Tower";
                break;
            case 21:
                comment = "Upgrade to Advanced\nMagic Tower";
                break;
            case 22:
                comment = "Upgrade to Advanced\nBomb Tower";
                break;
            case 23:
                comment = "Upgrade to Advanced\nFairy Tower";
                break;
            default:
                comment = "";
                break;
        }
    }
}
