using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton class
    public static GameManager Instance { get; private set; }

    public bool inPuzzleMode = false;
    public bool inDrivingMode = false;
    public bool inFreeCamMode = false;
    public bool gamePaused = false;
    public bool gameLoading;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GameObject FPSCounter;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject LevelStartFade;
    [SerializeField] private Transform outpost1SpawnPoint;
    [SerializeField] private Transform outpost2SpawnPoint;
    private float _startTimeScale;
    private float _startFixedDeltaTime;
    public int whaleBlood = 0;
    public string filePath;


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
    
    private void Start()
    {
        _startTimeScale = Time.timeScale;
        _startFixedDeltaTime = Time.fixedDeltaTime;
        LevelStartFadeEffect();
        // Initialize Data and Save it for the first time
        filePath = Path.Combine(Application.persistentDataPath, "whaleBloodReal.json");
        if (!File.Exists(filePath))
        {
            Data data = new Data();
            data.whaleBlood = 0;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Data saved to " + filePath);
        }
    }
    
    public void AddWhaleBlood()
    {
        Data data = JsonUtility.FromJson<Data>(File.ReadAllText(filePath));
        data.whaleBlood++;
        if(data.whaleBlood >= 2)
            SceneManager.LoadScene("EndGame");
        File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
        Debug.Log("Whale Blood: " + whaleBlood);
    }
    
    public int GetWhaleBlood()
    {
        Data data = JsonUtility.FromJson<Data>(File.ReadAllText(filePath));
        return data.whaleBlood;
    }
    
    void InitializePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("StoredBullets"))
        {
            PlayerPrefs.SetInt("StoredBullets", 14);
        }
        
        if (!PlayerPrefs.HasKey("BulletsInChamber"))
        {
            PlayerPrefs.SetInt("BulletsInChamber", 4);
        }

        if (!PlayerPrefs.HasKey("Money"))
        {
            PlayerPrefs.SetInt("Money", 20); 
        }
        
        if(!PlayerPrefs.HasKey("numPuzzlesSolved"))
        {
            PlayerPrefs.SetInt("numPuzzlesSolved", 0);
        }

        if (!PlayerPrefs.HasKey("EssenceBlood"))
        {
            PlayerPrefs.SetInt("EssenceBlood", 0);
        }
        
        PlayerPrefs.Save();
    }
    
    
    public void BeforeBuild()
    {
        PlayerPrefs.SetInt("StoredBullets", 14);
        PlayerPrefs.SetInt("BulletsInChamber", 4);
        PlayerPrefs.SetInt("Money", 20);
        PlayerPrefs.SetInt("numPuzzlesSolved", 0);
        PlayerPrefs.SetInt("EssenceBlood", 0);
        PlayerPrefs.Save();
    }
    
    public void SetFreezePlayer(bool value)
    {
        playerScript.canMove = !value;
    }
    
    
    
    public void ManualLevelStarFade()
    {
        LevelStartFade.SetActive(true);
        LevelStartFadeEffect();
    }
    
    private void LevelStartFadeEffect()
    {
        if(LevelStartFade.activeSelf)
            LevelStartFade.GetComponent<RawImage>().DOFade(0, 3f).SetEase(Ease.InQuad).OnComplete(() => LevelStartFade.SetActive(false)); 
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            FPSCounter.SetActive(!FPSCounter.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            gamePaused = !gamePaused;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
             AddWhaleBlood();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rigidbody rb = playerScript.gameObject.GetComponent<Rigidbody>();
            playerScript.gameObject.transform.position = outpost1SpawnPoint.position;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Rigidbody rb = playerScript.gameObject.GetComponent<Rigidbody>();
            playerScript.gameObject.transform.position = outpost2SpawnPoint.position;
        }
            
    }
    
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        InitializePlayerPrefs();
    }
    
    public void SetPlayerGodMode(bool value)
    {
        playerScript.GetComponent<PlayerStats>().godModeOn = value;
    }
    
    public Vector3 GetPlayerPosition()
    {
        return playerScript.transform.position;
    }
    
    public void SetEnableLoadScreen(bool value)
    {
        loadingScreen.SetActive(value);
    }
    
    public bool IsLoadingScreenOn()
    {
        return loadingScreen.activeSelf;
    }
    
    public void LoadScene(string sceneName)
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(sceneName);
    }

    public void SetShowWalkCrosshairAndGuns(bool show)
    {
        crosshair.SetActive(show);
        weaponHolder.SetActive(show);
    }
    
    public void SetDrivingMode(bool value)
    {
        inDrivingMode = value;
        SetShowWalkCrosshairAndGuns(!value);
        playerScript.enabled = !value;
        
    }
    
    public void SetPuzzleMode(bool value)
    {
        inPuzzleMode = value;
        SetShowWalkCrosshairAndGuns(!value);
    }
    
    public void RestartGame()
    {
        Time.timeScale = _startTimeScale;
        Time.fixedDeltaTime = _startFixedDeltaTime;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
    }
    
    
    public Vector3 GetCameraForward()
    {
        var pov = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
        float pitch = pov.m_VerticalAxis.Value;
        float yaw = pov.m_HorizontalAxis.Value;
        
        float pitchRad = pitch * Mathf.Deg2Rad;
        float yawRad = yaw * Mathf.Deg2Rad;
        
        float x = MathF.Cos(pitchRad) * Mathf.Sin(yawRad);
        float y = -MathF.Sin(pitchRad);
        float z = MathF.Cos(pitchRad) * Mathf.Cos(yawRad);
        return new Vector3(x, y, z);
    }


}
