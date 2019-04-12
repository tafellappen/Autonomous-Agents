using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Miranda Auriemma
 * Purpose: This script handles the steering forces for the zombies
 */
public class Zombie : Vehicle
{
    Vector3 ultimateForce;
    public GameObject nearestHuman { get; set; }
    //float humanDistance;

    //[SerializeField] 
    //private List<GameObject> humans;
    //private List<GameObject> zombies;

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        
    }

    /// <summary>
    /// Calculates the steering forces on the zombie gameobjects.
    /// This does not need to be called in this class - the parent class calls this method in it's Update method.
    /// </summary>
    public override void CalcSteeringForces()
    {
        ultimateForce = new Vector3(0, 0, 0);
        //humanLocation = human.transform.position;
        //Debug.Log("defined " + ultimateForce);

        //apply the force calculated from the seek method (use human location)
        //if (collisionManager.humans.Count >= 1)
        //{
        //    maxSpeed = maxNormalSpeed;
        //    nearestHuman = FindNearestHuman();
        //    ultimateForce += Pursue(nearestHuman);

        //    //Debug.Log("seeking force " + ultimateForce);
        //}
        //else
        //{
        //    ultimateForce += Wander();
        //    //Debug.Log("just wandering");
        //}

        //ultimateForce += KeepSeparated(collisionManager.zombies);


        //apply bounds forces
        ultimateForce += StayInBounds();
        //Debug.Log("after bounds check " + ultimateForce);

        //obstacle avoidance
        ultimateForce += ObstacleAvoidance();

        //apply the force
        ApplyForce(ultimateForce);
        
        //Debug.Log(ultimateForce);
    }

    //public override GameObject FindNearestOther()
    //{
    //    //throw new System.NotImplementedException();
    //    return null;
    //}

    /// <summary>
    /// Returns the human nearest to the zombie
    /// </summary>
    /// <returns></returns>
    //public GameObject FindNearestHuman()
    //{
    //    float shortestDistance = 0;
    //    float tempDistance = 0;
    //    GameObject closestHuman = null;

        
    //    foreach(GameObject human in manager.flockers)
    //    {
    //        tempDistance = (transform.position - human.transform.position).sqrMagnitude;
    //        if(tempDistance < shortestDistance || shortestDistance == 0)
    //        {
    //            shortestDistance = tempDistance;
    //            closestHuman = human;

    //            targetLocation = closestHuman.GetComponent<Human>().futurePosition;
    //        }

    //    }
    //    return closestHuman;
    //}
}
