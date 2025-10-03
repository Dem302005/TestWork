using System;
using Cinemachine;
using UnityEngine;

namespace Gamekit3D
{
    public class CameraSettings : MonoBehaviour
    {
        public enum InputChoice
        {
            KeyboardAndMouse,
            Controller
        }


        public Transform follow;
        public Transform lookAt;
        public CinemachineFreeLook keyboardAndMouseCamera;
        public CinemachineFreeLook controllerCamera;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;

        public CinemachineFreeLook Current =>
            inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera;

        private void Awake()
        {
            UpdateCameraSettings();
        }

        private void Reset()
        {
            var keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");
            if (keyboardAndMouseCameraTransform != null)
                keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook>();

            var controllerCameraTransform = transform.Find("ControllerFreeLookRig");
            if (controllerCameraTransform != null)
                controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook>();

            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.name == "Ellen")
            {
                follow = playerController.transform;

                lookAt = follow.Find("HeadTarget");

                if (playerController.cameraSettings == null)
                    playerController.cameraSettings = this;
            }
        }

        private void Update()
        {
            if (allowRuntimeCameraSettingsChanges) UpdateCameraSettings();
        }

        private void UpdateCameraSettings()
        {
            keyboardAndMouseCamera.Follow = follow;
            keyboardAndMouseCamera.LookAt = lookAt;
            keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            controllerCamera.Follow = follow;
            controllerCamera.LookAt = lookAt;

            keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
        }

        [Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }
    }
}