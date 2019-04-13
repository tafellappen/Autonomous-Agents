using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Author: Miranda Auriemma
 * Purpose: This script is the abstract class that the human and zombie classes inherit from.
 * it handles both of their start and update methods, forces (including seek and flee), debug lines, and keeps them in bounds
 */
public abstract class Vehicle : MonoBehaviour
{
    enum WanderType
    {
        TwoDimensions,
        ThreeDimensions
    }
    [SerializeField] WanderType type;
    [SerializeField] float heightOffset;
    // Vectors necessary for force-based movement
    protected Vector3 vehiclePosition;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 velocity;


    // Floats
    [SerializeField] float mass;
    //[SerializeField] protected float maxNormalSpeed;
    //[SerializeField] protected float burstSpeed;
    [SerializeField] protected float maxSpeed;

    // Fields for defining world bounds/keeping in bounds
    //public MeshRenderer floor;
    Vector3 worldBounds;
    Vector3 posXNormal;
    Vector3 posZNormal;
    Vector3 negXNormal;
    Vector3 negZNormal;
    [SerializeField] float boundsDistanceBuffer;
    [SerializeField] protected int maxFramesOutOfBounds;
    protected int outOfBoundsCount = 0; //count how many frames this is out of bounds for


    //public bool seeking = true;

    //seeking and fleeing vectors / debug "gameobjects"
    public Vector3 targetLocation { get; set; }
    //public Vector3 futurePosition { get; set; }
    //[SerializeField] int howMuchFuture;
    //[SerializeField] private GameObject futurePositionPrefab;
    //private GameObject futurePosObject;

    //separation
    [SerializeField] protected float personalSpace;
    //protected List<GameObject> humans;
    //protected List<GameObject> zombies;

    [SerializeField] protected CollisionManager collisionManager;
    [SerializeField] protected Manager manager;
    /// <summary>
    /// Value used to ensure that the game object never goes into the floor
    /// </summary>
    public float terrainHeight;

    //materials for debug lines
    [SerializeField] Material forwardCol;
    [SerializeField] Material rightCol;
    [SerializeField] Material targetCol;

    public bool shouldBeDestroyed { get; set; }
    //string seekingStr = "Seeking";

    //obstacle avoidance
    //GameObject[] obstacles;
    [SerializeField] float safeDistance;
    float sqrSafeDistance;
    MeshRenderer vehicleMesh;
    //List<Obstacle> obstacleScripts;

    //wandering fields
    [SerializeField] int wanderWaitTime;
    [SerializeField] float wanderCircleDistance;
    [SerializeField] float wanderCircleRadius;
    [SerializeField] float wanderAngleVariance;
    Vector3 wanderCircleLocation;
    Vector3 wanderVectorRadius;
    Vector3 wanderToPoint;
    float flatWanderAngle;
    float verticalWanderAngle;
    int wanderCountdown;

    //cohesion
    Vector3 flockCenter;
    [SerializeField] float maxCenterDistance;
    [SerializeField] float centroidNoSeekRadius;
    float sqrCentroidNoSeek;

    //alignment
    Vector3 flockAlignment;

    //weights
    [SerializeField] float seekWeight;
    [SerializeField] float fleeWeight;
    [SerializeField] float avoidWeight;
    [SerializeField] float stayInBoundsWeight;
    [SerializeField] float cohesionWeight;
    [SerializeField] float alignmentWeight;
    [SerializeField] float separationWeight;
    //[SerializeField] float wanderWeight;

    //[SerializeField] float maxOutlierDistance;
    protected float sqrMaxOutlierDistance;

    float maxTerrainHeight;
    public Vector3 WanderDirection { get; set; }


    //public bool isABet { get; set; }
    // Use this for initialization
    protected void Start()
    {
        //isABet = false;
        //notify = GameObject.Find("Win or Lose").GetComponent<Text>();
        sqrMaxOutlierDistance = Mathf.Pow(maxCenterDistance, 2);
        sqrCentroidNoSeek = Mathf.Pow(centroidNoSeekRadius, 2);

        //collisionManager = GameObject.Find("Game Manager").GetComponent<CollisionManager>();
        manager = GameObject.Find("Scene Manager").GetComponent<Manager>();

        //this only applies to 2d
        //worldBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //get world coordinates based on the plane's coordinates in the world
        //worldBounds = new Vector3(floor.bounds.max.x, 0, floor.bounds.max.z);
        worldBounds = manager.worldBounds;

        vehiclePosition = transform.position; //initialize vehicle position
        terrainHeight = vehiclePosition.y;
        maxTerrainHeight = manager.terrain.terrainData.size.y;
        //set up obstacle avoidance things
        //obstacles = manager.obstacles; //if the abount of obstacles is going to change, this will have to be updated elsewhere
        sqrSafeDistance = Mathf.Pow(safeDistance, 2);
        vehicleMesh = gameObject.GetComponent<MeshRenderer>();

        //instantiate the future position debug object at the vehicle's location
        //futurePosObject = Instantiate(futurePositionPrefab, vehiclePosition, Quaternion.identity);

        flatWanderAngle = Random.Range(0, 360);
        verticalWanderAngle = Random.Range(0, 360);

        shouldBeDestroyed = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (shouldBeDestroyed)
        {
            manager.flockers.Remove(gameObject);
            Debug.Log("Destroyed - " + manager.flockers.Count + " Left");
            Destroy(gameObject);
        }
        
        CalcSteeringForces();



        //apply velocity
        velocity += acceleration * Time.deltaTime;
        //Vector3.ClampMagnitude(velocity, maxSpeed);
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;    //calculate direction vector
        acceleration = Vector3.zero;        //reset acceleration after applying it
        
        //rotate
        //float targetHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //float targetVerticalAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        //keep vehicle above terrain
        terrainHeight = manager.terrain.SampleHeight(new Vector3(vehiclePosition.x, 0, vehiclePosition.z));

        if (type == WanderType.TwoDimensions || vehiclePosition.y <= terrainHeight)
        {
            vehiclePosition.y = terrainHeight + heightOffset;
        }

        //Debug.DrawLine(vehiclePosition, vehiclePosition + velocity, Color.green);

        //finally, apply everything to the actual gameobject
        //transform.rotation = Quaternion.Euler(targetVerticalAngle, targetHorizontalAngle, 0);
        transform.LookAt(direction + vehiclePosition);
        transform.position = vehiclePosition;
        
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.yellow);

        //define new future position
        //futurePosition = vehiclePosition + (velocity * howMuchFuture) * Time.deltaTime;
        //transform.LookAt(futurePosition);
        //create the game object showing it
        //    if (manager.drawDebug)
        //    {
        //        futurePosObject.transform.position = futurePosition;
        //    }
        //    else
        //    {
        //        // hide it underneath the scene floor. because it will be underneath the scene floor, 
        //        //it doesnt really matther where it goes, so just put it underneath the origin for simplicity
        //        futurePosObject.transform.position = new Vector3(0, -10, 0);
        //    }
    }

    #region debug lines
    private void OnRenderObject()
    {
        if (manager.drawDebug)
        {
            //Debug.Log("MANAGER DEBUG TRUE");
            DrawForwardDebug();
            DrawRightDebug();
            //DrawTargetDebug();
        }
    }

    /// <summary>
    /// Draws the forwards debug line
    /// </summary>
    private void DrawForwardDebug()
    {
        //set the meterial to use for the line
        forwardCol.SetPass(0);

        //draws one line
        GL.Begin(GL.LINES);
        GL.Vertex(vehiclePosition); //first endpoint
        GL.Vertex(vehiclePosition + direction * 4); //second endpoint
        GL.End();
    }

    /// <summary>
    /// Draws the forwards debug line
    /// </summary>
    private void DrawRightDebug()
    {
        //set the meterial to use for the line
        rightCol.SetPass(0);

        //draws one line
        GL.Begin(GL.LINES);
        GL.Vertex(vehiclePosition); //first endpoint
        GL.Vertex(vehiclePosition + transform.right * 10); //second endpoint
        GL.End();

        Debug.DrawLine(vehiclePosition, transform.position + transform.right);
    }

    /// <summary>
    /// Draws the forwards debug line
    /// </summary>
    private void DrawTargetDebug()
    {
        if (manager.flockers.Count >= 1)
        {
            //set the meterial to use for the line
            targetCol.SetPass(0);

            //draws one line
            GL.Begin(GL.LINES);
            GL.Vertex(vehiclePosition); //first endpoint
            GL.Vertex(targetLocation); //second endpoint
            GL.End();
        }
    }

    /// <summary>
    /// Draws the future position debug point
    /// </summary>
    //private void DrawFuturePosition()
    //{
    //    GL.Begin(GL.LINES);



    //}
    #endregion
    //private void OnGUI()
    //{
    //    //change color
    //    GUI.color = Color.black;

    //    //increase text size
    //    GUI.skin.box.fontSize = 20;

    //    //Draw the GUI box with text

    //    GUI.Box(new Rect(10, 10, 150, 50), seekingStr);
    //    Debug.Log(seekingStr);
    //}

    #region bounds

    ///// <summary>
    ///// Defines the bounds of the scene - this may become unnecessary
    ///// actually it probably is
    ///// 
    ///// as long as everything stays an unrotated square
    ///// 
    ///// this method as it stands becomes useless in the event that the world is rotated
    ///// idk why that would happen
    ///// but if it did this would not work
    ///// </summary>
    //public void DefineBounds()
    //{
    //    //This is probably a hack-y way of doing this but i got the short end of the 185 stick so this will have to do for now
    //    //i think ideally, the force applied would be proportionate to how close to the bounds the vehicle is
    //    //that may not be possible with this approach

    //    //define normal vectors of the edges of the plane to be the opposite 
    //    //of the vector from the center of the plane to the edge, by using the bounds
    //    posXNormal = (new Vector3(worldBounds.x, 0, 0) * -1).normalized;
    //    negXNormal = posXNormal * -1;

    //    posZNormal = (new Vector3(0, 0, worldBounds.z) * -1).normalized;
    //    negZNormal = posZNormal * -1;
    //}

    /// <summary>
    /// Keeps the game objects in the scene - this should be called by CalcSteeringForces
    /// </summary>
    public Vector3 StayInBounds()
    {
        if (vehiclePosition.x + boundsDistanceBuffer > worldBounds.x)
        {
            //construct the flee "target" from the vehicle's z position plus the world x bounds
            //because the "floor" is not rotated, the resulting vector is the point closest to the 
            //vehicle position that is on the bounds
            //Debug.Log("pos x bound");
            outOfBoundsCount++;
            return Flee(new Vector3(worldBounds.x, vehiclePosition.y, vehiclePosition.z)) * stayInBoundsWeight;
        }
        else if (vehiclePosition.x - boundsDistanceBuffer < 0)
        {
            //Debug.Log("neg x bound");
            outOfBoundsCount++;
            return Flee(new Vector3(0, vehiclePosition.y, vehiclePosition.z)) * stayInBoundsWeight;

        }
        else if (vehiclePosition.z + boundsDistanceBuffer > worldBounds.z)
        {
            //Debug.Log("pos y bound");
            outOfBoundsCount++;
            return Flee(new Vector3(vehiclePosition.x, vehiclePosition.y, worldBounds.z)) * stayInBoundsWeight;

        }
        else if (vehiclePosition.z - boundsDistanceBuffer < 0)
        {
            //Debug.Log("neg y bound");
            outOfBoundsCount++;
            return Flee(new Vector3(vehiclePosition.x, vehiclePosition.y, 0)) * stayInBoundsWeight;
        }
        else if(vehiclePosition.y + boundsDistanceBuffer > worldBounds.y)
        {           
            outOfBoundsCount++;
            return Flee(new Vector3(vehiclePosition.x, worldBounds.y, vehiclePosition.z)) * stayInBoundsWeight;
        }

        //Debug.Log("no bounds");
        outOfBoundsCount = 0;
        return new Vector3(0, 0, 0);
    }
    #endregion

    #region applying forces
    /// <summary>
    /// ApplyForce
    /// Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// Applies a force of friction to the vehicle
    /// </summary>
    public void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;
    }
    #endregion

    #region base steering forces (seek, flee)
    // SEEK METHOD
    // All Vehicles have the knowledge of how to seek
    // They just may not be calling it all the time
    /// <summary>
    /// Seek
    /// </summary>
    /// <param name="targetPosition">Vector3 position of desired target</param>
    /// <returns>Steering force calculated to seek the desired target</returns>
    public Vector3 Seek(Vector3 targetPosition)
    {
        
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate seeking steering force
        Vector3 seekingForce = (desiredVelocity - velocity) * seekWeight;

        //Debug.DrawLine(vehiclePosition, targetPosition, Color.blue);
        //Debug.DrawLine(vehiclePosition, vehiclePosition + seekingForce, Color.cyan);


        // Step 4: Return force
        return seekingForce;
    }

    /// <summary>
    /// Overloaded Seek
    /// </summary>
    /// <param name="target">GameObject of the target</param>
    /// <returns>Steering force calculated to seek the desired target</returns>
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    /// <summary>
    /// Steering force to go away from somewhere
    /// </summary>
    /// <returns></returns>
    public Vector3 Flee(Vector3 targetPosition)
    {
        //find desired velocity - it's just desired velocity of seek but negated
        Vector3 desiredVelocity = (targetPosition - vehiclePosition) * -1;

        //scale to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        //calculate fleeing steering force
        Vector3 fleeingForce = desiredVelocity - velocity;


        Debug.DrawLine( //create an offset so you can see both the seeking and fleeing debug lines
            new Vector3(vehiclePosition.x, vehiclePosition.y + 1, vehiclePosition.z),
            targetPosition,
            Color.red);
        //Debug.DrawLine(vehiclePosition, vehiclePosition + fleeingForce, Color.magenta);

        return fleeingForce * fleeWeight;
    }


    ///// <summary>
    ///// Directs the vehicle twoards the future position of the target
    ///// </summary>
    //public Vector3 Pursue(GameObject target)
    //{
    //    return Seek(target.GetComponent<Vehicle>().futurePosition);
    //}

    ///// <summary>
    ///// Directs the vehicle away from both the current and future positions of the target
    ///// </summary>
    //public Vector3 Evade(GameObject target)
    //{
    //    //flee both the object's current position and fucture position
    //    Vector3 evadeForce = Flee(target.transform.position);
    //    evadeForce += Flee(target.GetComponent<Vehicle>().futurePosition);
    //    return evadeForce;
    //}

    #endregion

    #region flocking methods (cohesion, alignment, separation)

    /// <summary>
    /// Returns the steering force to the center of the flock
    /// </summary>
    /// <returns></returns>
    public Vector3 Cohesion()
    {
        Vector3 toFlockCenter = manager.flockCenter - vehiclePosition;
        float distance = toFlockCenter.sqrMagnitude;

        if (Vector3.Dot(toFlockCenter, transform.forward) > 0 )
        {
            //if the center is in front of the flocker, seek the center
            return Seek(manager.flockCenter) * cohesionWeight;
        }
        else
        {
            //float tempWeight = 1;
            //if (distance > sqrCentroidNoSeek)
            //{
            //    tempWeight = cohesionWeight / distance;
            //}
            return Seek(manager.flockCenter) / cohesionWeight;
        }
    }

    /// <summary>
    /// Returns the steering force to align the characters in the flock
    /// </summary>
    /// <returns></returns>
    public Vector3 Alignment()
    {
        Vector3 desiredVelocity = Vector3.zero;
        //flockAlignment = Vector3.zero;

        ////find sum of the forward vectors of each member in the flock
        //foreach(GameObject flocker in manager.flockers)
        //{
        //    flockAlignment += flocker.transform.forward;
        //}

        //to compute desired velocity, normalize the sum and then multiply by max speed
        desiredVelocity = manager.flockDirection.normalized;
        desiredVelocity *= maxSpeed;

        //compute steering force: desired velocity - current velocity
        return (desiredVelocity - velocity) * alignmentWeight;

    }

    /// <summary>
    /// Keeps like gameobjects separated (zombies separated from other zombies and humans separated from other humans) functions veeery similar to plain cirlce collision
    /// </summary>
    /// <param name="likeGameObjects">This is the list of gameobjects stored in the collision manager. The child classes already have a reference to this list and pass it in</param>
    public Vector3 KeepSeparated(List<GameObject> likeGameObjects)
    {
        ////make it a list of mesh renderers to make it easier to loop through
        //List<MeshRenderer> likeObjectsMesh = new List<MeshRenderer>();
        //foreach(GameObject obj in likeGameObjects)
        //{
        //    likeObjectsMesh.Add(obj.GetComponent<MeshRenderer>());
        //}
        ////loop through each gameobject and check its bounds

        Vector3 separatingForce = Vector3.zero;
        float distance;
        float tempWeight = separationWeight;

        if (likeGameObjects == null)
        {
            //Debug.Log("list null");
            return separatingForce;
        }

        foreach (GameObject obj in likeGameObjects)
        {
            if (obj == null)
            {
                //Debug.Log("empty list");
                return separatingForce;
            }
            //get distance between each gameobject and the vehicle
            distance = (vehiclePosition - obj.transform.position).sqrMagnitude;
            if (distance > 0 && distance < Mathf.Pow(personalSpace, 2))
            {
                //https://www.mathsisfun.com/algebra/directly-inversely-proportional.html
                //"y is inversely proportional to x" is the same thing as "y is directly proportional to 1/x"
                //so, because this only runs if the other thing is within the threshold for separation, just divide separationWeight by the distance
                tempWeight = separationWeight / ((distance / personalSpace) * 100);
                separatingForce += Flee(obj.transform.position);
            }

        }

        return separatingForce * separationWeight;

        //if it is within it's "personal space", flee
    }

    #endregion

    #region wandering

    /// <summary>
    /// Calls methods for wandering based on wether the wander type is 2 or 3 dimensional
    /// </summary>
    /// <returns>Seeking force to the "wander to point"</returns>
    public Vector3 Wander()
    {
        if(type == WanderType.TwoDimensions)
        {
            WanderDirection = Wander2();
        }
        else
        {
            WanderDirection = SetFlockWander3();
        }

        return WanderDirection;
    }
    ////////public void FindFlockWander()
    ////////{
    ////////    wanderDirection = Vector3.zero;

    ////////    //find sum of the wander vectors of each member in the flock
    ////////    foreach (GameObject flocker in flockers)
    ////////    {
    ////////        wanderDirection += flocker.GetComponent<Flocker>().WanderDirection;
    ////////    }

    ////////    Debug.DrawLine(flockCenter, flockCenter + wanderDirection, Color.yellow);

    ////////}

    ////////public void MakeFlockWanderDirection()
    ////////{
    ////////    wanderDirection = flocker.GetComponent<Flocker>().Wander();
    ////////}

    /// <summary>
    /// Makes the vehicle wander randomly in 2 dimensions
    /// </summary>
    public Vector3 Wander2()
    {
        if (wanderCountdown > 0)
        {
            wanderCountdown--;
        }
        else
        {
            flatWanderAngle += Random.Range(-wanderAngleVariance, wanderAngleVariance); //change the wander angle by a random amount within a range
            wanderCountdown = wanderWaitTime;
        }

        //get location of projected circle by adding the direction multiplied by the distance of the wander circle
        wanderCircleLocation = vehiclePosition + (direction * wanderCircleDistance);
        //get the radius as a vector by multiplying direction (a normalized vector) by the set radius of the circle
        //wanderVectorRadius = direction * wanderCircleRadius; 
        //wanderToPoint = wanderCircleLocation + (Quaternion.Euler(0, 0, wanderAngle) * wanderVectorRadius);

        //wanderToPoint = new Vector3(
        //    wanderCircleLocation.x + Mathf.Cos(wanderAngle) * wanderCircleRadius,
        //    0,
        //    wanderCircleLocation.z + Mathf.Sin(wanderAngle) * wanderCircleRadius
        //    );

        wanderToPoint = wanderCircleLocation + ((Quaternion.AngleAxis(flatWanderAngle, Vector3.up) * transform.right) * wanderCircleRadius);

        //set as new target
        targetLocation = wanderToPoint;

        Debug.DrawLine(wanderCircleLocation, wanderToPoint, Color.gray);
        //Debug.Log("wandering " + wanderToPoint);
        //float mousePosAngle = Mathf.Atan2(mouseWorldPos.x, mouseWorldPos.y) * Mathf.Rad2Deg;
        return Seek(wanderToPoint) * seekWeight;
        //return Vector3.zero;
        //targetLocation = wanderToPoint;
    }

    /// <summary>
    /// Makes the vehicle wander randomly in 3 dimensions
    /// </summary>
    public Vector3 Wander3()
    {
        //get location of projected circle by adding the direction multiplied by the distance of the wander circle
        wanderCircleLocation = vehiclePosition + (direction * wanderCircleDistance);
        Debug.DrawLine(vehiclePosition, wanderCircleLocation, Color.cyan);

        wanderToPoint = wanderCircleLocation + manager.wanderDirection;

        //set as new target
        targetLocation = wanderToPoint;

        Debug.DrawLine(wanderCircleLocation, wanderToPoint, Color.gray);
        //Debug.Log("wandering " + wanderToPoint);
        //float mousePosAngle = Mathf.Atan2(mouseWorldPos.x, mouseWorldPos.y) * Mathf.Rad2Deg;
        return Seek(wanderToPoint) * seekWeight;
        //return Vector3.zero;
        //targetLocation = wanderToPoint;
    }

    public Vector3 SetFlockWander3()
    {
        if (wanderCountdown > 0)
        {
            wanderCountdown--;
        }
        else
        {
            flatWanderAngle += Random.Range(-wanderAngleVariance, wanderAngleVariance); //change the wander angle by a random amount within a range
            verticalWanderAngle += Random.Range(-wanderAngleVariance, wanderAngleVariance); //change the wander angle by a random amount within a range
            wanderCountdown = wanderWaitTime;
        }

        //get location of projected circle by adding the direction multiplied by the distance of the wander circle
        wanderCircleLocation = vehiclePosition + (direction * wanderCircleDistance);

        //get the radius as a vector by multiplying direction (a normalized vector) by the set radius of the circle
        //wanderVectorRadius = direction * wanderCircleRadius; 
        //wanderToPoint = wanderCircleLocation + (Quaternion.Euler(0, 0, wanderAngle) * wanderVectorRadius);

        //wanderToPoint = new Vector3(
        //    wanderCircleLocation.x + Mathf.Cos(wanderAngle) * wanderCircleRadius,
        //    0,
        //    wanderCircleLocation.z + Mathf.Sin(wanderAngle) * wanderCircleRadius
        //    );

        Vector3 WanderDirection =
            (Quaternion.AngleAxis(flatWanderAngle, Vector3.up)
            * (Quaternion.AngleAxis(verticalWanderAngle, Vector3.forward) * transform.right)
            * wanderCircleRadius);

        //wanderToPoint = wanderCircleLocation + WanderDirection;

        //wanderToPoint =
        //    wanderCircleLocation
        //    + (Quaternion.AngleAxis(flatWanderAngle, Vector3.up)
        //    * (Quaternion.AngleAxis(verticalWanderAngle, Vector3.forward) * transform.right)
        //    * wanderCircleRadius);

        //* ((Quaternion.AngleAxis(verticalWanderAngle, Vector3.up) * transform.right) 

        //set as new target
        targetLocation = wanderToPoint;
        Debug.DrawLine(wanderCircleLocation, transform.position, Color.red);
        Debug.DrawLine(wanderCircleLocation, wanderToPoint, Color.gray);
        Debug.DrawLine(transform.position, wanderToPoint, Color.cyan);
        //Debug.Log("wandering " + wanderToPoint);
        //float mousePosAngle = Mathf.Atan2(mouseWorldPos.x, mouseWorldPos.y) * Mathf.Rad2Deg;
        //return Seek(wanderToPoint) * seekWeight;
        //return Vector3.zero;
        //targetLocation = wanderToPoint;
        return WanderDirection;
    }

    /// <summary>
    /// "wanders" in the same direction as the flock
    /// </summary>
    public Vector3 WanderWithFlock()
    {
        //get location of projected circle by adding the direction multiplied by the distance of the wander circle
        wanderCircleLocation = vehiclePosition + (direction * wanderCircleDistance);
        Debug.DrawLine(vehiclePosition, wanderCircleLocation, Color.cyan);

        wanderToPoint = wanderCircleLocation + manager.wanderDirection;

        //set as new target
        targetLocation = wanderToPoint;

        Debug.DrawLine(wanderCircleLocation, wanderToPoint, Color.gray);
        //Debug.Log("wandering " + wanderToPoint);
        //float mousePosAngle = Mathf.Atan2(mouseWorldPos.x, mouseWorldPos.y) * Mathf.Rad2Deg;
        return Seek(wanderToPoint) * seekWeight;
        //return Vector3.zero;
        //targetLocation = wanderToPoint;
    }

    #endregion

    #region other forces (obstacle avoidance)
    /// <summary>
    /// Calcuates forces to avoid obstacles in the scene
    /// </summary>
    /// <returns></returns>
    public Vector3 ObstacleAvoidance() //maybe pass in the seek vector to check against
    {
        Vector3 desiredVelocity = Vector3.zero;

        //debug lines showing the "safe" area in front of the vehicle
        //Vector3 safeEnd = Vector3.ClampMagnitude(velocity * 10, safeDistance);
        //Vector3 vehicleEdge1 = new Vector3(vehiclePosition-)
        //Debug.DrawLine(vehicleMesh.bounds.max.x, vehiclePosition + safeEnd , Color.magenta);

        //make sure the obstacles in the scene have been properly added to the list we are checking
        if (manager.obstacles == null || manager.obstacles.Count <= 0)
        {
            return desiredVelocity;
        }

        //iterate through list of obstacles
        foreach (GameObject obstacle in manager.obstacles)
        {
            //Debug.DrawLine(obstacle.transform.position, new Vector3(obstacle.transform.position.x, obstacle.transform.position.y + 1000, obstacle.transform.position.z), Color.yellow);
            Vector3 distance = obstacle.transform.position - vehiclePosition;            

            //check forward or behind, by checking dot product of distance and this flocker's forward vector
            if (Vector3.Dot(distance, transform.forward) > 0)
            {
                //if its forward
                //check distance
                if(distance.sqrMagnitude < sqrSafeDistance)
                {
                    //clean up next if statement
                    float dotProduct = Vector3.Dot(transform.right, distance);
                    Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();

                    //obstacle has not been ruled out yet, check absolute value of dot product (of right vector and distance vector) against the extents of the vehicle and the obstacle
                    //(essentially projecting a "shadow" of the obstacle in the direction perpendicular to the vehicle's right hand vector.)
                    //(If the shadow "lands" on the vehicle, then the vehicle will collide with the obstacle if it does not change course.)
                    if(Mathf.Abs(dotProduct) < vehicleMesh.bounds.extents.x + obstacleScript.radius)
                    {
                        Debug.DrawLine(vehiclePosition, obstacle.transform.position, Color.red);
                        Debug.DrawLine(obstacle.transform.position, new Vector3(obstacle.transform.position.x, obstacle.transform.position.y + 100, obstacle.transform.position.z), Color.red);
                        //if it's greater than 0, its on the right (also for exactly forward)
                        //(so move left)
                        if (dotProduct >= 0)
                        {
                            
                            desiredVelocity += (maxSpeed * -transform.right);
                        }
                        else //if it's less than 0, its on the left (so move right)
                        {
                            desiredVelocity += (maxSpeed * transform.right);
                        }


                    }
                    else
                    {
                        Debug.DrawLine(vehiclePosition, obstacle.transform.position);

                    }
                }
            }
        }

        return desiredVelocity * avoidWeight - velocity;
    }

    #endregion


    #region abstract methods
    /// <summary>
    /// Abstract method for calculating the steering forces on the child vehicles
    /// </summary>
    public abstract void CalcSteeringForces();
    #endregion

}
