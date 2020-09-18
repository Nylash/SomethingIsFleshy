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
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        mainCanvas.enabled = false;
        levelSelectionCanvas.enabled = true;
    }

    public void ActivateOptionsCanvas()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        mainCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }
}
