using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EndGameCutscene : MonoBehaviour
{

    public GameObject button1;
    public GameObject button2;
    
    public GameObject friendDeadEnding;
    public GameObject brotherDeadEnding;
    
    private GameObject ending;

    public GameObject CreditsText;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(ButtonActiveDelay());
    }
    
    private IEnumerator ButtonActiveDelay()
    {
        yield return new WaitForSeconds(30f);
        button1.SetActive(true);
        button2.SetActive(true);
    }

    public void KillFriend()
    {
        ending = friendDeadEnding;
        ending.SetActive(true);
        Image image = ending.GetComponent<Image>();
        image.DOFade(1, 3f);
        CreditsText.SetActive(true);
        CreditsText.GetComponent<TextMeshProUGUI>().DOFade(1, 3f);
        CreditsText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1, 3f);
    }
    
    public void KillBrother()
    {
        ending = brotherDeadEnding;
        ending.SetActive(true);
        Image image = ending.GetComponent<Image>();
        image.DOFade(1, 3f);
        CreditsText.GetComponent<TextMeshProUGUI>().DOFade(1, 3f);
        CreditsText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1, 3f);
    }
    
}
