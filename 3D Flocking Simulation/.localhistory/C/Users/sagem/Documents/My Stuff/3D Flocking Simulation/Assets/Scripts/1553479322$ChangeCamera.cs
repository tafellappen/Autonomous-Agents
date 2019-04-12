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
    private GameObject fpsCamera;
    private float defaultVolume;
    // Use this for initialization
    void Start()
    {
        currentCameraIndex = 0;

        fpsCamera = GameObject.FindGameObjectWithTag("MainCamera");
        defaultVolume = AudioListener.volume;
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
            if (cameras[currentCameraIndex] != fpsCamera)
            {
                AudioListener.volume = 0f;
                //fpsCamera.GetComponent<AudioListener>().
            }
            else
            {
                AudioListener.volume = 1f;
                //fpsCamera.GetComponent<AudioListener>().enabled = true;

            }

        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 50), "Press the 'C' key to change cameras \n Press the D key to toggle debug lines");
        GUI.Box(new Rect(10, 60, 300, 50), cameras[currentCameraIndex].name);
    }


}
