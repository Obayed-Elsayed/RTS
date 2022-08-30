using UnityEngine;

using UnityEngine.EventSystems;// Required when using Event data.
public class BasicUi : MonoBehaviour
{
    [SerializeField]
    private Canvas mainCanvas;

    //set self to unactive and activate another menu
    public void GetBuildMenu(){
        
        //Debug.Log( mainCanvas.transform.GetChild(0));
        mainCanvas.transform.GetChild(1).gameObject.SetActive(false);
        mainCanvas.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void BuildMenuBack(){
        mainCanvas.transform.GetChild(1).gameObject.SetActive(true);
        mainCanvas.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void BuildMenuHouse2(){
        Debug.Log("fuck myy ree");
        Building.instance.buildMode = true;
        Building.instance.contructable_building = Building.instance.constructable_buildings_list[0];
        Building.instance.buildingX = 1;
        Building.instance.buildingZ = 1;
    }

    public void BuildMenuHouse8(){
        Building.instance.buildMode = true;
        Building.instance.contructable_building = Building.instance.constructable_buildings_list[1];
        Building.instance.buildingX = 2;
        Building.instance.buildingZ = 2;
    }



}
