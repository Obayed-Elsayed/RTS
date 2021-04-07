using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : MonoBehaviour
{
    // Start is called before the first frame update
    public bool selected= false;


    private void Update(){
      //  Debug.Log("SELECTED");
        if(selected){
            GetComponent<Renderer>().material.color = Color.red;
        }else{
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
