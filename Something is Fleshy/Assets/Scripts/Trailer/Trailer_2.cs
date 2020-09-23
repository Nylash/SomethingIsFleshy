using System.Collections;
using UnityEngine;

public class Trailer_2 : MonoBehaviour
{
    public GameObject cam;
    public float timeBeforeCutToCam;
    public SecondarySystem ss;
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Interact.started += ctx => StartCoroutine(CutToCam());
        actionsMap.Gameplay.SwitchCamera.started += ctx => StartSS();
    }

    private void StartSS()
    {
        SecondarySystemsManager.instance.LaunchSpecificSS(ss, null, LeverScript.RessourcesType.energy,false);
    }

    IEnumerator CutToCam()
    {
        yield return new WaitForSeconds(timeBeforeCutToCam);
        cam.SetActive(true);
        CameraManager.instance.VCamZoom.SetActive(false);
        cam.GetComponent<Animator>().SetTrigger("CameraMovement");
    }
}
