using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("UI Objects")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public TextMeshProUGUI UI_timerValue;
    public Canvas UI_defeatCanvas;
    public TextMeshProUGUI UI_defeatFullTimer;
    public TextMeshProUGUI UI_defeatActualTimer;
    public Button UI_defeatButton;
    public Canvas UI_victoryCanvas;
    public Button UI_victoryButton;
    public Canvas UI_pauseCanvas;
    [Header("Components")]
    ActionsMap actionsMap;
    [Header("Variables")]
    public GameObject lastSelected;

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

    private void Update()
    {
        if (HeartManager.instance.defeatOrVictory)
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                if(EventSystem.current.currentSelectedGameObject != lastSelected)
                {
                    if (lastSelected)
                        lastSelected.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                    lastSelected = EventSystem.current.currentSelectedGameObject;
                    lastSelected.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    //lastSelected.GetComponent<Animator>().SetTrigger("Selected");
                }
            }
        }
    }

    void Pause()
    {
        UI_pauseCanvas.enabled = !UI_pauseCanvas.enabled;
        if (UI_pauseCanvas.enabled)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
