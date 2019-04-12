//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PathFollwer : Vehicle {

//    Vector3 ultimateForce;
//    //[SerializeField] Manager manager;
//    int currentSeekingNode;
//    GameObject[] pathNodes;
//    [SerializeField] float nodeRadius;
//    float nodeSqrRad;
//	// Use this for initialization
//	void Start () {
//        base.Start();
//        pathNodes = manager.pathNodes;
//        currentSeekingNode = 0;
//        transform.position = manager.pathNodes[currentSeekingNode].transform.position;
//        nodeSqrRad = Mathf.Pow(nodeRadius, 2);
//	}
	
//	// Update is called once per frame
//	void Update () {
//        base.Update();
//	}

//    /// <summary>
//    /// Calculates the steering forces for the vehicle
//    /// </summary>
//    public override void CalcSteeringForces()
//    {
//        ultimateForce = Vector3.zero;
//        //seek the next node in the array
//        //move on to the next one if the follower is close enough
//        float distance = (vehiclePosition - manager.pathNodes[currentSeekingNode].transform.position).sqrMagnitude;
//        if (distance < nodeSqrRad)
//        {
//            Debug.Log(currentSeekingNode);

//            currentSeekingNode++;

//            if (currentSeekingNode >= manager.pathNodes.Length)
//            {
//                currentSeekingNode = 0;
//            }
//            Debug.Log(currentSeekingNode);
//        }

//        ultimateForce += Seek(manager.pathNodes[currentSeekingNode]);

//        ApplyForce(ultimateForce);
//    }
//}
