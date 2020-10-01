using System.Collections;
using UnityEngine;
using Cinemachine;

public class Trailer_2 : MonoBehaviour
{
    public GameObject cam;
    public float timeBeforeCutToCam;
    public SecondarySystem ss;
    public GameObject text1;
    public GameObject text2;
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Interact.started += ctx => StartCoroutine(CutToCam());
        actionsMap.Gameplay.Debug.started += ctx => StartSS();
        actionsMap.Gameplay.SwitchCamera.started += ctx => StartCoroutine(ShowText());
    }

    private void StartSS()
    {
        SecondarySystemsManager.instance.LaunchSpecificSS(ss, null, LeverScript.RessourcesType.energy,false);
    }

    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(1);
        text1.SetActive(true);
        text2.SetActive(true);
    }

    IEnumerator CutToCam()
    {
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        yield return new WaitForSeconds(timeBeforeCutToCam);
        cam.SetActive(true);
        CameraManager.instance.VCamZoom.SetActive(false);
        cam.GetComponent<Animator>().SetTrigger("CameraMovement");
        text1.SetActive(false);
    }
}
