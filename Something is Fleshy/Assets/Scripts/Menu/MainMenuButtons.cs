using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
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

    public void ActivateOptionsCanvas()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        mainCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }
}
