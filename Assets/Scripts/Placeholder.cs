using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 5, 0, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.gameObject.SetActive(false);
        // Should also "lock" the player here, so that it practises shooting and doesn't move
    }
}
