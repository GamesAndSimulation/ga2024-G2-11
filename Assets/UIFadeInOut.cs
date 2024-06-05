using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIFadeInOut : MonoBehaviour
{
    public enum UIFadeState
    {
        PingPong,
        FadeIn
    }
    
    public UIFadeState FadeType;
    public float FadeTime = 0.8f;
    public float StartDelay;
    
    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        _text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(PreformFade());
    }
    
    private IEnumerator PreformFade()
    {
        yield return new WaitForSeconds(StartDelay);
        switch (FadeType)
        {
            case UIFadeState.PingPong:
                _text.DOFade(0, FadeTime).SetLoops(-1, LoopType.Yoyo);
                break;
            case UIFadeState.FadeIn:
                _text.DOFade(1, FadeTime).SetEase(Ease.InOutSine);
                break;
        }
    }
}