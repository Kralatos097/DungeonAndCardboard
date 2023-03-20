using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenePause : MonoBehaviour
{
    public void ToMainMenu()
    {
        /*SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(DungeonManager.GetDungeonSceneName()));*/
        SceneManager.LoadScene("_MainScenes/MainMenu");
        SceneManager.LoadScene("_MainScenes/MainMenu");
    }
}
