using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: IHeapitem<Node>
{
    public bool walkable;
    public int gridX;
    public int gridY;
    public int gCost, hCost;
    public Vector3 worldPosition;
    public Node parent;

    int heapIndex;

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

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
	}

	public int CompareTo(Node node) {
		int compare = fcost.CompareTo(node.fcost);
        // if they are equal we compare hcost -> better hcost is the one we use
		if (compare == 0) {
			compare = hCost.CompareTo(node.hCost);
		}
		return -compare;
	}
}
