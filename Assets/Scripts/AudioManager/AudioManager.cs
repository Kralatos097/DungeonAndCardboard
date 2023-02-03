using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class 
    AudioManager : MonoBehaviour
{
    
    [SerializeField] public float volumeMusic, volumeSfx, volumeVoice;
    [MinMaxSlider(0, 2)]
    [SerializeField] private Vector2 pitchMinMax;
    
    [Header("Liste de Musique")]
    public Music[] music;
    
    [Header("Liste de Son")]
    public SFX[] sfx;
    
    [Header("Liste de Voix")]
    public Voice[] voice;

    private static AudioManager instance;
    
    private static AudioSource _musicSource;
    private static AudioSource _musicSourceTwo;
    private static AudioSource _sfxSource;
    
    private float _timer = 0;
    [SerializeField] private float fadeDuration;
    private bool _isFading = false;
    private int _currTrack = 2;
    private float[] _trackVolume = new float[]{0,0};
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            volumeMusic = PlayerPrefs.GetFloat("MusicVolume", 0);
        
        if (PlayerPrefs.HasKey("SFXVolume"))
            volumeSfx = PlayerPrefs.GetFloat("SFXVolume", 0);
        
        if (PlayerPrefs.HasKey("VoiceVolume"))
            volumeVoice = PlayerPrefs.GetFloat("VoiceVolume", 0);
        
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSourceTwo = gameObject.AddComponent<AudioSource>();
        
        _musicSource.volume = volumeMusic;
        _musicSourceTwo.volume = volumeMusic;
        
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.volume = volumeSfx;
        
        foreach (Voice s in voice)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume*volumeVoice;
        }
    }

    private void Update()
    {
        if (!_isFading) return;

        _timer += Time.deltaTime/fadeDuration;
        if(_currTrack == 1)
        {
            _musicSource.volume = Mathf.Lerp(0, _trackVolume[1], _timer);
            _musicSourceTwo.volume = Mathf.Lerp(_trackVolume[0], 0, _timer);
            if(_musicSourceTwo.volume == 0)
            {
                _isFading = false;
            }
        }
        else
        {
            _musicSource.volume = Mathf.Lerp(_trackVolume[0], 0, _timer);
            _musicSourceTwo.volume = Mathf.Lerp(0, _trackVolume[1], _timer);
            if(_musicSource.volume == 0)
            {
                _isFading = false;
            }
        }
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

        if(_currTrack == 1)
        {
            _trackVolume[0] = _musicSource.volume;
            _trackVolume[1] = s.volume * volumeMusic;
            _musicSourceTwo.clip = s.clip;
            _musicSourceTwo.volume = s.volume * volumeMusic;
            _musicSourceTwo.loop = s.loop;
            _musicSourceTwo.Play();
            _currTrack = 2;
        }
        else
        {
            _trackVolume[0] = s.volume * volumeMusic;
            _trackVolume[1] = _musicSourceTwo.volume;
            _musicSource.clip = s.clip;
            _musicSource.loop = s.loop;
            _musicSource.Play();
            _currTrack = 1;
        }
        _timer = 0;
        _isFading = true;
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
        float alea = Random.Range(pitchMinMax.x, pitchMinMax.y);
        
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
    
    public void ChangeMusicVolume(float volume)
    {
        volumeMusic = volume;

        if (_musicSource.isPlaying)
        {
            AudioClip a = _musicSource.clip;
            Music s = Array.Find(music, sound => sound.clip == a);
            _musicSource.volume = volume * s.volume;
        }

        if (_musicSourceTwo.isPlaying)
        {
            AudioClip b = _musicSourceTwo.clip;
            Music t = Array.Find(music, sound => sound.clip == b);
            _musicSourceTwo.volume = volume * t.volume;
        }
    
    }
    public void ChangeSFXVolume(float volume)
    {
        volumeSfx = volume;
    }
    
    public void ChangeVoiceVolume(float volume)
    {
        volumeVoice = volume;
    }
}
