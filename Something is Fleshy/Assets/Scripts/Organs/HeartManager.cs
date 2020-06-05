using UnityEngine;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
