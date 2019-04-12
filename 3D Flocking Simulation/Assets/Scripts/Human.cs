using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Miranda Auriemma
 * Purpose: This script handles the steering forces for the humans
 */
public class Human : Vehicle
{
    //fields for finding ultimate force
    Vector3 ultimateForce;
    Vector3 PSGLocation;
    Vector3 zombieLocation;
    float zombieDistance;
    float zombieFutureDist;
    [SerializeField] int zombieFleeRadius;
    float humanRadius;


    //[SerializeField]


    [SerializeField] GameObject PSG;
    //[SerializeField] float maxSpeed;
    //[SerializeField] float mass;
    [SerializeField] float coeffOfFriction;


    // Use this for initialization
    // new keyword means the names will not conflict - Vehicle's start method is protected
    new void Start()
    {
        base.Start();
        PSG = GameObject.Find("PSG");
        humanRadius = gameObject.GetComponent<MeshRenderer>().bounds.max.x - vehiclePosition.x;
        targetLocation = PSG.transform.position;
    }

    // Update is called once per frame
    new void Update()
    {
        //CalcSteeringForces();
        base.Update();
    }

    /// <summary>
    /// Calculates the steering forces on the human gameobjects.
    /// This does not need to be called in this class - the parent class calls this method in it's Update method.
    /// </summary>
    public override void CalcSteeringForces()
    {
        //reset ultimate force
        ultimateForce = Vector3.zero;

        //ultimateForce += base.Seek(PSG);

        if (collisionManager.zombies.Count > 0)
        {
            float sqrZombieRadius = Mathf.Pow(zombieFleeRadius, 2);

            GameObject zombie = FindNearestZombie();

            //calculate distance from the zombie
            zombieLocation = zombie.transform.position;
            //zombieFutureDist = (transform.position - zombie.GetComponent<Zombie>().futurePosition).sqrMagnitude;
            //find its future position as well
            //FindNearestHuman now also sets the zombie distance
            //zombieDistance = (zombieLocation - gameObject.transform.position).sqrMagnitude;


            //if the zombie is too close, call flee, otherwise wander
            if (zombieDistance < humanRadius + zombieFleeRadius || zombieFutureDist < humanRadius + zombieFleeRadius)
            {
                //ultimateForce += base.Evade(zombie);
                //maxSpeed = burstSpeed;
            }
            else
            {
                ultimateForce += Wander2();
                //maxSpeed = maxNormalSpeed;

                //base.ApplyFriction(coeffOfFriction);
            }

            ultimateForce += ObstacleAvoidance();
        }
        else
        {
            //maxSpeed = maxNormalSpeed;
            //Vector3 obstacleAvoidHolder = ObstacleAvoidance();
            //if (obstacleAvoidHolder == Vector3.zero)
            //{
            //    ultimateForce += Wander();
            //}
            //else
            //{
            //    ultimateForce += obstacleAvoidHolder;
            //}
            ultimateForce += ObstacleAvoidance();
            ultimateForce += Wander2();
            //Wander();
        }

        //scale ultimate force to human's max speed
        //square the max speed so that it is more comparable to the sqr magnitude
        //Vector3.ClampMagnitude(ultimateForce, maxSpeed);        

        ultimateForce += KeepSeparated(collisionManager.humans);

        ultimateForce += StayInBounds();

        //ultimateForce += ObstacleAvoidance();

        //obstacle avoidance
        //ultimateForce += base.ObstacleAvoidance();

        //apply ultimate force
        ApplyForce(ultimateForce);


    }

    //public override GameObject FindNearestOther()
    //{
    //    //throw new System.NotImplementedException();
    //    return null;
    //}


    /// <summary>
    /// Returns the zombie nearest to the human, and sets the distance of it
    /// </summary>
    /// <returns></returns>
    public GameObject FindNearestZombie()
    {
        float shortestDistance = 0;
        float tempDistance = 0;
        GameObject closestZombie = null;
        //Debug.Log(collisionManager.zombies.Count);
        foreach (GameObject zombie in collisionManager.zombies)
        {
            tempDistance = (transform.position - zombie.transform.position).sqrMagnitude;
            if (tempDistance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = tempDistance;
                closestZombie = zombie;
            }
        }
        zombieDistance = shortestDistance;
        return closestZombie;
    }

    
}
