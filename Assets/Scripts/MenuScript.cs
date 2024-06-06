using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;

public class MenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas mainMenuCanvas;
    public PlayableDirector introScene;

    public Button PlayButton;
    private Vector3 playInitialPosition;
    
    private void Start()
    {
        playInitialPosition = PlayButton.GetComponent<RectTransform>().anchoredPosition;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogWarning($"Pointer entered {eventData.pointerEnter.name}");
        if(eventData.pointerEnter == PlayButton.gameObject)
        {
            PlayButton.GetComponent<RectTransform>().DOAnchorPosX(playInitialPosition.x + 100, 0.5f);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.LogWarning($"Pointer exited {eventData.pointerEnter.name}");
        if(eventData.pointerEnter == PlayButton.gameObject)
        {
            PlayButton.GetComponent<RectTransform>().DOAnchorPosX(playInitialPosition.x, 0.5f);
        }
    }

    public void play()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        Debug.Log("Play clicked");
        SceneManager.LoadScene("FirstCutscenes");
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
