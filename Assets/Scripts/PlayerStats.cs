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
    public bool godModeOn;
    
    private float health;
    private int enemiesKilled;
    public int coins;
    public AudioClip[] hurtSounds;
    public AudioClip deathSound;
    public float hurtSoundCooldownTime = 1.25f;
    private float hurtSoundCooldownTimer;
    
    private void Start()
    {
        health = MaxHealth;
        healthBar.maxValue = MaxHealth;
       healthBar.value = MaxHealth;   
        Debug.Log(PlayerPrefs.GetInt("Money"));
        //AudioManager.Instance.AddAllSourcesToTimeIndie(); 
}
    
void Update()
    {
        if (hurtSoundCooldownTimer > 0)
        {
            hurtSoundCooldownTimer -= Time.deltaTime;
        }
    }    public void AddEnemyKill()
    {
        enemiesKilled++;
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Money", coins);
        PlayerPrefs.Save();
    }
    
    public void SpendCoins(int amount)
    {
        coins -= amount;
        PlayerPrefs.SetInt("Money", coins);
        PlayerPrefs.Save();
    }
    
    public void AddHealth(float amount)
    {
        health += amount;
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (godModeOn || health <= 0) return;
        health -= damage;
        healthBar.value = health;
        ScreenEffectUtils.Instance.DamageEffect();
        Debug.Log("Player health: " + health);
        if (hurtSoundCooldownTimer <= 0 && health > 0)
        {
            AudioManager.Instance.PlaySound(hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)]);
            hurtSoundCooldownTimer = hurtSoundCooldownTime;
        }
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    
    private IEnumerator Die()
    {
        AudioManager.Instance.PlaySound(deathSound);
        DeathScreen.SetActive(true);
        yield return new WaitForSeconds(4f);
        GameManager.Instance.RestartGame();
    }
}

