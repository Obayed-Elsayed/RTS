using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unit;
    public int number_to_spawn = 15;
    void Start()
    {
        for(int i =0;i < number_to_spawn; i++){
            GameObject spawned_unit = Instantiate(unit) as GameObject;
            unit.transform.position = new Vector3 (i,3,5);
        }
    }

}
