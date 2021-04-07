using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{

    private SelectionUtil selectionUtil;
    private RaycastHit hit;

    //[SerializeField] private LayerMask layerMask;
    private int layerMask;
    private bool startedSelection;
    private Vector3 initialPoint;
    private Vector3 finalPoint;
    private Vector2 p1;

    private void Start(){
        selectionUtil = GetComponent<SelectionUtil>();
        // 3 is the ground layer
        layerMask = (1<<8);
        // layerMask= ~layerMask;
        //Debug.Log(layerMask.CompareTo());
        startedSelection = false;
    }

    private void Update(){
        boxSelect();
    }

    public ShowMeshBounds bounds;
    private void boxSelect(){
    if(Input.GetMouseButtonDown(0) && !startedSelection){
        startedSelection = true;
        p1 = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(p1);
        if(Physics.Raycast(ray, out hit, 500000f, layerMask)){
            // Debug.Log(hit.point);
            initialPoint = hit.point;
           // Debug.Log("Initial: "+ initialPoint.ToString());
        }
    }

    if(Input.GetMouseButtonUp(0) && startedSelection ){

        // Alternatively only Unit layer objects
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
    
        startedSelection = false;
        if(!Input.GetKey(KeyCode.LeftShift)){
            selectionUtil.deselectAll();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 500000f, layerMask)){
            finalPoint = hit.point;
            //Debug.Log("Final: "+ finalPoint.ToString());
        }
        
        Vector3 size = new Vector3(Mathf.Abs(finalPoint.x - initialPoint.x), 20, Mathf.Abs(finalPoint.z - initialPoint.z));
        Vector3 midpoint = (finalPoint + initialPoint)/2;
        bounds.v3Center = midpoint;
        bounds.v3Extents = size/2;
        Bounds b = new Bounds(midpoint, size/2);
        int mask = (1<<6);
        RaycastHit[] hits = Physics.BoxCastAll(midpoint, size/2, Vector3.up, Quaternion.identity, 0, mask);
        foreach(RaycastHit hit in hits){

            selectionUtil.addSelected(hit.collider.gameObject);

        }
        // is it better or faster to keep an arraylist of Entities?
        // int found =0;
        // for(int i = 0; i < entities.Length; i++){
        //     Collider c = entities[i].GetComponent<Collider>();
        //     if(b.Intersects(c.bounds) || b.Contains(entities[i].transform.position)){
        //         found++;
        //         selectionUtil.addSelected(entities[i]);
        //     }
        // }
        //Debug.Log("Found: "+found.ToString());
    }
    if(initialPoint!= null && finalPoint !=null){
        // Debug.DrawLine(midpoint,midpoint+ new Vector3(1,1,1),Color.red);
        Debug.DrawLine(initialPoint, finalPoint, Color.green);
    }
}

    //private Texture2D texture;
    private void OnGUI(){
        if(startedSelection)
        {
            var rect = Utils.GetScreenRect(p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
