using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTipHandler : MonoBehaviour
{
    public AudioClip gameTipSound;
    public string gameTipTag;
    
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ShowGameTip());
    }
    
    private IEnumerator ShowGameTip()
    {
        GetComponent<BoxCollider>().enabled = false;
        GameTipHandler[] gameTipHandlers = FindObjectsOfType<GameTipHandler>();
        foreach (GameTipHandler gameTipHandler in gameTipHandlers)
        {
            if (gameTipHandler.gameTipTag == gameTipTag)
            {
                gameTipHandler.GetComponent<BoxCollider>().enabled = false;
            }
        }
        GameObject tipObj = GameObject.FindWithTag(gameTipTag);
        AudioManager.Instance.PlaySound(gameTipSound, false, 4f);
        foreach(Transform child in tipObj.transform)
        {
            if (child.GetComponent<RawImage>() != null)
            {
                //Fade in
                child.GetComponent<RawImage>().DOFade(0.96f, 0.5f);
            }
            else
            {
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.DOFade(1, 0.5f);
                }
            }
        }
        
        yield return new WaitForSeconds(5f);
        
        foreach(Transform child in tipObj.transform)
        {
            if (child.GetComponent<RawImage>() != null)
            {
                //Fade out
                child.GetComponent<RawImage>().DOFade(0f, 0.5f);
            }
            else
            {
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.DOFade(0, 0.5f);
                }
            }
        }
    }
}
