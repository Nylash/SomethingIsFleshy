using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [SerializeField] int timeToFinishAfterCompletion;
    [SerializeField] float timeBeforeFirstActivityAfterCompletion;
    [SerializeField] float timeBeforeFirstLeakCheckAfterCompletion;
    [Space]
    [SerializeField] SecondarySystem tutoSS;
    [SerializeField] List<SecondarySystem> associatedPack = new List<SecondarySystem>();
    [Space]
    [SerializeField] Transform triggerPointCameraTuto;
    [SerializeField] float radiusTriggerCameraTuto;
    [Space]
    [SerializeField] LeakZone tutoLeakZone;
    [SerializeField] List<LeakZone> leakZonesOfThisPipe = new List<LeakZone>();
    [SerializeField] LeverScript associatedLever;
    [SerializeField] int associatedPipe;
    [Space]
    [SerializeField] Canvas presentationCanvas;
    [SerializeField] Canvas primarySystemCanvas;
    [SerializeField] Canvas primarySystemCongratulationsCanvas;
    [SerializeField] Canvas secondarySystemCanvas;
    [SerializeField] Canvas secondarySystemCongratulationsCanvas;
    [SerializeField] Canvas secondarySystemFailureCanvas;
    [SerializeField] Canvas cameraCanvas;
    [SerializeField] Canvas leakCanvas;
    [SerializeField] Canvas completionCanvas;
    [Space]
    [SerializeField] Canvas objectiveCanvas;
    [SerializeField] TextMeshProUGUI textObj;
    [SerializeField] TextMeshProUGUI textTop;
    [SerializeField] TextMeshProUGUI textBot;
    [SerializeField] TextMeshProUGUI textUnique;
    [Space]
    [SerializeField] GameObject camSS;
    [SerializeField] GameObject camLeak;
    [Space]
    [SerializeField] GameObject[] leversUnusedAtStart;
#pragma warning restore 0649
    #endregion
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public TutorialStep currentStep;
    ActionsMap actionsMap;
    bool cameraTutoDone;
    public GameObject stockedCam;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();
        actionsMap.Gameplay.ValidationPopUp.started += ctx => Validation();
    }

    private void Start()
    {
        UI_Manager.instance.UI_timerCanvas.enabled = false;
        UI_Manager.instance.UI_scoreCanvas.enabled = false;

        GameManager.instance.levelPaused = true;
        presentationCanvas.enabled = true;
        CameraManager.instance.SwitchCamera();
    }

    private void Update()
    {
        if (Vector2.Distance(triggerPointCameraTuto.position, CharacterController2D.instance.transform.position) < radiusTriggerCameraTuto && !cameraTutoDone)
        {
            cameraTutoDone = true;
            CameraManager.instance.VCamGlobal.SetActive(false);
            CameraManager.instance.VCamZoom.SetActive(true);
            cameraCanvas.enabled = true;
            GameManager.instance.levelPaused = true;
            CharacterController2D.instance.animator.SetBool("Running", false);
        }
        switch (currentStep)
        {
            case TutorialStep.presentationMade:
                textTop.text = "Lungs : " + ((int)(LungsManager.instance.currentCapacity / GameManager.instance.maxCapacityPrimarySystem * 100)).ToString() + "%";
                textBot.text = "Stomach : " + ((int)(StomachManager.instance.currentCapacity / GameManager.instance.maxCapacityPrimarySystem * 100)).ToString() + "%";
                if (LungsManager.instance.currentCapacity/GameManager.instance.maxCapacityPrimarySystem >.9f 
                    && StomachManager.instance.currentCapacity / GameManager.instance.maxCapacityPrimarySystem > .9f && !GameManager.instance.levelPaused)
                {
                    GameManager.instance.levelPaused = true;
                    CharacterController2D.instance.animator.SetBool("Running", false);
                    primarySystemCongratulationsCanvas.enabled = true;
                    objectiveCanvas.enabled = false;
                }
                break;
            case TutorialStep.psFilledUp:
                textTop.text = "Time : " + ((int)(SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem - tutoSS.timerBeforeExpiration)).ToString() + " sec";
                textBot.text = "Energy : " + ((int)(tutoSS.currentEnergy / SecondarySystemsManager.instance.energyAmoutNeeded * 100)).ToString() + "%";
                if (!tutoSS.energyNeeded && !GameManager.instance.levelPaused)
                {
                    if(ScoreManager.instance.currentScore < 0)
                    {
                        GameManager.instance.levelPaused = true;
                        CharacterController2D.instance.animator.SetBool("Running", false);
                        secondarySystemFailureCanvas.enabled = true;
                    }
                    else
                    {
                        GameManager.instance.levelPaused = true;
                        CharacterController2D.instance.animator.SetBool("Running", false);
                        secondarySystemCongratulationsCanvas.enabled = true;
                        objectiveCanvas.enabled = false;
                    }
                }
                break;
            case TutorialStep.ssFilledUp:
                if(LeaksManager.instance.allLeaks.Count == 0 && !GameManager.instance.levelPaused)
                {
                    currentStep++;
                    GameManager.instance.levelPaused = true;
                    CharacterController2D.instance.animator.SetBool("Running", false);
                    completionCanvas.enabled = true;
                    objectiveCanvas.enabled = false;
                }
                break;
        }
    }

    void Validation()
    {
        if (cameraCanvas.enabled)
        {
            if (cameraCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
            {
                cameraCanvas.enabled = false;
                GameManager.instance.levelPaused = false;
                return;
            }
            return;
        }
        switch (currentStep)
        {
            case TutorialStep.start:
                if (presentationCanvas.enabled)
                {
                    if (presentationCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        presentationCanvas.enabled = false;
                        currentStep++;
                        primarySystemCanvas.enabled = true;
                    }
                }
                break;
            case TutorialStep.presentationMade:
                if (primarySystemCanvas.enabled)
                {
                    if (primarySystemCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        primarySystemCanvas.enabled = false;
                        GameManager.instance.levelPaused = false;
                        textObj.text = "Fill both";
                        textTop.text = "Lungs : 0%";
                        textBot.text = "Stomach : 0%";
                        objectiveCanvas.enabled = true;
                    }
                }
                else if (primarySystemCongratulationsCanvas.enabled)
                {
                    if (primarySystemCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        primarySystemCongratulationsCanvas.enabled = false;
                        secondarySystemCanvas.enabled = true;
                        currentStep++;
                        stockedCam = (CameraManager.instance.VCamGlobal.activeSelf ? CameraManager.instance.VCamGlobal : CameraManager.instance.VCamZoom);
                        stockedCam.SetActive(false);
                        camSS.SetActive(true);
                        SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                        Color color = new Color(1, 1, 1, 1);
                        foreach (GameObject item in leversUnusedAtStart)
                        {
                            item.GetComponent<BoxCollider2D>().enabled = true;
                            item.GetComponent<SpriteRenderer>().color = color;
                            item.GetComponent<SpriteRenderer>().sortingLayerName = "Lever";
                            item.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
                            item.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Lever";
                        }
                    }
                }
                break;
            case TutorialStep.psFilledUp:
                if (secondarySystemCanvas.enabled)
                {
                    if (secondarySystemCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        secondarySystemCanvas.enabled = false;
                        GameManager.instance.levelPaused = false;
                        camSS.SetActive(false);
                        if (!cameraTutoDone)
                        {
                            CameraManager.instance.VCamGlobal.SetActive(true);
                            CameraManager.instance.VCamZoom.SetActive(false);
                        }
                        else
                            stockedCam.SetActive(true);
                        textObj.text = "Supply muscle";
                        textTop.text = "Time : " + ((int)SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem).ToString() + " sec";
                        textBot.text = "Energy : 0%";
                        objectiveCanvas.enabled = true;
                    }
                }
                else if (secondarySystemCongratulationsCanvas.enabled)
                {
                    if (secondarySystemCongratulationsCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        currentStep++;
                        secondarySystemCongratulationsCanvas.enabled = false;
                        stockedCam = (CameraManager.instance.VCamGlobal.activeSelf ? CameraManager.instance.VCamGlobal : CameraManager.instance.VCamZoom);
                        stockedCam.SetActive(false);
                        camLeak.SetActive(true);
                        leakCanvas.enabled = true;
                        LeaksManager.instance.StartSpecificLeak(tutoLeakZone, leakZonesOfThisPipe, associatedLever, associatedPipe);
                        textObj.text = "Patch leack";
                        textUnique.text = "Patch current leak";
                        textTop.text = "";
                        textBot.text = "";
                        objectiveCanvas.enabled = true;
                    }
                }
                else if (secondarySystemFailureCanvas.enabled)
                {
                    if (secondarySystemFailureCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        ScoreManager.instance.currentScore = 0;
                        SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                        GameManager.instance.levelPaused = false;
                        secondarySystemFailureCanvas.enabled = false;
                        textTop.text = "Time : " + ((int)SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem).ToString() + " sec";
                        textBot.text = "Energy : 0%";
                    }
                }
                break;
            case TutorialStep.ssFilledUp:
                if (leakCanvas.enabled)
                {
                    if (leakCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        GameManager.instance.levelPaused = false;
                        leakCanvas.enabled = false;
                        camLeak.SetActive(false);
                        stockedCam.SetActive(true);
                        if (!cameraTutoDone)
                        {
                            CameraManager.instance.VCamGlobal.SetActive(true);
                            CameraManager.instance.VCamZoom.SetActive(false);
                        }
                    }
                }
                break;
            case TutorialStep.leakPatched:
                if (completionCanvas.enabled)
                {
                    if (completionCanvas.GetComponent<UI_ChildSelector>().NoMoreChilds())
                    {
                        TutorialCompleted();
                        GameManager.instance.levelPaused = false;
                        completionCanvas.enabled = false;
                    }
                }
                break;
        }
    }

    void TutorialCompleted()
    {
        GameManager.instance.levelDuration = timeToFinishAfterCompletion;
        ScoreManager.instance.currentTimer = timeToFinishAfterCompletion;
        SecondarySystemsManager.instance.timeBeforeFirstActivity = timeBeforeFirstActivityAfterCompletion;
        LeaksManager.instance.timeBeforeFirstCheckForLeak = timeBeforeFirstLeakCheckAfterCompletion;
        SecondarySystemsManager.instance.TutorialCompleted();
        LeaksManager.instance.TutorialCompleted();
        UI_Manager.instance.UI_timerCanvas.enabled = true;
        UI_Manager.instance.UI_scoreCanvas.enabled = true;
    }

    public enum TutorialStep
    {
        start, presentationMade, psFilledUp, ssFilledUp, leakPatched, tutorialCompleted
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(triggerPointCameraTuto.position, radiusTriggerCameraTuto);
    }
}
