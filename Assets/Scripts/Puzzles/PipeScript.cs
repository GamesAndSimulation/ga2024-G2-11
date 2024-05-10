using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePuzzle : MonoBehaviour 
{
    private readonly float[] _rotations = { 0, 90, 180, 270 };
    private bool _isPlacedCorrectly = false;

    public float[] correctRotation;
    public bool isPartOfSolution = false;

    public GameManagerPipes gameManagerPipeGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
            
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
