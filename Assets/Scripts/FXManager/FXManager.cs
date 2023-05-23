using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [Header("Liste de Particule")] 
    public Particules[] particules;

    /*private static FXManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }*/
    
    //FindObjectOfType<FXManager>().Play("NomDeParticule", transform);
    public void Play(string name, Transform context)
    {
        Particules s = Array.Find(particules, parti => parti.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Particule : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
        }

        Vector3 instPos = new Vector3(context.position.x, s.particule.transform.position.y, context.position.z);
        GameObject inst = Instantiate(s.particule, instPos, Quaternion.identity, context);
        inst.name = s.name;
    }
    
    //FindObjectOfType<FXManager>().Play("NomDeParticule", transform);
    public void PlayWoParent(string name, Transform context)
    {
        Particules s = Array.Find(particules, parti => parti.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Particule : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
        }

        Vector3 instPos = new Vector3(context.position.x, s.particule.transform.position.y, context.position.z);
        GameObject inst = Instantiate(s.particule, instPos, Quaternion.identity);
        inst.name = s.name;
    }
    
    //FindObjectOfType<FXManager>().Stop("NomDeParticule", transform);
    public void Stop(string name, Transform context)
    {
        Particules s = Array.Find(particules, parti => parti.name == name);
        if (s == null)
        {
            Debug.LogWarning("La Particule : " + name + " n'existe pas... Oublier de le mettre ou mal écrit");
        }

        Transform inst = context.Find(s.name);
        if(inst != null)
        {
            Destroy(inst.gameObject);
        }
    }
    
    //FindObjectOfType<FXManager>().StopAll(transform);
    public void StopAll(Transform context)
    {
        foreach (Particules p in particules)
        {
            Transform inst = context.Find(p.name);
            if(inst != null)
            {
                inst.gameObject.GetComponent<ParticleSystem>().Stop();
                Destroy(inst.gameObject);
            }
        }
    }
}
