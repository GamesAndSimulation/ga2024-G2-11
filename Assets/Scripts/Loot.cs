using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Loot : MonoBehaviour
{
    public enum LootType
    {
        Coins,
        Ammo,
        EssenceBlood
    }
    
    public LootType lootType;
    public int quantity;
    public AudioClip ammoSound;
    public AudioClip coinsSound;
    public AudioClip essenceBloodSound;
    
    private MeshRenderer _meshRenderer;
    private Revolver _revolver;
    private PlayerStats _playerStats;
    

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _revolver = FindObjectOfType<Revolver>();
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    public void Scavenge()
    {
        
        switch (lootType)
        {
            case LootType.Coins:
                Debug.Log("You got " + quantity + " coins!");
                AudioManager.Instance.PlaySound(coinsSound);
                _playerStats.AddCoins(quantity);
                break;
            case LootType.Ammo:
                Debug.Log("You got " + quantity + " ammo!");
                AudioManager.Instance.PlaySound(ammoSound);
                _revolver.AddAmmo(quantity);
                break;
            case LootType.EssenceBlood:
                Debug.Log("Collected essence blood");
                AudioManager.Instance.PlaySound(essenceBloodSound);
                PlayerPrefs.SetInt("EssenceBlood", PlayerPrefs.GetInt("EssenceBlood") + 1);
                FindObjectOfType<TresureRoom>().ExitCavern();
                break;
        }
        StartCoroutine(DoFadeOut());    
    }
    
    private IEnumerator DoFadeOut()
    {
        _meshRenderer.material.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
    
}
