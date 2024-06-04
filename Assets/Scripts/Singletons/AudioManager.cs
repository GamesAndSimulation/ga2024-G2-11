using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
 
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioSource> timeIndependentAudioSources = new List<AudioSource>();
 
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("AudioManager is null");
            }
            return instance;
        }
    }
 
    private void Awake()
    { 
        instance = this;
    }
 
    public void PlaySound(AudioClip clipToPlay, bool randomPitch = false, float volume = 0.4f)
    {
        audioSources.Add(this.AddComponent<AudioSource>());
        var audio = audioSources[audioSources.Count - 1];
        audio.volume = volume;
        audio.clip = clipToPlay;
        if(randomPitch)
        {
            audio.pitch = Random.Range(0.8f, 1.2f);
        }
        audio.Play();
        Destroy(audio, clipToPlay.length);
    }

    public void PlaySoundLooping(AudioClip clipToPlay, float volume = 0.4f, bool timeIndependent = false)
    {
        audioSources.Add(this.AddComponent<AudioSource>());
        var audio = audioSources[audioSources.Count - 1];
        if (timeIndependent)
            timeIndependentAudioSources.Add(audioSources[audioSources.Count - 1]);
        audio.clip = clipToPlay;
        audio.volume = volume;
        audio.loop = true;
        audio.Play();
    }
 
    public void StopSoundLooping()
    {
        foreach (var audio in this.GetComponents<AudioSource>())
        {
            if(audio.loop == true)
            {
                audio.loop = false;
                audio.Stop();
                Destroy(audio);
            }
        }
    }
    
    public void StartSlowMo(float slowMoScale)
    {
        foreach (AudioSource audio in this.GetComponents<AudioSource>())
        {
            if(timeIndependentAudioSources.Contains(audio))
                continue;
            
            float startPitch = audio.pitch;
            DOTween.To(() => audio.pitch, x => audio.pitch = x, startPitch * slowMoScale, 0.15f).SetEase(Ease.Linear);
        }
    }

    public void StopSlowMo(float slowMoScale)
    {
        foreach (AudioSource audio in this.GetComponents<AudioSource>())
        {
            if(timeIndependentAudioSources.Contains(audio))
                continue;
            
            audio.DOPitch(1, 0.6f).SetEase(Ease.Linear).OnComplete(() =>
            {
                audio.pitch = 1f;
            });
        }
    }
    
    private IEnumerator FadeOutSound(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
 
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}