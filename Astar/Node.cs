using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public int gridX;
    public int gridY;
    public int gCost, hCost;
    public Vector3 worldPosition;
    public Node parent;


    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY) {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fcost{
        get{
            return gCost + hCost;
        }
    }
}
