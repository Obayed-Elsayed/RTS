using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public Vector2 gridWorldSize;
    // space per node -> smaller size is more expensive
    public float nodeRadius;
    [SerializeField]public LayerMask unwalkable;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    void Start() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //unwalkable = LayerMask.NameToLayer("Buildings and Resources");
        createGrid();
    }

    void Update() {
        if(Input.GetKey(KeyCode.Keypad3)){
                createGrid();
        }
    }

    void createGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        // Vector3.forward -> (0,0,1)
        // Vector3.right -> (1,0,0)
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 castPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable =!(Physics.CheckSphere(castPoint, nodeRadius, unwalkable));
                grid[x,y] = new Node(walkable, castPoint, x, y);
            }
        }
    }

    public List<Node> path;
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid!=null){
            foreach(Node node in grid){
                Gizmos.color = (node.walkable)?Color.white:Color.red;
                if(path!=null){
                    if(path.Contains(node)){
                        Gizmos.color= Color.black;
                    }
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter-0.1f));
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition){
        // percetange of the object on the board
        float percentX = (worldPosition.x + gridWorldSize.x/2)/gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y/2)/gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // percentage to actual position
        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        return grid[x,y];

    }

    public List<Node> GetNeightbours(Node n){
        List<Node> neightbours = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0){
                    continue;
                }
                int checkX = n.gridX + x;
                int checkY = n.gridY + y;
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY )  {
                    neightbours.Add(grid[checkX,checkY]);
                }
            }
        }
        return neightbours;
    }

}
