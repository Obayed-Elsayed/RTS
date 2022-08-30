
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Building : MonoBehaviour
{
    public bool buildMode = false;
    public Camera cam;
    private Vector3 mousePos;


    private static Building _instance;
    public static Building instance {get {return _instance;}}

    // Building Display Related
    public GameObject contructable_building;
    public int buildingX, buildingZ;
    private Material construction_material;
    public Material placeable_material,unplaceable_material;
    private Quaternion buildingRotation;

    public List<GameObject> constructable_buildings_list;

    public EventSystem main;

    private List<GameObject> finished_buildings;
    void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }else{
            _instance = this;
        }

        main = GetComponent<EventSystem>();
    }

    public void Start(){
        construction_material = placeable_material;
        buildingRotation = Quaternion.identity;
        finished_buildings = new List<GameObject>();
    }
    void Update()
    {
        // if(Input.GetKeyUp(KeyCode.B)){
        //     buildMode = !buildMode;
        // }
        if(main.IsPointerOverGameObject()){
            Debug.Log("over an obj");
        }
        if(buildMode){
            // avoid always ray casting after doing on the last update in the same spot
            if(mousePos!= Input.mousePosition){
                RaycastHit hitinfo;
                // ray cast to terrain
                if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hitinfo,Mathf.Infinity,(1<<8)) &&
                    contructable_building != null){
                    GameObject child =contructable_building.transform.GetChild(0).gameObject;
                    MeshFilter meshFilter =child.GetComponent<MeshFilter>();

                    Vector2Int intworldpoint = WorldGenerator.positionToWorldPoint(hitinfo.point);
                    Vector3 displayedPos = new Vector3(intworldpoint.x, hitinfo.point.y, intworldpoint.y);
                    // get the prefab transform matrix
                    applyBuildingRotation();
                    addBuilding(intworldpoint, hitinfo.point.y);
                    Matrix4x4 m4 = Matrix4x4.TRS(displayedPos+ child.transform.localPosition,buildingRotation,child.transform.localScale);
                    if(checkBuildingFits(intworldpoint))
                        construction_material = placeable_material;
                    else
                        construction_material = unplaceable_material;

                    
                    // cycle through all of the sub-meshes -> each material = different submesh
                    for (int i = 0; i < child.GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
                    {
                        Graphics.DrawMesh(meshFilter.sharedMesh,m4, construction_material, 0, cam, i);
                    }
                    //Graphics.DrawMesh(meshFilter.sharedMesh,hitinfo.point,Quaternion.identity,contruction_material, 0, cam,1);//????????????
                }
            }
            //mousePos = Input.mousePosition;
        }
    }

    private bool checkBuildingFits(Vector2Int pos){
            for(int x =0; x < buildingX; x++){
                for(int z =0; z < buildingZ; z++){
                    if(WorldGenerator.instance.gridMapInfo[pos.x+x,pos.y+z].Item2!=0){
                        return false;
                    }
                }
            }

        return true;
    }

    private void updateWorldGridInfo(Vector2Int pos){
            for(int x =0; x < buildingX; x++){
                for(int z =0; z < buildingZ; z++){
                    WorldGenerator.instance.gridMapInfo[pos.x+x,pos.y+z].Item2 = 3;
                }
            }
    }

    private void applyBuildingRotation(){
        if(Input.GetKeyUp(KeyCode.R)){
            buildingRotation.eulerAngles += new Vector3(0,90,0);
        }
    }

    private void addBuilding(Vector2Int intworldpoint, float ypos){
        if(Input.GetMouseButtonUp(0) && checkBuildingFits(intworldpoint) && !main.IsPointerOverGameObject()){
            // instance building
            GameObject built = Instantiate(contructable_building) as GameObject;
            // built.transform.eulerAngles = buildingRotation.eulerAngles;
            built.transform.GetChild(0).transform.eulerAngles = buildingRotation.eulerAngles;
            built.transform.position = new Vector3(intworldpoint.x, ypos, intworldpoint.y);
            updateWorldGridInfo(intworldpoint);
            // UI disable selection
            // kinda bad? cant i subscribe to deselection event?
            if(main.currentSelectedGameObject == null){
                buildMode = false;
            }
        }
    }

}
