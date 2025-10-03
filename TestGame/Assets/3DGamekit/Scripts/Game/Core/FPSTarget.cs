using UnityEngine;

namespace Gamekit3D
{
    public class FPSTarget : MonoBehaviour
    {
        public int targetFPS = 60;

        private void OnEnable()
        {
            SetTargetFPS(targetFPS);
        }

        public void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}