using System;
using TMPro;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    public TextMeshProUGUI textPlacer;
    private int currentLine = -1;
    
    private String[] dialogue =
    {
        "Ooooh!",
        "Yara, Yara! Look!",
        "It's so cool!",
        "Can you teach me how to shoot it?",
        "Wow, careful! That's not a toy!",
        "Here, I'll show you."
    };
    
    void Start()
    {
        currentLine = -1;
    }

    public void nextDialogueLine()
    {
        currentLine++;
        textPlacer.SetText(dialogue[currentLine]);
    }
    
}
