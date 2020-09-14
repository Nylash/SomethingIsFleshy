using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintSecondarySystemManager : MonoBehaviour
{
    public static HintSecondarySystemManager instance;

#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Curve use to make the hint follow screen's edges.")]
    public AnimationCurve pivotValue;
    [SerializeField] GameObject hintPrefab;
    [SerializeField] Sprite oxygenSprite;
    [SerializeField] Sprite energySprite;
#pragma warning restore 0649
    [Space]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    [Header("Variables")]
    public List<SecondarySystem> activeSecondarySystems = new List<SecondarySystem>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (CameraManager.instance.VCamZoom.activeSelf)
        {
            foreach (SecondarySystem item in activeSecondarySystems)
            {
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(item.transform.position);
                bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if (onScreen)
                {
                    if (item.associatedHint)
                    {
                        if (item.associatedHint.activeSelf)
                            item.associatedHint.SetActive(false);
                    }
                }
                else
                {
                    if (item.associatedHint)
                    {
                        if(!item.associatedHint.activeSelf)
                            item.associatedHint.SetActive(true);
                    }  
                    else
                    {
                        item.associatedHint = Instantiate(hintPrefab, transform);
                        if (item.energyNeeded)
                            item.associatedHint.GetComponent<Image>().sprite = energySprite;
                        else
                            item.associatedHint.GetComponent<Image>().sprite = oxygenSprite;
                        item.associatedTimerHint = item.associatedHint.transform.GetChild(0).GetComponent<Image>();
                    }
                    float angle = Vector2.SignedAngle(item.associatedHint.transform.up, (Camera.main.WorldToScreenPoint(item.transform.position) - item.associatedHint.transform.position).normalized);
                    item.associatedHint.transform.Rotate(new Vector3(0, 0, angle));
                    RectTransform hintRectRansform = item.associatedHint.GetComponent<Image>().rectTransform;
                    float newPivotY = -pivotValue.Evaluate(Mathf.Abs(WrapAngle(hintRectRansform.localEulerAngles.z)));
                    hintRectRansform.pivot = new Vector2(hintRectRansform.pivot.x, newPivotY);
                    hintRectRansform.position = hintRectRansform.parent.position;
                }
            }
        }
        else
        {
            foreach (SecondarySystem item in activeSecondarySystems)
            {
                if (item.associatedHint && item.associatedHint.activeSelf)
                    item.associatedHint.SetActive(false);
            }
        }
    }

    float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;
        return angle;
    }
}
