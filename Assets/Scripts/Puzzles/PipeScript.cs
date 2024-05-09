using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePuzzle : MonoBehaviour 
{

    float[] rotations = { 0, 90, 180, 270 };

    public float[] correctRotation;
    private bool isPlacedCorrectly = false;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, rotations.Length);
        transform.eulerAngles = new Vector3(0, 0, rotations[rand]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        transform.Rotate(new Vector3(0, 0, 90));

        for (int i = 0; i < correctRotation.Length; i++)
        {
            if (transform.eulerAngles.z == correctRotation[i])
                isPlacedCorrectly = true;
        }
        
        
    }
}
