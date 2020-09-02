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
        levelSelectionCanvas.enabled = false;
        mainCanvas.enabled = true;
    }

    public void LoadTuto()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadHuman()
    {
        SceneManager.LoadScene("Human");
    }

    public void LoadCow()
    {
        SceneManager.LoadScene("Cow");
    }

    public void LoadAnt()
    {
        SceneManager.LoadScene("Ant");
    }

    public void LoadAlien()
    {
        SceneManager.LoadScene("Alien");
    }
}
