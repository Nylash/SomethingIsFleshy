using UnityEngine.SceneManagement;
using UnityEngine;

public class TrailerScenesSelection : MonoBehaviour
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

    public void Load1()
    {
        SceneManager.LoadScene("1");
    }
    public void Load2()
    {
        SceneManager.LoadScene("2");
    }
    public void Load3()
    {
        SceneManager.LoadScene("3");
    }
    public void Load4()
    {
        SceneManager.LoadScene("4");
    }
    public void Load5()
    {
        SceneManager.LoadScene("5");
    }
    public void Load6()
    {
        SceneManager.LoadScene("6");
    }
}
