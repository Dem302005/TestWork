// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// C # manual conversion work by Yun Kyu Choi

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

internal enum SaveFormat
{
    Triangles,
    Quads
}

internal enum SaveResolution
{
    Full = 0,
    Half,
    Quarter,
    Eighth,
    Sixteenth
}

internal class ExportTerrain : EditorWindow
{
    private static TerrainData terrain;
    private static Vector3 terrainPos;
    private int counter;
    private readonly int progressUpdateInterval = 10000;
    private SaveFormat saveFormat = SaveFormat.Triangles;
    private SaveResolution saveResolution = SaveResolution.Half;

    private int tCount;
    private int totalCount;

    private void OnGUI()
    {
        if (!terrain)
        {
            GUILayout.Label("No terrain found");
            if (GUILayout.Button("Cancel")) GetWindow<ExportTerrain>().Close();
            return;
        }

        saveFormat = (SaveFormat)EditorGUILayout.EnumPopup("Export Format", saveFormat);

        saveResolution = (SaveResolution)EditorGUILayout.EnumPopup("Resolution", saveResolution);

        if (GUILayout.Button("Export")) Export();
    }

    [MenuItem("Terrain/Export To Obj...")]
    private static void Init()
    {
        terrain = null;
        var terrainObject = Selection.activeObject as Terrain;
        if (!terrainObject) terrainObject = Terrain.activeTerrain;
        if (terrainObject)
        {
            terrain = terrainObject.terrainData;
            terrainPos = terrainObject.transform.position;
        }

        GetWindow<ExportTerrain>().Show();
    }

    private void Export()
    {
        var fileName = EditorUtility.SaveFilePanel("Export .obj file", "", "Terrain", "obj");
        var w = terrain.heightmapResolution;
        var h = terrain.heightmapResolution;
        var meshScale = terrain.size;
        var tRes = (int)Mathf.Pow(2, (int)saveResolution);
        meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
        var uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
        var tData = terrain.GetHeights(0, 0, w, h);

        w = (w - 1) / tRes + 1;
        h = (h - 1) / tRes + 1;
        var tVertices = new Vector3[w * h];
        var tUV = new Vector2[w * h];

        int[] tPolys;

        if (saveFormat == SaveFormat.Triangles)
            tPolys = new int[(w - 1) * (h - 1) * 6];
        else
            tPolys = new int[(w - 1) * (h - 1) * 4];

        // Build vertices and UVs
        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
        {
            tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x)) + terrainPos;
            tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
        }

        var index = 0;
        if (saveFormat == SaveFormat.Triangles)
            // Build triangle indices: 3 indices into vertex array for each triangle
            for (var y = 0; y < h - 1; y++)
            for (var x = 0; x < w - 1; x++)
            {
                // For each grid cell output two triangles
                tPolys[index++] = y * w + x;
                tPolys[index++] = (y + 1) * w + x;
                tPolys[index++] = y * w + x + 1;

                tPolys[index++] = (y + 1) * w + x;
                tPolys[index++] = (y + 1) * w + x + 1;
                tPolys[index++] = y * w + x + 1;
            }
        else
            // Build quad indices: 4 indices into vertex array for each quad
            for (var y = 0; y < h - 1; y++)
            for (var x = 0; x < w - 1; x++)
            {
                // For each grid cell output one quad
                tPolys[index++] = y * w + x;
                tPolys[index++] = (y + 1) * w + x;
                tPolys[index++] = (y + 1) * w + x + 1;
                tPolys[index++] = y * w + x + 1;
            }

        // Export to .obj
        var sw = new StreamWriter(fileName);
        try
        {
            sw.WriteLine("# Unity terrain OBJ File");

            // Write vertices
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            counter = tCount = 0;
            totalCount =
                (tVertices.Length * 2 + (saveFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) /
                progressUpdateInterval;
            for (var i = 0; i < tVertices.Length; i++)
            {
                UpdateProgress();
                var sb = new StringBuilder("v ", 20);
                // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                // Which is important when you're exporting huge terrains.
                sb.Append(tVertices[i].x.ToString()).Append(" ").Append(tVertices[i].y.ToString()).Append(" ")
                    .Append(tVertices[i].z.ToString());
                sw.WriteLine(sb);
            }

            // Write UVs
            for (var i = 0; i < tUV.Length; i++)
            {
                UpdateProgress();
                var sb = new StringBuilder("vt ", 22);
                sb.Append(tUV[i].x.ToString()).Append(" ").Append(tUV[i].y.ToString());
                sw.WriteLine(sb);
            }

            if (saveFormat == SaveFormat.Triangles)
                // Write triangles
                for (var i = 0; i < tPolys.Length; i += 3)
                {
                    UpdateProgress();
                    var sb = new StringBuilder("f ", 43);
                    sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").Append(tPolys[i + 1] + 1)
                        .Append("/").Append(tPolys[i + 1] + 1).Append(" ").Append(tPolys[i + 2] + 1).Append("/")
                        .Append(tPolys[i + 2] + 1);
                    sw.WriteLine(sb);
                }
            else
                // Write quads
                for (var i = 0; i < tPolys.Length; i += 4)
                {
                    UpdateProgress();
                    var sb = new StringBuilder("f ", 57);
                    sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").Append(tPolys[i + 1] + 1)
                        .Append("/").Append(tPolys[i + 1] + 1).Append(" ").Append(tPolys[i + 2] + 1).Append("/")
                        .Append(tPolys[i + 2] + 1).Append(" ").Append(tPolys[i + 3] + 1).Append("/")
                        .Append(tPolys[i + 3] + 1);
                    sw.WriteLine(sb);
                }
        }
        catch (Exception err)
        {
            Debug.Log("Error saving file: " + err.Message);
        }

        sw.Close();

        terrain = null;
        EditorUtility.DisplayProgressBar("Saving file to disc.", "This might take a while...", 1f);
        GetWindow<ExportTerrain>().Close();
        EditorUtility.ClearProgressBar();
    }

    private void UpdateProgress()
    {
        if (counter++ == progressUpdateInterval)
        {
            counter = 0;
            EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
        }
    }
}