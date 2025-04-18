using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class CreateOutpost : MonoBehaviour
{
    public GameObject enemy;
    public GameObject hallway; // Add this line to define the hallway prefab

    // ----------- Constants -----------
    // Walls
    private const int SimpleWall = 0;
    private const int StrongWall = 1;
    private const int StrongWallDoor = 2;
    // Corners
    private const int SimpleCorner = 3;
    private const int StrongCorner = 4;
    // Wall End
    private const int StrongEnd = 5;
    // Tower
    private const int Tower = 6;
    private const int DestructibleWall = 7;
    
    private Vector3 puzzlePosition;

    // ----------- Constant Measures -----------
    private const int SimpleWallLength = 4;
    private const int StrongWallLength = 6;
    private const int WallsHeight = 10;

    // ----------- Assets -----------
    // Walls
    public GameObject simpleWall;
    public GameObject strongWall;
    public GameObject strongWallDoor;
    public GameObject destructibleWall;
    // Corners
    public GameObject simpleCorner;
    public GameObject strongCorner;
    // Wall End
    public GameObject strongEnd;
    // Tower
    public GameObject tower;
    public GameObject PuzzleHallway;
    public float puzzleRotX;
    public float puzzleRotY;
    public float puzzleRotZ;

    // NavMeshSurface
    public GameObject navMeshSurfacePrefab;

    // ----------- Measures -----------
    public int xWidth;
    public int zLength;

    // ----------- Entrance -----------
    public enum EntrancePositionSelector { Front, Back, Left, Right };
    public EntrancePositionSelector selectedEntrance = EntrancePositionSelector.Front;

    // ----------- Starting Point -----------
    public bool useSpecifiedStartingPosition;
    public GameObject specifiedObject; // probably an empty one
    public Vector3 specifiedObjectPosition;

    private GameObject[] assets;

    private float heightOffset = 0;
    private float rotationOffset = 0;

    [Range(0, 10)]
    public int numberOfObstacles = 5;

    private NavMeshSurface navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        enemy.SetActive(true);
        // If no initial position specified, use zero, otherwise, use the object position
        specifiedObjectPosition = !useSpecifiedStartingPosition ? Vector3.zero : specifiedObject.transform.position;

        assets = new[] { simpleWall, strongWall, strongWallDoor, simpleCorner, strongCorner, strongEnd, tower, destructibleWall };

        Build();
        RepositionBuilding();
        CreateNavMeshSurface();
        GenerateObstacles();
        navMeshSurface.BuildNavMesh(); // Bake the NavMeshSurface
        GenerateEnemies();
    }

    private void Build()
    {
        // ----------- Position Corners -----------
        for (var x = 0; x < 2; x += 1)
        {
            for (var z = 0; z < 2; z += 1)
            {
                // Calculate position and rotation
                Vector3 position = new Vector3(specifiedObjectPosition.x + x * xWidth, specifiedObjectPosition.y, specifiedObjectPosition.z + z * zLength);
                Vector3 rotation = new Vector3(0, (x + z + 2) * 90, 0);

                // Set the rotation for the "special corner"
                if (z == 0 && x == 1)
                    rotation = new Vector3(0, 90, 0);

                // Create the object instance and set its parent to the specified one
                GameObject myInstance = Instantiate(strongCorner, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }

        // ----------- Front & Back facade (x axis) -----------
        for (var x = StrongWallLength; x < xWidth; x += StrongWallLength)
        {
            // ----------- Front facade -----------
            GameObject instance = GetFortressComponent(true, 0);
            Vector3 position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z);
            Vector3 rotation = new Vector3(rotationOffset, 90, 0);

            GameObject myInstance;

            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Front &&
                  ((xWidth / 2 - 6) <= x && x <= (xWidth / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }

            // ----------- Back facade -----------
            instance = GetFortressComponent(true, 0);
            position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + zLength);
            rotation = new Vector3(rotationOffset, 90, 0);

            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Back &&
                  ((xWidth / 2 - 6) <= x && x <= (xWidth / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }

        // ----------- Lateral facade (z axis) -----------
        for (var z = StrongWallLength; z < zLength; z += StrongWallLength)
        {
            GameObject instance;
            GameObject myInstance;
            Vector3 position;
            Vector3 rotation;

            if (z == StrongWallLength || z == StrongWallLength * 2 || z == StrongWallLength * 3) 
            {
                if(z == StrongWallLength)
                    puzzlePosition = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
                //Spawn breakable wall
                instance = GetFortressComponent(false, DestructibleWall);
                position = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + z);
                rotation = new Vector3(rotationOffset, 0, 0);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);

                continue;
            }
            // ----------- First facade -----------
            instance = GetFortressComponent(true, 0);
            position = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + z);
            rotation = new Vector3(rotationOffset, 0, 0);

            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Left &&
                  ((zLength / 2 - 6) <= z && z <= (zLength / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }

            // ----------- Second facade -----------
            instance = GetFortressComponent(true, 0);
            rotation = new Vector3(rotationOffset, 0, 0);
            position = new Vector3(specifiedObjectPosition.x + xWidth, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + z);

            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Right &&
                  ((zLength / 2 - 6) <= z && z <= (zLength / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }
        
        Instantiate(PuzzleHallway, puzzlePosition, Quaternion.Euler(puzzleRotX, puzzleRotY, puzzleRotZ), specifiedObject.transform);
        
    }

    private GameObject GetFortressComponent(bool isRandom, int assetNum)
    {
        // Reset the offset variables to zero
        heightOffset = 0;
        rotationOffset = 0;
        // Set idx to the provided asset number for cases where we don't need to generate a random one
        int idx = assetNum;
        // Array of possible structures that can be generated
        int[] possibleRandomStructs = { SimpleWall, StrongWall, StrongWallDoor, Tower, DestructibleWall };

        // If isRandom is set to true, get one of the indexes from possibleRandomStructs
        if (isRandom)
            idx = possibleRandomStructs[Random.Range(0, possibleRandomStructs.Length)];

        // Calculate the offset variables according to the type of asset selected
        if (idx == Tower) heightOffset = 3.028076f;
        if (idx == SimpleWall) heightOffset = -0.7219238f;
        if (idx == DestructibleWall) { rotationOffset = 90; heightOffset = -0.3112183f; }

        return assets[idx];
    }

    private void RepositionBuilding()
    {
        float yOffset = 4;
        specifiedObject.transform.position += new Vector3(0, yOffset, 0);
    }

    private void CreateNavMeshSurface()
    {
        // Instantiate the NavMeshSurface prefab
        GameObject navMeshSurfaceObject = Instantiate(navMeshSurfacePrefab, specifiedObject.transform);
        
        // Position it to cover the outpost area
        navMeshSurfaceObject.transform.position = new Vector3(specifiedObjectPosition.x + xWidth / 2, specifiedObjectPosition.y, specifiedObjectPosition.z + zLength / 2);
        
        // Scale it to cover the outpost area plus a little extra
        navMeshSurfaceObject.transform.localScale = new Vector3(xWidth + 10, 1, zLength + 10);
        
        // Get the NavMeshSurface component
        navMeshSurface = navMeshSurfaceObject.GetComponent<NavMeshSurface>();
        
        // Build the NavMesh
        navMeshSurface.BuildNavMesh();
    }

    private void GenerateObstacles()
    {
        float securityOffset = 8;       // Distance between the obstacles and the borders of the fortress
        float yPos = 0;                 // Initialization of the yPosition
        Vector3 instancePos = default;  // Initialization of a dummy position vector

        // While we haven't placed all the objects
        for (int i = 0; i < numberOfObstacles; i++)
        {
            var foundValid = false;
            // Search for a valid position
            while (!foundValid)
            {
                // Generate random x and z coordinates
                var xPos = Random.Range(specifiedObjectPosition.x + securityOffset,
                    specifiedObjectPosition.x + xWidth - securityOffset);
                var zPos = Random.Range(specifiedObjectPosition.z + securityOffset,
                    specifiedObjectPosition.z + zLength - securityOffset);

                instancePos = new Vector3(xPos, 100, zPos);

                // Raycast to see if there is anything on the ground or if the obstacle can be placed
                RaycastHit hit;
                if (Physics.Raycast(instancePos, transform.TransformDirection(Vector3.down), out hit, 100f,
                        1 << LayerMask.NameToLayer("Ground")))
                {
                    yPos = hit.point.y;
                    foundValid = true;
                }

            }
            // Get the destructible wall instance
            GameObject instance = GetFortressComponent(false, DestructibleWall);

            // Generate a random rotation and a position with the values from before
            var rotation = new Vector3(0, Random.Range(0, 360), 0);
            var position = new Vector3(instancePos.x, yPos + 2, instancePos.z);

            // Create the instance and set its parent as the specified object
            var myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);

        }
    }

    private void GenerateEnemies()
{
    float securityOffset = 8;       // Distance between the obstacles and the borders of the fortress
    float yPos = 0;                 // Initialization of the yPosition
    Vector3 instancePos = default;  // Initialization of a dummy position vector

    // While we haven't placed all the objects
    for (int i = 0; i < numberOfObstacles; i++)
    {
        var foundValid = false;
        // Search for a valid position
        while (!foundValid)
        {
            // Generate random x and z coordinates
            var xPos = Random.Range(specifiedObjectPosition.x + securityOffset,
                specifiedObjectPosition.x + xWidth - securityOffset);
            var zPos = Random.Range(specifiedObjectPosition.z + securityOffset,
                specifiedObjectPosition.z + zLength - securityOffset);

            instancePos = new Vector3(xPos, 100, zPos);

            // Raycast to see if there is anything on the ground or if the obstacle can be placed
            RaycastHit hit;
            if (Physics.Raycast(instancePos, transform.TransformDirection(Vector3.down), out hit, 100f,
                    1 << LayerMask.NameToLayer("Ground")))
            {
                yPos = hit.point.y;
                foundValid = true;
            }

        }

        // Generate a random rotation and a position with the values from before
        var rotation = new Vector3(0, Random.Range(0, 360), 0);
        var position = new Vector3(instancePos.x, yPos, instancePos.z);

        // Create the instance and set its parent as the specified object
        var myInstance = Instantiate(enemy, position, Quaternion.Euler(rotation), specifiedObject.transform);
        myInstance.GetComponent<NavMeshAgent>().enabled = false; // Disable the NavMeshAgent before positioning
        myInstance.transform.SetParent(specifiedObject.transform);
        myInstance.GetComponent<NavMeshAgent>().enabled = true; // Enable the NavMeshAgent after positioning
    }
    enemy.SetActive(false);
    
}


    
}
