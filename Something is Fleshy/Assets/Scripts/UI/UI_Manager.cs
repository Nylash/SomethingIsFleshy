using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("UI Objects")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public Canvas UI_startCanvas;
    public TextMeshProUGUI UI_startText;
    public TextMeshProUGUI UI_timerValue;
    public Canvas UI_defeatCanvas;
    public TextMeshProUGUI UI_defeatFullTimer;
    public TextMeshProUGUI UI_defeatActualTimer;
    public Canvas UI_victoryCanvas;
    public Canvas UI_pauseCanvas;
    public Canvas UI_leakGaugeCanvas;
    public Image UI_leakGaugeIn;
    public Canvas UI_timerCanvas;
    [Header("Components")]
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Pause.started += ctx => Pause();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial")
            StartCoroutine(AnimStart());
        else
            StartGame();
    }

    public void Pause()
    {
        if (!HeartManager.instance.defeatOrVictory)
        {
            UI_pauseCanvas.enabled = !UI_pauseCanvas.enabled;
            if (UI_pauseCanvas.enabled)
            {
                Time.timeScale = 0;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                Time.timeScale = 1;
            }
        }
    }

    IEnumerator AnimStart()
    {
        yield return new WaitForSeconds(1);
        UI_startText.text = "Starts in 2";
        yield return new WaitForSeconds(1);
        UI_startText.text = "Starts in 1";
        CameraManager.instance.SwitchCamera();
        yield return new WaitForSeconds(1);
        UI_startText.text = "Go !";
        yield return new WaitForSeconds(1);
        StartGame();
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    void Dezoom()
    {
        CameraManager.instance.SwitchCamera();
    }

    void StartGame()
    {
        UI_startCanvas.enabled = false;
        GameManager.instance.levelStarted = true;
        SecondarySystemsManager.instance.StartGame();
        LeaksManager.instance.StartGame();
    }
}
