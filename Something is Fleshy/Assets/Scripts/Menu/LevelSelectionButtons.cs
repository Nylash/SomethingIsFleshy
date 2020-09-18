using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSelectionButtons : MonoBehaviour
{
    public Canvas mainCanvas;

    Canvas levelSelectionCanvas;

    private void Start()
    {
        levelSelectionCanvas = GetComponent<Canvas>();
    }

    public void BackToMainCanvas()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        levelSelectionCanvas.enabled = false;
        mainCanvas.enabled = true;
    }

    public void LoadTuto()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadHuman()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        SceneManager.LoadScene("Human");
    }

    public void LoadCow()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        SceneManager.LoadScene("Cow");
    }

    public void LoadAnt()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        SceneManager.LoadScene("Ant");
    }

    public void LoadAlien()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
        SceneManager.LoadScene("Alien");
    }
}
