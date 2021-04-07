using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUtil : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable;

    private void Start(){
        selectedTable = new Dictionary<int, GameObject>();
    }
    
    public void addSelected(GameObject go){
        int id = go.GetInstanceID();
        if(!selectedTable.ContainsKey(id)){
            selectedTable.Add(id, go);
            var comp = selectedTable[id].GetComponent<Selected>(); 
            comp.selected = true;
        }
        Debug.Log("Count in add: "+selectedTable.Count.ToString());
    }


    // public void deselect(int id){
    //     if(selectedTable[id]!= null){
    //         Debug.Log("Removing one");
    //         Destroy(selectedTable[id].GetComponent<Selected>());
    //         selectedTable.Remove(id);
    //     }
    // }

    public void deselectAll(){
        if(selectedTable== null){
            Debug.Log("Null for some fking reason");
        }
        Debug.Log("Count in remove: "+selectedTable.Count.ToString());
        foreach(KeyValuePair<int, GameObject> kvp in selectedTable){
            if(kvp.Value != null){
                var comp = selectedTable[kvp.Key].GetComponent<Selected>(); 
                comp.selected = false;
            }
        }
        selectedTable.Clear();

    }

}
