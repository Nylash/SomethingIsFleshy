using System.Collections.Generic;
using UnityEngine;

public class LeaksManager : MonoBehaviour
{
    public static LeaksManager instance;

    [Header("PARAMETERS")]
#pragma warning disable 0649
    [SerializeField] GameObject leakPrefab;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public List<LeverScript> leversWithLeaksZones = new List<LeverScript>();

    ActionsMap actionsMap;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();
        actionsMap.Gameplay.Debug.started += ctx => StartLeak();
    }

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    void StartLeak()
    {
        if (leversWithLeaksZones.Count != 0)
        {
            bool checkIfLeakAvailable = false;
            foreach (LeverScript item in leversWithLeaksZones)
            {
                for (int i = 0; i < 2; i++)
                {
                    if(item.allLeaksZones[i] != null)
                    {
                        checkIfLeakAvailable = true;
                        goto LoopEnd;
                    }
                }
            }
            LoopEnd:
            if (!checkIfLeakAvailable)
                return; // no leak available
            int lever;
            do
                lever = Random.Range(0, leversWithLeaksZones.Count);
            while 
            (leversWithLeaksZones[lever].allLeaksZones[0] == null && leversWithLeaksZones[lever].allLeaksZones[1] == null);
            int pipe;
            if(leversWithLeaksZones[lever].allLeaksZones[0] != null && leversWithLeaksZones[lever].allLeaksZones[1] != null)
                pipe = Random.Range(0, 2);
            else
            {
                if (leversWithLeaksZones[lever].allLeaksZones[0] != null)
                    pipe = 0;
                else
                    pipe = 1;
            }
            int leakZone = Random.Range(0, leversWithLeaksZones[lever].allLeaksZones[pipe].Count);
            GameObject leak = Instantiate(leakPrefab, leversWithLeaksZones[lever].allLeaksZones[pipe][leakZone].GetRandomLeakPosition(), Quaternion.identity);
            Leak leakScript = leak.GetComponent<Leak>();
            leakScript.associatedPipeLeaksZones = leversWithLeaksZones[lever].allLeaksZones[pipe];
            leakScript.associatedLever = leversWithLeaksZones[lever];
            leakScript.associatedPipe = pipe;
            leversWithLeaksZones[lever].allLeaksZones[pipe] = null;
        }
    }
}
