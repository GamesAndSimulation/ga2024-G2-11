using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FpsCounter : MonoBehaviour
{

    private TextMeshProUGUI _fpsText;
    public float fpsUpdateDelay;
    private float _fpsUpdateDelayCounter;

    private void Start()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _fpsUpdateDelayCounter -= Time.deltaTime;
        if (_fpsUpdateDelayCounter <= 0)
        {
            _fpsUpdateDelayCounter = fpsUpdateDelay;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            _fpsText.text = (1f / Time.unscaledDeltaTime).ToString("F1", ci) + " FPS";
        }
    }
}
