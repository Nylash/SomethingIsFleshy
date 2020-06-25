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

    private void Start()
    {
        TPS[0] = transform.GetChild(0).gameObject;
        TPS[1] = transform.GetChild(1).gameObject;
        foreach (GameObject item in TPS)
            item.GetComponent<SpriteRenderer>().color = TPS_Color;
    }

    public Vector3 GetTPLocation(GameObject currentTP)
    {
        if (currentTP != TPS[0])
            return TPS[0].transform.position;
        else
            return TPS[1].transform.position;
    }
}
