using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HintLeakManager : MonoBehaviour
{
    public static HintLeakManager instance;

#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Curve use to make the hint follow screen's edges.")]
    [SerializeField] GameObject hintPrefab;
#pragma warning restore 0649
    [Space]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    [Header("Variables")]
    public List<Leak> activesLeaks = new List<Leak>();
    AnimationCurve pivotValue;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        pivotValue = HintSecondarySystemManager.instance.pivotValue;
    }

    private void Update()
    {
        if (CameraManager.instance.VCamZoom.activeSelf)
        {
            foreach (Leak item in activesLeaks)
            {
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(item.transform.position);
                bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if (onScreen)
                {
                    if (item.associatedHint && item.associatedHint.activeSelf)
                        item.associatedHint.SetActive(false);
                }
                else
                {
                    if (item.associatedHint)
                    {
                        if (!item.associatedHint.activeSelf)
                            item.associatedHint.SetActive(true);
                    }
                    else
                    {
                        item.associatedHint = Instantiate(hintPrefab, transform);
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
            foreach (Leak item in activesLeaks)
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
