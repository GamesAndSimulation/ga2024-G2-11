using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    public bool isEntrance;
    
    [SerializeField]
    public Transform linkedPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (isEntrance && other.CompareTag("Player"))
        {
            other.transform.position = linkedPortal.position;
        }
    }
}
