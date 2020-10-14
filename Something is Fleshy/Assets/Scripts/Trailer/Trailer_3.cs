using UnityEngine;
using System.Collections.Generic;

public class Trailer_3 : MonoBehaviour
{
    public LeakZone leakZone;
    public List<LeakZone> pipeLeakZones;
    public LeverScript associatedLever;
    public int associatedPipe;
    PlayerMap playerMap;

    private void OnEnable() => playerMap.Gameplay.Enable();
    private void OnDisable() => playerMap.Gameplay.Disable();

    private void Awake()
    {
        playerMap = new PlayerMap();

        //actionsMap.Gameplay.SwitchCamera.started += ctx => LeaksManager.instance.StartSpecificLeak(leakZone, pipeLeakZones, associatedLever,associatedPipe);
    }
}
