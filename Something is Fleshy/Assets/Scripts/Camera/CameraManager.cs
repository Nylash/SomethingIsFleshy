using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [SerializeField] float shakeDuration = .5f;
    [SerializeField] float shakeAmplitude = .3f;
    [SerializeField] float shakeFrequency = .3f;
#pragma warning restore 0649
    #endregion

    [Header("COMPONENTS")]
    public GameObject VCamZoom;
    public GameObject VCamGlobal;
    CinemachineBasicMultiChannelPerlin VCamZoomNoise;
    CinemachineBasicMultiChannelPerlin VCamGlobalNoise;

    [Header("VARIABLES")]
    Coroutine shakeCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        VCamZoom = GameObject.FindGameObjectWithTag("CamZoom");
        VCamGlobal = GameObject.FindGameObjectWithTag("CamGlobal");
        VCamZoomNoise = VCamZoom.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        VCamGlobalNoise = VCamGlobal.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        VCamGlobal.SetActive(false);
    }

    public void SwitchCameraFromScript()
    {
        if (VCamZoom.activeSelf)
        {
            VCamZoom.SetActive(false);
            VCamGlobal.SetActive(true);
        }
        else
        {
            VCamZoom.SetActive(true);
            VCamGlobal.SetActive(false);
        }
    }

    void SwitchCameraFromInput()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (VCamZoom.activeSelf)
                {
                    VCamZoom.SetActive(false);
                    VCamGlobal.SetActive(true);
                }
                else
                {
                    VCamZoom.SetActive(true);
                    VCamGlobal.SetActive(false);
                }
            }
        }
    }

    public void ShakeScreen()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (shakeCoroutine == null)
                    shakeCoroutine = StartCoroutine(DoShakeScreen());
            }
        }
    }

    IEnumerator DoShakeScreen()
    {
        if (VCamZoom.activeSelf)
        {
            VCamZoomNoise.m_AmplitudeGain = shakeAmplitude;
            VCamZoomNoise.m_FrequencyGain = shakeFrequency;
            yield return new WaitForSeconds(shakeDuration);
            VCamZoomNoise.m_AmplitudeGain = 0;
        }
        else
        {
            VCamGlobalNoise.m_AmplitudeGain = shakeAmplitude;
            VCamGlobalNoise.m_FrequencyGain = shakeFrequency;
            yield return new WaitForSeconds(shakeDuration);
            VCamGlobalNoise.m_AmplitudeGain = 0;
        }
        yield return new WaitForSeconds(shakeDuration);
        shakeCoroutine = null;
    }
}
