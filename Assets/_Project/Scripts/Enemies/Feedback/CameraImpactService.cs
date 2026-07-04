using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraImpactService : ICameraImpactService
{
    private readonly Camera camera;

    public CameraImpactService()
    {
        camera = Camera.main;
    }

    public void PlaySmallImpact()
    {
        PlaySmallImpactAsync().Forget();
    }

    private async UniTaskVoid PlaySmallImpactAsync()
    {
        if (camera == null)
            return;

        Transform cameraTransform = camera.transform;
        Vector3 startPosition = cameraTransform.localPosition;
        Vector3 impactPosition = startPosition + Vector3.forward * 0.08f;

        cameraTransform.localPosition = impactPosition;
        await UniTask.Delay(50);

        if (cameraTransform != null)
            cameraTransform.localPosition = startPosition;
    }
}
