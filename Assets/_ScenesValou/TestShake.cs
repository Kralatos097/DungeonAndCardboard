using EZCameraShake;
using UnityEngine;

public class TestShake : MonoBehaviour
{
    private void Update()
    {
        //crit
        if (Input.GetMouseButtonDown(1))
        {
            CameraShaker.Instance.ShakeOnce(1f, 10f, .1f,1f);
        }
        
        //hit
        if (Input.GetMouseButtonDown(0))
        {
            CameraShaker.Instance.ShakeOnce(1f, 3f, .1f,0.4f);
        }
    }
}
