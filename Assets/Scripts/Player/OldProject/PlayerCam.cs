using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sens;

    public Transform orientation;

    private float _xRotation;
    private float _yRotation;
    private float _zRotation;

    public PlayerScript playerScript;

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;
    private float _shakeTimer;

    private void Start(){
        var parent = transform.parent;
        //parent.forward = orientation.forward;

        Vector3 currentRotation = parent.rotation.eulerAngles;
        _xRotation = currentRotation.x;
        _yRotation = currentRotation.y;
        
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

        var mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        var mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        _yRotation += mouseX;
        //zRotation = playerScript.currentRoll;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);


        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, _zRotation);
        orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);

        if(_shakeTimer > 0){
            Quaternion shakeOffset = Quaternion.Euler(PerlinShake() * shakeMagnitude);
            transform.parent.rotation = Quaternion.Euler(_xRotation, _yRotation, _zRotation) * shakeOffset;
            _shakeTimer -= Time.deltaTime;
        }
    }

    public void Shake(){
        _shakeTimer = shakeDuration;
    }

   private Vector3 PerlinShake(){
    var x = (Mathf.PerlinNoise(Time.time, 0f) * 2 - 1) * shakeMagnitude;
    var y = (Mathf.PerlinNoise(0f, Time.time) * 2 - 1) * shakeMagnitude;
    return new Vector3(x, y, 0f);
} 

    public void DoFov(float endValue)
    {
        //GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

}
