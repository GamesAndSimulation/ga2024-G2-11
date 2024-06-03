using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBuildings : MonoBehaviour
{
    private const int WINDOW = 1;
    
    public int minPieces = 5;
    public int maxPieces = 20;

    public GameObject[] walls;
    public GameObject door;
    public GameObject shadow;
    public GameObject[] roofs;
    public GameObject specialRoof;

    public int xWidth;
    public int zLength;

    public bool useSpecifiedStartingPosition;
    public GameObject specifiedObject; // probably an empty one
    public Vector3 specifiedObjectPosition; 

    [Range(0, 100)] public double maxNumberOfWindows;

    private int numberOfWindows;

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
        //front & back facade (x axis)
        for (var x = 1; x < xWidth; x += 2)
        {
            GameObject instance = getWall();
            
            // Front facade
            Vector3 position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z);
            Vector3 rotation = new Vector3(0, 0, 0);
            
            GameObject myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
            
            // Back facade
            instance = getWall();
            position = new Vector3(specifiedObjectPosition.x + x, specifiedObjectPosition.y, specifiedObjectPosition.z + zLength);
            
            myInstance = Instantiate(instance, position, Quaternion.Euler(rotation),specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
        }
        
        // lateral facade (z axis)
        for (var z = 1; z < zLength; z += 2)
        {
            GameObject instance = getWall();
            
            // First facade
            Vector3 position = new Vector3(specifiedObjectPosition.x, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            Vector3 rotation = new Vector3(0, 90, 0);
            
            GameObject myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
            
            // Second facade
            instance = getWall();
            position = new Vector3(specifiedObjectPosition.x + xWidth, specifiedObjectPosition.y, specifiedObjectPosition.z + z);
            
            myInstance = Instantiate(instance, position, Quaternion.Euler(rotation), specifiedObject.transform);
            myInstance.transform.SetParent(specifiedObject.transform);
        }
    }
    
    private GameObject getWall()
    {
        GameObject instance = door;
        if (numberOfWindows < maxNumberOfWindows)
        {
            int index = Random.Range(0, walls.Length);
            instance = walls[index];

            if (index == WINDOW)
                numberOfWindows++;
        
        }
        
        return instance;
    }
}

