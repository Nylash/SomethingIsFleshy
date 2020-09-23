using UnityEngine;
using System.Collections.Generic;

public class Trailer_3 : MonoBehaviour
{
    public LeakZone leakZone;
    public List<LeakZone> pipeLeakZones;
    public LeverScript associatedLever;
    public int associatedPipe;
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.SwitchCamera.started += ctx => LeaksManager.instance.StartSpecificLeak(leakZone, pipeLeakZones, associatedLever,associatedPipe);
    }
}
