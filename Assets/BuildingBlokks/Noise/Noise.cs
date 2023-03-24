using UnityEngine;

namespace BuildingBlokks.Noise
{
    public static class Noise
    {
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax) =>
            outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);

        public static float Remap01(float value, float outMin, float outMax) =>
            Remap(value, 0, 1, outMin, outMax);

        public static int Remap01Int(float value, float outMin, float outMax) =>
            (int)Remap01(value, outMin, outMax);

        public static float Redistribute(float noise, NoiseSettings settings) =>
            Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);

        public static float OctavePerlin(float x, float z, NoiseSettings settings)
        {
            x *= settings.zoom;
            z *= settings.zoom;

            x += settings.zoom;
            z += settings.zoom;

            var total = 0f;
            var frequency = 1f;
            var amplitude = 1f;
            
            // used to normalize the value back between 0 and 1
            var ampSum = 0f;

            for (var i = 0; i < settings.octaves; i++)
            {
                var sampleX = (settings.offset.x + settings.worldOffset.x + x) * frequency;
                var sampleY = (settings.offset.y + settings.worldOffset.y + z) * frequency;
                total += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;

                ampSum += amplitude;

                amplitude *= settings.persistance;
                frequency *= 2;
            }

            // normalization
            return total / ampSum;
        }

        public static float[,] HeightMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, Vector2 sample)
        {
            scale = Mathf.Clamp(scale, 0.0001f, float.MaxValue);

            var prng = new System.Random(seed);
            
            var amplitude = 1f;
            var frequency = 1f;
            
            var octaveOffsets = new Vector2[octaves];
            for (var i = 0; i < octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    prng.Next(-100_000, 100_000) + offset.x + sample.x, 
                    prng.Next(-100_000, 100_000) - offset.y - sample.y
                );

                amplitude *= persistance;
            }
            
            var map = new float[width, height];

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    amplitude = 1f;
                    frequency = 1f;
                    var noiseHeight = 0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        var octaveOffset = octaveOffsets[i];
                        var sampleX = (x - halfWidth + octaveOffset.x) / scale * frequency;
                        var sampleY = (y - halfHeight + octaveOffset.y) / scale * frequency;

                        var val = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        
                        noiseHeight += val * amplitude;
                        
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;
                    
                    map[x, y] = noiseHeight;
                }
            }

            // normalize noise back to values between 0 and 1
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
                }
            }

            return map;
        }
    }
}
