using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class GameManagerPipes : MonoBehaviour
{
    public GameObject PipesHolder;
    public GameObject[] Pipes;
    public GameObject Gear;

    public int criticalPathPipes = 0;
    public int correctPipes = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        criticalPathPipes = PipesHolder.transform.childCount;
        Pipes = new GameObject[criticalPathPipes];

        for (int i = 0; i < Pipes.Length; i++)
        {
            Pipes[i] = PipesHolder.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (correctPipes == criticalPathPipes)
        {
            Debug.Log("You won the game");
            Gear.transform.Rotate(0, 0, 5,  Space.Self);
            
        }
    }

    public void AddCorrectPipe()
    {
        correctPipes++;
        Debug.Log("Added pipe");
    }

    public void RemoveCorrectPipe()
    {
        correctPipes--;
        Debug.Log("Removed pipe");
    }

    public bool IsGameWon()
    {
        return correctPipes == criticalPathPipes;
    }
}
