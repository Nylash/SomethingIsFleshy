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
    public Canvas UI_scoreCanvas;
    public TextMeshProUGUI UI_scoreValue;
    public Canvas UI_startCanvas;
    public TextMeshProUGUI UI_startText;
    public TextMeshProUGUI UI_timerValue;
    public Canvas UI_endCanvas;
    public TextMeshProUGUI UI_endScore;
    public Image UI_medal;
    public TextMeshProUGUI UI_goldScore;
    public TextMeshProUGUI UI_silverScore;
    public TextMeshProUGUI UI_bronzeScore;
    public Canvas UI_pauseCanvas;
    public Canvas UI_leakGaugeCanvas;
    public Image UI_leakGaugeIn;
    public Canvas UI_timerCanvas;
    public AudioSource UI_audioSource;
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
        UI_audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name != "Tutorial")
            StartCoroutine(AnimStart());
        else
            StartGame();
    }

    public void Pause()
    {
        if (!ScoreManager.instance.levelEnded)
        {
            UI_pauseCanvas.enabled = !UI_pauseCanvas.enabled;
            GameManager.instance.levelPaused = !GameManager.instance.levelPaused;
            if (UI_pauseCanvas.enabled)
            {
                HintSecondarySystemManager.instance.DisableOnPause();
                foreach (TimerSecondarySystem item in GameObject.FindObjectsOfType<TimerSecondarySystem>())
                    item.DisableOnPause();
                Time.timeScale = 0;
            }
            else
            {
                SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, UI_audioSource);
                EventSystem.current.SetSelectedGameObject(null);
                Time.timeScale = 1;
            }
        }
    }

    IEnumerator AnimStart()
    {
        yield return new WaitForSeconds(1);
        UI_startText.text = "Starts in 2";
        UI_startCanvas.GetComponent<Animator>().SetTrigger("AnimSize");
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Ready, UI_audioSource);
        yield return new WaitForSeconds(1);
        UI_startText.text = "Starts in 1";
        UI_startCanvas.GetComponent<Animator>().SetTrigger("AnimSize");
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Set, UI_audioSource);
        CameraManager.instance.SwitchCameraFromScript();
        yield return new WaitForSeconds(1);
        UI_startText.text = "Go !";
        UI_startCanvas.GetComponent<Animator>().SetTrigger("AnimSize");
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Go, UI_audioSource);
        yield return new WaitForSeconds(1);
        StartGame();
    }

    public void ReloadLevel()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, UI_audioSource);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, UI_audioSource);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTrailerMainMenu()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, UI_audioSource);
        Time.timeScale = 1;
        SceneManager.LoadScene("TrailerMenu");
    }

    void StartGame()
    {
        UI_startCanvas.enabled = false;
        GameManager.instance.levelStarted = true;
        SecondarySystemsManager.instance.StartGame();
        LeaksManager.instance.StartGame();
    }
}
