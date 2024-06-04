using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    public Slider healthBar;
    
    [Header("Player Stats")]
    public float MaxHealth;

    public GameObject DeathScreen;
    
    private float health;
    private int enemiesKilled;

    private void Start()
    {
        health = MaxHealth;
        
        healthBar.maxValue = MaxHealth;
        healthBar.value = MaxHealth;
    }
    
    public void AddEnemyKill()
    {
        enemiesKilled++;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health;
        ScreenEffectUtils.Instance.DamageEffect();
        Debug.Log("Player health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        DeathScreen.SetActive(true);
    }
}

