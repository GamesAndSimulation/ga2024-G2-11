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
    
    private void Start()
    {
        numPuzzlesSolved = 0;
        TreasureRoom.SetActive(false);
        TRoomLightIntensity = TreasureRoom.GetComponentInChildren<Light>().intensity;
        TreasureRoom.GetComponentInChildren<Light>().intensity = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
            TeleportToTreasureRoom();
    }

    public void AddPuzzleSolved()
    {
        numPuzzlesSolved++;
        if (numPuzzlesSolved >= 2)
        {
            TeleportToTreasureRoom();
        }
        
    }
    
    private void TeleportToTreasureRoom()
    {
        TreasureRoom.SetActive(true);
        Destroy(GameObject.FindWithTag("WaveFunction"));
        GameManager.Instance.gameLoading = false;
        GameManager.Instance.inPuzzleMode = false;
        GameManager.Instance.gamePaused = false;
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = treasurePlayerSpawnPoint.position;
        Transform lootUrn = TreasureRoom.transform.Find("LootUrn");
        Vector3 lookDirection = (lootUrn.position - player.transform.position).normalized;
        player.transform.forward = lookDirection;
        //player.transform.LookAt(TreasureRoom.transform.Find("LootUrn").position);
        TreasureRoom.GetComponentInChildren<Light>().DOIntensity(119.8f, 3f).SetEase(Ease.InOutSine);
        numPuzzlesSolved = 0;
    }
    
    

}
