using UnityEngine;
using Cinemachine;

public class ResponsiveCinemachineZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Landscape")]
    [SerializeField] private float landscapeOrthoSize = 5f;
    [SerializeField] private float referenceAspect = 16f / 9f;

    [Header("Portrait")]
    [SerializeField] private float maxPortraitOrthoSize = 8f;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        UpdateZoom();
    }

    private void Update()
    {
        UpdateZoom();
    }

    private void UpdateZoom()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        float adjustedSize = landscapeOrthoSize * (referenceAspect / currentAspect);

        virtualCamera.m_Lens.OrthographicSize = Mathf.Min(adjustedSize, maxPortraitOrthoSize);
    }
}