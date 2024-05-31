using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIFadeInOut : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.DOFade(0, 0.8f).SetLoops(-1, LoopType.Yoyo);
    }
}