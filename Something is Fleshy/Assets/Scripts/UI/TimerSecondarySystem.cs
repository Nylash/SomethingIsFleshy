using UnityEngine;
using UnityEngine.UI;

public class TimerSecondarySystem : MonoBehaviour
{
    public SecondarySystem associatedSystem;
    Image timer;
    Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        timer = GetComponent<Image>();
        timer.fillAmount = 0f;
    }

    private void Update()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                transform.position = Camera.main.WorldToScreenPoint(associatedSystem.transform.position);
                if (!canvas.enabled)
                    canvas.enabled = true;
                timer.fillAmount = associatedSystem.timerBeforeExpiration / SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem;
                if (!associatedSystem.energyNeeded && !associatedSystem.oxygenNeeded)
                    Destroy(gameObject);
            }
            else
            {
                if (canvas.enabled)
                    canvas.enabled = false;
            }
        }
    }

    public void DisableOnPause()
    {
        if (canvas.enabled)
            canvas.enabled = false;
    }
}
