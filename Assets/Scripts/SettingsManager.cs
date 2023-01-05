using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle FullScreen;
    private bool IsWindow = true;

    private void Update()
    {
        //Bouton Fullscreen
        if (FullScreen.isOn && IsWindow)
        {
            IsWindow = false;
            Debug.Log("Ahah fullScreen");
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        if (FullScreen.isOn == false && !IsWindow)
        {
            IsWindow = true;
            Debug.Log("Bruh Window");
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        
        //Check
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            IsWindow = false;
            FullScreen.isOn = true;
        }
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            IsWindow = true;
            FullScreen.isOn = false;
        }
    }
}
