using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    [Header("Components")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public AudioSource interactionSource;
    ActionsMap actionsMap;

    [Header("Variables")]
    public PlayerAnimationsMethods animMethodsScript;
    bool canInteract;
    GameObject interactableObject;
    InteractableType currentInteractableType;
    float holdTimer;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Interact.started += ctx => InteractionStarted();
        actionsMap.Gameplay.Interact.canceled += ctx => InteractionHoldCanceled();

    }

    private void Update()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (CharacterController2D.instance.animator.GetBool("Holding"))
                {
                    holdTimer += Time.deltaTime;
                    UI_Manager.instance.UI_leakGaugeIn.fillAmount = holdTimer / LeaksManager.instance.timeToRepair;
                    UI_Manager.instance.UI_leakGaugeCanvas.transform.position = new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2f, 0)).y, 0);
                    if (holdTimer >= LeaksManager.instance.timeToRepair)
                    {
                        UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                        interactableObject.GetComponent<Leak>().PatchLeak();
                        CharacterController2D.instance.animator.SetBool("Holding", false);
                    }
                }
            }
        }
    }

    void InteractionStarted()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused && CharacterController2D.instance.AnimationNotCurrentlyBlocking())
            {
                if (canInteract)
                {
                    CharacterController2D.instance.rb.velocity = Vector2.zero;
                    switch (currentInteractableType)
                    {
                        case InteractableType.lever:
                            interactableObject.GetComponent<LeverScript>().Switch();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            break;
                        case InteractableType.electricLever:
                            interactableObject.GetComponent<ElectricSwitch>().Switch();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            break;
                        case InteractableType.leak:
                            holdTimer = 0f;
                            UI_Manager.instance.UI_leakGaugeCanvas.transform.position = new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,2f,0)).y, 0);
                            UI_Manager.instance.UI_leakGaugeIn.fillAmount = 0;
                            UI_Manager.instance.UI_leakGaugeCanvas.enabled = true;
                            goto HoldInteractionAnim;
                        case InteractableType.teleporter:
                            if (CharacterController2D.instance.AnimationNotCurrentlyBlocking())
                            {
                                animMethodsScript.tpPosition = interactableObject.GetComponentInParent<Teleporters>().GetTPLocation(interactableObject.gameObject);
                                CharacterController2D.instance.animator.SetTrigger("StartTeleporting");
                                CharacterController2D.instance.animator.SetBool("Teleporting", true);
                                SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.TeleportationIn, interactionSource);
                                animMethodsScript.tpSource = interactionSource;
                            }
                            break;
                        case InteractableType.inversionBlockLever:
                            interactableObject.GetComponent<InversionBlocks>().InverseBlocks();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            break;
                    }
                    if (CharacterController2D.instance.AnimationNotCurrentlyBlocking())
                    {
                        CharacterController2D.instance.animator.SetTrigger("StartInteracting");
                        CharacterController2D.instance.animator.SetBool("Interacting", true);
                    }
                    return;
                HoldInteractionAnim:
                    if (CharacterController2D.instance.AnimationNotCurrentlyBlocking())
                    {
                        CharacterController2D.instance.animator.SetTrigger("StartHolding");
                        CharacterController2D.instance.animator.SetBool("Holding", true);
                    }
                }
            }
        }
    }

    void InteractionHoldCanceled()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (canInteract && CharacterController2D.instance.animator.GetBool("Holding"))
                {
                    switch (currentInteractableType)
                    {
                        case InteractableType.leak:
                            CharacterController2D.instance.animator.SetBool("Holding", false);
                            UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "TP":
                currentInteractableType = InteractableType.teleporter;
                goto case "GENERAL CASE";
            case "Lever":
                currentInteractableType = InteractableType.lever;
                goto case "GENERAL CASE";
            case "ElectricLever":
                currentInteractableType = InteractableType.electricLever;
                goto case "GENERAL CASE";
            case "Leak":
                currentInteractableType = InteractableType.leak;
                goto case "GENERAL CASE";
            case "InversionBlock":
                currentInteractableType = InteractableType.inversionBlockLever;
                goto case "GENERAL CASE";
            case "GENERAL CASE":
                canInteract = true;
                interactableObject = collision.gameObject;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Lever":
                if (currentInteractableType == InteractableType.lever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "ElectricLever":
                if (currentInteractableType == InteractableType.electricLever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "Leak":
                if (currentInteractableType == InteractableType.leak)
                {
                    if (UI_Manager.instance.UI_leakGaugeCanvas)
                        UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                    InteractionHoldCanceled();
                    goto case "CLEAN CASE";
                }
                else
                    break;
            case "TP":
                if (currentInteractableType == InteractableType.teleporter && !CharacterController2D.instance.animator.GetBool("Teleporting"))
                    goto case "CLEAN CASE";
                else
                    break;
            case "InversionBlock":
                if (currentInteractableType == InteractableType.inversionBlockLever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "CLEAN CASE":
                canInteract = false;
                interactableObject = null;
                currentInteractableType = InteractableType.none;
                break;
        }
    }

    public enum InteractableType
    {
        none, lever, electricLever, leak, teleporter, inversionBlockLever
    }
}
