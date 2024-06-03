using System;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FreeMoveCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    public bool isFreeCameraActive = false;
    private CinemachineVirtualCamera freeMovCam;
    private CinemachineVirtualCamera mainCam;
    private Transform player;
    [SerializeField] private TextMeshProUGUI _freeCamOnText;
    private float _startTimeScale;

    private void Start()
    {
        freeMovCam = GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindWithTag("Player").transform;
        mainCam = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _freeCamOnText.text = "Free Moving Camera ON\n<size=80%>E : Raise Camera\nQ : Lower Camera</size>";
        _startTimeScale = Time.timeScale;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            isFreeCameraActive = !isFreeCameraActive;
            GameManager.Instance.inFreeCamMode = isFreeCameraActive;

            if (isFreeCameraActive)
            {
                _freeCamOnText.DOFade(1, 0.2f);
                transform.position = player.transform.position;
                transform.rotation = GameObject.FindWithTag("MainCamera").transform.rotation;
                //freeMovCam.transform.forward = mainCam.transform.forward;
                freeMovCam.Priority = 10;
                mainCam.Priority = 0;
            }
            else
            {
                _freeCamOnText.DOFade(0, 0.2f);
                freeMovCam.Priority = 0;
                mainCam.Priority = 10;
            }
            
        }
        
    }

    private void LateUpdate()
    {
        if (isFreeCameraActive)
        {
            MoveCamera();
            RotateCamera();
        }
        
    }

    void MoveCamera()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveY = 0;

        if (Input.GetKey(KeyCode.E)) // move up
        {
            moveY = moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q)) // move down
        {
            moveY = -moveSpeed * Time.deltaTime;
        }

        transform.Translate(new Vector3(moveX, moveY, moveZ));
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        Vector3 rotation = transform.localEulerAngles;
        rotation.y += mouseX;
        rotation.x -= mouseY;
        transform.localEulerAngles = rotation;
    }
}