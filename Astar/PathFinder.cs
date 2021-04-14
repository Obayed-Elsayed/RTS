using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
   GridMap grid;
   void Awake(){
       grid = GetComponent<GridMap>();
   }
   // hcost = distance from destination
   // gcost = added walking cost from start node
   // fcost = h+g
   public void FindPath(Vector3 startPos, Vector3 destination){
       Node startNode = grid.NodeFromWorldPosition(startPos);
       Node destinationNode = grid.NodeFromWorldPosition(destination);

       List<Node> openSet = new List<Node>();
       HashSet<Node> closedSet = new HashSet<Node>();

       openSet.Add(startNode);
        // Stop looping when there is no more nodes in the open set
        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                // lower F cost-> probability of being close to target or if same fcost -> closer to target
                if(openSet[i].fcost< currentNode.fcost || openSet[i].fcost == currentNode.fcost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode == destinationNode){
                retracePath(startNode, destinationNode);
                return;
            }
            // calculate neighbour costs and add them to open set 
            foreach(Node neighbour in grid.GetNeightbours(currentNode)) {
                if(!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost+ getDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, destinationNode);
                    neighbour.parent = currentNode;
                    if(!openSet.Contains(neighbour)){
                        openSet.Add(neighbour);
                    }
                }
            }
        }

   }
    void retracePath(Node start, Node finish){
        List<Node> path = new List<Node>();
        Node currentNode = finish;

        while(currentNode != start){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }
    int getDistance(Node n1, Node n2) {
        int distX = Mathf.Abs(n1.gridX - n2.gridX);
        int distY = Mathf.Abs(n1.gridY - n2.gridY);
        if (distX > distY) {
            return 14*distY + 10*(distX-distY);
        }
        else if(distX < distY) {
            return 14*distX + 10*(distY-distX);
        }
        return 14*distX;
    }


}
