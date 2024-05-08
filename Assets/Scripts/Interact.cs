using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Interact : MonoBehaviour
{
    private Vector3 forward;
    private RaycastHit hit;

    [SerializeField] private GameObject playerCamera;

    private Animation _doorAnimation;
    private bool _isOpened = false;

    void Start()
    {
        _doorAnimation = GetComponent<Animation>();
        _doorAnimation.clip.wrapMode = WrapMode.Once;
        playerCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        forward = playerCamera.transform.TransformDirection(Vector3.forward);
        if(Physics.Raycast(playerCamera.transform.position, forward, out hit, 30f))
        {
            switch (hit.transform.gameObject.tag) 
            {
                case "Puzzle":
                    InteractWithPuzzle();
                    break;
            }
        }
        //else
        //{
        //    interactIcon.SetActive(false);
        //}

        if(Input.GetKeyDown(KeyCode.Q))
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
    }

}
