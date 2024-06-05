using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    
    [SerializeField] private GameObject PuzzleUI;
    [SerializeField] private GameObject PuzzleTutorial;
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private AudioClip DoorEnterSound;
    
    private Vector3 forward;
    private RaycastHit hit;

    private GameObject _currentPuzzle;
    private GameObject _board;

    void Start()
    {
        _board = GameObject.FindWithTag("Board");
    }

    void Update()
    {
        var playerCamera = Camera.main.transform;
        Vector3 forwardVec = GetCameraForward();
        Debug.DrawRay(playerCamera.position, forwardVec * 10f, Color.green);
        if(Physics.Raycast(playerCamera.position, forwardVec, out hit, 10f))
        {
            switch (hit.transform.gameObject.tag)
            {
                case "Puzzle":
                    interactIcon.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
                    _currentPuzzle = hit.transform.gameObject;
                    InteractWithPuzzle();
                    break;
                case "Board":
                    interactIcon.GetComponentInChildren<TextMeshProUGUI>().text = "Drive";
                    interactIcon.SetActive(true);
                    InteractWithBoard();
                    break;
                case "CaveEntrance":
                    interactIcon.GetComponentInChildren<TextMeshProUGUI>().text = "Enter";
                    if(!GameManager.Instance.IsLoadingScreenOn())
                        interactIcon.SetActive(true);
                    EnterCave();
                    break;
                case "Loot":
                    interactIcon.GetComponentInChildren<TextMeshProUGUI>().text = "Scavenge";
                    interactIcon.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<Loot>().Scavenge();
                    }
                    break;
                default:
                    if(Input.GetKeyDown(KeyCode.X))
                        Debug.LogWarning($"Hit tag: {hit.transform.gameObject.tag} with name {hit.transform.gameObject.name}");
                    interactIcon.SetActive(false);
                    break;
            }
        }
        else
        {
            interactIcon.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ExitPuzzle();
        }
        
    }
    
    private void EnterCave()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EnterCavern());
        }
    }
    
    private IEnumerator EnterCavern()
    {
        GameManager.Instance.SetEnableLoadScreen(true);
        interactIcon.SetActive(false);
        AudioManager.Instance.StopSoundLooping();
        AudioManager.Instance.PlaySound(DoorEnterSound, false, 1f);
        yield return new WaitForSeconds(DoorEnterSound.length);
        GameManager.Instance.LoadScene("Cavern");
    }
    
    private void InteractWithBoard()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactIcon.SetActive(false);
            hit.collider.GetComponent<BoardController>().enabled = true;
            GameManager.Instance.SetDrivingMode(true);
            GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 0;
            hit.collider.GetComponentInChildren<CinemachineFreeLook>().Priority = 1;
        }
    }

    // Get the forward vector of the camera.
    // Using default .forward doesn't seem to work well with cinemachine POV... 
    private Vector3 GetCameraForward()
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

    public void ResetPuzzleBoard()
    {
        _currentPuzzle.transform.root.GetComponent<SigilPuzzle>().ResetPuzzleBoard();
    }
    
    void InteractWithPuzzle() 
    {
        if(GameManager.Instance.inPuzzleMode)
            return;
        interactIcon.SetActive(true);
        //interactIcon.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactIcon.SetActive(false);
            //cameraController.enabled = false;
            FadePuzzleUI(true);
            _currentPuzzle.transform.root.GetComponent<SigilPuzzle>().enabled = true;
            InputManager.Instance.SetLookLock(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.SetPuzzleMode(true);
            GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 0;
            _currentPuzzle.transform.root.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 1;
            Debug.Log("Interact With Puzzle");
        }
    }
    
    public void ExitPuzzle()
    {
        //cameraController.enabled = true;
        FadePuzzleUI(false);
        interactIcon.SetActive(false);
        InputManager.Instance.SetLookLock(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.SetPuzzleMode(false);
        GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 1;
        _currentPuzzle.transform.root.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 0;
        _currentPuzzle.transform.root.GetComponent<SigilPuzzle>().enabled = false;
    }

    public void FadePuzzleUI(bool fadeIn)
    {
        foreach(Image img in PuzzleUI.GetComponentsInChildren<Image>())
        {
            if (fadeIn)
                img.DOFade(1, 0.5f);
            else
                img.DOFade(0, 0.5f);
        }
        PuzzleTutorial.GetComponentInChildren<RawImage>().DOFade(fadeIn ? 0.5f : 0, 0.5f);
        
        foreach(Image img in PuzzleTutorial.GetComponentsInChildren<Image>())
        {
            if (fadeIn)
                img.DOFade(1, 0.5f);
            else
                img.DOFade(0, 0.5f);
        }
        
        foreach(TextMeshProUGUI text in PuzzleTutorial.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (fadeIn)
                text.DOFade(1, 0.5f);
            else
                text.DOFade(0, 0.5f);
        }
    }
    

}
