using System;
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
   public void FindPath(PathRequest request, Action<PathResult> callback){
       Node startNode = grid.NodeFromWorldPosition(request.start);
       Node destinationNode = grid.NodeFromWorldPosition(request.dest);

       Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
       HashSet<Node> closedSet = new HashSet<Node>();

       bool pathCalculated = false;
       Vector3[] pathdestinations = new Vector3[0];

       openSet.Add(startNode);
        // Stop looping when there is no more nodes in the open set
        while (openSet.Count > 0) {
            // removing first from node means removing lowest fcost element
            Node currentNode = openSet.removeFirst();


            closedSet.Add(currentNode);
            if (currentNode == destinationNode){
               pathCalculated = true;
                break;
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
        if(pathCalculated){
            pathdestinations = retracePath(startNode, destinationNode);
        }
        callback(new PathResult(pathdestinations, pathCalculated, request.callback));
   }
    Vector3[] retracePath(Node start, Node finish){
        List<Node> path = new List<Node>();
        Node currentNode = finish;

        while(currentNode != start){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] destinations = pathDirections(path);
        System.Array.Reverse(destinations);
        return destinations;
    }

    public Vector3[] pathDirections(List<Node> path){
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for(int i =1; i< path.Count; i++){
            Vector2 dirNew = new Vector2(path[i-1].gridX-path[i].gridX, path[i-1].gridY-path[i].gridY); 
            if(directionOld!=dirNew){
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = dirNew;
        }
        return waypoints.ToArray();
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
