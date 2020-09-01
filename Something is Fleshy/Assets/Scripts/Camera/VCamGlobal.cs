using UnityEngine;
using Cinemachine;

public class VCamGlobal : MonoBehaviour
{
    Transform target;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("LevelCenter").transform;
        GetComponent<CinemachineVirtualCamera>().Follow = target;
    }
}
