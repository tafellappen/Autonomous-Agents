using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a Perlin noise-based heightmap
// Placed on Terrain Object

public class PerlinTerrainGen : MonoBehaviour 
{

	private TerrainData myTerrainData;
	[SerializeField] Vector3 worldSize;
	[SerializeField] int resolution = 129;			// number of vertices along X and Z axes
    [SerializeField] float timeStep = 1;
	float[,] heightArray;


	void Start () 
	{
		myTerrainData = gameObject.GetComponent<TerrainCollider> ().terrainData;
        //worldSize = new Vector3 (200, 50, 200);
        //myTerrainData.size = worldSize;
        worldSize = myTerrainData.size;
		myTerrainData.heightmapResolution = resolution;
		heightArray = new float[resolution, resolution];

		Perlin();

		// Assign values from heightArray into the terrain object's heightmap
		myTerrainData.SetHeights (0, 0, heightArray);
	}
	

	void Update () 
	{
		
	}

    /// <summary>
    /// Assigns heightsArray values using Perlin noise
    /// </summary>
    void Perlin()
	{
        //float interval = 0.007f;
        // Fill heightArray with Perlin-based values
        
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float perlin = Mathf.PerlinNoise(6f / resolution * i, 6f / resolution * j);
                heightArray[i, j] = perlin;
            }
        }
    }
}
