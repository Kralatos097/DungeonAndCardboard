using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLand : MonoBehaviour
{
    [SerializeField] private string LesCouillesDeTibo;
    public void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play(LesCouillesDeTibo);
    }
    
    public void StopSound()
    {
        FindObjectOfType<AudioManager>().Stop(LesCouillesDeTibo);
    }
    
    public void PauseSound()
    {
        FindObjectOfType<AudioManager>().Pause(LesCouillesDeTibo);
    }
    
    public void UnPauseSound()
    {
        FindObjectOfType<AudioManager>().UnPause(LesCouillesDeTibo);
    }
}
