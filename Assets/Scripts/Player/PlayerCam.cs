using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sens;

    public Transform orientation;

    float xRotation;
    float yRotation;
    float zRotation;

    public PlayerScript playerScript;

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;
    private float shakeTimer;

    private void Start(){
        
        transform.parent.forward = orientation.forward;

        Vector3 currentRotation = transform.parent.rotation.eulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            sens += 50;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            sens -= 50;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        yRotation += mouseX;
        //zRotation = playerScript.currentRoll;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        if(shakeTimer > 0){
            Quaternion shakeOffset = Quaternion.Euler(PerlinShake() * shakeMagnitude);
            transform.parent.rotation = Quaternion.Euler(xRotation, yRotation, zRotation) * shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
    }

    public void Shake(){
        shakeTimer = shakeDuration;
    }

   private Vector3 PerlinShake(){
    float x = (Mathf.PerlinNoise(Time.time, 0f) * 2 - 1) * shakeMagnitude;
    float y = (Mathf.PerlinNoise(0f, Time.time) * 2 - 1) * shakeMagnitude;
    return new Vector3(x, y, 0f);
} 

    public void DoFov(float endValue)
    {
        //GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

}
