using UnityEngine;

namespace Gamekit3D
{
    public class SetCursorMode : MonoBehaviour
    {
        public bool visible = true;
        public CursorLockMode lockMode = CursorLockMode.None;

        private void Start()
        {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }
    }
}