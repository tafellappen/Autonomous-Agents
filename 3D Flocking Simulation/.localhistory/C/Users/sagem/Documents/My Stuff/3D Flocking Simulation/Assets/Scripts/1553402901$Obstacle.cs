using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Miranda Auriemma
 * Purpose: holds the radius of the obstacles
 */
public class Obstacle : MonoBehaviour
{    
    public float radius { get; set; }
    [SerializeField] float addToRadius;

    // Use this for initialization
    void Start()
    {
        radius = gameObject.GetComponent<MeshRenderer>().bounds.extents.x + addToRadius;
        Debug.Log(radius);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
