using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragSelect : MonoBehaviour
{
    public Camera mainCam;

    [SerializeField]
    RectTransform boxVisual;

    Rect selectionBox;
    Vector2 startPos;
    Vector2 endPos;
    void Start()
    {
        startPos = Vector2.zero;
        endPos = Vector2.zero;
        DrawBox();
    }

    // Update is called once per frame
    void Update()
    {
        // click
        if(Input.GetMouseButtonDown(0)){
            startPos = Input.mousePosition;
        }
        //holding
        if(Input.GetMouseButton(0)){
            endPos = Input.mousePosition;
            DrawBox();
            DrawSelection();
        }
        // release
        if(Input.GetMouseButtonUp(0)){
            SelectUnits();
            startPos = Vector2.zero;
            endPos = Vector2.zero;
            DrawBox();

        }
    }
    // visual box
    void DrawBox(){
        Vector2 boxStart = startPos;
        Vector2 boxEnd = endPos;
        Vector2 boxCenter = (boxStart + boxEnd)/2;

        boxVisual.position = boxCenter;
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x-boxEnd.x),Mathf.Abs(boxStart.y-boxEnd.y));
        boxVisual.sizeDelta = boxSize;

    }
    // logic box
    void DrawSelection(){
        // Dragging right 
        if(Input.mousePosition.x < startPos.x){
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPos.x;
        }else{ // Dragging left
            selectionBox.xMin = startPos.x;
            selectionBox.xMax = Input.mousePosition.x;
        }


        // dragging down
        if(Input.mousePosition.y < startPos.y){
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPos.y;
        }else{ // Dragging Up
            selectionBox.yMin = startPos.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }
    void SelectUnits(){
        // Example using lambda expressions 
        UnitSelections.instance.unitList.ForEach(unit=>{
            if(selectionBox.Contains(mainCam.WorldToScreenPoint(unit.transform.position))){
                UnitSelections.instance.DragSelect(unit);
            }
        });
        // foreach(var unit in UnitSelections.instance.unitList){
        //     // we can convert world space to screen space and check if it is in the box!
        //     if(selectionBox.Contains(mainCam.WorldToScreenPoint(unit.transform.position))){
        //         UnitSelections.instance.DragSelect(unit);
        //     }
        // }
    }
}
