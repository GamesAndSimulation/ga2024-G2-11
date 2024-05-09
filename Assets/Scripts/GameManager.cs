using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton class
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }
    
    public bool inPuzzleMode = false;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHolder;
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void SetShowWalkCrosshairAndGuns(bool show)
    {
        crosshair.SetActive(show);
        weaponHolder.SetActive(show);
    }


}
