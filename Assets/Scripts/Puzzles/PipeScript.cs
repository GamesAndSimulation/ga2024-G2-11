using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PipeScript : MonoBehaviour 
{
    private readonly float[] _rotations = { 0, 90, 180, 270 };
    public bool _isPlacedCorrectly = false;

    public float[] correctRotation;
    public bool isPartOfSolution = false;

    public GameManagerPipes gameManagerPipeGameObject;

    private Vector3 _initialPos;
    private int _currRotationIndex;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
        _initialPos = transform.localPosition;
            
        int rand = Random.Range(0, _rotations.Length);
        _currRotationIndex = rand;

        Debug.Log($"PIPE {gameObject.name} initial random rotation: {_rotations[rand]}");

        // Sets a random rotation
        transform.eulerAngles = new Vector3(0, 0, _rotations[rand]);

        Debug.Log($"PIPE {gameObject.name} rotation after applying random rotation: {transform.eulerAngles.z}");

        if (isPartOfSolution)
        {
            // Check if the pipe is correctly placed from the start
            for (int i = 0; i < correctRotation.Length; i++)
            {
                if (Mathf.Approximately(_rotations[_currRotationIndex], correctRotation[i]))
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
        if (gameManagerPipeGameObject.IsGameWon() && isPartOfSolution)
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
        // Round incrementation of the current index
        _currRotationIndex = (_currRotationIndex + 1) % _rotations.Length;

        // Increments 90 degrees
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

                Debug.Log(i);
                Debug.Log("PIPE euler" + transform.eulerAngles.z);
                Debug.Log("PIPE correct rotation" + correctRotation[i]);
                
                // If it is placed correctly, note that
                // Compares the current rotation with the available correct ones
                if (Mathf.Approximately(_rotations[_currRotationIndex], correctRotation[i]))
                {
                    _isPlacedCorrectly = true;
                    gameManagerPipeGameObject.AddCorrectPipe();
                    break;
                }
            }
        }
    }
}
