using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    [Header("Components")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    ActionsMap actionsMap;

    [Header("Variables")]
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
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
            {
                if (CharacterController2D.instance.animator.GetBool("Holding"))
                {
                    holdTimer += Time.deltaTime;
                    UI_Manager.instance.UI_leakGaugeIn.fillAmount = holdTimer / LeaksManager.instance.timeToRepair;
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
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused && CharacterController2D.instance.AnimationNotCurrentlyBlocking())
            {
                if (canInteract)
                {
                    CharacterController2D.instance.rb.velocity = Vector2.zero;
                    switch (currentInteractableType)
                    {
                        case InteractableType.lever:
                            interactableObject.GetComponent<LeverScript>().Switch();
                            break;
                        case InteractableType.electricLever:
                            interactableObject.GetComponent<ElectricSwitch>().Switch();
                            break;
                        case InteractableType.leak:
                            holdTimer = 0f;
                            UI_Manager.instance.UI_leakGaugeCanvas.transform.position = new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,2f,0)).y, 0);
                            UI_Manager.instance.UI_leakGaugeIn.fillAmount = 0;
                            UI_Manager.instance.UI_leakGaugeCanvas.enabled = true;
                            goto HoldInteractionAnim;
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
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
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
            case "Lever":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.lever;
                break;
            case "ElectricLever":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.electricLever;
                break;
            case "Leak":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.leak;
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
                //if gauge destroy it
                if (currentInteractableType == InteractableType.leak)
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
        none, lever, electricLever, leak,
    }
}
