using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    
    [Header("Player Stats")]
    public float MaxHealth;

    public GameObject DeathScreen;
    public bool godModeOn;
    
    private float health;
    private int enemiesKilled;
    public int coins;
    public AudioClip[] hurtSounds;
    public float hurtSoundCooldownTime = 1.25f;
    private float hurtSoundCooldownTimer;
    
    private void Start()
    {
        health = MaxHealth;
        Debug.Log(PlayerPrefs.GetInt("Money"));
        AudioManager.Instance.AddAllSourcesToTimeIndie();
    }
    
    void Update()
    {
        if (hurtSoundCooldownTimer > 0)
        {
            hurtSoundCooldownTimer -= Time.deltaTime;
        }
    }
    
    public void AddEnemyKill()
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
        if (godModeOn) return;
        health -= damage;
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
        DeathScreen.SetActive(true);
        yield return new WaitForSeconds(4f);
        GameManager.Instance.RestartGame();
    }
}

