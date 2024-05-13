using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlatformScript : MonoBehaviour
{
    
    private Vector3 _initialPos;
    private Vector3 _currPos;
    private float _currDir;

    public float speed = 5.0f;
    private Vector3 maxPos;
    public float initialDirection = 1.0f;
    
    public GameManagerPipes gameManagerPipeGameObject; 
    
    // Start is called before the first frame update
    void Start()
    {
        _initialPos = transform.localPosition;
        _currPos = _initialPos;
        
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Platform is going from _initialPos to _maxPos
        // I want to check if it's getting close from the destination, so basically, if the distance is bigger than a
        // certain constant
        
        // Then, if it is, reduce the speed until it achieves the same value, but negative
        
        // Position should be calculated with the speed and the speed must be positive or negative 
        
        _currPos = transform.localPosition;
        
        if (gameManagerPipeGameObject.IsGameWon())
        {
            // If the platform is going in the z direction, but still didn't reach 
            if (_currPos.z > _initialPos.z && _currPos.z < maxPos.z)
            {
                
            }
        }
    }
}
