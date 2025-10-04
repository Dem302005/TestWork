using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    [SelectionBase]
    [CustomEditor(typeof(GameCommandReceiver), true)]
    public class CommandReceiverEditor : Editor
    {
        private readonly List<SendGameCommand> senders = new List<SendGameCommand>();

        private void OnEnable()
        {
            var interactive = target as GameCommandReceiver;
            senders.Clear();
            foreach (SendGameCommand si in Resources.FindObjectsOfTypeAll(typeof(SendGameCommand)))
                if (si.interactiveObject == interactive)
                    senders.Add(si);
        }

        private void OnSceneGUI()
        {
            foreach (var i in senders) SendGameCommandEditor.DrawInteraction(i);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginVertical("box");
            GUILayout.Label("Senders");
            foreach (var i in senders)
                EditorGUILayout.ObjectField(i, typeof(SendGameCommand), true);
            GUILayout.EndVertical();
        }
    }
}