﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////this camera smoothes out rotation around the y-axix and height
////horizontal distance to the target is always fixed
////for every one of those smoothed values, calculate the wanted value and the current value
////smooth it using the lerp function and apply the smoothed values to the transform's position

//public class SmoothFollow : MonoBehaviour
//{

//    //public Transform target;
//    //public float distance = 3.0f;
//    //public float height = 1.50f;
//    //public float heightDamping = 2.0f;
//    //public float positionDamping = 2.0f;
//    //public float rotationDamping = 2.0f;

//    public Transform target;
//    public float distance = 3.0f;
//    public float height = 1.50f;
//    public float heightDamping = 2.0f;
//    public float positionDamping = 2.0f;
//    public float rotationDamping = 2.0f;
//    [SerializeField] bool backwardsFacingCamera;

//    // Update is called once per frame
//    void LateUpdate()
//    {
//        //early exit if no target
//        if (!target) return;

//        float wantedHeight = target.position.y + height;
//        float currentHeight = transform.position.y;

//        //damp the height
//        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

//        //set the camera position
//        //if (!backwardsFacingCamera)
//        //{
//        //    Vector3 wantedPosition = target.position - target.forward * distance;
//        //}
//        //else
//        //{
//        //    Vector3 wantedPosition = target.position - target.forward * distance;
//        //}
//        Vector3 wantedPosition = target.position - (target.forward * distance);
//        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);

//        //adjust height of camera
//        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

//        //set forward to rotate with time
//        if (!backwardsFacingCamera)
//        {
//            transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationDamping);
//        }
//        else
//        {
//            transform.forward = Vector3.Lerp(transform.forward, -target.forward, Time.deltaTime * rotationDamping);

//        }
//    }

//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this camera smoothes out rotation around the y-axix and height
//horizontal distance to the target is always fixed
//for every one of those smoothed values, calculate the wanted value and the current value
//smooth it using the lerp function and apply the smoothed values to the transform's position

public class SmoothFollow : MonoBehaviour
{

    //public Transform target;
    //public float distance = 3.0f;
    //public float height = 1.50f;
    //public float heightDamping = 2.0f;
    //public float positionDamping = 2.0f;
    //public float rotationDamping = 2.0f;

    public Transform target;
    public float distance = 3.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping = 2.0f;
    public float rotationDamping = 2.0f;
    [SerializeField] bool backwardsFacingCamera;
    [SerializeField] bool dontRotateCamera;
    [SerializeField] GameObject centroid;
    // Update is called once per frame
    void LateUpdate()
    {
        //early exit if no target
        if (!target) return;

        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;

        //damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        //set the camera position
        //if (!backwardsFacingCamera)
        //{
        //    Vector3 wantedPosition = target.position - target.forward * distance;
        //}
        //else
        //{
        //    Vector3 wantedPosition = target.position - target.forward * distance;
        //}
        Vector3 wantedPosition = target.position - (target.forward * distance);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);

        //adjust height of camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        transform.LookAt(target);
        ////set forward to rotate with time
        //if (!backwardsFacingCamera || !dontRotateCamera)
        //{
        //    transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationDamping);
        //}
        //else
        //{
        //    transform.forward = Vector3.Lerp(transform.forward, -target.forward, Time.deltaTime * rotationDamping);

        //}
        //if (dontRotateCamera)
        //{
        //    //set vector to fwd, then rotate down
        //    Vector3 targetRotation = new Vector3(target.forward.x - 45, target.forward.y, target.forward.z);
        //    transform.forward = Vector3.Lerp(transform.forward, -targetRotation, Time.deltaTime * rotationDamping);
        //    //transform.forward = Vector3.Lerp(transform.forward, new Vector3(-90, 0, target.forward.z), Time.deltaTime * rotationDamping);
        //    //transform.rotation = Quaternion.Euler(new Vector3(45, transform.rotation.y, transform.rotation.z));
        //}
    }

}

