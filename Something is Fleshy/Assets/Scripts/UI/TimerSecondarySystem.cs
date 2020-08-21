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
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
            {
                transform.position = Camera.main.WorldToScreenPoint(associatedSystem.transform.position);
                if (associatedSystem.filling)
                {
                    if (canvas.enabled)
                        canvas.enabled = false;
                }
                else
                {
                    if (!canvas.enabled)
                        canvas.enabled = true;
                    timer.fillAmount = associatedSystem.timerBeforeExplosion / SecondarySystemsManager.instance.timeBeforeSSexplosion;
                }
                if (!associatedSystem.energyNeeded && !associatedSystem.oxygenNeeded)
                    Destroy(gameObject);
            }
        }
    }
}
