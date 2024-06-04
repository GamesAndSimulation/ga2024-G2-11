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
    public AudioClip[] hurtSounds;
    public float hurtSoundCooldownTime;
    private float hurtSoundCooldownTimer;
    
    private void Start()
    {
        health = MaxHealth;
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

