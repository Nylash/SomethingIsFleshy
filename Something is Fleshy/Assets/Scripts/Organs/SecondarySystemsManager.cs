using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SecondarySystemsManager : MonoBehaviour
{
    public static SecondarySystemsManager instance;

    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Time before the first activity.")]
    [SerializeField] float timeBeforeFirstActivity = 10f;
    [Tooltip("Minimal time between two activities.")]
    [SerializeField] float minTimeBetweenActivities = 15f;
    [Tooltip("Maximal time between two activities.")]
    [SerializeField] float maxTimeBetweenActivities = 30f;
    [Tooltip("Time during which player knows that a secondary system will need a ressource, but without HP loss.")]
    [SerializeField] float timeBeforeHealthLoss = 5f;
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public SecondarySystem[] refSecondarySystems;
    public List<SecondarySystem> secondarySystems = new List<SecondarySystem>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        refSecondarySystems = FindObjectsOfType<SecondarySystem>();
        secondarySystems.AddRange(refSecondarySystems);

        Invoke("StartActivity", timeBeforeFirstActivity);
    }

    void StartActivity()
    {
        if(secondarySystems.Count > 0)
        {
            int index = Random.Range(0, secondarySystems.Count);
            secondarySystems[index].animator.SetBool("OnActivity", true);
            int type = Random.Range(0, 2);
            switch (type)
            {
                case 0:
                    secondarySystems[index].energyGauge.SetActive(true);
                    break;
                case 1:
                    secondarySystems[index].oxygenGauge.SetActive(true);
                    break;
            }
            StartCoroutine(LaunchActivity(index, type));
        }
        Invoke("StartActivity", Random.Range(minTimeBetweenActivities, maxTimeBetweenActivities));
    }

    IEnumerator LaunchActivity(int index, int type)
    {
        yield return new WaitForSeconds(timeBeforeHealthLoss);
        switch (type)
        {
            case 0:
                secondarySystems[index].energyNeeded = true;
                secondarySystems[index].currentEnergy = 0f;
                break;
            case 1:
                secondarySystems[index].oxygenNeeded = true;
                secondarySystems[index].currentOxygen = 0f;
                break;
        }
        secondarySystems.RemoveAt(index);
    }

    public void StopActivityCall()
    {
        CancelInvoke("StartActivity");
        StopCoroutine("LaunchActivity");
    }
}
