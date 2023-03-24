using BuildingBlokks.World;
using UnityEngine;
using UnityEngine.Serialization;

namespace BuildingBlokks.Noise
{
    public class NoiseGenerator : MonoBehaviour
    {
        public NoiseSettings noiseSettings;
        
        [Min(1)]
        public int width;
        
        [Min(1)]
        public int height;
        
        [Min(0.1f)]
        public float scale;
        
        [Min(0)]
        public int octaves;
        
        [Range(0, 1)]
        public float persistance;
        
        [Min(1)]
        public float lacunarity;

        public int seed;
        
        public Vector2 offset;

        public bool autoUpdate;

        public int GetSurfaceHeightNoise(Vector2 pos)
        {
            var y = Noise.OctavePerlin(pos.x, pos.y, noiseSettings);
            y = Noise.Redistribute(y, noiseSettings);
            
            return Noise.Remap01Int(y, 0, Chunk.Height);
        }

        public float[,] GenerateMap(Vector2 sample) => 
            BuildingBlokks.Noise.Noise.HeightMap(Chunk.Size, Chunk.Size, seed, scale, octaves, persistance, lacunarity, offset, sample);

        public void DrawMap()
        {
            var map = BuildingBlokks.Noise.Noise.HeightMap(width, height, seed, scale, octaves, persistance, lacunarity, offset, Vector2.zero);
            var display = FindObjectOfType<MapDisplay>();
            display.DrawMap(map);
        }
    }
}
