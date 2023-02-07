using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSession : MonoBehaviour
{
    [SerializeField] private string SceneNameToLoad;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneNameToLoad);
    }
}
