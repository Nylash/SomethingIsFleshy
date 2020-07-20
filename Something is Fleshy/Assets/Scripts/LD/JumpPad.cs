using UnityEngine;

public class JumpPad : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Bonus for vertical jump from this jumpPad.")]
    public float jumpPadForce = 5;
#pragma warning restore 0649
    #endregion
}
