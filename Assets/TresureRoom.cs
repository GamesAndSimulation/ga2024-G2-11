using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TresureRoom : MonoBehaviour
{
    public Light light;
    public GameObject EssenceThing;
    public AudioClip WooshSound;
    
    void Start()
    {
        light.DOIntensity(119.8f, 3f).SetEase(Ease.InOutSine);
    }
    
    public void ExitCavern()
    {
        StartCoroutine(ExitCavernRoutine());
    }
    
    private IEnumerator ExitCavernRoutine()
    {
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySound(WooshSound);
        EssenceThing.transform.DOMoveY(10f, 3.5f);
        yield return new WaitForSeconds(3.5f);
        GameManager.Instance.SetEnableLoadScreen(true);
        GameManager.Instance.AddWhaleBlood();
        if (GameManager.Instance.GetWhaleBlood() >= 2)
            SceneManager.LoadScene("EndGame");
        else
            SceneManager.LoadScene("World");
    }

}
