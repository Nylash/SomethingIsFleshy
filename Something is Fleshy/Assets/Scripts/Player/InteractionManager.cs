using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("Components")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public AudioSource interactionSource;
    public GameObject interactFX;
    public GameObject interactLeakFX;
    CharacterController2D player;
    PlayerInput playerInput;


    [Header("Variables")]
    public PlayerAnimationsMethods animMethodsScript;
    bool canInteract;
    GameObject interactableObject;
    InteractableType currentInteractableType;
    float holdTimer;
    GameObject currentInteractionLeakFX;

    public AlchemicalMachine.Ressource ressourceCarried;
    GameObject currentRessourceBall;

    private void Start()
    {
        player = GetComponent<CharacterController2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void AskInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InteractionStarted();
        else if (ctx.canceled)
            InteractionHoldCanceled(true);
    }

    private void Update()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (player.animator.GetBool("Holding"))
                {
                    if (!UI_Manager.instance.UI_leakGaugeCanvas.enabled)
                        UI_Manager.instance.UI_leakGaugeCanvas.enabled = true;
                    holdTimer += Time.deltaTime;
                    UI_Manager.instance.UI_leakGaugeIn.fillAmount = holdTimer / LeaksManager.instance.timeToRepair;
                    UI_Manager.instance.UI_leakGaugeCanvas.transform.position = new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2f, 0)).y, 0);
                    if (holdTimer >= LeaksManager.instance.timeToRepair)
                    {
                        UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                        interactableObject.GetComponent<Leak>().PatchLeak(interactionSource);
                        player.animator.SetBool("Holding", false);
                    }
                }
            }
            else
            {
                if (UI_Manager.instance.UI_leakGaugeCanvas.enabled)
                {
                    UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                    player.animator.SetBool("Holding", false);
                    if (currentInteractionLeakFX)
                        Destroy(currentInteractionLeakFX);
                }
            }
        }
    }

    void InteractionStarted()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused && player.AnimationNotCurrentlyBlocking())
            {
                if (canInteract)
                {
                    player.rb.velocity = Vector2.zero;
                    switch (currentInteractableType)
                    {
                        case InteractableType.lever:
                            interactableObject.GetComponent<LeverScript>().Switch();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            Instantiate(interactFX, interactableObject.transform.position, Quaternion.identity);
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
                            if (player.AnimationNotCurrentlyBlocking())
                            {
                                animMethodsScript.tpPosition = interactableObject.GetComponentInParent<Teleporters>().GetTPLocation(interactableObject.gameObject);
                                player.animator.SetTrigger("StartTeleporting");
                                player.animator.SetBool("Teleporting", true);
                                SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.TeleportationIn, interactionSource);
                                animMethodsScript.tpSource = interactionSource;
                            }
                            break;
                        case InteractableType.inversionBlockLever:
                            interactableObject.GetComponent<InversionBlocks>().InverseBlocks();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            break;


                        case InteractableType.pipeLever:
                            interactableObject.GetComponent<PipeLever>().Interact();
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            Instantiate(interactFX, interactableObject.transform.position, Quaternion.identity);
                            break;
                        case InteractableType.alchemicalMachine:
                            AlchemicalMachine.Ressource ressourceColor =  interactableObject.GetComponent<AlchemicalMachine>().GetRessource();
                            if(ressourceColor != AlchemicalMachine.Ressource.none)
                            {
                                ressourceCarried = ressourceColor;
                                if (currentRessourceBall)
                                    Destroy(currentRessourceBall);
                                currentRessourceBall = Instantiate(GameManager.instance.ressourceBallPrefab, transform);
                                currentRessourceBall.GetComponent<SpriteRenderer>().color = AlchemicalMachine.instance.RessourceToColor(ressourceCarried);
                            }
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            Instantiate(interactFX, interactableObject.transform.position, Quaternion.identity);
                            break;
                        case InteractableType.humanPartMachine:
                            interactableObject.GetComponent<HumanPartMachine>().Interact(ressourceCarried);
                            ressourceCarried = AlchemicalMachine.Ressource.none;
                            Destroy(currentRessourceBall);
                            SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.LeverInteraction, interactionSource);
                            Instantiate(interactFX, interactableObject.transform.position, Quaternion.identity);
                            break;
                    }
                    if (player.AnimationNotCurrentlyBlocking())
                    {
                        player.animator.SetTrigger("StartInteracting");
                        player.animator.SetBool("Interacting", true);
                    }
                    return;
                HoldInteractionAnim:
                    if (player.AnimationNotCurrentlyBlocking())
                    {
                        player.animator.SetTrigger("StartHolding");
                        player.animator.SetBool("Holding", true);
                        currentInteractionLeakFX = Instantiate(interactLeakFX, interactableObject.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    void InteractionHoldCanceled(bool inputCanceled)
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (inputCanceled)
                {
                    if (playerInput.enabled)
                    {
                        if (canInteract && player.animator.GetBool("Holding"))
                        {
                            switch (currentInteractableType)
                            {
                                case InteractableType.leak:
                                    player.animator.SetBool("Holding", false);
                                    UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                                    if (currentInteractionLeakFX)
                                        Destroy(currentInteractionLeakFX);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (canInteract && player.animator.GetBool("Holding"))
                    {
                        switch (currentInteractableType)
                        {
                            case InteractableType.leak:
                                player.animator.SetBool("Holding", false);
                                UI_Manager.instance.UI_leakGaugeCanvas.enabled = false;
                                if (currentInteractionLeakFX)
                                    Destroy(currentInteractionLeakFX);
                                break;
                        }
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


            case "PipeLever":
                currentInteractableType = InteractableType.pipeLever;
                goto case "GENERAL CASE";
            case "AlchemicalMachine":
                currentInteractableType = InteractableType.alchemicalMachine;
                goto case "GENERAL CASE";
            case "HumanPartMachine":
                currentInteractableType = InteractableType.humanPartMachine;
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
                    InteractionHoldCanceled(false);
                    goto case "CLEAN CASE";
                }
                else
                    break;
            case "TP":
                if (currentInteractableType == InteractableType.teleporter && !player.animator.GetBool("Teleporting"))
                    goto case "CLEAN CASE";
                else
                    break;
            case "InversionBlock":
                if (currentInteractableType == InteractableType.inversionBlockLever)
                    goto case "CLEAN CASE";
                else
                    break;


            case "PipeLever":
                if (currentInteractableType == InteractableType.pipeLever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "AlchemicalMachine":
                if (currentInteractableType == InteractableType.alchemicalMachine)
                    goto case "CLEAN CASE";
                else
                    break;
            case "HumanPartMachine":
                if (currentInteractableType == InteractableType.humanPartMachine)
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
        none, lever, electricLever, leak, teleporter, inversionBlockLever, pipeLever, alchemicalMachine, humanPartMachine
    }
}
