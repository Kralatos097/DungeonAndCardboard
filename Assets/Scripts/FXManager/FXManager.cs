using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [Header("Liste de Particule")] 
    public Particules[] particules;
    
    public static FXManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    //FindObjectOfType<FXManager>().Play("NomDeParticule", transform);
    public void Play (string name, Transform context)
    {
        Particules s = Array.Find(particules, parti => parti.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Particule : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
        }

        Instantiate(s.particule, context.position, transform.rotation, transform);
    }
}
