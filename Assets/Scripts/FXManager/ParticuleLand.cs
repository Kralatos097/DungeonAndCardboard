using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleLand : MonoBehaviour
{
    public GameObject PlayerModel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) FindObjectOfType<FXManager>().Play("Heal", PlayerModel.transform);
        if(Input.GetKeyDown(KeyCode.C)) FindObjectOfType<FXManager>().Play("Cure", PlayerModel.transform);

    }
}
