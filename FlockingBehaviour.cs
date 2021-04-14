using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingBehaviour : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 destination;
    private float velocityMagnitude = 20f;
    private float perceptionRange = 15f;
    private float seperationRange = 10f;

    public float maxForce;
    public float maxForceSeperation;
    public float maxForceAlign;
    public float maxVelocity;

    private bool order = false;
    private bool run = false;

    private SelectionUtil selectUtil;

    void Start()
    {
        Debug.Log("unga bunga");
        position = GetComponent<Transform>().position;
        velocity = new Vector3();
        acceleration = new Vector3();
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        selectUtil = go.GetComponent<SelectionController>().selectionUtil;


    }

    public void setDestinationVecloity(Vector3 dest)
    {
        velocity = dest * 10;
    }
    // Update is called once per frame
    void Update()
    {
        //transform.position += velocity * velocityMagnitude * Time.deltaTime;
        randomize();
        if (run)
        {
            // cohesion();
            seperation();
            //align();
        }
        if(order){
            seek(destination);
        }
        velocity += acceleration;
        transform.position += velocity * Time.deltaTime;
        acceleration *= 0;
    }

    public void align()
    {

        Vector3 desiredVelocity = new Vector3();
        int total = 0;
        foreach (var unit in selectUtil.selectedTable.Values)
        {
            var dist = Vector3.Distance(unit.transform.position, transform.position);

            if (unit.GetInstanceID() != GetInstanceID() && dist < perceptionRange)
            {
                desiredVelocity += unit.GetComponent<FlockingBehaviour>().velocity;
                total++;
            }
        }

        if (total > 0)
        {
            // Craig Reynolds steering formula Steering = desired velocity - current velocity
            desiredVelocity /= total;
            // setting magnitude to max velocity
            desiredVelocity.Normalize();
            desiredVelocity *= maxVelocity;
            desiredVelocity -= velocity;
            Vector3.ClampMagnitude(desiredVelocity, maxForceAlign);
        }
        acceleration += desiredVelocity;
    }

    public void cohesion()
    {

        Vector3 averageLocation = new Vector3();
        int total = 0;
        // THIS WILL STOP WORKING IOCNE sELECTED BECAUSE WE ITERATE THRU SELECTED OBJECTS TO AVERAGE SHIT
        // Keep copy until a move order is finished
        foreach (var unit in selectUtil.selectedTable.Values)
        {
            var dist = Vector3.Distance(unit.transform.position, transform.position);
            if (unit.GetInstanceID() != GetInstanceID() && dist < perceptionRange)
            {
                averageLocation += unit.transform.position;
                total++;
            }
        }

        if (total > 0)
        {

            // Craig Reynolds steering formula Steering = desired velocity - current velocity
            averageLocation /= total;
            // Vector that points in average direction
            averageLocation -= transform.position;
            averageLocation.Normalize();
            averageLocation *= maxVelocity;
            averageLocation -= velocity;
            Vector3.ClampMagnitude(averageLocation, maxForce);
        }
        acceleration += averageLocation;
    }

    public void seperation()
    {
        Vector3 averageLocation = new Vector3();
        int total = 0;
        // THIS WILL STOP WORKING once SELECTED BECAUSE WE ITERATE THRU SELECTED OBJECTS TO AVERAGE SHIT
        // Keep copy until a move order is finished
        foreach (var unit in selectUtil.selectedTable.Values)
        {
            float dist = Vector3.Distance(unit.transform.position, transform.position);
            if (unit.GetInstanceID() != GetInstanceID() && dist < seperationRange)
            {
                // create vector pointing at other units, it scales with inverse proportions
                // average them and thats where we need to move to get seperated
                Vector3 diff = transform.position - unit.transform.position;
                averageLocation += diff;
                total++;
            }
        }
        if (total > 0)
        {
            averageLocation /= total;
            averageLocation.Normalize();
            averageLocation *= maxVelocity;
            averageLocation -= velocity;
            Vector3.ClampMagnitude(averageLocation, maxForceSeperation);
        }
        acceleration += averageLocation;
    }

    public float maxSeekSpeed; 
    public float maxSeekForce; 

    private void seek(Vector3 location)
    {
        if(Vector3.Distance(location, transform.position)<1){
            velocity = new Vector3();
            order = false;
            return; 
        }
        Vector3 desiredVelocity = location - transform.position;
        desiredVelocity.Normalize();
        desiredVelocity*=maxSeekSpeed;
        
        Vector3 steering = desiredVelocity - velocity;
        Vector3.ClampMagnitude(steering, maxSeekForce);
        
        acceleration += steering;
    }

    public void giveOrder(Vector3 dest){
        order = true;
        destination = dest;
    }

    private void randomize()
    {
        if (Input.GetKey(KeyCode.Keypad0) && GetComponent<Selected>().selected)
        {
            if (!run)
            {
                Vector2 randomVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                randomVector.Normalize();
                velocityMagnitude = Random.Range(3f, 7f);
                velocity.x = randomVector.x * velocityMagnitude;
                velocity.z = randomVector.y * velocityMagnitude;
            }
            run = true;
        }
    }
}
