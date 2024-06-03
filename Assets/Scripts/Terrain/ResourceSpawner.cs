// Based on https://pastebin.com/pzfqGmFZ

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject resourcePrefab;
    public float spawnChance;
 
    [Header("Raycast setup")]
    public float distanceBetweenCheck;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;

    public GameObject yaraHut;
 
    private void Start()
    {
        //SpawnResources();
    }

    /*void SpawnResources()
    {
        for (int x = 0; x < yaraHut.transform.position.x + 2; x++)
        {
            for (int z = 0; z < yaraHut.transform.position.z + 2; z++)
            {
                Instantiate(resourcePrefab, new Vector3(x, yaraHut.transform.position.y, z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);

            } 
        }
    }*/
 
    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            DeleteResources();
            SpawnResources();
        }
    }
 
    void SpawnResources()
    {
        for(float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for(float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if(spawnChance > Random.Range(0f, 101f))
                    {
                        //Debug.Log("Resource placed");
                        Instantiate(resourcePrefab, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                    }
                }
            }
        }
    }
 
    void DeleteResources()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }*/
}