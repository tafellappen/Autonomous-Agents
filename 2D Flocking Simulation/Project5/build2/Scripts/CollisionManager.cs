using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Miranda Auriemma
 * Purpose: This script handles collision detection and turns humans into zombies when they collide
 */
public class CollisionManager : MonoBehaviour
{

    //bool colliding = false;
    //public SpriteRenderer player;
    //[SerializeField] GameObject human;
    //[SerializeField] GameObject zombie;

    //public List<GameObject> humans;
    //public List<GameObject> zombies;
    //private List<GameObject> playerCollidingWith = new List<GameObject>();

    //private GUIManager guiManager;

    //public GameObject powerup;
    // private SpriteRenderer powerupHolder; //i only want one powerup in at a time, therefore no list

    public GameObject human { get; set; }
    public GameObject zombie { get; set; }

    private List<GameObject> newZombies;

    public List<GameObject> humans { get; set; }
    public List<GameObject> zombies { get; set; }


    public enum CollisionType
    {
        AABB,
        BoundingCircle
    }
    public CollisionType currentCollisionType;

    Manager manager;
    private Vector3 worldBounds;
    private float worldXBound;
    private float worldYBound;

    // Use this for initialization
    void Start()
    {
        manager = GetComponent<Manager>();
        worldBounds = manager.worldBounds;
        worldXBound = worldBounds.x;
        worldYBound = worldBounds.y;


        newZombies = new List<GameObject>();
        //guiManager = gameObject.GetComponent<GUIManager>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject human in humans)
        {
            manager.MovePSG(human);
        }

        CheckCollisions();
        //if (gameObject.GetComponent<GUIManager>().gameMode == GUIManager.GameMode.playing)
        //{
        //    CheckCollisions();
        //    SpawnPowerup();
        //}
        //else
        //{
        //    if (asteroids != null)
        //    {
        //        foreach (SpriteRenderer aster in asteroids)
        //        {
        //            aster.GetComponent<AsteroidBehavior>().shouldDespawn = true;
        //            aster.GetComponent<AsteroidBehavior>().isGameOver = true;
        //        }
        //    }
        //}
    }

    private void CheckCollisions()
    {
        //collision plan: each zombie has a field holding the closest human
        //collision script checks zombie's location against that 
        //human's location. 

        foreach (GameObject newZomb in newZombies)
        {
            zombies.Add(newZomb);
        }

        newZombies.Clear();

        foreach (GameObject zombie in zombies)
        {
            foreach (GameObject human in humans)
            {

                switch (currentCollisionType)
                {
                    case CollisionType.AABB:

                        if (AABBCollision(human, zombie) && !human.GetComponent<Human>().shouldBeDestroyed)
                        {
                            HumanToZombie(human);
                            Debug.Log("HE DED");
                        }

                        break;

                    case CollisionType.BoundingCircle:
                        // CircleCollision(planet);
                        if (CircleCollision(human, zombie) && !human.GetComponent<Human>().shouldBeDestroyed)
                        {
                            HumanToZombie(human);
                            Debug.Log("HE DED");
                        }
                        else
                        {
                            //aster.color = Color.white;
                            // player.color = Color.white;
                        }
                        break;

                    default:
                        break;

                }
            }
        }

        //BulletCollision();
        //PowerupCollision();

    }
    /// <summary>
    /// Checks collision via AABB
    /// </summary>
    /// <param name="human">the non player object to check the collision with</param>
    /// <returns>whether or not a collision is detected</returns>
    bool AABBCollision(GameObject human, GameObject zombie)
    {
        //convert game objects to mesh renederers so its easier
        MeshRenderer humanMesh = human.GetComponent<MeshRenderer>();
        MeshRenderer zombieMesh = zombie.GetComponent<MeshRenderer>();

        //check player max x vs planets min x                  and player min x vs planets max x
        if (humanMesh.bounds.max.x > zombieMesh.bounds.min.x && zombieMesh.bounds.min.x < humanMesh.bounds.max.x)
        {
            //so, if the x shows that the player may collide, check the y
            if (humanMesh.bounds.max.z > zombieMesh.bounds.min.z && humanMesh.bounds.min.z < zombieMesh.bounds.max.z)
            {
                //convert humans to zombies
                zombies.Add(human);
                return true;
            }
        }

        //code will only get here if there is no collision
        //if (playerCollidingWith.Contains(asteroid))
        //    {
        //        playerCollidingWith.Remove(asteroid);
        //    }

        return false;

    }

    /// <summary>
    /// Checks collision via circles
    /// </summary>
    /// <param name="human"></param>
    /// <returns></returns>
    bool CircleCollision(GameObject human, GameObject zombie)
    {
        //Debug.Log("circle collision!");
        //convert game objects to mesh renederers so its easier
        MeshRenderer humanMesh = human.GetComponent<MeshRenderer>();
        MeshRenderer zombieMesh = zombie.GetComponent<MeshRenderer>();

        Vector3 humanPosition = humanMesh.gameObject.transform.position;
        Vector3 zombiePosition = zombieMesh.transform.position;

        //get the distance between the player position and the planet position
        Vector3 distanceBetween = humanPosition - zombiePosition;

        //get radii based on x values of position and bounds - this is arbitrary, the x just works as a good point
        float humanCircleRadius = humanMesh.bounds.max.x - humanPosition.x;
        float zombieCircleRadius = zombieMesh.bounds.max.x - zombiePosition.x;

        //float 
        //Debug.Log(distanceBetween.sqrMagnitude);
        //Debug.Log(Mathf.Pow((humanCircleRadius + zombieCircleRadius), 2));
        if (distanceBetween.sqrMagnitude < Mathf.Pow((humanCircleRadius + zombieCircleRadius), 2))
        {
            //playerCollidingWith.Add(human);
            return true;

        }


        return false;
    }

    /// <summary>
    /// Turns the specified human into a zombie
    /// </summary>
    /// <param name="newZombie"></param>
    public void HumanToZombie(GameObject newZombie)
    {
        //destroy the human and spawn a zombie in it's location
        //remove from human list and add to zombie list
        //humans.Remove(newZombie);


        newZombies.Add(Instantiate(zombie, newZombie.transform.position, Quaternion.identity));

        newZombie.GetComponent<Human>().shouldBeDestroyed = true;

        

    }

}
