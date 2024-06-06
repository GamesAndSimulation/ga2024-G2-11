using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Cutscenes : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool _startedPlaying;
    
    public void Update()
    {
        if(videoPlayer.isPlaying)
            _startedPlaying = true;
        //when video is finishes, load the next scene
        if (videoPlayer.isPlaying == false && _startedPlaying)
        {
            SceneManager.LoadScene("World");
        }
    }
}
