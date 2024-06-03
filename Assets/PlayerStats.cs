using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    
    [Header("Player Stats")]
    public float MaxHealth;

    public GameObject DeathScreen;
    
    private float health;
    private int enemiesKilled;

    private void Start()
    {
        health = MaxHealth;
    }
    
    public void AddEnemyKill()
    {
        enemiesKilled++;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        ScreenEffectUtils.Instance.DamageEffect();
        Debug.Log("Player health: " + health);
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    
    private IEnumerator Die()
    {
        DeathScreen.SetActive(true);
        yield return new WaitForSeconds(4f);
        GameManager.Instance.RestartGame();
    }
}

