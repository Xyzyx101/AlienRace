using UnityEngine;
using UnityEditor;
using System.Collections;

public class HeightmapFromTexture : ScriptableWizard
{
    public Texture2D heightmap;
    public Terrain terrain;

    [MenuItem("Terrain/HeightmapFromTexture")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<HeightmapFromTexture>("Create Heightmap", "Create");
    }

    void OnWizardCreate()
    {
        if (heightmap == null)
        {
            EditorUtility.DisplayDialog("No Texture Selected", "You must select a texture", "Cancel");
            return;
        }
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("No Terrain Object Selected", "You must select a terrain object.", "Cancel");
            return;
        }
        Undo.RecordObject(terrain.terrainData, "Heightmap from Texture");
        TerrainData terrainData = terrain.terrainData;
        int w = heightmap.width;
        int h = heightmap.height;
        int w2 = terrainData.heightmapWidth;
        float[,] heightmapData = terrainData.GetHeights(0, 0, w2, w2);
        Color[] heightmapColors = heightmap.GetPixels();
        Color[] terrainColors = new Color[w2 * w2];

        if (w2 != w || h != w)
        {
            if (heightmap.filterMode == FilterMode.Point) // Nearest Neighbour filtering
            {
                Debug.Log("Using Nearest Neighbour filtering");
                float dx = (float)w / (float)w2;
                float dy = (float)h / (float)w2;
                for (int y = 0; y < w2; ++y)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating Texture", Mathf.InverseLerp(0f, w2, y));
                    }
                    int heightmapY = (int)(dy * y) * w;
                    int terrainY = y * w2;
                    for (int x = 0; x < w2; ++x)
                    {
                        terrainColors[terrainY + x] = heightmapColors[heightmapY + (int)(dx * x)];
                    }
                }
            }
            else // Bilinear Filtering
            {
                Debug.Log("Using Bilinear Filtering");
                float ratioX = (float)(w - 1) / (float)(w2);
                float ratioY = (float)(h - 1) / (float)(w2);
                for (int y = 0; y < w2; ++y)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating Texture", Mathf.InverseLerp(0f, w2, y));
                    }
                    int yy = (int)Mathf.Floor((float)y * ratioY);
                    int y1 = (int)(yy * (float)w);
                    int y2 = (int)((yy + 1f) * (float)w);
                    int yTerrain = y * w2;
                    for (int x = 0; x < w2; ++x)
                    {
                        int xx = (int)(Mathf.Floor((float)x * ratioX));

                        Color bl = heightmapColors[y1 + xx];
                        Color br = heightmapColors[y1 + xx+1];
                        Color tl = heightmapColors[y2 + xx];
                        Color tr = heightmapColors[y2 + xx + 1];

                        float xLerp = (float)x * ratioX - xx;
                        float yLerp = (float)y * ratioY - yy;

                        Color b = Color.Lerp(bl, br, xLerp);
                        Color t = Color.Lerp(tl, tr, xLerp);
                        terrainColors[yTerrain + x] = Color.Lerp(b, t, yLerp);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        else
        {
            terrainColors = heightmapColors;
        }

        for (int y = 0; y < w2; ++y)
        {
            for (int x = 0; x < w2; ++x)
            {
                heightmapData[y, x] = terrainColors[y * w2 + x].grayscale;
            }
        }
        terrainData.SetHeights(0, 0, heightmapData);
    }
}
