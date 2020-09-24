using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UI_HoldAnimation : MonoBehaviour
{
    Image filling;
    Button button;
    float holdToSubmit = .5f;
    float holdTime;
    bool holding;

    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Submit.started += ctx => StartHolding();
        actionsMap.Gameplay.Submit.canceled += ctx => CancelHolding();
    }

    private void Start()
    {
        filling = transform.GetChild(1).GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject && holding)
            holding = false;
        if (holding)
            holdTime += Time.unscaledDeltaTime;
        else if (holdTime > 0f)
            holdTime -= Time.unscaledDeltaTime;
        else
            holdTime = 0;
        filling.fillAmount = holdTime / holdToSubmit;
        if(holdTime > holdToSubmit)
        {
            button.onClick.Invoke();
            holding = false;
        }
    }

    void StartHolding()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            holding = true;
    }

    void CancelHolding()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            holding = false;
    }
}
