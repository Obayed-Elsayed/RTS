using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : MonoBehaviour
{
    // Start is called before the first frame update
    public bool selected = false;

    private int layerMask = (1 << 8);
    private RaycastHit hit;

    GameObject controller;
    FlockingBehaviour movementBehaviour;

    public void Start() {
        controller = GameObject.FindGameObjectWithTag("GameController");
        movementBehaviour = GetComponent<FlockingBehaviour>();
    }
    private void Update()
    {

        if (selected)
        {
            GetComponent<Renderer>().material.color = Color.red;
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500000f, layerMask))
                {
                    PathFinderManager.requestPath(new PathRequest(transform.position, hit.point, movementBehaviour.giveOrder));
                    //GetComponent<FlockingBehaviour>().giveOrder(hit.point);
                }
            }
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
