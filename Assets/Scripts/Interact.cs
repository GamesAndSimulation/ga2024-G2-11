using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class Interact : MonoBehaviour
{
    private Vector3 forward;
    private RaycastHit hit;

    private bool _isOpened = false;
    private GameObject _currentPuzzle;

    void Start()
    {
    }

    void Update()
    {
        //forward = playerCamera.transform.TransformDirection(Vector3.forward);
        var playerCamera = Camera.main.transform;
        Debug.DrawRay(playerCamera.position, playerCamera.forward * 30f, Color.green);
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 30f))
        {
            switch (hit.transform.gameObject.tag) 
            {
                case "Puzzle":
                    _currentPuzzle = hit.transform.gameObject;
                    InteractWithPuzzle();
                    break;
            }
        }
        //else
        //{
        //    interactIcon.SetActive(false);
        //}

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ExitPuzzle();
        }
        
    }

    void InteractWithPuzzle() 
    {
        //interactIcon.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            //cameraController.enabled = false;
            InputManager.Instance.SetLookLock(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.instance.inPuzzleMode = true;
            GameManager.instance.SetShowWalkCrosshairAndGuns(false);
            GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 0;
            _currentPuzzle.transform.root.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 1;
            Debug.Log("Interact With Puzzle");
        }
    }

    public void ExitPuzzle()
    {
        //cameraController.enabled = true;
        InputManager.Instance.SetLookLock(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.inPuzzleMode = false;
        GameManager.instance.SetShowWalkCrosshairAndGuns(true);
        GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 1;
        _currentPuzzle.transform.root.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 0;
    }

}
