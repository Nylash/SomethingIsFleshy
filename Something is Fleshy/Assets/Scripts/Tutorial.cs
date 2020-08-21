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
#pragma warning restore 0649
    #endregion
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public TutorialStep currentStep;
    ActionsMap actionsMap;
    bool cameraTutoDone;

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
        print("Hello ! You are Celli, and your job is to help your host by providing oxygen and energy to his muscles." +
            "To do this you need to use the pipe system present in the host. If you fail to supply a muscle the heart will be damaged, if the heart take too much damage the host dies.");
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
            print("At anytime you can switch between a global and a zoomed camera. On zoomed camera some hints appear to help you, they point toward muscles in need and leak (we will talk about leaks soon).");
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
                    print("Good job ! The lungs and the stomach are now full and ready to distribute ressources to muscles in need.");
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
                        print("You don't suceed to help this muscle so it damaged the heart, but don't worry, for this time I will heal the damage and you will try again.");
                    }
                    else
                    {
                        GameManager.instance.levelPaused = true;
                        CharacterController2D.instance.animator.SetBool("Running", false);
                        secondarySystemCongratulationsCanvas.enabled = true;
                        print("Well done ! You have succesfully supply this muscle !");
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
                    print("Perfect, you now know all the basis to help your host, let's try to help this one a little bit more and after you could try with other host. Don't worry, this one is easy ;)");
                }
                break;
        }
    }

    void Validation()
    {
        if (cameraCanvas.enabled)
        {
            cameraCanvas.enabled = false;
            GameManager.instance.levelPaused = false;
            return;
        }
        switch (currentStep)
        {
            case TutorialStep.start:
                if (presentationCanvas.enabled)
                {
                    presentationCanvas.enabled = false;
                    currentStep++;
                    if (!cameraTutoDone)
                    {
                        CameraManager.instance.VCamGlobal.SetActive(false);
                        CameraManager.instance.VCamZoom.SetActive(true);
                    }
                    primarySystemCanvas.enabled = true;
                    print("For starting, you need to fill up the lungs and the stomach. You can't do it simultaneously, currently the lungs are filling up, you can use this lever at anytime to switch to the stomach."
                        +" Try to fill up both of this organs. E/A to interact");
                }
                break;
            case TutorialStep.presentationMade:
                if (primarySystemCanvas.enabled)
                {
                    primarySystemCanvas.enabled = false;
                    if (!cameraTutoDone)
                    {
                        CameraManager.instance.VCamGlobal.SetActive(true);
                        CameraManager.instance.VCamZoom.SetActive(false);
                    }
                    GameManager.instance.levelPaused = false;
                }
                else if (primarySystemCongratulationsCanvas.enabled)
                {
                    primarySystemCongratulationsCanvas.enabled = false;
                    secondarySystemCanvas.enabled = true;
                    currentStep++;
                    //Switch camera to ss
                    print("This muscle need a ressource, the color inside indicate which ressource is needed, blue for oxygen and yellow for energy. Here this muscle need energy, so you will have to use severals levers to activate the right pipes to bring it to it." +
                        "Some to activate the goods pipes to come to it and another to select the right ressource (this one is generally situated near the lungs or the stomach)"
                        + "Timer blabla dépend du system qu'on fait bobo coeur");
                    SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                }
                break;
            case TutorialStep.psFilledUp:
                if (secondarySystemCanvas.enabled)
                {
                    secondarySystemCanvas.enabled = false;
                    GameManager.instance.levelPaused = false;
                    //turn off ss camera
                    if (!cameraTutoDone)
                    {
                        CameraManager.instance.VCamGlobal.SetActive(true);
                        CameraManager.instance.VCamZoom.SetActive(false);
                    }
                }
                else if (secondarySystemCongratulationsCanvas.enabled)
                {
                    currentStep++;
                    secondarySystemCongratulationsCanvas.enabled = false;
                    //switch camera to leak
                    leakCanvas.enabled = true;
                    print("Oh no ! Looks like one pipe gots a leak, you should repair it quickly, otherwise your ressources' stock might be running low. You can repair it by holding interaction button near to it.");
                    LeaksManager.instance.StartSpecificLeak(tutoLeakZone, leakZonesOfThisPipe, associatedLever, associatedPipe);
                }
                else if (secondarySystemFailureCanvas.enabled)
                {
                    HeartManager.instance.currentHealth = GameManager.instance.maxHealth;
                    SecondarySystemsManager.instance.LaunchSpecificSS(tutoSS, associatedPack);
                    GameManager.instance.levelPaused = false;
                    secondarySystemFailureCanvas.enabled = false;
                }
                break;
            case TutorialStep.ssFilledUp:
                if (leakCanvas.enabled)
                {
                    GameManager.instance.levelPaused = false;
                    leakCanvas.enabled = false;
                    //turn off leak camera
                    if (!cameraTutoDone)
                    {
                        CameraManager.instance.VCamGlobal.SetActive(true);
                        CameraManager.instance.VCamZoom.SetActive(false);
                    }
                }
                break;
            case TutorialStep.leakPatched:
                if (completionCanvas.enabled)
                {
                    TutorialCompleted();
                    GameManager.instance.levelPaused = false;
                    completionCanvas.enabled = false;
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
