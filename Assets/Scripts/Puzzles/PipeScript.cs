using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PipePuzzle : MonoBehaviour 
{
    private readonly float[] _rotations = { 0, 90, 180, 270 };
    public bool _isPlacedCorrectly = false;

    public float[] correctRotation;
    public bool isPartOfSolution = false;

    public GameManagerPipes gameManagerPipeGameObject;

    private Vector3 _initialPos;

    // Start is called before the first frame update
    void Start()
    {
        
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
        _initialPos = transform.localPosition;
            
        int rand = Random.Range(0, _rotations.Length);
        transform.eulerAngles = new Vector3(0, 0, _rotations[rand]);

        if (isPartOfSolution)
        {
            // Check if the pipe is correctly placed from the start
            for (int i = 0; i < correctRotation.Length; i++)
            {
                if (transform.eulerAngles.z == correctRotation[i])
                {
                    _isPlacedCorrectly = true;
                    gameManagerPipeGameObject.AddCorrectPipe();
                    break;
                }
            }
        }
    }

    private void Update()
    {
        // After the pipes are connected, starts vibrating to simulate the steam passing through
        if (gameManagerPipeGameObject.IsGameWon())
        {
            var speed = 5.0f;
            var intensity = 0.1f;
            
            // Vibrating code
            transform.localPosition = _initialPos + intensity * new Vector3(
                Mathf.PerlinNoise(speed * Time.time, 1),
                Mathf.PerlinNoise(speed * Time.time, 2),
                Mathf.PerlinNoise(speed * Time.time, 3));
        }
            
    }

    void OnMouseDown()
    {
        transform.Rotate(new Vector3(0, 0, 90));

        if (isPartOfSolution)
        {

            for (int i = 0; i < correctRotation.Length; i++)
            {
                // If it was previously placed correctly, and now it's not, remove the flag
                if (_isPlacedCorrectly)
                {
                    _isPlacedCorrectly = false;
                    gameManagerPipeGameObject.RemoveCorrectPipe();
                }

                // If it is placed correctly, note that
                if (transform.eulerAngles.z == correctRotation[i])
                {
                    _isPlacedCorrectly = true;
                    gameManagerPipeGameObject.AddCorrectPipe();
                    break;
                }
            }
        }


    }
}
