using UnityEngine;

public class Teleporters : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Color of the two TPS associated to this script.")]
    [SerializeField] Color TPS_Color;
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public GameObject[] TPS = new GameObject[2];
    Gradient fxColor;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    private void Start()
    {
        fxColor = new Gradient();
        colorKey = new GradientColorKey[2];
        colorKey[0].color = TPS_Color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 1.0f;
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        fxColor.SetKeys(colorKey, alphaKey);

        TPS[0] = transform.GetChild(0).gameObject;
        TPS[1] = transform.GetChild(1).gameObject;
        foreach (GameObject item in TPS)
        {
            item.transform.GetChild(0).GetComponent<SpriteRenderer>().color = TPS_Color;
            ParticleSystem.MainModule fx = item.transform.GetChild(1).GetComponent<ParticleSystem>().main;
            fx.startColor = fxColor;
        }  
    }

    public Vector3 GetTPLocation(GameObject currentTP)
    {
        if (currentTP != TPS[0])
            return TPS[0].transform.position;
        else
            return TPS[1].transform.position;
    }
}
