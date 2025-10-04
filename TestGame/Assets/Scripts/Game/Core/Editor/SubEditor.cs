using System;
using UnityEditor;

namespace Gamekit3D
{
    public abstract class SubEditor<T>
    {
        private Action defer;

        private Editor editor;
        public abstract void OnInspectorGUI(T instance);

        public void Init(Editor editor)
        {
            this.editor = editor;
        }

        public void Update()
        {
            if (defer != null) defer();
            defer = null;
        }

        protected void Defer(Action fn)
        {
            defer += fn;
        }

        protected void Repaint()
        {
            editor.Repaint();
        }
    }
}