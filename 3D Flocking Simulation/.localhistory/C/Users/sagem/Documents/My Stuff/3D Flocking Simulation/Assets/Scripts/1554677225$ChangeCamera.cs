using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Miranda Auriemma
 * Purpose: This script handles the logic for switching between the different camera angles
 */
public class ChangeCamera : MonoBehaviour {
    
    public Camera[] cameras;
    private int currentCameraIndex;
    [SerializeField] GameObject centroid;

    // Use this for initialization
    void Start()
    {
        currentCameraIndex = 0;
        
        //turn all cameras off except for the first default one
        for(int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //press C to cycle through cameras int the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraIndex++;

            //make sure currentCameraIndex is within bounds of the array
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);

            }
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }

            //turn off audio listener when not on fps character
            if (cameras[currentCameraIndex].tag != "MainCamera")
            {
                AudioListener.pause = true;
            }
            else
            {
                AudioListener.pause = false;

            }
        }

        if (Input.GetKey(KeyCode.L))
        {
            if(cameras[currentCameraIndex].tag == "MainCamera")
            {
                cameras[currentCameraIndex].transform.LookAt()
            }
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 50), "Press the 'C' key to change cameras \n Press the 'V' key to toggle debug lines");
        GUI.Box(new Rect(10, 60, 300, 50), cameras[currentCameraIndex].name);

        if (cameras[currentCameraIndex].tag == "MainCamera")
        {
            GUI.Box(new Rect(10, 110, 300, 70), "Use the mouse to look around and \n the WASD keys to move \n Press the 'L' key to look twoards the \n flock automatically");
        }
    }


}
