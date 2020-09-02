using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashArt : MonoBehaviour
{
    InputAction anyButton;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void EnableLoading()
    {
        anyButton = new InputAction(binding: "/*/<button>");
        anyButton.Enable();
        anyButton.started += ctx => LoadGame();
    }

    void LoadGame()
    {
        anyButton.Disable();
        SceneManager.LoadScene("MainMenu");
    }
}
