using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorScript : MonoBehaviour 
{
    private Vector3 _initialPos;
    private Vector3 _currPos;
    private int _speed;
    private int _initialSpeed = 5;
    private float _accumHeight;
    
    public GameManagerPipes gameManagerPipeGameObject;
    public Vector3 finalPos = new Vector3(470.3f, 36.0f, -367.7f); // distance can be positive or negative
    
    // Start is called before the first frame update
    void Start()
    {
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
        _initialPos = transform.localPosition;
        _currPos = transform.localPosition;
        _speed = _initialSpeed;
        _accumHeight = 0;

    }

    private void Update()
    {
        
        
        // After the pipes are connected, starts vibrating to simulate the steam passing through
        if (gameManagerPipeGameObject.IsGameWon() && CanStillMove())
        {
            var speed = 5.0f;
            var intensity = 0.1f;

            if (_speed == 0)
            {
                _accumHeight += 0.2f;
                _speed = _initialSpeed;
            } else
                _speed -= 1;
            

            Debug.Log("Code vibrating");
            
            // Vibrating code
            transform.localPosition = _initialPos + new Vector3(0, _accumHeight, 0) + intensity * new Vector3(
                Mathf.PerlinNoise(speed * Time.time, 1),
                Mathf.PerlinNoise(speed * Time.time, 2),
                Mathf.PerlinNoise(speed * Time.time, 3));
        }
            
    }

    private bool CanStillMove()
    {
        //bool x = transform.localPosition.x <= finalPos.x;
        bool y = transform.localPosition.y <= finalPos.y;
        //bool z = transform.localPosition.z <= finalPos.z;
        
        return y;
    }
}