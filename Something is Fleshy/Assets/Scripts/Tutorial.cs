using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] GameObject camSS;
    [SerializeField] GameObject camLeak;
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

        GameManager.instance.levelPaused = true;
        presentationCanvas.enabled = true;
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
                if(LungsManager.instance.currentCapacity/GameManager.instance.maxCapacityPrimarySystem >.9f 
                    && StomachManager.instance.currentCapacity / GameManager.instance.maxCapacityPrimarySystem > .9f && !GameManager.instance.levelPaused)
                {
                    GameManager.instance.levelPaused = true;
                    CharacterController2D.instance.animator.SetBool("Running", false);
                    primarySystemCongratulationsCanvas.enabled = true;
                }
                break;
            case TutorialStep.psFilledUp:
                if(!tutoSS.energyNeeded && !GameManager.instance.levelPaused)
                {
                    if(HeartManager.instance.currentHealth != GameManager.instance.maxHealth)
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
                }
                break;
        }
    }

    void Validation()
    {
        if (cameraCanvas.enabled)
        {
            if (cameraCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
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
                    if (presentationCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
                    {
                        presentationCanvas.enabled = false;
                        currentStep++;
                        if (!cameraTutoDone)
                        {
                            CameraManager.instance.VCamGlobal.SetActive(false);
                            CameraManager.instance.VCamZoom.SetActive(true);
                        }
                        primarySystemCanvas.enabled = true;
                    }
                }
                break;
            case TutorialStep.presentationMade:
                if (primarySystemCanvas.enabled)
                {
                    if (primarySystemCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
                    {
                        primarySystemCanvas.enabled = false;
                        if (!cameraTutoDone)
                        {
                            CameraManager.instance.VCamGlobal.SetActive(true);
                            CameraManager.instance.VCamZoom.SetActive(false);
                        }
                        GameManager.instance.levelPaused = false;
                    }
                }
                else if (primarySystemCongratulationsCanvas.enabled)
                {
                    if (primarySystemCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
                    {
                        primarySystemCongratulationsCanvas.enabled = false;
                        secondarySystemCanvas.enabled = true;
                        currentStep++;
                        stockedCam = (CameraManager.instance.VCamGlobal.activeSelf ? CameraManager.instance.VCamGlobal : CameraManager.instance.VCamZoom);
                        stockedCam.SetActive(false);
                        camSS.SetActive(true);
                        SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                    }
                }
                break;
            case TutorialStep.psFilledUp:
                if (secondarySystemCanvas.enabled)
                {
                    if (secondarySystemCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
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
                    }
                }
                else if (secondarySystemCongratulationsCanvas.enabled)
                {
                    if (secondarySystemCongratulationsCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
                    {
                        currentStep++;
                        secondarySystemCongratulationsCanvas.enabled = false;
                        stockedCam = (CameraManager.instance.VCamGlobal.activeSelf ? CameraManager.instance.VCamGlobal : CameraManager.instance.VCamZoom);
                        stockedCam.SetActive(false);
                        camLeak.SetActive(true);
                        leakCanvas.enabled = true;
                        LeaksManager.instance.StartSpecificLeak(tutoLeakZone, leakZonesOfThisPipe, associatedLever, associatedPipe);
                    }
                }
                else if (secondarySystemFailureCanvas.enabled)
                {
                    if (secondarySystemFailureCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
                    {
                        HeartManager.instance.currentHealth = GameManager.instance.maxHealth;
                        HeartManager.instance.TakeDamage(0);//Use to update gauge
                        SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                        GameManager.instance.levelPaused = false;
                        secondarySystemFailureCanvas.enabled = false;
                    }
                }
                break;
            case TutorialStep.ssFilledUp:
                if (leakCanvas.enabled)
                {
                    if (leakCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
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
                    if (completionCanvas.GetComponent<UI_ChildSelector>().UpdateChilds())
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
        GameManager.instance.timeToFinishLevel = timeToFinishAfterCompletion;
        HeartManager.instance.currentTimer = timeToFinishAfterCompletion;
        SecondarySystemsManager.instance.timeBeforeFirstActivity = timeBeforeFirstActivityAfterCompletion;
        LeaksManager.instance.timeBeforeFirstCheckForLeak = timeBeforeFirstLeakCheckAfterCompletion;
        SecondarySystemsManager.instance.TutorialCompleted();
        LeaksManager.instance.TutorialCompleted();
        UI_Manager.instance.UI_timerCanvas.enabled = true;
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
