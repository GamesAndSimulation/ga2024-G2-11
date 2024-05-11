using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton class
    public static GameManager Instance { get; private set; }

    public bool inPuzzleMode = false;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private MeshRenderer playerBody;
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetShowWalkCrosshairAndGuns(bool show)
    {
        crosshair.SetActive(show);
        weaponHolder.SetActive(show);
        playerBody.enabled = show;
    }
    
    public void SetPuzzleMode(bool value)
    {
        inPuzzleMode = value;
        SetShowWalkCrosshairAndGuns(!value);
    }


}
