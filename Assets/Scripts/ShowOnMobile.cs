using UnityEngine;

public class ShowOnMobile : MonoBehaviour
{
    private void Start()
    {
        // Show touch controls only on mobile devices.
        // Desktop/WebGL on computers will hide them.

        gameObject.SetActive(Application.isMobilePlatform);
    }
}