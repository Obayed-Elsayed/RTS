using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelections : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> selectedUnitsList = new List<GameObject>();

    private static UnitSelections _instance;
    public static UnitSelections instance {get {return _instance;}}


    void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }else{
            _instance = this;
        }
    }

    public void ClickSelect(GameObject unit){
        DeselectAll();
        selectedUnitsList.Add(unit);
        unit.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void shiftClickSelect(GameObject unit){
        if(!selectedUnitsList.Contains(unit)){
            selectedUnitsList.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }else{
            unit.transform.GetChild(0).gameObject.SetActive(false);
            selectedUnitsList.Remove(unit);
        }
    }
    public void DragSelect(GameObject unit){
        if(!selectedUnitsList.Contains(unit)){
            selectedUnitsList.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void DeselectAll(){
        foreach(var unit in selectedUnitsList){
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        selectedUnitsList.Clear();
    }
    public void Deselect(GameObject unit){
        unit.transform.GetChild(0).gameObject.SetActive(false);
        selectedUnitsList.Remove(unit);
    }

}
