using System;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

public class 
    AudioManager : MonoBehaviour
{
    public float timer;
    
    public float volumeMusic, volumeSfx, volumeVoice;
    
    [Header("Liste de Musique")]
    public Music[] music;
    
    [Header("Liste de Son")]
    public SFX[] sfx;
    
    [Header("Liste de Voix")]
    public Voice[] voice;

    private static AudioManager instance;

    private AudioSource _sfxSource;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        foreach (Music s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume*volumeMusic;
            s.source.loop = s.loop;
        }
        _sfxSource = gameObject.AddComponent<AudioSource>();
        
        foreach (Voice s in voice)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume*volumeVoice;
        }
    }

    //Joue un son au démarage 
    private void Start()
    {
        //Play("Theme");
    }
    
    //Joue une musique depuis le début : FindObjectOfType<AudioManager>().Play("NomDuSon");
    public void Play(string name)
    {
        Music s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Musique : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }
            
        s.source.Play();
    }
    
    //Arrête une musique : FindObjectOfType<AudioManager>().Stop("NomDuSon");
    public void Stop(string name)
    {
        Music s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Musique : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }
            
        s.source.Stop();
    }
    
    public void AllMusicStop()
    {
        foreach (Music s in music)
        {
            if (s == null)
            {
                Debug.LogWarning("La Musique : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
                return;
            }
            
            s.source.Stop();
        }
    }
    
    //Met en pause une musique : FindObjectOfType<AudioManager>().Pause("NomDuSon");
    public void Pause(string name)
    {
        Music s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Musique : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }
            
        s.source.Pause();
    }
    
    //Reprend une musique en pause : FindObjectOfType<AudioManager>().UnPause("NomDuSon");
    public void UnPause(string name)
    {
        Music s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Musique : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }
            
        s.source.UnPause();
    }
    
    //Joue un son : FindObjectOfType<AudioManager>().OneShot("NomDuSon");
    public void OneShot(string name)
    {
        SFX s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Son : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }

        _sfxSource.volume = s.volume * volumeSfx;
        _sfxSource.pitch = s.pitch;
        _sfxSource.PlayOneShot(s.clip);
        
        
    }
    
    //Produit un son avec un pitch aléatoire : FindObjectOfType<AudioManager>().RandomPitch("NomDuSon");
    public void RandomPitch(string name)
    {
        float alea = Random.Range(0.6f, 1.5f);
        
        SFX s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Le Son : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
            return;
        }

        _sfxSource.volume = s.volume * volumeSfx;
        _sfxSource.pitch = s.pitch * alea;
        _sfxSource.PlayOneShot(s.clip);
    }

    //Je n'est pas mis de fonction pour jouer de voix
    //Ajouter des fonctions Fade In / Out pour les musiques
}
