using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CreateOutpost : MonoBehaviour
{
    // ----------- Constants -----------
    // Walls
    private const int SimpleWall = 0;
    private const int StrongWall = 1;
    private const int StrongWallDoor = 2;
    // Corners
    private const int SimpleCorner = 1;
    private const int StrongCorner = 1;
    // Wall End
    private const int StrongEnd = 1;
    // Tower
    private const int Tower = 1;
    
    // ----------- Constant Measures -----------
    private const int SimpleWallLength = 4;
    private const int StrongWallLength = 6;
    private const int WallsHeight = 10;
    
    // ----------- Assets -----------
    // Walls
    public GameObject simpleWall;
    public GameObject strongWall;
    public GameObject strongWallDoor;
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

    // Start is called before the first frame update
    void Start()
    {
        // if no initial position specified, use zero, otherwise, use the object position
        if (!useSpecifiedStartingPosition)
            specifiedObjectPosition = Vector3.zero;
        else
            specifiedObjectPosition = specifiedObject.transform.position;
        
        Build();
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
        for (var x = step; x < xWidth - step; x += step)
        {
            GameObject instance = strongWall;
            
            // Front facade
            Vector3 position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z);
            Vector3 rotation = new Vector3(0, 90, 0);
            
            GameObject myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
            
            // Back
            position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z + zLength);
            
            myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
        }

        
        // lateral facade (z axis)
        /*for (var z = 1; z < zLength; z += 2)
        {
            GameObject instance = null;
            
            // First facade
            Vector3 position = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            Vector3 rotation = new Vector3(0, 90, 0);
            
            GameObject myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
            
            // Second facade
            instance = null;
            position = new Vector3(specifiedObjectPosition.x + xWidth, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            
            myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
        }*/
    }
}

