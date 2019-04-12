using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocker : Vehicle {

    Vector3 ultimateForce;
    Vector3 PSGLocation;
    [SerializeField] GameObject PSG;

    // Use this for initialization
    void Start () {
        base.Start();
        //PSG = GameObject.Find("PSG");
        //humanRadius = gameObject.GetComponent<MeshRenderer>().bounds.max.x - vehiclePosition.x;
        //targetLocation = PSG.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    /// <summary>
    /// Calculates the steering forces placed on an object
    /// </summary>
    public override void CalcSteeringForces()
    {
        float distance = (manager.flockCenter - vehiclePosition).sqrMagnitude;
        ultimateForce = Vector3.zero;
        ultimateForce += Wander();
        //ultimateForce += FlockWander3();
        ultimateForce += Cohesion();
        if(distance < sqrMaxOutlierDistance)
        {
            ultimateForce += Alignment();

        }

        ultimateForce += KeepSeparated(manager.flockers);
        
        
        ultimateForce += ObstacleAvoidance();
        ultimateForce += StayInBounds();


        ApplyForce(ultimateForce);
    }
}
