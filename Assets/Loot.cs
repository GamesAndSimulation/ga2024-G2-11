using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public enum LootType
    {
        Coins,
        Ammo
    }
    
    public LootType lootType;
    public int quantity;
    
    private MeshRenderer _meshRenderer;
    private Revolver _revolver;
    

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _revolver = FindObjectOfType<Revolver>();
    }

    public void Scavenge()
    {
        switch (lootType)
        {
            case LootType.Coins:
                Debug.Log("You got " + quantity + " coins!");
                break;
            case LootType.Ammo:
                Debug.Log("You got " + quantity + " ammo!");
                _revolver.AddAmmo(quantity);
                break;
        }

        _meshRenderer.material = Resources.Load<Material>("BronzeTransperant");
        _meshRenderer.material.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }
    
}
