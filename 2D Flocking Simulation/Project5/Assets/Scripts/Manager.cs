using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Miranda Auriemma
 * Purpose: This script handles misellaneous things, such as initial spawning and 
 * whether or not to have debug lines on
 */
public class Manager : MonoBehaviour
{
    [SerializeField] GameObject centroid;
    [SerializeField] GameObject flocker;
    //[SerializeField] GameObject follower;
    [SerializeField] int flockerCount;
    [SerializeField] int zombieStartCount;

    public List<GameObject> flockers { get; set; }

    //fields used when moving the PSG
    [SerializeField] Terrain editorTerrain; //god PLEASE fix this later
    public Terrain terrain { get; set; }
    [SerializeField] TerrainData terrainData;
    [SerializeField] TerrainData terrainBounds;
    [SerializeField] public Vector3 worldBounds { get; set; }
    /// <summary>
    /// Percent of the worldspace that things are allowed to be in
    /// </summary>
    [SerializeField] float percentOfFloor;
    private GameObject[] obstaclesArray;
    public List<GameObject> obstacles { get; set; }
    public List<Obstacle> obstacleScripts { get; set; }

    //debug
    public bool drawDebug { get; set; }
    [SerializeField] Material centroidDebug;
    [SerializeField] Material centroidClear;
    Renderer centroidRenderer;

    //flocking fields
    public Vector3 flockCenter { get; set; }
    [SerializeField] GameObject flockCenterPrefab;
    public Vector3 flockDirection { get; set; }
    [SerializeField] Material flockDirectionCol;
    [SerializeField] float flockDebugLength;
    public Vector3 wanderDirection { get; set; }

    //path following
    public GameObject[] pathNodes;
    [SerializeField] GameObject pathFollower;
    private void Awake()
    {
        //pathNodes = GameObject.FindGameObjectsWithTag("Path Node");
        
    }

    // Use this for initialization
    void Start()
    {
        drawDebug = false;
        centroidRenderer = centroid.GetComponent<Renderer>();

        obstacles = new List<GameObject>();
        terrain = editorTerrain;        
        //worldBounds = floor.transform.localScale;
        terrainData = terrain.terrainData;
        //get world coordinates based on the terrain
        worldBounds = new Vector3(terrainData.bounds.max.x, 0, terrainData.bounds.max.z);
        worldBounds = worldBounds * (percentOfFloor/100);

        flockers = new List<GameObject>();

        //create flockers
        for(int i = 0; i < flockerCount; i++)
        {
            AddFlocker();
        }

        obstaclesArray = GameObject.FindGameObjectsWithTag("Obstacle"); //fix null issues
        
        foreach (GameObject obstacle in obstaclesArray)
        {
            obstacles.Add(obstacle);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(Vector3.zero, worldBounds, Color.white);
        //float distance = (flocker.transform.position - PSG.transform.position).sqrMagnitude;
        //if (distance < 5)
        //{
        //    PSG.transform.position = new Vector3(
        //        Random.Range(-worldBounds.x, worldBounds.x),
        //        PSG.transform.position.y, //just keep the old Y position
        //        Random.Range(-worldBounds.z, worldBounds.z)
        //        );
        //}

        FindFlockCenter();
        FindFlockAlignment();
        MakeFlockWanderDirection();
        centroid.transform.position = flockCenter;
        centroid.transform.forward = flockDirection;

        if (Input.GetKeyUp(KeyCode.D))
        {
            drawDebug = !drawDebug;
        }

        if (drawDebug)
        {
            centroidRenderer.material = centroidDebug;
        }
        else
        {
            centroidRenderer.material = centroidClear;
        }
    }


    #region debug line methods
    private void OnRenderObject()
    {
        if (drawDebug)
        {
            DrawFlockDirection();
            DrawPath();            
        }
    }

    /// <summary>
    /// Draws the debug line for the flock direction
    /// </summary>
    public void DrawFlockDirection()
    {
        //flockDirectionCol.SetPass(0);

        //GL.Begin(GL.LINES);
        ////draw them in a foreach loop. 
        //for(int i = 0; i < pathNodes.Length; i++)
        //{
        //    Debug.DrawLine(pathNodes[i].transform.position, new Vector3(pathNodes[i].transform.position.x, pathNodes[i].transform.position.y + 100, pathNodes[i].transform.position.z));
        //    //GL.Vertex(node.transform.position);
        //    if (i == 0)
        //    {
        //        GL.Vertex(pathNodes[pathNodes.Length - 1].transform.position);
        //    }
        //    else
        //    {
        //        GL.Vertex(pathNodes[i-1].transform.position);
        //    }
        //    GL.Vertex(pathNodes[i].transform.position);
        //}
        //GL.End();
        
    }

    /// <summary>
    /// Draws the debug lines for the path
    /// </summary>
    public void DrawPath()
    {
        //just using the same material because im lazy
        flockDirectionCol.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Vertex(flockCenter);
        GL.Vertex(flockCenter + flockDirection * flockDebugLength);
        GL.End();
    }

    #endregion

    /// <summary>
    /// Generates a random point within worldspace
    /// </summary>
    /// <returns></returns>
    public Vector3 CreateRandomPoint()
    {
        float xPos = Random.Range(0, worldBounds.x);
        float zPos = Random.Range(0, worldBounds.z);
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
        return new Vector3(xPos, yPos, zPos);
        //return Random.Range(-worldBounds.x, worldBounds.x);
    }

    public void MovePSG(GameObject human)
    {
        float distance = (human.transform.position - centroid.transform.position).sqrMagnitude;
        if (distance < 5)
        {
            centroid.transform.position = new Vector3(
                Random.Range(-worldBounds.x, worldBounds.x),
                centroid.transform.position.y, //just keep the old Y position
                Random.Range(-worldBounds.z, worldBounds.z)
                );
        }
    }

    /// <summary>
    /// Adds a human to the scene and creates necessary references to things
    /// </summary>
    public void AddFlocker()
    {
        GameObject thisFlocker = Instantiate(flocker, CreateRandomPoint(), Quaternion.identity);
        //thisFlocker.GetComponentInChildren<TextMesh>().text = flockers.Count + 1 + "";
        
        flockers.Add(thisFlocker);

    }

    public void MakeFlockWanderDirection()
    {
        wanderDirection = flockers[0].GetComponent<Flocker>().Wander();
        Debug.DrawLine(flockCenter, flockCenter + wanderDirection, Color.yellow);

    }

    #region flock management

    /// <summary>
    /// Finds the flock center
    /// </summary>
    public void FindFlockCenter()
    {
        flockCenter = Vector3.zero;

        //flock center is average position of all members of the flock
        foreach (GameObject flocker in flockers)
        {
            flockCenter += flocker.transform.position;
        }
        flockCenter /= flockers.Count;

    }

    /// <summary>
    /// Finds the average alignment of the flock
    /// </summary>
    public void FindFlockAlignment()
    {
        flockDirection = Vector3.zero;

        //find sum of the forward vectors of each member in the flock
        foreach (GameObject flocker in flockers)
        {
            flockDirection += flocker.transform.forward;
        }

        ////to compute desired velocity, normalize the sum and then multiply by max speed
        //desiredVelocity = flockDirection.normalized;
        //desiredVelocity *= maxSpeed;

        ////compute steering force: desired velocity - current velocity
        //return (desiredVelocity - velocity) * alignmentWeight;

    }

    #endregion
}
