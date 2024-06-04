using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    public int numPuzzlesSolved;
    [SerializeField] private GameObject TreasureRoom;
    private float TRoomLightIntensity;
    public Transform treasurePlayerSpawnPoint;
    public string currentPuzzlePrefabPath;
    public string[] puzzlePrebabPaths = new string[]
    {
        "Prefabs/Puzzles/Puzzle (Medium)",
        "Prefabs/Puzzles/Puzzle (Harder)"
    };

    public Transform player;
    
    private void Start()
    {
        numPuzzlesSolved = 0;
        currentPuzzlePrefabPath = puzzlePrebabPaths[numPuzzlesSolved];
        TreasureRoom.SetActive(false);
        TRoomLightIntensity = TreasureRoom.GetComponentInChildren<Light>().intensity;
        TreasureRoom.GetComponentInChildren<Light>().intensity = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
            TeleportToTreasureRoom();
    }

    public void AddPuzzleSolved()
    {
        numPuzzlesSolved++;
        if (numPuzzlesSolved < puzzlePrebabPaths.Length)
        {
            currentPuzzlePrefabPath = puzzlePrebabPaths[numPuzzlesSolved];
        }
        
        if (numPuzzlesSolved >= 2)
        {
            TeleportToTreasureRoom();
        }
        
    }
    
    private void TeleportToTreasureRoom()
    {
        AudioManager.Instance.StopSoundLooping();
        TreasureRoom.SetActive(true);
        Destroy(GameObject.FindWithTag("WaveFunction"));
        GameManager.Instance.gameLoading = false;
        GameManager.Instance.inPuzzleMode = false;
        GameManager.Instance.gamePaused = false;
        //Transform playerTransform = (player.transform.parent == null) ? player.transform : player.transform.parent;
        player.position = treasurePlayerSpawnPoint.position;
        Transform lootUrn = TreasureRoom.transform.Find("LootUrn");
        Vector3 lookDirection = (lootUrn.position - player.transform.position).normalized;
        player.transform.forward = lookDirection;
        //player.transform.LookAt(TreasureRoom.transform.Find("LootUrn").position);
        TreasureRoom.GetComponentInChildren<Light>().DOIntensity(119.8f, 3f).SetEase(Ease.InOutSine);
        numPuzzlesSolved = 0;
    }
    
    

}
