using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    public class InteractOnButton : InteractOnTrigger
    {
        public string buttonName = "X";
        public UnityEvent OnButtonPress;

        private bool canExecuteButtons;

        private void Update()
        {
            if (canExecuteButtons && Input.GetButtonDown(buttonName)) OnButtonPress.Invoke();
        }

        protected override void ExecuteOnEnter(Collider other)
        {
            canExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            canExecuteButtons = false;
        }
    }
}