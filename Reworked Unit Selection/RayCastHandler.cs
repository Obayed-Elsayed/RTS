using UnityEngine;

public class RayCastHandler : MonoBehaviour
{
    /*
    * this class is for all things ray cast
    * Used to avoid making multiple raycasts for different purposes
    */
    public Camera cam;
    public LayerMask clickable;


    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hitinfo;
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hitinfo,Mathf.Infinity,clickable)){
                if(Input.GetKey(KeyCode.LeftShift)){
                    UnitSelections.instance.shiftClickSelect(hitinfo.collider.gameObject);
                }else{
                    UnitSelections.instance.ClickSelect(hitinfo.collider.gameObject);
                }

            }else{
                if(!Input.GetKey(KeyCode.LeftShift)){
                    UnitSelections.instance.DeselectAll();
                }
            }
        }
        // else if(Input.GetMouseButtonDown(1)){
        //     RaycastHit hitinfo;
        //     if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hitinfo,Mathf.Infinity,(1<<8))){
        //         Debug.Log("moving");
        //         UnitMovement.move(hitinfo.point, new Vector3(1,0,0));
        //     }
        // }


    }
}
