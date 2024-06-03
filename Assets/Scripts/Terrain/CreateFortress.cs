using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CreateOutpost : MonoBehaviour
{
  
    Terrain[] _terrains;
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        _terrains = Terrain.activeTerrains;
        
        // if no initial position specified, use zero, otherwise, use the object position
        if (!useSpecifiedStartingPosition)
            specifiedObjectPosition = Vector3.zero;
        else
            specifiedObjectPosition = specifiedObject.transform.position;

        assets = new GameObject[]
            { simpleWall, strongWall, strongWallDoor, simpleCorner, strongCorner, strongEnd, tower, destructibleWall };

        
        Build();
        RepositionBuilding();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void Build()
    {
        
        // Position corners
        for (var x = 0; x < 2; x += 1)
        {
            for (var z = 0; z < 2; z += 1)
            {
                
                Vector3 position = new Vector3(specifiedObjectPosition.x + x * xWidth, specifiedObjectPosition.y, specifiedObjectPosition.z + z * zLength);
                Vector3 rotation = new Vector3(0, (x + z + 2) * 90, 0);
                
                if (z == 0 && x == 1)
                    rotation = new Vector3(0, 90, 0);
            
                GameObject myInstance = Instantiate(strongCorner, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }


        int step = StrongWallLength;
        // Front & Back facade (x axis)
        for (var x = step; x < xWidth; x += step)
        {
            GameObject instance = GetFortressComponent(true, 0);
            
            // Front facade
            Vector3 position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z);
            Vector3 rotation = new Vector3(rotationOffset, 90, 0);

            GameObject myInstance;
            
            // Make sure the position is not supposed to be the entrance
            if (!(selectedEntrance == EntrancePositionSelector.Front &&
                  ((xWidth / 2 - 6) <= x && x <= (xWidth / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
            
            // Back
            instance = GetFortressComponent(true, 0);
            position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + zLength);
            rotation = new Vector3(rotationOffset, 90, 0);
            
            if (!(selectedEntrance == EntrancePositionSelector.Back &&
                  ((xWidth / 2 - 6) <= x && x <= (xWidth / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }

        
        // Lateral facade (z axis)
        for (var z = step; z < zLength; z += step)
        {
            GameObject instance = GetFortressComponent(true, 0);
            
            // First facade
            Vector3 position = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + z);
            Vector3 rotation = new Vector3(rotationOffset, 0, 0);
            
            GameObject myInstance;
            if (!(selectedEntrance == EntrancePositionSelector.Left &&
                  ((zLength / 2 - 6) <= z && z <= (zLength / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }

            instance = GetFortressComponent(true, 0);
            rotation = new Vector3(rotationOffset, 0, 0);
            
            // Second facade
            position = new Vector3(specifiedObjectPosition.x + xWidth, specifiedObjectPosition.y + heightOffset, specifiedObjectPosition.z + z);
            if (!(selectedEntrance == EntrancePositionSelector.Right &&
                  ((zLength / 2 - 6) <= z && z <= (zLength / 2 + 6))))
            {
                myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
                myInstance.transform.SetParent(specifiedObject.transform);
            }
        }
    }

    private GameObject GetFortressComponent(bool isRandom, int assetNum)
    {
        heightOffset = 0;
        rotationOffset = 0;
        int idx = assetNum;

        int[] possibleRandomStructs = { SimpleWall, StrongWall, StrongWallDoor, Tower, DestructibleWall };

        if (isRandom)
            idx = possibleRandomStructs[Random.Range(0, possibleRandomStructs.Length)];

        if (idx == Tower) heightOffset = 3.028076f;
        if (idx == SimpleWall) heightOffset = -0.7219238f;

        if (idx == DestructibleWall) { rotationOffset = 90;
            heightOffset = -0.3112183f;
        }
            
        return assets[idx]; 
    }

    void RepositionBuilding()
    {
        /*float yOffset = 0;
        float yVal = Terrain.activeTerrain.SampleHeight(new Vector3(specifiedObjectPosition.x, 0, specifiedObjectPosition.z));

        //Apply Offset if needed
        yVal += yOffset;

        specifiedObject.transform.position = new Vector3(specifiedObjectPosition.x, yVal, specifiedObjectPosition.z);
        */
        float yOffset = 4;
        specifiedObject.transform.position += new Vector3(0, yOffset, 0);
    }

    void GenerateObstacles()
    {
        float securityOffset = 8;
        
        // Generate random x and z coordinates
        float xPos = Random.Range(specifiedObjectPosition.x + securityOffset,
            specifiedObjectPosition.x + xWidth - securityOffset);
        float zPos = Random.Range(specifiedObjectPosition.z + securityOffset,
            specifiedObjectPosition.z + zLength - securityOffset);

        Vector3 instancePos = new Vector3(xPos, 0, zPos);
        float yPos = _terrains[GetClosestTerrain(instancePos)].SampleHeight(instancePos);
        
        
            
        
    }
    int GetClosestTerrain(Vector3 positionToBeChecked)
    {
        int terrainIndex = 0;
        float lowDist = float.MaxValue;

        for (int i = 0; i < _terrains.Length; i++)
        {
            var center = new Vector3(_terrains[i].transform.position.x + _terrains[i].terrainData.size.x / 2, positionToBeChecked.y, _terrains[i].transform.position.z + _terrains[i].terrainData.size.z / 2);

            float dist = Vector3.Distance(center, positionToBeChecked);

            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return terrainIndex;
    }

}

