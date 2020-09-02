using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public Canvas levelSelectionCanvas;
    public Canvas optionsCanvas;

    Canvas mainCanvas;

    private void Start()
    {
        mainCanvas = GetComponent<Canvas>();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActivateLevelSelectionCanvas()
    {
        mainCanvas.enabled = false;
        levelSelectionCanvas.enabled = true;
    }

    public void ActivateOptionsCanvas()
    {
        mainCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }
}
