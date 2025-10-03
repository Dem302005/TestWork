using UnityEngine;

namespace ProBuilder.Examples
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class SimpleGrid : MonoBehaviour
    {
        public int lines = 10;
        public float scale = 1f;

        private void Start()
        {
            GetComponent<MeshFilter>().sharedMesh = GridMesh(lines, scale);
            transform.position = Vector3.zero;
        }

        private Mesh GridMesh(int lineCount, float scale)
        {
            var half = lineCount / 2f * scale;

            lineCount++;

            var lines = new Vector3[lineCount * 4];
            var normals = new Vector3[lineCount * 4];
            var uv = new Vector2[lineCount * 4];
            var indices = new int[lineCount * 4];

            var n = 0;
            for (var y = 0; y < lineCount; y++)
            {
                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(y * scale - half, 0f, -half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(y * scale - half, 0f, half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(-half, 0f, y * scale - half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(half, 0f, y * scale - half);
            }

            for (var i = 0; i < lines.Length; i++) normals[i] = Vector3.up;

            var tm = new Mesh();

            tm.name = "GridMesh";
            tm.vertices = lines;
            tm.normals = normals;
            tm.subMeshCount = 1;
            tm.SetIndices(indices, MeshTopology.Lines, 0);
            tm.uv = uv;

            return tm;
        }
    }
}