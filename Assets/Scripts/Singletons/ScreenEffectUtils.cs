using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

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
