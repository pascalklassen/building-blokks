using BuildingBlokks.Noise;
using UnityEngine;

namespace BuildingBlokks.World.Biome
{
    public class HillsBiomeGenerator : IBiomeGenerator
    {
        public void GenerateBiome(ChunkData data, World world, NoiseGenerator noise)
        {
            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var z = 0; z < Chunk.Size; z++)
                {
                    var position = new Vector3Int(x, 0, z);
                    var height = noise.GetSurfaceHeightNoise(new Vector2(x, z));

                    for (var y = 0; y < height; y++)
                    {
                        position.y = y;
                        data[position] = BlockType.Dirt;
                    }

                    for (var y = height; y < Chunk.Height; y++)
                    {
                        position.y = y;
                        data[position] = BlockType.Air;
                    }
                }
            }
        }
    }
}