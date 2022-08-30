using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnitSelections.instance.unitList.Add(this.gameObject);   
    }
    void onDestroy(){
        UnitSelections.instance.unitList.Remove(this.gameObject);   

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
