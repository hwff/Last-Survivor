using UnityEngine;
using System.Collections;

public enum MouseStateType
{
    OnMap,
    OutMap
} 

public class Mouse
{
    public static Vector3 mouse;
    public static Vector3 click;

    public static MouseStateType mouseState;

    public static bool isClicked;

    public static void MouseToBoard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            mouse.x = hit.point.x;
            mouse.y = hit.point.y;
            mouse.z = hit.point.z;
            mouseState = MouseStateType.OnMap;
        }
        else
        {
            mouseState = MouseStateType.OutMap;
        }
    }

    public static void SetClick()
    {
        click = mouse;
        isClicked = true;
    }
    public Mouse() { mouseState = MouseStateType.OutMap; }
}
