using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    
    private Vector3 _initialPos;
    private Vector3 _maxPos;
    private Vector3 _currPos;
    private float _currDir;

    public float Speed = 5.0f;
    public float Distance = 0;
    
    public GameManagerPipes gameManagerPipeGameObject; 
    
    // Start is called before the first frame update
    void Start()
    {
        _initialPos = transform.localPosition;
        _maxPos = _initialPos + new Vector3(0, 0, Distance);
        
        gameManagerPipeGameObject = GameObject.Find("GameManagerPipes").GetComponent<GameManagerPipes>();
    }

    // Update is called once per frame
    void Update()
    {
        _currPos = transform.localPosition;
        
        if (gameManagerPipeGameObject.IsGameWon())
        {
            // If the platform is going in the z direction, but still didn't reach 
            if (_currPos.z > _initialPos.z && _currPos.z < _maxPos.z)
            {
                
            }
        }
    }
}
