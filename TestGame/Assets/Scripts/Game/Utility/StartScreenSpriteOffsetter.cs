using UnityEngine;

namespace Gamekit3D
{
    public class StartScreenSpriteOffsetter : MonoBehaviour
    {
        public float spriteOffset;
        private Vector3 initialPosition;
        private Vector3 newPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            transform.position = new Vector3(initialPosition.x + spriteOffset * Input.mousePosition.x,
                initialPosition.y + spriteOffset * Input.mousePosition.y, initialPosition.z);
        }
    }
}