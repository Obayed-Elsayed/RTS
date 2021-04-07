using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public float zoomSpeed = 20f;

    public bool lockCamera = false;
    // Update is called once per frame
    void Update(){
        Vector3 pos = transform.position;
        if (Input.GetKeyUp("space")) {
            Debug.Log(lockCamera);
            lockCamera = (!lockCamera);
        }
        if (!lockCamera) {
            if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness){
                pos.z+= panSpeed * Time.deltaTime;
            }
            if(Input.GetKey("s") || Input.mousePosition.y <= 0 + panBorderThickness){
                pos.z-= panSpeed * Time.deltaTime;
            }
            if(Input.GetKey("d") || Input.mousePosition.x >= Screen.width + panBorderThickness){
                pos.x+= panSpeed * Time.deltaTime;
            }
            if(Input.GetKey("a") || Input.mousePosition.x <= 0 + panBorderThickness){
                pos.x-= panSpeed * Time.deltaTime;
            }
            // Zoom in
            if(Input.mouseScrollDelta.y>0){
                pos += Vector3.Normalize(transform.forward) *zoomSpeed;
            }
            // Zoom out
            if(Input.mouseScrollDelta.y<0){
                pos -= Vector3.Normalize(transform.forward) *zoomSpeed;
            }
        }
        // if(Input.GetKey("s")){
        //     pos.z-= panSpeed * Time.deltaTime;
        // }
        // if(Input.GetKey("w")){
        //     pos.x+= panSpeed * Time.deltaTime;
        // }
        // if(Input.GetKey("w")){
        //     pos.x+= panSpeed * Time.deltaTime;
        // }
        transform.position = pos;
    }
}
