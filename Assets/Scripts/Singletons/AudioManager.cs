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
    public List<AudioSource> immuneSources;
 
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
    
    public void AddImmuneSources()
    {
        foreach (var source in this.GetComponents<AudioSource>())
        {
            if(source.loop)
                immuneSources.Add(source);
        }
    }
    
    public void PlaySound(AudioClip clipToPlay, bool randomPitch = false, float volume = 0.4f, bool timeIndependent = false)
    {
        audioSources.Add(this.AddComponent<AudioSource>());
        var audio = audioSources[audioSources.Count - 1];
        if (timeIndependent)
            timeIndependentAudioSources.Add(audioSources[audioSources.Count - 1]);
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
            if(audio.loop && !immuneSources.Contains(audio))
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
    
    public void PlaySoundAtPosition(AudioClip clipToPlay, Vector3 position, float volume = 0.4f, bool randomPitch = false, bool timeIndependent = false)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clipToPlay;
        audioSource.volume = volume;
        if (randomPitch)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
        audioSource.spatialBlend = 1.0f; // make the audio fully 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        audioSource.Play();
        Destroy(tempGO, clipToPlay.length);

        audioSources.Add(audioSource);
        if (timeIndependent)
        {
            timeIndependentAudioSources.Add(audioSource);
        }
    }
    
    public void FadeOutAllLoopingSounds(float fadeTime)
    {
        foreach (var audio in this.GetComponents<AudioSource>())
        {
            if (audio.loop && !immuneSources.Contains(audio))
            {
                StartCoroutine(FadeOutSound(audio, fadeTime));
            }
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