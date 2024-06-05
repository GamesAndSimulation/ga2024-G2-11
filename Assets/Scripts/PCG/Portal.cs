using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    public bool isEntrance = true;
    
    [SerializeField]
    public Transform linkedPortal;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Portal Triggered by tag {other.tag}");
        if (isEntrance && (other.CompareTag("Player") || other.transform.parent.CompareTag("Player")))
        {
            //other.transform.position = linkedPortal.position;
            other.transform.parent.position = GameObject.FindWithTag("PuzzlePiece").transform.parent.parent
                .Find("PuzzleCamera").position;
        }

        if (tag == "PlayerPortal")
        {
            
        }
    }
    
    public void SetLinkedPortal(Transform portal)
    {
        linkedPortal = portal;
    }
}
