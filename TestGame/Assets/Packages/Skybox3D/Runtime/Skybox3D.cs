using UnityEngine;

namespace Gamekit3D.SkyboxVolume
{
    [RequireComponent(typeof(Camera))]
    public class Skybox3D : MonoBehaviour
    {
        [Tooltip("The main camera in the scene. If null, Camera.main is used.")]
        public new Camera camera;

        [Tooltip("A smaller value here increases the scale of the skybox.")]
        public float movementCoefficient = 0.01f;

        private Transform cameraTransform;

        private Camera skyCam;

        private void Start()
        {
            camera.clearFlags = CameraClearFlags.Depth;
            cameraTransform = camera.transform;
            skyCam = GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            if (camera != null)
            {
                skyCam.fieldOfView = camera.fieldOfView;
                transform.rotation = cameraTransform.rotation;
                transform.localPosition = cameraTransform.position * movementCoefficient;
            }
        }
    }
}