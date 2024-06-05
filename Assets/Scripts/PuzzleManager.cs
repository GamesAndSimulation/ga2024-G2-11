using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{

    public int numPuzzlesSolved;
    [SerializeField] private GameObject TreasureRoom;
    private float TRoomLightIntensity;
    public Transform treasurePlayerSpawnPoint;
    public string currentPuzzlePrefabPath;
    public string[] puzzlePrebabPaths;
    public GameObject EssenceThing;
    public AudioClip WooshSound;

    public Transform player;
    
    private void Start()
    {
        AudioManager.Instance.AddImmuneLoopSources();
        numPuzzlesSolved = PlayerPrefs.GetInt("numPuzzlesSolved");
        if (numPuzzlesSolved >= puzzlePrebabPaths.Length)
            numPuzzlesSolved = 0;
        currentPuzzlePrefabPath = puzzlePrebabPaths[numPuzzlesSolved];
        //TreasureRoom.SetActive(false);
        TRoomLightIntensity = TreasureRoom.GetComponentInChildren<Light>().intensity;
        TreasureRoom.GetComponentInChildren<Light>().intensity = 0;
        
        Debug.Log(PlayerPrefs.GetInt("numPuzzlesSolved"));
        
    }

    public void ExitCavern()
    {
        StartCoroutine(ExitCavernRoutine());
    }

    private IEnumerator ExitCavernRoutine()
    {
        
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySound(WooshSound);
        EssenceThing.transform.DOMoveY(10f, 3.5f).OnComplete(() =>
        {
            GameManager.Instance.SetEnableLoadScreen(true);
            SceneManager.LoadScene("World");
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
            TeleportToTreasureRoom();
    }

    public void AddPuzzleSolved()
    {
        numPuzzlesSolved++;
        PlayerPrefs.SetInt("numPuzzlesSolved", numPuzzlesSolved);
        PlayerPrefs.Save();
 
        if (numPuzzlesSolved < puzzlePrebabPaths.Length)
        {
            currentPuzzlePrefabPath = puzzlePrebabPaths[numPuzzlesSolved];
        }
        
        if (numPuzzlesSolved % 2 == 0)
        {
            Debug.LogError("HERE 1");
            TeleportToTreasureRoom();
        }
        
    }
    
    private void TeleportToTreasureRoom()
    {
        //GameManager.Instance.SetFreezePlayer(true);
        Debug.LogError("HERE 2");
        AudioManager.Instance.immuneSources.Clear();
        AudioManager.Instance.StopSoundLooping();
        Debug.LogError("HERE 3");
        //TreasureRoom.SetActive(true);
        //Destroy(GameObject.FindWithTag("WaveFunction"));
        //GameManager.Instance.gameLoading = false;
        //GameManager.Instance.inPuzzleMode = false;
        //GameManager.Instance.gamePaused = false;
        ////Transform playerTransform = (player.transform.parent == null) ? player.transform : player.transform.parent;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        player.position = treasurePlayerSpawnPoint.position;
        rb.isKinematic = false;
        SceneManager.LoadScene("TresureRoom");
        //GameObject.FindWithTag("PlayerPortal").GetComponent<BoxCollider>().enabled = true;
        Debug.LogError("HERE 4");
        //Transform lootUrn = TreasureRoom.transform.Find("LootUrn");
        //Vector3 lookDirection = (lootUrn.position - player.transform.position).normalized;
        ////player.forward = lookDirection;
        //player.transform.LookAt(TreasureRoom.transform.Find("LootUrn").position);
        TreasureRoom.GetComponentInChildren<Light>().DOIntensity(119.8f, 3f).SetEase(Ease.InOutSine);
        Debug.LogError("HERE 5");
        numPuzzlesSolved = 0;
        Debug.LogError("HERE 6");
        //GameManager.Instance.SetFreezePlayer(false);
    }
    
    

}
