using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PaletteWizard : ScriptableWizard
{
    public Texture2D sourceImage;

    [Range(1, 5)] public int count = 4;

    public List<Color> palette;

    private Texture2D _sourceImage;
    private float totalWorkToComplete, totalWork;

    private void OnEnable()
    {
        palette = new List<Color>();
    }

    private void OnWizardCreate()
    {
        if (palette.Count == 0)
            Refresh();
        var tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        var pixels = tex.GetPixels();
        foreach (var c in palette)
        {
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = c;
            tex.SetPixels(pixels);
            var name = "Assets/" + ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") +
                       ((int)(c.b * 255)).ToString("X2") + ".png";
            var filename = AssetDatabase.GenerateUniqueAssetPath(name);
            File.WriteAllBytes(filename, tex.EncodeToPNG());
        }

        AssetDatabase.Refresh();
    }

    private void OnWizardOtherButton()
    {
        Refresh();
    }

    private void OnWizardUpdate()
    {
        helpString = sourceImage == null ? "Select an image to generate a palette from." : "";
        isValid = sourceImage != null;
        if (sourceImage != null && sourceImage != _sourceImage)
            Refresh();
        _sourceImage = sourceImage;
    }

    [MenuItem("Assets/Create/Palette Wizard")]
    private static void CreateWizard()
    {
        DisplayWizard<PaletteWizard>("Create Palette Textures", "Save Textures", "Refresh");
    }

    private void Refresh()
    {
        if (sourceImage == null) return;
        Color[] pixels;
        try
        {
            pixels = sourceImage.GetPixels();
            errorString = "";
        }
        catch (UnityException)
        {
            errorString = "Texture must be read/write enabled.";
            return;
        }

        try
        {
            var cuts = new Queue<Color[]>();
            cuts.Enqueue(pixels);
            var loops = (int)Mathf.Pow(2, count);
            totalWork = 0;
            totalWorkToComplete = loops * 4 + 1;
            IncrementProgress();
            while (cuts.Count < loops)
            {
                var p = cuts.Dequeue();
                Color[] top, bottom;
                ExtractColors(p, out top, out bottom);
                cuts.Enqueue(top);
                cuts.Enqueue(bottom);
            }

            palette.Clear();
            while (cuts.Count > 0)
            {
                var cut = cuts.Dequeue();
                var color = (Vector4)Color.black;
                foreach (var i in cut)
                    color += (Vector4)i;
                color /= cut.Length;
                color.w = 1;
                palette.Add(color);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void IncrementProgress()
    {
        totalWork += 1;
        EditorUtility.DisplayProgressBar("Palette Wizard", "Calculating colors from image...",
            totalWork / totalWorkToComplete);
    }

    private void ExtractColors(Color[] pixels, out Color[] top, out Color[] bottom)
    {
        var min = Color.white;
        var max = Color.black;
        foreach (var i in pixels)
        {
            min.r = Mathf.Min(min.r, i.r);
            min.g = Mathf.Min(min.g, i.g);
            min.b = Mathf.Min(min.b, i.b);
            max.r = Mathf.Min(max.r, i.r);
            max.g = Mathf.Min(max.g, i.g);
            max.b = Mathf.Min(max.b, i.b);
        }

        IncrementProgress();
        var range = max - min;
        var channel = 2;
        if (range.r >= range.g && range.r >= range.b)
            channel = 0;
        else if (range.g >= range.b)
            channel = 1;
        var keys = new float[pixels.Length];
        for (var i = 0; i < pixels.Length; i++)
            keys[i] = pixels[i][channel];
        IncrementProgress();
        Array.Sort(keys, pixels);
        IncrementProgress();
        var size = pixels.Length / 2;
        top = new Color[size];
        bottom = new Color[size];
        Array.Copy(pixels, 0, top, 0, size);
        Array.Copy(pixels, size, bottom, 0, size);
        IncrementProgress();
    }
}