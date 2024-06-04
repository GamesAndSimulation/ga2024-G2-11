using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Unity.AI.Navigation;

public class CreateBuilding : MonoBehaviour
{

    private int rotationOffset = 90;
    
    // ----------- Constants -----------
    // Walls and other similar stuff
    private const int SimpleWall = 0;
    private const int WindowWall = 1;
    private const int Door = 2;
    private const int Corner = 3;
    private const int Shadow = 4;
    // Roofs
    private const int SimpleRoof = 5;
    private const int SideRoof = 6;
    private const int CornerRoof = 7;
    
    // ----------- Constant Measures -----------
    private const int WallLength = 2;
    private const float WallHeight = 3.5f;
    
    // ----------- Assets -----------
    // Walls
    public GameObject simpleWall;
    public GameObject windowWall;
    public GameObject door;
    public GameObject corner;
    public GameObject shadow;
    // Roofs
    public GameObject simpleRoof;
    public GameObject sideRoof;
    public GameObject cornerRoof;

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
    
    
    void Start()
    {
        // If no initial position specified, use zero, otherwise, use the object position
        specifiedObjectPosition = !useSpecifiedStartingPosition ? Vector3.zero : specifiedObject.transform.position;

        assets = new[] { simpleWall, windowWall, door, corner, shadow, simpleRoof, sideRoof, cornerRoof };
        
        Build();
        RepositionBuilding();
        GenerateObstacles();
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
                Vector3 rotation = new Vector3(0, (x + z + 2) * 90 + 180, 0);
                
                // Set the rotation for the "special corner"
                if (z == 0 && x == 1)
                    rotation = new Vector3(0, 90 + 180, 0);
            
                // Create the object instance and set its parent to the specified one
                GameObject myInstance = Instantiate(corner, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }

        // ----------- Front & Back facade (x axis) -----------
        int doorPosition = xWidth / 2 + xWidth % 2;
        int offset = doorPosition % 2;
        
        for (var x = WallLength; x < xWidth; x += WallLength)
        {
            
            // ----------- Front facade -----------
            GameObject instance = GetFortressComponent(true, 0);
            Vector3 position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z - 0.689f);
            Vector3 rotation = new Vector3(0, 0, 0);

            GameObject myInstance;
            
            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Front && ((xWidth / 2 - WallLength) <= x && x <= (xWidth / 2 + WallLength))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Door
            if (selectedEntrance == EntrancePositionSelector.Front && x == doorPosition + offset)
            {
                // Position the door
                instance = GetFortressComponent(false, Door);
                position = new Vector3(specifiedObjectPosition.x + x - WallLength - 0.069458f, specifiedObjectPosition.y - 0.2584741f, specifiedObjectPosition.z  - 0.689f);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Door
            /*if(selectedEntrance == EntrancePositionSelector.Front && x == xWidth/2)
            {
                // Position a random wall besides the door
                instance = GetFortressComponent(true, 0);
                position = new Vector3(specifiedObjectPosition.x + x - WallLength, specifiedObjectPosition.y, specifiedObjectPosition.z - 0.689f);

                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
                
                // Position the door
                instance = GetFortressComponent(false, Door);
                position = new Vector3(specifiedObjectPosition.x + x - 0.069458f, specifiedObjectPosition.y - 0.2584741f, specifiedObjectPosition.z  - 0.689f);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }*/
            
            // ----------- Back facade -----------
            instance = GetFortressComponent(true, 0);
            position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z + zLength + 0.689f);
            rotation = new Vector3(0, 0, 0);
            
            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Back &&
                  ((xWidth / 2 - WallLength) <= x && x <= (xWidth / 2 + WallLength))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Door
            if(selectedEntrance == EntrancePositionSelector.Back && x == xWidth/2)
            {
                // Position a random wall besides the door
                instance = GetFortressComponent(true, 0);
                position = new Vector3(specifiedObjectPosition.x + x - WallLength, specifiedObjectPosition.y, specifiedObjectPosition.z + zLength + 0.689f);

                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
                
                // Position the door
                instance = GetFortressComponent(false, Door);
                position = new Vector3(specifiedObjectPosition.x + x - 0.069458f, specifiedObjectPosition.y - 0.2584741f, specifiedObjectPosition.z + zLength + 0.689f);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }

        
        // ----------- Lateral facade (z axis) -----------
        
        // Right now, the solution works for situations where the zLength/2 gives us a par number
        doorPosition = zLength / 2 + zLength % 2;
        offset = doorPosition % 2;
        
        for (var z = WallLength; z < zLength; z += WallLength)
        {
            // ----------- First facade -----------
            GameObject instance = GetFortressComponent(true, 0);
            Vector3 position = new Vector3(specifiedObjectPosition.x - 0.689f, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            Vector3 rotation = new Vector3(0, 90, 0);
            
            GameObject myInstance;
            
            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Right &&
                  ((zLength / 2 - WallLength) <= z && z <= (zLength / 2 + WallLength))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Door
            if (selectedEntrance == EntrancePositionSelector.Right && z == doorPosition + offset)
            {
                // Position the door
                instance = GetFortressComponent(false, Door);
                position = new Vector3(specifiedObjectPosition.x - 0.689f, specifiedObjectPosition.y - 0.2584741f, specifiedObjectPosition.z + z + 0.069458f);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }

            // ----------- Second facade -----------
            instance = GetFortressComponent(true, 0);
            rotation = new Vector3(0, 90, 0);
            position = new Vector3(specifiedObjectPosition.x + xWidth + 0.689f, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            
            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Left &&
                  ((zLength / 2 - WallLength) <= z && z <= (zLength / 2 + WallLength))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Door
            if (selectedEntrance == EntrancePositionSelector.Left && z == doorPosition + offset)
            {
                // Position the door
                instance = GetFortressComponent(false, Door);
                position = new Vector3(specifiedObjectPosition.x + xWidth + 0.689f, specifiedObjectPosition.y - 0.2584741f, specifiedObjectPosition.z + z + 0.069458f);
                
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }
    }

    private GameObject GetFortressComponent(bool isRandom, int assetNum)
    {

        // Set idx to the provided asset number for cases where we don't need to generate a random one
        int idx = assetNum;
        // Array of possible structures that can be generated
        int[] possibleRandomStructs = { SimpleWall, WindowWall };

        // If isRandom is set to true, get one of the indexes from possibleRandomStructs
        if (isRandom)
            idx = possibleRandomStructs[Random.Range(0, possibleRandomStructs.Length)];
            
        return assets[idx]; 
    }

    private void RepositionBuilding()
    {
        float yOffset = 4;
        specifiedObject.transform.position += new Vector3(0, yOffset, 0);
    }

    private void GenerateObstacles()
    {
        /*float securityOffset = 8;       // Distance between the obstacles and the borders of the fortress
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
            
        */
        
        
    }

    private void GenerateEnemies()
    {
        
        /*float securityOffset = 8;       // Distance between the obstacles and the borders of the fortress
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
            myInstance.transform.SetParent(specifiedObject.transform);

        }*/
        
    }
    
}

