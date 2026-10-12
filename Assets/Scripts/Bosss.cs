using UnityEngine;

public class Bosss : MonoBehaviour
{
    public GameObject Boss;
    public GameObject Player;

    private void Start()
    {
        Player = FindAnyObjectByType<CharacterMovement>().gameObject;
    }
    private void Update()
    {
        if (Boss == null)
        {
            SceneManagement sceneManager = FindAnyObjectByType<SceneManagement>();
            if (sceneManager != null)
            {
                sceneManager.WinScene();
            }
            else
            {
                Debug.LogError("SceneManagement component not found in the scene.");
            }
        }
        if (Player == null)
        {
            SceneManagement sceneManager = FindAnyObjectByType<SceneManagement>();
            if (sceneManager != null)
            {
                sceneManager.LoseScene();
            }
            else
            {
                Debug.LogError("SceneManagement component not found in the scene.");
            }
        }

    }
}
