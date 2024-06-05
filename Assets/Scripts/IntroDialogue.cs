using System;
using System.Collections;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogue : MonoBehaviour
{
    public TextMeshProUGUI textPlacer;
    public TextMeshProUGUI characterPlacer;
    public Image dialogueBackground;
    public CinemachineVirtualCamera[] camerasContainer;
    public GameObject player;
    
    private int currentLine;

    private int _totalCharacters;
    private int _currentChar;
    private float _timeBetweenCharacters = 0.2f;

    

    private String[] characters = { "Symon:", "Yara:"};

    private int[] charactersSequence = { 0, 0, 0, 0, 1, 1 };
    
    private String[] dialogue =
    {
        "Wow!",
        "Yara, Yara! Look!",
        "It's so cool!",
        "Can you teach me how to shoot it?",
        "Oh, careful! That's not a toy!",
        "Here, I'll show you."
    };
    
    void Start()
    {
        currentLine = -1;
        _currentChar = 0;
        dialogueBackground.gameObject.SetActive(false);
        foreach (var cam in camerasContainer)
        {
            cam.Priority = 21;
            cam.gameObject.SetActive(true);
        }
    }

    public void endCutscene()
    {
        foreach (var cam in camerasContainer)
        {
            cam.Priority = 0;
            cam.gameObject.SetActive(false);
            
        }
        
        
        var children = player.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = 7;
        }

        
    }

    public void nextDialogueLine()
    {
        dialogueBackground.gameObject.SetActive(true);
        
        // Updates the line counter and sets it in the text placer
        currentLine++;
        textPlacer.SetText(dialogue[currentLine]);
        characterPlacer.SetText(characters[charactersSequence[currentLine]]);
        
        // Updates the variables related to the sentence of dialogue
        _totalCharacters = dialogue[currentLine].Length;
        _currentChar = 0;
        
        // Sets the number of visible characters to 0
        textPlacer.maxVisibleCharacters = 0;
        
        // Calls the coroutine to show the rest of the sentence 
        StartCoroutine(showText());
    }
    
    private IEnumerator showText()
    {
        while (true)
        {
            // Increments the current char variable
            _currentChar++;

            // If the sentence is still not fully written/show, shows another character
            if (_currentChar <= _totalCharacters)
                textPlacer.maxVisibleCharacters = _currentChar;

            textPlacer.ForceMeshUpdate();
            yield return new WaitForSeconds(_timeBetweenCharacters);
        }
    }
    
}
