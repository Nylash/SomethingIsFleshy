using UnityEngine;
using Cinemachine;

public class VCamZoom : MonoBehaviour
{
    Transform target;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("LevelCenter").transform;
        GetComponent<CinemachineVirtualCamera>().Follow = target;
    }
}
