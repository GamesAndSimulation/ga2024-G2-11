using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MenuScript : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public PlayableDirector introScene;

    public void play()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        Debug.Log("Play clicked");
        introScene.Play();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    
    void OnGUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void options()
    {
        Debug.Log("Options clicked");
    }
    
    public void quit()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }
}
