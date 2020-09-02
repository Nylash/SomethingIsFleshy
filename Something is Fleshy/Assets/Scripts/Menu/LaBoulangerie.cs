using UnityEngine;

public class LaBoulangerie : MonoBehaviour
{
    public Animator splashScreenAnim;

    void DisplaySplashScreen()
    {
        splashScreenAnim.SetTrigger("Display");
    }
}
