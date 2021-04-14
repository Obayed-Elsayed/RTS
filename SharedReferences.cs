using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedReferences : MonoBehaviour
{
    // Start is called before the first frame update
    public SelectionController controller;
    void Start()
    {
        controller = GetComponent<SelectionController>();
    }

    public SelectionController getController(){
        return controller;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
