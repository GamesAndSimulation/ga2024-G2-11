using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private Enemy _enemyMainScript;

    private void Start()
    {
        _enemyMainScript = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Sight trigger: {other.tag}");
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position,  other.transform.parent.position - transform.position, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    _enemyMainScript.currentEnemyState = Enemy.EnemyState.Chase;
                }
            }
        }
    }
}
