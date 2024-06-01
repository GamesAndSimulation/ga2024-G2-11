using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenEffectUtils : MonoBehaviour
{

    
    private static ScreenEffectUtils _instance;
    
    public static ScreenEffectUtils Instance
    {
        get
        {
            return _instance;
        }
    }
    
    private static CinemachineVirtualCamera _virtualCamera;
    private static Volume _postProcessingVolume;
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        _virtualCamera = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _postProcessingVolume = GetComponentInChildren<Volume>();
    }

    public void DamageEffect()
    {
        StartCoroutine(DamageEffectRoutine());
    }
    
    private IEnumerator DamageEffectRoutine()
    {
        DOTween.To(() => _postProcessingVolume.weight, x => _postProcessingVolume.weight = x, 1f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        DOTween.To(() => _postProcessingVolume.weight, x => _postProcessingVolume.weight = x, 0f, 0.2f);
    }
    
    public void ShakeScreen(float shakeDuration, float shakeAmplitude)
    {
        StartCoroutine(ShootScreenShake(shakeDuration, shakeAmplitude));
    }
    
    private IEnumerator ShootScreenShake(float shakeDuration, float shakeAmplitude)
    {
        var noiseComponent = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noiseComponent.m_AmplitudeGain = shakeAmplitude;
        yield return new WaitForSeconds(shakeDuration);
        noiseComponent.m_AmplitudeGain = 0.0f;
    }
    
}
