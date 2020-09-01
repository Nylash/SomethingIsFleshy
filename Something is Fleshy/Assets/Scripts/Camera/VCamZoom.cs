using UnityEngine;
using Cinemachine;

public class VCamZoom : MonoBehaviour
{
    Transform target;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<CinemachineVirtualCamera>().Follow = target;
    }
}
