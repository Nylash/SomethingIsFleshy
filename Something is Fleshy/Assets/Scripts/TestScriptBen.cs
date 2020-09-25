using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptBen : MonoBehaviour
{
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Debug.started += ctx => Test();
    }

    void Test()
    {
        CharacterController2D.instance.animator.SetTrigger("Stress");
    }
}
