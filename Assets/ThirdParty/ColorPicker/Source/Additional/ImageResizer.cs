#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageResizer : EditorWindow {

    [MenuItem("Window/Editor/Image Resizer")]
    public static void Init()
    {
        ImageResizer imgres = (ImageResizer)EditorWindow.GetWindow(typeof(ImageResizer));
        imgres.minSize = new Vector2(400, 235);
        imgres.maxSize = new Vector2(405, 240);

        imgres.Show();
    }

    string x = "1024";
    string y = "768";
    Texture2D tex;

    private void OnGUI()
    {
        GUILayout.Label("Image Resizer");
        EditorGUILayout.HelpBox("NOTICE: Target texture to resize must be in ARGB32, RGBA32, RGB24 or Alpha8 Format!", MessageType.Warning);
        tex = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Texture"), tex, typeof(Texture2D));
        if (tex)
            GUILayout.Label("Current Image Size: " + tex.width.ToString() + " x " + tex.height.ToString());
        GUILayout.Label("Width");
        x = GUILayout.TextArea(x);
        GUILayout.Label("Height");
        y = GUILayout.TextArea(y);

        if (GUILayout.Button("Resize"))
        {
            int xf = 0;
            int yf = 0;
            if(!int.TryParse(x, out xf) || !int.TryParse(y, out yf))
            {
                Debug.LogError("Image resizer: you've entered incorrect numeric characters! 0,1,2,3,4,5,6,7,8,9 are only allowed characters.");
                return;
            }
            if(!tex)
            {
                Debug.LogError("Image resizer: target texture is missing.");
                return;
            }

            Texture2D t = ResizeMethod(tex, xf, yf);
            string path = EditorUtility.OpenFolderPanel("Choose path to save", Application.dataPath,Path.GetFileName(Application.dataPath));
            if(!string.IsNullOrEmpty(path))
            {
                File.Create(path+ tex.name + "_Resized.png").Dispose();
                File.WriteAllBytes(path+ tex.name + "_Resized.png", t.EncodeToJPG());
                System.Diagnostics.Process.Start(path);
            }
        }
    }

    private Texture2D ResizeMethod(Texture2D source, int width, int height)
    {
        Texture2D result = new Texture2D(width, height, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / source.width) * ((float)source.width / width);
        float incY = ((float)1 / source.height) * ((float)source.height / height);

        for (int px = 0; px < rpixels.Length; px++)
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % width),
                              incY * ((float)Mathf.Floor(px / width)));
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

}
#endif