using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement: MonoBehaviour 
{
    
    Vector3 startPos;
    Vector3 endPos;

    public Camera cam;


    public void Start(){
 
    }

    RaycastHit hitinfoStart;
    RaycastHit hitinfoEnd;
    public int max_formation_length = 3;
    public void Update(){
        
        // record initial right click position
        if(Input.GetMouseButtonDown(1)){
            startPos = Input.mousePosition;
            Physics.Raycast(cam.ScreenPointToRay(startPos),out hitinfoStart,Mathf.Infinity,(1<<8));
        }
        //record final right click position and calculate unit formation
        else if(Input.GetMouseButtonUp(1)){
            endPos = Input.mousePosition;
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hitinfoEnd,Mathf.Infinity,(1<<8))){
                // use default move

                if(Vector3.Distance(startPos, endPos) <5){
                    Debug.Log("default move");
                    move(hitinfoEnd.point, new Vector3(1,0,0));
                }else{
                    //calculate move line
                    Debug.Log("line");
                    Vector3 formationLine = hitinfoEnd.point- hitinfoStart.point;
                    formationLine.y = 0;
                    move(hitinfoStart.point, formationLine);
                }
            }
            

            startPos = Vector2.zero;
            endPos = Vector2.zero;
        }
    }

    public void updateUnitLookDirection(){

    }

    public void move(Vector3 pos, Vector3 formationLine){
        // possible check for illegal moves that arent included in nav-mesh
        //(int, int) gridPos = WorldGenerator.worldPointToGrid(pos);


        List<GameObject> unitList = UnitSelections.instance.selectedUnitsList;
        int i=0;
        int row_num =0;
        int test =0;
        formationLine.Normalize();
        // Vector3 formation_direction = Vector3.Normalize(formationLine);
        // Debug.Log(formation_direction.magnitude);
        //Vector3 rotationDirection = new Vector3(formationLine.z,0, formationLine.x) * -1;
        Vector2 perp_fomration= Vector2.Perpendicular(new Vector2(formationLine.x,formationLine.z));
        Vector3 rotationDirection = new Vector3(perp_fomration.x,0,perp_fomration.y) * -1;
        rotationDirection.Normalize();
        Debug.Log(rotationDirection.magnitude);
        foreach(var unit in unitList){
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            agent.destination = (pos + (formationLine *i) + (rotationDirection * row_num));
            //agent.destination = pos + (rotationDirection * test++);
            if(i >= max_formation_length){
                i = 0;
                row_num++;
                Debug.Log(row_num);
            }else{
                i++;
            }
        }
    }
}
