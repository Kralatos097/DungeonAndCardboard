using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectWhenAnimFinish : MonoBehaviour
{
    public GameObject DestroyThis;

    public void DestroyNow()
    {
        Destroy(DestroyThis);
    }
}
