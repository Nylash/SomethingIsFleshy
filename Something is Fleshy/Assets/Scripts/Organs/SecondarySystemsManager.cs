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
    [SerializeField] List<SecondarySystem> packA = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packB = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packC = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packD = new List<SecondarySystem>();
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    List<List<SecondarySystem>> refAllSecondarySystems = new List<List<SecondarySystem>>();
    List<List<SecondarySystem>> allSecondarySystems = new List<List<SecondarySystem>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);  

        if(packA.Count > 0)
            refAllSecondarySystems.Add(packA);
        if (packB.Count > 0)
            refAllSecondarySystems.Add(packB);
        if (packC.Count > 0)
            refAllSecondarySystems.Add(packC);
        if (packD.Count > 0)
            refAllSecondarySystems.Add(packD);

        allSecondarySystems.AddRange(refAllSecondarySystems);

        Invoke("StartActivity", timeBeforeFirstActivity);
    }

    void StartActivity()
    {
        if(allSecondarySystems.Count > 0)
        {
            int selectedPack = Random.Range(0, allSecondarySystems.Count);
            int selectedSecondarySystem = Random.Range(0, allSecondarySystems[selectedPack].Count);
            allSecondarySystems[selectedPack][selectedSecondarySystem].animator.SetBool("OnActivity", true);
            int type = Random.Range(0, 2);
            switch (type)
            {
                case 0:
                    allSecondarySystems[selectedPack][selectedSecondarySystem].currentEnergy = 0f;
                    allSecondarySystems[selectedPack][selectedSecondarySystem].energyGauge.SetActive(true);
                    break;
                case 1:
                    allSecondarySystems[selectedPack][selectedSecondarySystem].currentOxygen = 0f;
                    allSecondarySystems[selectedPack][selectedSecondarySystem].oxygenGauge.SetActive(true);
                    break;
            }
            StartCoroutine(LaunchActivity(allSecondarySystems[selectedPack], allSecondarySystems[selectedPack][selectedSecondarySystem], type));
        }
        Invoke("StartActivity", Random.Range(minTimeBetweenActivities, maxTimeBetweenActivities));
    }

    IEnumerator LaunchActivity(List<SecondarySystem> selectedPack, SecondarySystem selectedSystem, int type)
    {
        yield return new WaitForSeconds(timeBeforeHealthLoss);
        switch (type)
        {
            case 0:
                selectedSystem.energyNeeded = true;
                break;
            case 1:
                selectedSystem.oxygenNeeded = true;
                break;
        }
        allSecondarySystems.Clear();
        allSecondarySystems.AddRange(refAllSecondarySystems);
        allSecondarySystems.Remove(selectedPack);
    }

    public void StopActivityCall()
    {
        CancelInvoke("StartActivity");
        StopCoroutine("LaunchActivity");
    }
}
