using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public GameObject player;

    public void Update()
    {
        if(player == null)
        {
            LoseScene();
        }
    }
    public void Start()
    {
        if (!PlayerPrefs.HasKey("InitialRooms"))
        {
            PlayerPrefs.SetInt("InitialRooms", 6); 
        }
    }

    public void SetHardness(int num)
    {
        PlayerPrefs.SetInt("InitialRooms", num);
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void WinScene()
    {
        Invoke("StartWinScene", 1f);
    }
    public void LoseScene()
    {
        Invoke("StartLoseScene", 1f);
    }

    private void StartWinScene()
    {
        SceneManager.LoadScene("WinScene");
    }
    private void StartLoseScene()
    {
        SceneManager.LoadScene("LoseScene");
    }
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
#else
        Application.Quit(); // Quits the game in a built application
#endif
    }
}
