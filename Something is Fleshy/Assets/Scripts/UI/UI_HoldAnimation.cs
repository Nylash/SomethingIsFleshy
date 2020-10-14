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

    PlayerMap playerMap;

    private void OnEnable() => playerMap.UI.Enable();
    private void OnDisable() => playerMap.UI.Disable();

    private void Awake()
    {
        playerMap = new PlayerMap();

        playerMap.UI.Submit.started += ctx => StartHolding();
        playerMap.UI.Submit.canceled += ctx => CancelHolding();
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
