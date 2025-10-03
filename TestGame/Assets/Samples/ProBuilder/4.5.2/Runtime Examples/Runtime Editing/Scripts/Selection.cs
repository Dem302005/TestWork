using System;
using System.Collections.Generic;
using UnityEngine.ProBuilder;

namespace ProBuilder.Examples
{
    internal static class Selection
    {
        private static readonly HashSet<ProBuilderMesh> s_Selection = new HashSet<ProBuilderMesh>();

        public static IEnumerable<ProBuilderMesh> meshes => s_Selection;

        public static bool Add(ProBuilderMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            return s_Selection.Add(mesh);
        }

        public static void Remove(ProBuilderMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            if (s_Selection.Contains(mesh))
                s_Selection.Remove(mesh);
        }

        public static bool Contains(ProBuilderMesh mesh)
        {
            return s_Selection.Contains(mesh);
        }

        public static void Clear()
        {
            s_Selection.Clear();
        }
    }
}