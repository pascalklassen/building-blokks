using UnityEngine;

namespace BuildingBlokks
{
    public class MapDisplay : MonoBehaviour
    {
        public new Renderer renderer;

        public void DrawMap(float[,] map)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            var texture = new Texture2D(width, height);

            var colors = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colors[y * width + x] = Color.Lerp(Color.black, Color.white, map[x, y]);
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();

            renderer.sharedMaterial.mainTexture = texture;
            renderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}