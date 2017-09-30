using UnityEngine;
using System.Collections;

public class PathNode{

    public Vector3 position;
    public PathNode next;

    public PathNode()
    {
        position = new Vector3(0.0f, 0.0f, 0.0f);
        next = null;
    }

    public PathNode(Vector3 pos, PathNode n)
    {
        position = pos;
        next = n;
    }
}
