using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTurrets : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Turret"))
            other.transform.GetComponent<EnemyFov>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Turret"))
            other.transform.GetComponent<EnemyFov>().enabled = false;
    }
}
